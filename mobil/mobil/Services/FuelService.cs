using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mobil.Services
{
    public class FuelService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        public FuelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResponse<Fuel>?> MyFuels(int page = 1, int pageSize = 25)
        {
            var response = await _httpClient.GetAsync($"fuellogs/mine?page={page}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return null;
            var body = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    var items = JsonSerializer.Deserialize<List<Fuel>>(body, _jsonOptions) ?? [];
                    return new PagedResponse<Fuel>
                    {
                        Items = items,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = items.Count,
                        TotalPages = 1
                    };
                }
                var paged = JsonSerializer.Deserialize<PagedResponse<Fuel>>(body, _jsonOptions);
                if (paged?.Items is not null && paged.Items.Count > 0)
                    return paged;
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        var items = JsonSerializer.Deserialize<List<Fuel>>(prop.Value.GetRawText(), _jsonOptions) ?? [];
                        int totalCount = items.Count;
                        if (root.TryGetProperty("totalCount", out var tc)) totalCount = tc.GetInt32();
                        else if (root.TryGetProperty("total", out var t)) totalCount = t.GetInt32();
                        int totalPages = 1;
                        if (root.TryGetProperty("totalPages", out var tp)) totalPages = tp.GetInt32();
                        else if (totalCount > 0 && pageSize > 0) totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                        return new PagedResponse<Fuel>
                        {
                            Items = items,
                            Page = page,
                            PageSize = pageSize,
                            TotalCount = totalCount,
                            TotalPages = totalPages
                        };
                    }
                }
                return new PagedResponse<Fuel> { Page = page, PageSize = pageSize };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string?> CreateFuel(FuelCreate fuel)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(fuel.Date.ToString("o")), "date");
            content.Add(new StringContent(fuel.OdometerKm.ToString()), "odometerKm");
            content.Add(new StringContent(fuel.Liters.ToString(System.Globalization.CultureInfo.InvariantCulture)), "liters");
            content.Add(new StringContent(fuel.TotalCost.ToString(System.Globalization.CultureInfo.InvariantCulture)), "totalCost");
            if (!string.IsNullOrWhiteSpace(fuel.StationName))
                content.Add(new StringContent(fuel.StationName), "stationName");
            if (!string.IsNullOrWhiteSpace(fuel.LocationText))
                content.Add(new StringContent(fuel.LocationText), "locationText");
            if (fuel.File != null)
            {
                var stream = await fuel.File.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(
                    fileContent,
                    "file",
                    fuel.File.FileName
                );
            }
            var response = await _httpClient.PostAsync("fuellogs", content);
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
                catch { }
                return body.Trim('"');
            }
            return null;
        }

        public async Task<string?> DeleteFuel(ulong fuelId)
        {
            var response = await _httpClient.PatchAsync($"fuellogs/delete/{fuelId}", null);
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
                { }
                return body.Trim('"');
            }
            return null;
        }
    }
}
