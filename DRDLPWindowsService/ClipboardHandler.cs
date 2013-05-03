using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace DRDLPWindowsService
{
	/// <summary>
	/// TODO: SetWinEventHook is working too slow 
	/// Run in separate thread
	/// </summary>
	public class ClipboardHandler
	{
		private readonly List<Process> _processes;
		private readonly List<Process> _processesToAdd;
		private readonly List<Process> _processesToRemove;

		private readonly int _checkInterval;
		private readonly string _defaultClipboardWarningMessage;

		private delegate void OnAddRemovePorocess();
		private event OnAddRemovePorocess AddProcessFromProcessing;
		private event OnAddRemovePorocess RemoveProcessFromProcessing;

		public bool StopHandling { get; set; }

		private bool _clipboardHandlingIsActive;
		public bool ClipboardHandlingIsActive {
			get { return _clipboardHandlingIsActive; } 
			private set {
							if (!value)
								_processes.Clear(); 
						}
		}

		[DllImport("user32")]
		private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int processId);

		[DllImport("user32.dll")]
		private static extern IntPtr GetForegroundWindow();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="processIDs">process id`s, that in white list (user can copy data between them)</param>
		/// <param name="checkInterval">interval of active application check in milliseconds</param>
		/// <param name="defaultClipboardWarningMessage"></param>
		public ClipboardHandler(IEnumerable<int> processIDs, int checkInterval = 500, string defaultClipboardWarningMessage = "You do not have permission to copy this data")
		{
			if (processIDs == null)
				throw new ArgumentNullException("processIDs");

			if (!processIDs.All(el => Process.GetProcesses().Any(elem => elem.Id == el)))
				throw new ArgumentException("some of the process in pocessIDs not found in system");

			_checkInterval = checkInterval;
			_defaultClipboardWarningMessage = defaultClipboardWarningMessage;

			_processes = new List<Process>(Process.GetProcesses().Where(el => processIDs.Contains(el.Id)));
			_processesToAdd = new List<Process>();
			_processesToRemove = new List<Process>();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="processID">process id, that in white list (user can copy data between them)</param>
		/// <param name="checkInterval">interval of active application check in milliseconds</param>
		/// <param name="defaultClipboardWarningMessage"></param>
		public ClipboardHandler(int processID, int checkInterval = 500, string defaultClipboardWarningMessage = "You do not have permission to copy this data")
		{
			if (!Process.GetProcesses().Any(elem => elem.Id == processID))
				throw new ArgumentException("some of the process in pocessIDs not found in system");

			_checkInterval = checkInterval;
			_defaultClipboardWarningMessage = defaultClipboardWarningMessage;

			_processes = new List<Process> { Process.GetProcesses().First(elem => elem.Id == processID) };
			_processesToAdd = new List<Process>();
			_processesToRemove = new List<Process>();
		}

		public ClipboardHandler(int checkInterval = 500, string defaultClipboardWarningMessage = "You do not have permission to copy this data")
		{
			_checkInterval = checkInterval;
			_defaultClipboardWarningMessage = defaultClipboardWarningMessage;

			_processes = new List<Process>();
			_processesToAdd = new List<Process>();
			_processesToRemove = new List<Process>();
		}

		public void HandleCrossAppBufferDataTransmission()
		{
			var previousClipboardText = string.Empty;
			var previousPID = 0;
			int currentPID;
			var returnClipboardData = false;
			ClipboardHandlingIsActive = true;
			while (_processes.Any(el => !el.HasExited) && !StopHandling)
			{
				GetWindowThreadProcessId(GetForegroundWindow(), out currentPID);

				if (previousPID != currentPID)
				{
					var previousPIDIsAllowed = _processes.Any(el => el.Id == previousPID);
					var currentPIDIsAllowed = _processes.Any(el => el.Id == currentPID);

					if (previousPIDIsAllowed && !currentPIDIsAllowed)
					{
						previousClipboardText = Clipboard.GetText();

						returnClipboardData = true;
						Clipboard.SetText(_defaultClipboardWarningMessage);
					}
					else
					{
						if (currentPIDIsAllowed && returnClipboardData)
						{
							if (!string.IsNullOrEmpty(previousClipboardText) && (_defaultClipboardWarningMessage == Clipboard.GetText()))
								Clipboard.SetText(previousClipboardText);

							previousClipboardText = string.Empty;
							returnClipboardData = false;
						}
						
					}
					previousPID = currentPID;
				}

				if (AddProcessFromProcessing != null)
					AddProcessFromProcessing();

				if (RemoveProcessFromProcessing != null)
					RemoveProcessFromProcessing();

				Thread.Sleep(_checkInterval);
			}

			ClipboardHandlingIsActive = false;
		}

		private void AddNewProcesses()
		{
			_processes.AddRange(_processesToAdd);
			_processesToAdd.Clear();

			AddProcessFromProcessing -= AddNewProcesses;
		}

		private void RemoveProcesses()
		{
			foreach (var process in _processes.Where(el => _processesToRemove.Any(elem => elem.Id == el.Id)).ToArray())
			{
				_processes.Remove(process);
			}

			_processesToRemove.Clear();

			RemoveProcessFromProcessing -= RemoveProcesses;
		}

		public void AddNewProcess(int pid)
		{
			if (!Process.GetProcesses().Any(elem => elem.Id == pid))
				throw new ArgumentException("some of the process in pocessIDs not found in system");

			if (ClipboardHandlingIsActive)
			{
				_processesToAdd.Add(Process.GetProcesses().First(el => el.Id == pid));
				AddProcessFromProcessing += AddNewProcesses;
			}
			else
				_processes.Add(Process.GetProcesses().First(el => el.Id == pid));
		}

		public void AddNewProceses(IEnumerable<int> processIDs)
		{
			if (processIDs == null)
				throw new ArgumentNullException("processIDs");

			if (!processIDs.All(el => Process.GetProcesses().Any(elem => elem.Id == el)))
				throw new ArgumentException("some of the process in pocessIDs not found in system");


			if (ClipboardHandlingIsActive)
			{
				_processesToAdd.AddRange(Process.GetProcesses().Where(el => processIDs.Contains(el.Id)));
				AddProcessFromProcessing += AddNewProcesses;
			}
			else
				_processes.AddRange(Process.GetProcesses().Where(el => processIDs.Contains(el.Id)));
		}

		public void RemoveProcess(int pid)
		{
			if (!_processes.Any(el => el.Id == pid))
				throw new ArgumentException("some of the process in pocessIDs not found in process collection");

			if (ClipboardHandlingIsActive)
			{
				_processesToRemove.Add(_processes.First(el => el.Id == pid));

				RemoveProcessFromProcessing += RemoveProcesses;
			}
			else
				_processes.Clear();
		}

		public void RemoveProcess(IEnumerable<int> processIDs)
		{
			if (processIDs == null)
				throw new ArgumentNullException("processIDs");

			if (!processIDs.All(el => _processes.Any(elem => elem.Id == el)))
				throw new ArgumentException("some of the process in pocessIDs not found in process collection");

			if (ClipboardHandlingIsActive)
			{
				_processesToRemove.AddRange(_processes.Where(el => processIDs.Contains(el.Id)));

				RemoveProcessFromProcessing += RemoveProcesses;
			}
			else
				_processes.Clear();
		}

		public IEnumerable<Process> GetAllProceses()
		{
			return _processes.ToArray();
		}

		public bool PIDIsCurrentlyHandled(int pid)
		{
			return _processes.Any(el => el.Id == pid);
		}
	}
}
