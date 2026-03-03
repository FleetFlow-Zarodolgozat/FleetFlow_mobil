using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.Services
{
    public class SessionService
    {
        public async Task SaveToken(string token)
        {
            await SecureStorage.SetAsync("bearer_token", token);
        }

        public async Task<string?> GetToken()
        {
            return await SecureStorage.GetAsync("bearer_token");
        }

        public void Logout()
        {
            SecureStorage.Remove("bearer_token");
        }
    }
}
