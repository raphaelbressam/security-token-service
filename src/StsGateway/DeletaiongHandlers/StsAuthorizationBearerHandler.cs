using StsGateway.Contracts;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace StsGateway.DeletaiongHandlers
{
    public class StsAuthorizationBearerHandler : DelegatingHandler
    {
        readonly IStsGateway _stsGateway;
        public StsAuthorizationBearerHandler(StsGateway stsGateway)
        {
            _stsGateway = stsGateway;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await AddBearerTokenHeader(request);

            return await base.SendAsync(request, cancellationToken);
        }

        private async Task AddBearerTokenHeader(HttpRequestMessage request)
        {
            string? token = await _stsGateway.GetAccessTokenAsync();
            if (token is null)
                throw new ArgumentNullException(nameof(token), "STS Token is null!");

            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
