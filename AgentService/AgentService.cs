using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace CriticalResults
{
	public partial class AgentService : ServiceBase
	{
		HybridAgent _Agent;
		
		public AgentService()
		{
			InitializeComponent();
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
		}

		/// <summary>
		/// Added to make sure the service terminates on an unhandled exception.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Trace.WriteLine(string.Format("Terminating: Unhandled Exception\r\nExiting{1}\r\nException Follows\r\n{0}", e.ExceptionObject.ToString(), e.IsTerminating));
			if (!e.IsTerminating)
			{
				System.Environment.Exit(-1);
			}
		}

		protected override void OnStart(string[] args)
		{
			_Agent = new HybridAgent();
		}

		protected override void OnStop()
		{
			_Agent = null;
		}
	}
}
