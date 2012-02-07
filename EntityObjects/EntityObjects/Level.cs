using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Level
	{
		private string _Name;
		[DataMember]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		private string _ShortDescription;
		[DataMember]
		public string ShortDescription
		{
			get { return _ShortDescription; }
			set { _ShortDescription = value; }
		}
		private string _Description;
		[DataMember]
		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
		}
		private string _ColorValue;
		[DataMember]
		public string ColorValue
		{
			get { return _ColorValue; }
			set { _ColorValue = value; }
		}
		
		private DateTime _DueTimeout;
		[DataMember]
		public DateTime DueTimeout
		{
			get { return _DueTimeout; }
			set { _DueTimeout = value; }
		}


		private DateTime _EscalationTimeout;
		[DataMember]
		public DateTime EscalationTimeout
		{
			get { return _EscalationTimeout; }
			set { _EscalationTimeout = value; }
		}

		private Guid _Uuid;
		[DataMember]
		public Guid Uuid
		{
			get { return _Uuid; }
			set { _Uuid = value; }
		}


		private List<Result> _Results;
		[DataMember]
		public Result [] Results
		{
			get 
			{
				if (_Results != null)
					return _Results.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_Results = new List<Result>(value);
			}
		}
		private List<Transport> _Transports;
		[DataMember]
		public Transport[] Transports
		{
			get
			{
				if (_Transports != null)
					return _Transports.ToArray();
				return null;
			}
			set
			{
				if (value != null)
					_Transports = new List<Transport>(value);
			}
		}

		private List<Transport> _MandatoryTransports;
		[DataMember]
		public Transport[] MandatoryTransports
		{
			get
			{
				if (_MandatoryTransports != null)
					return _MandatoryTransports.ToArray();
				return null;
			}
			set
			{
				if (value != null)
					_MandatoryTransports = new List<Transport>(value);
			}
		}

		private List<UserTransport> _UserTransports;
		[DataMember]
		public UserTransport[] UserTransports
		{
			get 
			{
				if (_UserTransports != null)
					return _UserTransports.ToArray();
				return null;
			}
			set 
			{
				if (value != null)
					_UserTransports = new List<UserTransport>(value);
			}
		}

		private bool _DirectContactRequired;
		[DataMember]
		public bool DirectContactRequired
		{
			get { return _DirectContactRequired; }
			set { _DirectContactRequired = value; }
		}
				
		private LevelEntity _Entity;

		public Level() { }

		public Level(LevelEntity e)
		{
			_Entity = e;
			_Name = e.Name;
			_ShortDescription = e.ShortDescription;
			_Description = e.Description;
			_ColorValue = e.ColorValue;
			_EscalationTimeout = e.EscalationTimeout;
			_DueTimeout = e.DueTimeout;
			_Uuid = e.Uuid;
			_DirectContactRequired = e.DirectContactRequired;
		}

		public bool ResolveResults()
		{
			if (_Entity == null)
				return false;
			if (_Entity.Results == null)
				return false;
			
			_Results = new List<Result>();
			foreach (ResultEntity entity in _Entity.Results)
			{
				Result r = new Result(entity);
				_Results.Add(r);
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Mandatory flag was added to TransportLevelEntity.  This change has been propagated by adding a bool? Mandatory flag to the Transport and Level.
		///		The mandatory flag from the TransportLevelEntity is applied to either of these objects to hide the changes to the Entities and avoid changing the object model.
		///		
		/// Change: 2009-06-01, John M., Jeremy R.
		///		Mandatory flag removed from Level.cs and replaced with _MandatoryTransports List.
		/// </remarks>
		public bool ResolveTransports()
		{
			if (_Entity == null)
				return false;
			if (_Entity.TransportLevels == null)
				return false;
			_Transports = new List<Transport>();
			_MandatoryTransports = new List<Transport>();
			foreach (TransportLevelEntity e in _Entity.TransportLevels)
			{
				Transport t = new Transport(e.Transport);
				if (e.Mandatory)
				{
					_MandatoryTransports.Add(t);
				}
				_Transports.Add(t);
			}
			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Fixed method, where Transports were being resolved instead of UserTransports
		/// </remarks>
		public bool ResolveUserTransports()
		{
			if (_Entity == null)
				return false;
			if (_Entity.UserTransports == null)
				return false;

			_UserTransports = new List<UserTransport>();
			foreach (UserTransportEntity e in _Entity.UserTransports)
			{
				UserTransport t = new UserTransport(e);
				_UserTransports.Add(t);
			}
			return true;
		}
	}
}
