using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace mobil.Services
{
    public class NotificationService
    {
        private readonly HttpClient _http;
        public NotificationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Notification>?> GetMine()
        {
            var response = await _http.GetAsync("notifications");
            if (!response.IsSuccessStatusCode)
                return null;
            return await response.Content.ReadFromJsonAsync<List<Notification>>();
        }

        public async Task MarkAsAllRead()
        {
            await _http.PatchAsync("read", null);
        }
    }
}
