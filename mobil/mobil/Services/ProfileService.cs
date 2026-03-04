using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace mobil.Services
{
    public class ProfileService
    {
        private readonly HttpClient _http;
        public ProfileService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> UpdateMyData(EditProfileData data)
        {
            var response = await _http.PatchAsJsonAsync("profile/edit", data);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(body) ? $"Error: {(int)response.StatusCode} {response.ReasonPhrase}" : body;
            }
            return null;
        }
    }
}
