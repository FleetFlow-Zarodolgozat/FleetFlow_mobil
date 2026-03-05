using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace mobil.Services
{
    public class DashboardService
    {
        private readonly HttpClient _http;
        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Driver?> MyProfileData()
        {
            var response = await _http.GetAsync("profile/mine");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Driver>();
        }

        public async Task<Vehicle?> MyVehicle()
        {
            var response = await _http.GetAsync("profile/assigned-vehicle");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Vehicle>();
        }

        public async Task<bool?> HaveUnreadMessage()
        {
            var response = await _http.GetAsync("notifications/unread-status");
            if (!response.IsSuccessStatusCode)
                return null;
            return bool.Parse(await response.Content.ReadAsStringAsync());
        }

        public async Task<Stats?> MyStats()
        {
            var response = await _http.GetAsync("statistics/mine?months=12");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<Stats>();
        }

        public async Task<List<Calendarevent>?> MyCalEvent()
        {
            var response = await _http.GetAsync("calendarevents");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Calendarevent>>();
        }

        public async Task<ImageSource?> GetDriverThumbnail(ulong userId)
        {
            var response = await _http.GetAsync($"files/thumbnail/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;
            var stream = await response.Content.ReadAsStreamAsync();
            return ImageSource.FromStream(() => stream);
        }

        public async Task<string?> CreateEvent(Calendarevent ev)
        {
            var response = await _http.PostAsJsonAsync("calendarevents", ev);
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

        public async Task<bool> DeleteEvent(ulong id)
        {
            var response = await _http.DeleteAsync($"calendarevents/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
