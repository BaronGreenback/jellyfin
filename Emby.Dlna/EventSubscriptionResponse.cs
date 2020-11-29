using System.Collections.Generic;

namespace Emby.Dlna
{
    /// <summary>
    /// Defines the <see cref="EventSubscriptionResponse" />.
    /// </summary>
    public class EventSubscriptionResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSubscriptionResponse"/> class.
        /// </summary>
        public EventSubscriptionResponse()
        {
            Headers = new Dictionary<string, string>();
            Content = string.Empty;
            ContentType = string.Empty;
        }

        /// <summary>
        /// Gets or sets the subscription response content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the subscription response contentType.
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets the subscription response headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; }
    }
}
