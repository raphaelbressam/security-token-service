using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StsGateway.Contracts;
using StsGateway.Enums;
using StsGateway.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace StsGateway
{
    public class StsGateway : IStsGateway
    {
        private const string _OAuth2StsGatewayCache = "OAuth2StsGatewayCacheObject";
        private readonly IMemoryCache? _memoryCache;
        private readonly StsGatewayOptions _options;
        private readonly ILogger? _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public StsGateway(
            IOptions<StsGatewayOptions> options,
            IHttpClientFactory httpClientFactory,
            IMemoryCache? memoryCache = null,
            ILogger<StsGateway>? logger = null
            )
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "Options is required!");
            _options.ValidateProperties();

            _memoryCache = memoryCache;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string?> GetAccessTokenAsync()
        {
            LogInformation("Trying to get the access token.");

            if (TryGetOAuth2EntityFromCache(out var cachedEntity))
                return cachedEntity!.AccessToken;

            using HttpClient httpClient = _httpClientFactory.CreateClient("StsGateway");
            var requestContent = new Dictionary<string, string>
            {
                { "grant_type", _options.GrantType! },
                { "client_id", _options.ClientId! },
                { "client_secret", _options.ClientSecret! }
            };
            LogDebug("grant_type:{_options.GrantType} client_id:{_options.ClientId} client_secret:{_options.ClientSecret}", _options.GrantType, _options.ClientId, _options.ClientSecret);

            var response = await httpClient.PostAsync(_options.RequestUri, new FormUrlEncodedContent(requestContent));
            LogInformation("RequestUri:{RequestMessage.RequestUri} StatusCode:{StatusCode}", response.RequestMessage.RequestUri.ToString(), response.StatusCode);

            if (!response.IsSuccessStatusCode)
                return default;

            var responseContent = await response.Content.ReadAsStringAsync();
            var oAuth2Entity = OAuth2Entity.FromJsonString(responseContent) ?? throw new ArgumentNullException(responseContent, nameof(OAuth2Entity));

            if (!oAuth2Entity.IsValid())
            {
                LogInformation("The OAuth object is not valid!", nameof(oAuth2Entity));
                return default;
            }

            SetOAuth2EntityCache(oAuth2Entity);

            return oAuth2Entity.AccessToken;
        }

        private bool TryGetOAuth2EntityFromCache(out OAuth2Entity? entity)
        {
            LogInformation("Checking the cache.");
            entity = null;

            if (_options.CacheType != CacheTypeEnum.MemoryCache)
                return false;

            if (_memoryCache == null)
            {
                LogInformation($"{nameof(_memoryCache)} is null", nameof(_memoryCache));
                return false;
            }

            if (_memoryCache.TryGetValue<OAuth2Entity>(_OAuth2StsGatewayCache, out var cachedObject) && cachedObject != null)
            {
                LogInformation(cachedObject.IsValid() ? "Cache found and is valid" : "Cache found but is not valid");
                entity = cachedObject;
            }
            else
            {
                LogInformation("Cache not found");
            }

            return entity != null;
        }

        private void SetOAuth2EntityCache(OAuth2Entity entity)
        {
            LogInformation("Set OAuth object in the cache.");

            if (!entity.IsValid())
            {
                LogInformation("The OAuth object is not valid!!!");
                return;
            }

            var expiresAt = new DateTimeOffset(DateTime.UtcNow).AddSeconds(entity.ExpiresIn - 5);
            if (_options.CacheType == CacheTypeEnum.MemoryCache)
            {
                if (_memoryCache == null)
                {
                    LogInformation($"{nameof(_memoryCache)} is null", nameof(_memoryCache));
                    return;
                }

                _memoryCache.Set(_OAuth2StsGatewayCache, entity, expiresAt);
                LogInformation("Selected CacheType: MemoryCache. Set OAuth object in the cache completed. Expires At: {expiresAt}", expiresAt.ToString("s"));
            }
        }

        private void LogInformation(string message, params object[] args)
        {
            _logger?.LogInformation(message, args);
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger?.LogDebug(message, args);
        }
    }
}