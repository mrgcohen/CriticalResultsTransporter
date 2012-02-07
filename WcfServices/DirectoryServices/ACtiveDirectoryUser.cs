using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

using System.Xml;
using System.Xml.Serialization;
using System.IO;

///Modified: 2010-06-04 - John Morgan
///Added additional data members to serialize only required subset of active directory
namespace CriticalResults
{
	[DataContract]
	public class ActiveDirectoryUser
	{
		List<KeyValuePair<string, string>>  _Properties;

		[DataMember]
		public KeyValuePair<string, string> [] Properties
		{
			get { return _Properties.ToArray(); }
			set { _Properties = new List<KeyValuePair<string, string>>(value); }
		}

		[DataMember]
		public string cn { get; set; }
		[DataMember]
		public string sn { get; set; }
		[DataMember]
		public string givenName { get; set; }
		[DataMember]
		public string mail { get; set; }
		[DataMember]
		public string[] proxyAddresses { get; set; }
		[DataMember]
		public string ANCRAccountUserName { get; set; }
		[DataMember]
		public string displayName { get; set; }
		[DataMember]
		public bool Enabled { get; set; }

		public ActiveDirectoryUser() 
		{
			_Properties = new List<KeyValuePair<string, string>>();
		}

		public string GetProperty(string key)
		{
			foreach (KeyValuePair<string, string> pair in _Properties)
			{
				if (pair.Key == key)
					return pair.Value;
			}
			return null;
		}

		public static ActiveDirectoryUser[] GetUsersFromXml(string xmlString)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ActiveDirectoryUser[]));
			object o = serializer.Deserialize(new StringReader(xmlString));
			return (ActiveDirectoryUser[])o;
		}
		public static string WriteUsersToXml(ActiveDirectoryUser [] users)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(ActiveDirectoryUser[]));
			StringWriter sw = new StringWriter();
			serializer.Serialize(sw, users);
			return sw.ToString();
		}

		public void AddProperty(string propertyName, string propertyValue)
		{
			_Properties.Add(new KeyValuePair<string,string>(propertyName, propertyValue));
		}
		public KeyValuePair<string,string>[] GetProperties(string propertyName)
		{
			List<KeyValuePair<string, string>> matches = new List<KeyValuePair<string, string>>();
			
			foreach (KeyValuePair<string, string> property in _Properties)
			{
				if (property.Key == propertyName)
					matches.Add(property);
			}
			return matches.ToArray();
		}

		[XmlRoot("Properties")]
		public class PropertyDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
		{
			#region IXmlSerializable Members
			public System.Xml.Schema.XmlSchema GetSchema()
			{
				return null;
			}

			public const string PROPERTY_ELEMENT_STRING = "Property";
			public const string NAME_ELEMENT_STRING = "Name";
			public const string VALUE_ELEMENT_STRING = "Value";

			public void ReadXml(System.Xml.XmlReader reader)
			{
				XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
				XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

				bool wasEmpty = reader.IsEmptyElement;
				reader.Read();
				if (wasEmpty)
					return;

				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					reader.ReadStartElement(PROPERTY_ELEMENT_STRING);
					reader.ReadStartElement(NAME_ELEMENT_STRING);
					TKey propertyName = (TKey)keySerializer.Deserialize(reader);
					reader.ReadEndElement();
					reader.ReadStartElement(VALUE_ELEMENT_STRING);
					TValue propertyValue = (TValue)valueSerializer.Deserialize(reader);
					reader.ReadEndElement();
					this.Add(propertyName, propertyValue);
					reader.ReadEndElement();
					reader.MoveToContent();
				}
				reader.ReadEndElement();
			}

			public void WriteXml(System.Xml.XmlWriter writer)
			{
				XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
				XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

				foreach (TKey propertyName in this.Keys)
				{
					writer.WriteStartElement(PROPERTY_ELEMENT_STRING);
					writer.WriteStartElement(NAME_ELEMENT_STRING);
					keySerializer.Serialize(writer, propertyName);
					writer.WriteEndElement();
					writer.WriteStartElement(VALUE_ELEMENT_STRING);
					TValue propertyValue = this[propertyName];
					valueSerializer.Serialize(writer, propertyValue);
					writer.WriteEndElement();
					writer.WriteEndElement();
				}
			}
			#endregion
		}

	}
}
