using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DRDLPRegistry;
using DRDLPClientOnlyNet4;

namespace DRDLPFileTransformation
{
	static class Program
	{
		private const char ICON_CONTAINER_ICON_NUMBER_SEPARATOR = ',';
		private const int BALOON_TIME_OUT = 400;

		[DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
		private static extern int ExtractIconEx(string fullFilePath, int iconIndex, out IntPtr largeIconVersion, out IntPtr smallIconVersion, int iconsQuantity);

		private static readonly NotifyIcon NOTIFY_ICON = new NotifyIcon
																{
																	BalloonTipIcon = ToolTipIcon.Info,
																	BalloonTipText = @"Please wait while file opening procedure is finished, it may take awhile...",
																	BalloonTipTitle = @"File is opening",
																	Visible = true
																};

		private static Icon ExtractIconFromResource(string fullFilePath, int iconIndex, bool useLargeIcon = true)
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

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (!args.Any() || !File.Exists(args[0]))
				return;

			var selctedFile = args[0];
			var fileExtingtion = Path.GetExtension(selctedFile);
			var selectedImagePath = RegistryWork.GetDefaultAssociationImage(fileExtingtion);

			if (!string.IsNullOrEmpty(selectedImagePath))
			{
				int iconIndex;
				var lastIndexOfIconContainerIconSeparator = selectedImagePath.LastIndexOf(ICON_CONTAINER_ICON_NUMBER_SEPARATOR);

				if ((selectedImagePath.LastIndexOf('.') < lastIndexOfIconContainerIconSeparator) 
					&& int.TryParse(selectedImagePath.Substring(lastIndexOfIconContainerIconSeparator + 1), out iconIndex) )
				{
					var extractedIcon = ExtractIconFromResource(selectedImagePath.Substring(0, selectedImagePath.LastIndexOf(ICON_CONTAINER_ICON_NUMBER_SEPARATOR)), iconIndex);

					NOTIFY_ICON.Icon = extractedIcon ?? new Icon(selectedImagePath);
				}
				else
					NOTIFY_ICON.Icon = new Icon(selectedImagePath);
			}
			else
				NOTIFY_ICON.Icon = Resources.appIco;

			NOTIFY_ICON.ShowBalloonTip(BALOON_TIME_OUT);

			var namedPipeClient = new NamedPipeClientNet4();

			namedPipeClient.StartConversation(NamedPipeClientNet4.GetPerconalConversationPipeName(), selctedFile, NamedPipeClientNet4.Action.GetDecryptedFile);

			NOTIFY_ICON.Dispose();
		}
	}
}
