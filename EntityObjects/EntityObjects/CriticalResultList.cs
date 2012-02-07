using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	public class Utility
	{
		public const string ISO8601_FORMAT_STRING = "yyyy-MM-dd HH:mm:ss";
		public const string SHORT_FORMAT_STRING = "MM-dd hh:mm tt";
	}

	[DataContract]
	public class ResultViewEntry
	{
		private Guid _ResultUuid;
		[DataMember]
		public Guid ResultUuid
		{
			get { return _ResultUuid; }
			set { _ResultUuid = value; }
		}
		private string _ResultMessage;
		[DataMember]
		public string ResultMessage
		{
			get { return _ResultMessage; }
			set { _ResultMessage = value; }
		}
		private DateTime _CreationTime;
		[DataMember]
		public DateTime CreationTime
		{
			get { return _CreationTime; }
			set { _CreationTime = value; }
		}
		private string _SenderUserName;
		[DataMember]
		public string SenderUserName
		{
			get { return _SenderUserName; }
			set { _SenderUserName = value; }
		}
		private string _SenderName;
		[DataMember]
		public string SenderName
		{
			get { return _SenderName; }
			set { _SenderName = value; }
		}
		private string _ReceiverUserName;
		[DataMember]
		public string ReceiverUserName
		{
			get { return _ReceiverUserName; }
			set { _ReceiverUserName = value; }
		}
		private string _ReceiverName;
		[DataMember]
		public string ReceiverName
		{
			get { return _ReceiverName; }
			set { _ReceiverName = value; }
		}
		private string _ResultContextJson;
		[DataMember]
		public string ResultContextJson
		{
			get { return _ResultContextJson; }
			set { _ResultContextJson = value; }
		}
		private string _ResultContextXml;
		[DataMember]
		public string ResultContextXml
		{
			get { return _ResultContextXml; }
			set { _ResultContextXml = value; }
		}

		private Guid _LevelUuid;
		[DataMember]
		public Guid LevelUuid
		{
			get { return _LevelUuid; }
			set { _LevelUuid = value; }
		}
		private string _ContextTypeName;
		[DataMember]
		public string ContextTypeName
		{
			get { return _ContextTypeName; }
			set { _ContextTypeName = value; }
		}
		private string _ContextTypeUri;
		[DataMember]
		public string ContextTypeUri
		{
			get { return _ContextTypeUri; }
			set { _ContextTypeUri = value; }
		}
		private DateTime? _AcknowledgmentTime;
		[DataMember]
		public DateTime? AcknowledgmentTime
		{
			get { return _AcknowledgmentTime; }
			set { _AcknowledgmentTime = value; }
		}
		private DateTime? _EscalationTime;
		[DataMember]
		public DateTime? EscalationTime
		{
			get { return _EscalationTime; }
			set { _EscalationTime = value; }
		}
		private DateTime? _DueTime;
		[DataMember]
		public DateTime? DueTime
		{
			get { return _DueTime; }
			set { _DueTime = value; }
		}

		[DataMember]
		public string CreationTimeFormatted
		{
			get
			{
				if (_CreationTime != null)
					return _CreationTime.ToString(Utility.SHORT_FORMAT_STRING);
				return null;
			}
			set { }
		}


		[DataMember]
		public string AcknowledgmentTimeFormatted
		{
			get
			{
				if (_AcknowledgmentTime != null)
					return _AcknowledgmentTime.Value.ToString(Utility.SHORT_FORMAT_STRING);
				return null;
			}
			set { }
		}

		[DataMember]
		public string EscalationTimeFormatted
		{
			get 
			{
				if (_EscalationTime != null)
					return _EscalationTime.Value.ToString(Utility.SHORT_FORMAT_STRING);
				return null;
			}
			set { }
		}

		[DataMember]
		public string DueTimeFormatted
		{
			get
			{
				if (_DueTime != null)
					return _DueTime.Value.ToString(Utility.SHORT_FORMAT_STRING);
				return null;
			}
			set { }
		}

		[DataMember]
		public string EscalationTimeOffset
		{
			get 
			{
				if (_EscalationTime == null)
					return null;
				TimeSpan delta = _EscalationTime.Value - DateTime.Now;
				string prefix = "";
				if (delta < TimeSpan.Zero)
					prefix = "-";
				return string.Format("{2} {0}h {1}m", Math.Abs((int)delta.TotalHours), Math.Abs(delta.Minutes), prefix);
			}
			set { }
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

		public ResultViewEntry(){}

		/// <summary>
		/// Modified: 2010-02-25, Jeremy Richardson - fixed issue with Due Time not being set.
		/// </summary>
		/// <param name="e"></param>
		public ResultViewEntry(ResultListEntity e)
		{
			_ResultUuid = e.ResultUuid;
			_ResultMessage = e.ResultMessage;
			_CreationTime = e.CreationTime;
			_SenderUserName = e.SenderUserName;
			_SenderName = string.Format("{0} {1} {2}", e.SenderTitle, e.SenderFirstName, e.SenderLastName);
			_ReceiverUserName = e.ReceiverUserName;
			_ReceiverName = string.Format("{0} {1} {2}", e.ReceiverTitle, e.ReceiverFirstName, e.ReceiverLastName);
			_ResultContextJson = e.ResultContextJson;
			_ResultContextXml = e.ResultContextXml;
			_LevelUuid = e.LevelUuid;
			_ContextTypeName = e.ContextTypeName;
			_ContextTypeUri = e.ContextTypeUri;
			_AcknowledgmentTime = e.AcknowledgmentTime;
			_EscalationTime = e.EscalationTime;
			_DueTime = e.DueTime;
			_PatientKey = e.PatientKey;
			_ExamKey = e.ExamKey;
		}

	}
}
