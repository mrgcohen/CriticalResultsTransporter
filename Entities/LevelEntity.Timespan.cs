using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CriticalResults
{
    public partial class LevelEntity
    {
        public TimeSpan EscalationTimespan
        {
            get
            {
                TimeSpan ts = EscalationTimeout - new DateTime(1900, 1, 1);
                return ts;
            }
            set
            {
                EscalationTimeout = new DateTime(1900, 1, 1).Add(value);
            }
        }
		public TimeSpan DueTimespan
		{
			get
			{
				TimeSpan ts = DueTimeout - new DateTime(1900, 1, 1);
				return ts;
			}
			set
			{
				DueTimeout = new DateTime(1900, 1, 1).Add(value);
			}
		}
    }
}
