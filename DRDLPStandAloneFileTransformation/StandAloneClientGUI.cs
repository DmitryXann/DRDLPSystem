using System.IO;
using DRDLPCore.Helpres.ClientGUIHelpers;

namespace DRDLPStandAloneFileTransformation
{
	internal class StandAloneClientGUI : ClientGUIMainAbstractClass
	{
		internal string FullFilePath { get; private set; }

		internal StandAloneClientGUI(string fullFilePath)
		{
			if (!File.Exists(fullFilePath))
				throw new FileNotFoundException("fullFilePath");

			FullFilePath = fullFilePath;
		}

		internal void OpenFile()
		{
			ShowBalloonTip(FullFilePath);
			OpenFile(FullFilePath);
		}
	}
}
