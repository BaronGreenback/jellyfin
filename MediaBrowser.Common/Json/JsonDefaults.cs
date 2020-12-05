using System.Text.Json;
using System.Text.Json.Serialization;
using MediaBrowser.Common.Json.Converters;

namespace MediaBrowser.Common.Json
{
    /// <summary>
    /// Helper class for having compatible JSON throughout the codebase.
    /// </summary>
    public static class JsonDefaults
    {
        /// <summary>
        /// Pascal case json profile media type.
        /// </summary>
        public const string PascalCaseMediaType = "application/json; profile=\"PascalCase\"";

        /// <summary>
        /// Camel case json profile media type.
        /// </summary>
        public const string CamelCaseMediaType = "application/json; profile=\"CamelCase\"";

        /// <summary>
        /// Gets the default <see cref="JsonSerializerOptions" /> options.
        /// </summary>
        /// <remarks>
        /// When changing these options, update
        ///     Jellyfin.Server/Extensions/ApiServiceCollectionExtensions.cs
        ///         -> AddJellyfinApi
        ///             -> AddJsonOptions.
        /// </remarks>
        /// <returns>The default <see cref="JsonSerializerOptions" /> options.</returns>
        public static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Disallow,
                WriteIndented = false,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };

            options.Converters.Add(new JsonGuidConverter());
            options.Converters.Add(new JsonVersionConverter());
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new JsonNullableStructConverterFactory());

            return options;
        }

        /// <summary>
        /// Gets camelCase json options.
        /// </summary>
        /// <returns>The camelCase <see cref="JsonSerializerOptions" /> options.</returns>
        public static JsonSerializerOptions GetCamelCaseOptions()
        {
            var options = GetOptions();
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            return options;
        }

        /// <summary>
        /// Gets PascalCase json options.
        /// </summary>
        /// <returns>The PascalCase <see cref="JsonSerializerOptions" /> options.</returns>
        public static JsonSerializerOptions GetPascalCaseOptions()
        {
            var options = GetOptions();
            options.PropertyNamingPolicy = null;
            return options;
        }
    }
}
