using System;

namespace StsGateway
{
    public class StsGatewayOptions
    {
        public Uri? RequestUri { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string GrantType { get; set; } = string.Empty;
        public Enums.CacheTypeEnum CacheType { get; set; } = Enums.CacheTypeEnum.None;

        /// <summary>
        /// Throws an ArgumentNullException if any of the required properties are null or whitespace.
        /// </summary>
        public void ValidateProperties()
        {
            ThrowIfPropertyNullOrWhitespace(RequestUri?.ToString() ?? string.Empty, nameof(RequestUri));
            ThrowIfPropertyNullOrWhitespace(ClientId, nameof(ClientId));
            ThrowIfPropertyNullOrWhitespace(ClientSecret, nameof(ClientSecret));
            ThrowIfPropertyNullOrWhitespace(GrantType, nameof(GrantType));
        }

        private static void ThrowIfPropertyNullOrWhitespace(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(paramName);
        }


    }

}