using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

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
                var response = await _http.GetAsync("statistics/mine");
                if (!response.IsSuccessStatusCode)
                    return null;
                return await response.Content.ReadFromJsonAsync<Stats>();
        }
    }
}
