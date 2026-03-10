using mobil.Models;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
                var originalStream = await data.File.OpenReadAsync();
                var compressedStream = await CompressImage(originalStream);
                var fileContent = new StreamContent(compressedStream);
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

        public async Task<bool> DeleteProfileImg()
        {
            var response = await _http.PatchAsync($"profile/delete-profile-image", null);
            return response.IsSuccessStatusCode;
        }

        private async Task<Stream> CompressImage(Stream originalStream)
        {
            using var bitmap = SKBitmap.Decode(originalStream);
            int maxWidth = 512;
            int newWidth = maxWidth;
            int newHeight = bitmap.Height * maxWidth / bitmap.Width;
            var resizedBitmap = bitmap.Resize(
                new SKImageInfo(newWidth, newHeight),
                SKFilterQuality.Medium);
            using var image = SKImage.FromBitmap(resizedBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 80);
            return new MemoryStream(data.ToArray());
        }
    }
}
