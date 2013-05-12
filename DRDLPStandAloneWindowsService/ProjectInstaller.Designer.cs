namespace DRDLPStandAloneWindowsService
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
			this.DRDLPServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.DRDLPServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// DRDLPServiceProcessInstaller
			// 
			this.DRDLPServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.DRDLPServiceProcessInstaller.Password = null;
			this.DRDLPServiceProcessInstaller.Username = null;
			// 
			// DRDLPServiceInstaller
			// 
			this.DRDLPServiceInstaller.Description = "DRDLPService";
			this.DRDLPServiceInstaller.DisplayName = "DRDLPService";
			this.DRDLPServiceInstaller.ServiceName = "DRDLPService";
			this.DRDLPServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DRDLPServiceProcessInstaller,
            this.DRDLPServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceProcessInstaller DRDLPServiceProcessInstaller;
		private System.ServiceProcess.ServiceInstaller DRDLPServiceInstaller;
	}
}