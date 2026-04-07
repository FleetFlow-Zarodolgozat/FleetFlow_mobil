using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.Services
{
    public class SessionService
    {
        private string? _pendingLoginError;

        public async Task SaveToken(string token)
        {
            await SecureStorage.SetAsync("bearer_token", token);
        }

        public async Task<string?> GetToken()
        {
            return await SecureStorage.GetAsync("bearer_token");
        }

        public void SetPendingLoginError(string message)
        {
            _pendingLoginError = message;
        }

        public string? ConsumePendingLoginError()
        {
            var message = _pendingLoginError;
            _pendingLoginError = null;
            return message;
        }

        public void Logout()
        {
            SecureStorage.Remove("bearer_token");
        }
    }
}
