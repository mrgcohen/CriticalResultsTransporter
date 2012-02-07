using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mobile
{
	public class RadiologyContext
	{
		public PatientName PatientName { get; set; }
		public DOB DOB { get; set; }
		public ORG ORG { get; set; }
		public MRN MRN { get; set; }
		public Accession Accession { get; set; }
		public ExamDescription ExamDescription { get; set; }
		public ExamTime ExamTime { get; set; }
        public ExamDate ExamDate { get; set; }
	}

	public class PatientName
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class DOB
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class ORG
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class MRN
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class Accession
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class ExamDescription
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

	public class ExamTime
	{
		public string name { get; set; }
		public string value { get; set; }
		public string displayName { get; set; }
	}

    public class ExamDate
    {
        public string name { get; set; }
        public string value { get; set; }
        public string displayName { get; set; }
    }
}
