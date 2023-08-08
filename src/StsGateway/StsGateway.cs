using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using StsGateway.Contracts;
using StsGateway.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StsGateway
{
    public class StsGateway : IStsGateway
    {
        const string _OAuth2StsGatewayCache = "OAuth2StsGatewayCacheObject";
        readonly IMemoryCache? _memoryCache;
        readonly StsGatewayOptions _options;

        public StsGateway(IOptions<StsGatewayOptions> options, IMemoryCache? memoryCache = null)
        {
            if (options?.Value is null)
                throw new ArgumentException("Options is required!", nameof(options.Value));

            options.Value.ThrowIfPropertiesNull();

            _options = options.Value;
            _memoryCache = memoryCache;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            if (TryGetOAuth2EntityFromCache(out var cachedEntity))
                return cachedEntity!.AccessToken;

            using HttpClient httpClient = new HttpClient();

            FormUrlEncodedContent requestContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", _options.GrantType! },
                { "client_id", _options.ClientId! },
                { "client_secret", _options.ClientSecret! }
            });

            var response = await httpClient.PostAsync(_options.RequestUri, requestContent);

            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            var oAuth2Entity = OAuth2Entity.FromJsonString(responseContent);

            if (oAuth2Entity is null)
                throw new ArgumentNullException(responseContent, nameof(oAuth2Entity));

            SetOAuth2EntityCache(oAuth2Entity);

            return oAuth2Entity.AccessToken;
        }

        private bool TryGetOAuth2EntityFromCache(out OAuth2Entity? entity)
        {
            entity = default;

            switch (_options.CacheType)
            {
                case StsGatewayOptions.CacheTypeEnum.MemoryCache:
                    if (_memoryCache == null)
                        throw new ArgumentNullException(nameof(_memoryCache), $"{nameof(_memoryCache)} is null");

                    if (_memoryCache.TryGetValue(_OAuth2StsGatewayCache, out OAuth2Entity? cachedObject) && cachedObject != null)
                        entity = cachedObject;

                    break;
            }

            return entity != null;
        }

        private void SetOAuth2EntityCache(OAuth2Entity entity)
        {
            switch (_options.CacheType)
            {
                case StsGatewayOptions.CacheTypeEnum.MemoryCache:
                    if (_memoryCache == null)
                        throw new ArgumentNullException(nameof(_memoryCache), $"{nameof(_memoryCache)} is null");

                    _memoryCache.Set(_OAuth2StsGatewayCache, entity, new DateTimeOffset(DateTime.UtcNow).AddSeconds(entity.ExpiresIn - 5));
                    break;
            }
        }
    }
}