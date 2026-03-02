using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace mobil.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        public AuthService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> Login(Login loginData)
        {
            var response = await _http.PostAsJsonAsync(
                "login",
                loginData);
            if (!response.IsSuccessStatusCode)
                return null;
            var token = await response.Content.ReadAsStringAsync();
            return token.Trim('"');
        }
    }
}
