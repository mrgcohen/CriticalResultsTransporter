using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class ResultContext
	{
		private string _XmlValue;
		[DataMember]
		public string XmlValue
		{
			get { return _XmlValue; }
			set { _XmlValue = value; }
		}
		private string _JsonValue;
		[DataMember]
		public string JsonValue
		{
			get { return _JsonValue; }
			set { _JsonValue = value; }
		}

		private ContextType _ContextType = null;
		[DataMember]
		public ContextType ContextType
		{
			get { return _ContextType; }
			set { _ContextType = value; }
		}

		private string _PatientKey;
		[DataMember]
		public string PatientKey
		{
			get { return _PatientKey; }
			set { _PatientKey = value; }
		}
		private string _ExamKey;
		[DataMember]
		public string ExamKey
		{
			get { return _ExamKey; }
			set { _ExamKey = value; }
		}

		private ResultContextEntity _Entity;

		public ResultContext() { }

		public ResultContext(ResultContextEntity e)
		{
			_Entity = e;
			_JsonValue = e.JsonValue;
			_XmlValue = e.XmlValue;
			_PatientKey = e.PatientKey;
			_ExamKey = e.ExamKey;
		}
		public bool ResolveType()
		{
			if (_Entity == null)
				return false;
			if (_Entity.ContextType == null)
				return false;
			
			_ContextType = new ContextType(_Entity.ContextType);
			return true;
		}
	}
	[DataContract]
	public class ContextType
	{
		private Guid _Uuid;
		[DataMember]
		public Guid Uuid
		{
			get { return _Uuid; }
			set { _Uuid = value; }
		}
		private string _Name;
		[DataMember]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		private string _Description;
		[DataMember]
		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
		}
		private string _NamespaceUri;
		[DataMember]
		public string NamespaceUri
		{
			get { return _NamespaceUri; }
			set { _NamespaceUri = value; }
		}
		private string _XmlSchema;
		[DataMember]
		public string XmlSchema
		{
			get { return _XmlSchema; }
			set { _XmlSchema = value; }
		}
		private string _JsonTemplate;
		[DataMember]
		public string JsonTemplate
		{
			get { return _JsonTemplate; }
			set { _JsonTemplate = value; }
		}

		private ContextTypeEntity _Entity;

		public ContextType() { }

		public ContextType(ContextTypeEntity e)
		{
			_Entity = e;
			_Uuid = e.Uuid;
			_Name = e.Name;
			_NamespaceUri = e.NamespaceUri;
			_Description = e.Description;
			_XmlSchema = e.XmlSchema;
			_JsonTemplate = e.JsonTemplate;
		}
	}
}
