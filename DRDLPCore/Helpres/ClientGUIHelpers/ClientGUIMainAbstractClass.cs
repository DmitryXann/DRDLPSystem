using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using DRDLPCore.FileTranfsormation;
using DRDLPCore.Registry;

namespace DRDLPCore.Helpres.ClientGUIHelpers
{
	public abstract class ClientGUIMainAbstractClass
	{
		protected const char ICON_CONTAINER_ICON_NUMBER_SEPARATOR = ',';
		protected const string ICON_DEFAULT_EXTINGTION = ".ico";

		[DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		protected static extern int ExtractIconEx(string fullFilePath, int iconIndex, out IntPtr largeIconVersion, out IntPtr smallIconVersion, uint iconsQuantity);

		protected static Icon ExtractIconFromResource(string fullFilePath, int iconIndex, bool useLargeIcon = true)
		{
			IntPtr largeIcon;
			IntPtr smallIcon;

			ExtractIconEx(fullFilePath, iconIndex, out largeIcon, out smallIcon, 1);

			try
			{
				return Icon.FromHandle(useLargeIcon ? largeIcon : smallIcon);
			}
			catch
			{
				return null;
			}
		}

		public virtual void ShowBalloonTip(string fullPathToFile, int ballonTimeOut = 400, uint timerTimeOut = 10000,
			string ballonTipText = @"Please wait while file opening procedure is finished, it may take awhile...", string ballonTipTitle = @"File is opening")
		{
			if (!File.Exists(fullPathToFile))
				return;

			var notifyIcon = new NotifyIcon
			{
				BalloonTipIcon = ToolTipIcon.Info,
				BalloonTipText = ballonTipText,
				BalloonTipTitle = ballonTipTitle,
				Visible = true
			};


			var fileExtingtion = Path.GetExtension(fullPathToFile);
			var selectedImagePath = RegistryWork.GetDefaultAssociationImage(fileExtingtion);

			if (!string.IsNullOrEmpty(selectedImagePath))
			{
				var iconResourceExtingtion = Path.GetExtension(selectedImagePath);

				if ((iconResourceExtingtion != null) && (iconResourceExtingtion.ToLower() == ICON_DEFAULT_EXTINGTION))
					notifyIcon.Icon = new Icon(selectedImagePath);
				else
				{
					var lastIndexOfIconContainerIconSeparator = selectedImagePath.LastIndexOf(ICON_CONTAINER_ICON_NUMBER_SEPARATOR);
					int iconIndex;

					if ((selectedImagePath.LastIndexOf('.') < lastIndexOfIconContainerIconSeparator)
						&& int.TryParse(selectedImagePath.Substring(lastIndexOfIconContainerIconSeparator + 1), out iconIndex))
						notifyIcon.Icon =
							ExtractIconFromResource(
								selectedImagePath.Substring(0, selectedImagePath.LastIndexOf(ICON_CONTAINER_ICON_NUMBER_SEPARATOR)), iconIndex) ??
							Resources.appIco;
					else
						notifyIcon.Icon = ExtractIconFromResource(selectedImagePath, 0) ?? Resources.appIco;
				}
			}
			else
				notifyIcon.Icon = Resources.appIco;

			notifyIcon.ShowBalloonTip(ballonTimeOut);
			new System.Threading.Timer(el => notifyIcon.Dispose(), null, timerTimeOut, Timeout.Infinite);
		}

		public virtual void OpenFile(string selctedFile)
		{
			if (!File.Exists(selctedFile))
				return;

			var fileTransformation = new FileTransformation(selctedFile);

			if (fileTransformation.IsFileProtected)
				fileTransformation.DecryptAndOpenFile();
			else
				fileTransformation.OpenAndDecryptAfterClose();
		}
	}
}
