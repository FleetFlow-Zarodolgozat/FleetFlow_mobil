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
            var response = await _http.PostAsJsonAsync("login-mobile", loginData);
            if (!response.IsSuccessStatusCode)
                return null;
            return (await response.Content.ReadAsStringAsync()).Trim('"');
        }

        public async Task ForgotPassword(ForgotPassword email)
        {
            await _http.PostAsJsonAsync("profile/forgot-password", email);
        }
    }
}
