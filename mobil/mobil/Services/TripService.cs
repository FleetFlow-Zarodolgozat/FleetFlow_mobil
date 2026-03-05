using mobil.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mobil.Services
{
    public class TripService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public TripService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResponse<Trip>?> MyTrips(int page = 1, int pageSize = 25)
        {
            var response = await _httpClient.GetAsync($"trips/mine?page={page}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadAsStringAsync();
            Debug.WriteLine($"[TripService] trips/mine response: {body[..Math.Min(body.Length, 500)]}");

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // Direct array response
                if (root.ValueKind == JsonValueKind.Array)
                {
                    var items = JsonSerializer.Deserialize<List<Trip>>(body, _jsonOptions) ?? [];
                    return new PagedResponse<Trip>
                    {
                        Items = items,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = items.Count,
                        TotalPages = 1
                    };
                }

                // Paginated object response
                var paged = JsonSerializer.Deserialize<PagedResponse<Trip>>(body, _jsonOptions);
                if (paged?.Items is not null && paged.Items.Count > 0)
                    return paged;

                // Fallback: find any array property in the object
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        var items = JsonSerializer.Deserialize<List<Trip>>(prop.Value.GetRawText(), _jsonOptions) ?? [];

                        int totalCount = items.Count;
                        if (root.TryGetProperty("totalCount", out var tc)) totalCount = tc.GetInt32();
                        else if (root.TryGetProperty("total", out var t)) totalCount = t.GetInt32();

                        int totalPages = 1;
                        if (root.TryGetProperty("totalPages", out var tp)) totalPages = tp.GetInt32();
                        else if (totalCount > 0 && pageSize > 0) totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                        return new PagedResponse<Trip>
                        {
                            Items = items,
                            Page = page,
                            PageSize = pageSize,
                            TotalCount = totalCount,
                            TotalPages = totalPages
                        };
                    }
                }

                return new PagedResponse<Trip> { Page = page, PageSize = pageSize };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TripService] Deserialization error: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> CreateTrip(TripCreate trip)
        {
            var response = await _httpClient.PostAsJsonAsync("trips", trip);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                    return $"Error {(int)response.StatusCode}";
                try
                {
                    var json = JsonDocument.Parse(body);
                    if (json.RootElement.TryGetProperty("message", out var message))
                        return message.GetString();
                }
                catch (Exception ex)
                {

                }
                return body.Trim('"');
            }
            return null;
        }

        public async Task<string?> DeleteTrip(ulong tripId)
        {
            var response = await _httpClient.PatchAsync($"trips/delete/{tripId}", null);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(body))
                    return $"Error {(int)response.StatusCode}";
                try
                {
                    var json = JsonDocument.Parse(body);
                    if (json.RootElement.TryGetProperty("message", out var message))
                        return message.GetString();
                }
                catch (Exception ex)
                {
                }
                return body.Trim('"');
            }
            return null;
        }
    }
}
