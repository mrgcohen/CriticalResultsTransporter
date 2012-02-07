using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CriticalResults
{
	public class RadiologyContext
	{
		public class Entry
		{
			public string name { get; set; }
			public string displayName { get; set; }
			public string value { get; set; }
		}

		public Entry PatientName { get; set; }
		public Entry Accession { get; set; }
		public Entry MRN { get; set; }
		public Entry ExamDescription { get; set; }
		public Entry DOB { get; set; }
		public Entry ORG { get; set; }
	}
	public class SP2I2_AddExamAndPatientKeys
	{
		public static int Run()
		{
			CriticalResultsEntityManager manager = new CriticalResultsEntityManager();

			ResultEntity [] results = manager.QueryResultEntity("", null, null);
			
			foreach (ResultEntity result in results)
			{
				if (!result.ResultContexts.IsLoaded)
				{
					result.ResultContexts.Load();
				}
				if (result.ResultContexts.Count() != 1)
				{
					Console.Write("-yowza!-");
				}
				else
				{
					ResultContextEntity context = result.ResultContexts.First();
					if (string.IsNullOrEmpty(context.PatientKey) && string.IsNullOrEmpty(context.ExamKey))
					{
						RadiologyContext ctx = Newtonsoft.Json.JsonConvert.DeserializeObject<RadiologyContext>(result.ResultContexts.First().JsonValue);
						context.PatientKey = ctx.MRN.value;
						context.ExamKey = ctx.Accession.value;
						Console.Write("-change-");
					}
					else
					{
						Console.Write("-good-");
					}

				}
			}
			manager.ObjectContext.SaveChanges();
			return results.Count();
		}
	}
}
