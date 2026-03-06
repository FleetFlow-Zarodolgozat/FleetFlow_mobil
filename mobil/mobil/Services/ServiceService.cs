using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mobil.Services
{
    public class ServiceService
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };
        public ServiceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<PagedResponse<Service>?> MyServices(int page = 1, int pageSize = 25)
        {
            var response = await _httpClient.GetAsync($"service-requests/mine?page={page}&pageSize={pageSize}");
            if (!response.IsSuccessStatusCode)
                return null;
            var body = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    var items = JsonSerializer.Deserialize<List<Service>>(body, _jsonOptions) ?? [];
                    return new PagedResponse<Service>
                    {
                        Items = items,
                        Page = page,
                        PageSize = pageSize,
                        TotalCount = items.Count,
                        TotalPages = 1
                    };
                }
                var paged = JsonSerializer.Deserialize<PagedResponse<Service>>(body, _jsonOptions);
                if (paged?.Items is not null && paged.Items.Count > 0)
                    return paged;
                foreach (var prop in root.EnumerateObject())
                {
                    if (prop.Value.ValueKind == JsonValueKind.Array)
                    {
                        var items = JsonSerializer.Deserialize<List<Service>>(prop.Value.GetRawText(), _jsonOptions) ?? [];
                        int totalCount = items.Count;
                        if (root.TryGetProperty("totalCount", out var tc)) totalCount = tc.GetInt32();
                        else if (root.TryGetProperty("total", out var t)) totalCount = t.GetInt32();
                        int totalPages = 1;
                        if (root.TryGetProperty("totalPages", out var tp)) totalPages = tp.GetInt32();
                        else if (totalCount > 0 && pageSize > 0) totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                        return new PagedResponse<Service>
                        {
                            Items = items,
                            Page = page,
                            PageSize = pageSize,
                            TotalCount = totalCount,
                            TotalPages = totalPages
                        };
                    }
                }
                return new PagedResponse<Service> { Page = page, PageSize = pageSize };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string?> CreateService(ServiceCreate service)
        {
            var response = await _httpClient.PostAsJsonAsync("service-requests", service);
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

        public async Task<string?> DeleteService(ulong serviceId)
        {
            var response = await _httpClient.DeleteAsync($"service-requests/cancel/{serviceId}");
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

        public async Task<string?> UploadServiceDetails(ulong serviceId, ServiceDetailUpload service)
        {
            using var content = new MultipartFormDataContent();
            content.Add(new StringContent(service.DriverReportCost.ToString()), "driverReportCost");
            if (!string.IsNullOrWhiteSpace(service.DriverCloseNote))
                content.Add(new StringContent(service.DriverCloseNote), "driverCloseNote");
            if (service.File != null)
            {
                var stream = await service.File.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(
                    fileContent,
                    "file",
                    service.File.FileName
                );
            }
            var response = await _httpClient.PatchAsync($"service-requests/upload-details/{serviceId}", content);
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

        public async Task<string?> EditUploadedDetails(ulong serviceId, ServiceDetailUpload service)
        {
            using var content = new MultipartFormDataContent();
            if (service.DriverReportCost > 0)
                content.Add(new StringContent(service.DriverReportCost.ToString()), "driverReportCost");
            if (!string.IsNullOrWhiteSpace(service.DriverCloseNote))
                content.Add(new StringContent(service.DriverCloseNote), "driverCloseNote");
            if (service.File != null)
            {
                var stream = await service.File.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(
                    fileContent,
                    "file",
                    service.File.FileName
                );
            }
            var response = await _httpClient.PatchAsync($"service-requests/edit-uploaded-data/{serviceId}", content);
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
    }
}
