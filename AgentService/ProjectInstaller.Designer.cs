namespace CriticalResults
{
	partial class ProjectInstaller
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._ServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this._ServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// _ServiceProcessInstaller
			// 
			this._ServiceProcessInstaller.Password = null;
			this._ServiceProcessInstaller.Username = null;
			// 
			// _ServiceInstaller
			// 
			this._ServiceInstaller.Description = "Agent Service for Critical Results";
			this._ServiceInstaller.DisplayName = "Critical Results Agent";
			this._ServiceInstaller.ServiceName = "Critical Results Agent";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this._ServiceProcessInstaller,
            this._ServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller _ServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller _ServiceInstaller;
	}
}