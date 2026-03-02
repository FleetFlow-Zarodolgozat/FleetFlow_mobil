using mobil.Services;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace mobil.Handlers
{
    public class AuthHttpHandler : DelegatingHandler
    {
        private readonly SessionService _session;
        public AuthHttpHandler(SessionService session)
        {
            _session = session;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _session.GetToken();
            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
