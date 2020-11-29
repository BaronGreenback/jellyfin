#nullable enable
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace MediaBrowser.Model.Dlna
{
    /// <summary>
    /// Defines the <see cref="DeviceIdentification" />.
    /// </summary>
    public class DeviceIdentification
    {
        /// <summary>
        /// Gets or sets the name of the friendly.
        /// </summary>
        public string FriendlyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model number.
        /// </summary>
        public string ModelNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        public string SerialNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the model.
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model description.
        /// </summary>
        public string ModelDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the model URL.
        /// </summary>
        public string ModelUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Manufacturer.
        /// </summary>
        public string Manufacturer { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the manufacturer URL.
        /// </summary>
        public string ManufacturerUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Headers.
        /// </summary>
        public HttpHeaderInfo[] Headers { get; set; } = Array.Empty<HttpHeaderInfo>();

        /// <summary>
        /// Compares this item with <paramref name="profileInfo"/>.
        /// </summary>
        /// <param name="profileInfo">The <see cref="DeviceProfile"/>.</param>
        /// <returns><c>True</c> if they match.</returns>
        public bool IsMatch(DeviceIdentification profileInfo)
        {
            if (profileInfo == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(profileInfo.FriendlyName))
            {
                if (FriendlyName == null || !IsRegexOrSubstringMatch(FriendlyName, profileInfo.FriendlyName))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.Manufacturer))
            {
                if (Manufacturer == null || !IsRegexOrSubstringMatch(Manufacturer, profileInfo.Manufacturer))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.ManufacturerUrl))
            {
                if (ManufacturerUrl == null || !IsRegexOrSubstringMatch(ManufacturerUrl, profileInfo.ManufacturerUrl))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.ModelDescription))
            {
                if (ModelDescription == null || !IsRegexOrSubstringMatch(ModelDescription, profileInfo.ModelDescription))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.ModelName))
            {
                if (ModelName == null || !IsRegexOrSubstringMatch(ModelName, profileInfo.ModelName))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.ModelNumber))
            {
                if (ModelNumber == null || !IsRegexOrSubstringMatch(ModelNumber, profileInfo.ModelNumber))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.ModelUrl))
            {
                if (ModelUrl == null || !IsRegexOrSubstringMatch(ModelUrl, profileInfo.ModelUrl))
                {
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(profileInfo.SerialNumber))
            {
                if (SerialNumber == null || !IsRegexOrSubstringMatch(SerialNumber, profileInfo.SerialNumber))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns details of this item.
        /// </summary>
        /// <returns>The device details as a readable string.</returns>
        public string GetDetails()
        {
            var builder = new StringBuilder();
            builder.AppendLine("No matching device profile found. The default will need to be used.");
            builder.Append("FriendlyName:").AppendLine(FriendlyName);
            builder.Append("Manufacturer:").AppendLine(Manufacturer);
            builder.Append("ManufacturerUrl:").AppendLine(ManufacturerUrl);
            builder.Append("ModelDescription:").AppendLine(ModelDescription);
            builder.Append("ModelName:").AppendLine(ModelName);
            builder.Append("ModelNumber:").AppendLine(ModelNumber);
            builder.Append("ModelUrl:").AppendLine(ModelUrl);
            builder.Append("SerialNumber:").AppendLine(SerialNumber);
            return builder.ToString();
        }

        private static bool IsRegexOrSubstringMatch(string input, string pattern)
        {
            try
            {
                return input.Contains(pattern, StringComparison.OrdinalIgnoreCase)
                    || Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }
            catch (ArgumentException)
            {
                // _logger.LogError(ex, "Error evaluating regex pattern {Pattern}", pattern);
                return false;
            }
        }
    }
}
