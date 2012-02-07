using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CriticalResults
{
	[DataContract]
	public class Transport
	{
		private string _Name;
		[DataMember]
		public string Name
		{
			get { return _Name; }
			set { _Name = value; }
		}
		private string _FriendlyName;
		[DataMember]
		public string FriendlyName
		{
			get { return _FriendlyName; }
			set { _FriendlyName = value; }
		}
		private string _TransportUri;
		[DataMember]
		public string TransportUri
		{
			get { return _TransportUri; }
			set { _TransportUri = value; }
		}

		private List<Level> _Levels;
		[DataMember]
		public Level [] Levels
		{
			get
			{
				if (_Levels != null)
					return _Levels.ToArray();
				return null;
			}
			set { if (value != null) { _Levels = new List<Level>(value); } }
		}

		private List<Level> _MandatoryLevels;
		[DataMember]
		public Level[] MandatoryLevels
		{
			get
			{
				if (_MandatoryLevels != null)
					return _MandatoryLevels.ToArray();
				return null;
			}
			set { if (value != null) { _MandatoryLevels = new List<Level>(value); } }
		}

		private List<UserTransport> _UserTransports;
		[DataMember]
		public UserTransport [] UserTransports
		{
			get
			{
				if (_UserTransports != null)
					return _UserTransports.ToArray();
				return null;
			}
			set { if (value != null) { _UserTransports = new List<UserTransport>(value); } }
		}

		private TransportEntity _Entity;

		public Transport() { }

		public Transport(TransportEntity e)
		{
			_Entity = e;
			Name = e.Name;
			TransportUri = e.TransportURI;
			FriendlyName = e.FriendlyAddressName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <remarks>
		/// Change: 2009-05-31, Jeremy R.
		///		Mandatory flag was added to TransportLevelEntity.  This change has been propagated by adding a bool? Mandatory flag to the Transport and Level.
		///		The mandatory flag from the TransportLevelEntity is applied to either of these objects to hide the changes to the Entities and avoid changing the object model.
		///	Change: 2009-06-01, John C.
		///		Manadaroty flag removed and replaced with _MandatoryLevels array.
		/// </remarks>
		public bool ResolveLevels()
		{
			if (_Entity == null)
				return false;
			if (_Entity.TransportLevels == null)
				return false;

			_Levels = new List<Level>();
			_MandatoryLevels = new List<Level>();
			foreach (TransportLevelEntity e in _Entity.TransportLevels)
			{
				Level level = new Level(e.Level);
				if (e.Mandatory)
				{
					_MandatoryLevels.Add(level);
				}
				_Levels.Add(level);
			}
			return true;
		}
		public bool ResolveUserTransports()
		{

			if (_Entity.UserTransports != null)
			{
				_UserTransports = new List<UserTransport>();
				foreach (UserTransportEntity e in _Entity.UserTransports)
				{
					UserTransport t = new UserTransport(e);
					_UserTransports.Add(t);
				}
			}
			else
			{
				return false;
			}
			return true;
		}


	}
}
