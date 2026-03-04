using mobil.Models;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
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
            var content = new MultipartFormDataContent();
            if (!string.IsNullOrWhiteSpace(data.FullName))
                content.Add(new StringContent(data.FullName), "fullName");
            if (!string.IsNullOrWhiteSpace(data.Phone))
                content.Add(new StringContent(data.Phone), "phone");
            if (!string.IsNullOrWhiteSpace(data.Password))
                content.Add(new StringContent(data.Password), "password");
            if (!string.IsNullOrWhiteSpace(data.PasswordAgain))
                content.Add(new StringContent(data.PasswordAgain), "passwordAgain");
            if (data.File != null)
            {
                var stream = await data.File.OpenReadAsync();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(
                    fileContent,
                    "file",
                    data.File.FileName
                );
            }
            var response = await _http.PatchAsync("profile/edit", content);
            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(body) ? $"Error {(int)response.StatusCode}" : body;
            }
            return null;
        }

        public async Task<bool> DeleteProfileImg(ulong id)
        {
            var response = await _http.DeleteAsync($"files/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
