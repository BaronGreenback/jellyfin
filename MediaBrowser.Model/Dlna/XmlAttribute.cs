using System.Xml.Serialization;

namespace MediaBrowser.Model.Dlna
{
    /// <summary>
    /// Defines the <see cref="XmlAttribute" />.
    /// </summary>
    public class XmlAttribute
    {
        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        [XmlAttribute("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        [XmlAttribute("value")]
        public string Value { get; set; } = string.Empty;
    }
}
