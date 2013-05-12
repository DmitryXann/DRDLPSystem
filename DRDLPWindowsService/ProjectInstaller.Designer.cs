namespace DRDLPWindowsService
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
			this.DRDLPWindowsServiceServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			this.DRDLPWindowsServiceInstaller = new System.ServiceProcess.ServiceInstaller();
			// 
			// DRDLPWindowsServiceServiceProcessInstaller
			// 
			this.DRDLPWindowsServiceServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.DRDLPWindowsServiceServiceProcessInstaller.Password = null;
			this.DRDLPWindowsServiceServiceProcessInstaller.Username = null;
			// 
			// DRDLPWindowsServiceInstaller
			// 
			this.DRDLPWindowsServiceInstaller.Description = "DRDLPWindowsService";
			this.DRDLPWindowsServiceInstaller.DisplayName = "DRDLPWindowsService";
			this.DRDLPWindowsServiceInstaller.ServiceName = "DRDLPService";
			// 
			// ProjectInstaller
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.DRDLPWindowsServiceServiceProcessInstaller,
            this.DRDLPWindowsServiceInstaller});

		}

		#endregion

		private System.ServiceProcess.ServiceInstaller DRDLPWindowsServiceInstaller;
		public System.ServiceProcess.ServiceProcessInstaller DRDLPWindowsServiceServiceProcessInstaller;
	}
}