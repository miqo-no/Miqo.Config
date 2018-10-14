using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Miqo.Config.Formatters;

namespace Miqo.Config.Tests
{
    /*
     * Here be dragons.
     */
    public class XmlConfigurationFormatter : IConfigurationFormatter
    {
        private readonly Type _t;

        public XmlConfigurationFormatter(Type t)
        {
            _t = t;
        }

        public T Parse<T>(string data) where T : new()
        {
            if (data.IsNull())
                throw new ArgumentNullException(nameof(data));

            var serializer = new XmlSerializer(_t);
            var reader = new StringReader(data);
            return (T)serializer.Deserialize(reader);
        }

        public string Serialize(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var serializer = new XmlSerializer(_t);
            var settings = new XmlWriterSettings
            {
                Encoding = new UnicodeEncoding(false, false),
                Indent = true,
                OmitXmlDeclaration = true
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var writer = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(writer, settings))
                {
                    serializer.Serialize(xmlWriter, data, ns);
                }
                return writer.ToString();
            }
        }
    }
}
