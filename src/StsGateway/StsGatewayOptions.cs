using System;

namespace StsGateway
{
    public class StsGatewayOptions
    {
        public Uri? RequestUri { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? GrantType { get; set; }
        public CacheTypeEnum CacheType { get; set; } = StsGatewayOptions.CacheTypeEnum.None;

        public void ThrowIfPropertiesNull()
        {
            if (RequestUri == null) throw new ArgumentNullException(nameof(RequestUri));
            if (string.IsNullOrEmpty(ClientId)) throw new ArgumentNullException(nameof(ClientId));
            if (string.IsNullOrEmpty(ClientSecret)) throw new ArgumentNullException(nameof(ClientSecret));
            if (string.IsNullOrEmpty(GrantType)) throw new ArgumentNullException(nameof(GrantType));
        }

        public enum CacheTypeEnum
        {
            None = 0,
            MemoryCache = 1
        }
    }
}