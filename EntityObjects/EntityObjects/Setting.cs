using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Setting
	{
		[DataMember]
		public string Uuid { get; set; }
		[DataMember]
		public string EntryKey { get; set; }
		[DataMember]
		public string Owner { get; set; }
		[DataMember]
		public string Value { get; set; }
		[DataMember]
		public string XmlValue { get; set; }

		public Setting() { }
		public Setting(SettingEntity e)
		{
			Uuid = e.Uuid.ToString();
			EntryKey = e.EntryKey;
			Owner = e.Owner;
			Value = e.Value;
			XmlValue = e.XmlValue;
		}
	}
}
