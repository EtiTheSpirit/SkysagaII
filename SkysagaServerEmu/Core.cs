using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;
using SkysagaServerEmu.LauncherClasses;
using NetworkingShared.Network;
using Newtonsoft.Json;
using System.Reflection;

namespace SkysagaServerEmu
{
    class Core {

		public struct PROCESS_INFORMATION {
			public IntPtr hProcess;
			public IntPtr hThread;
			public uint dwProcessId;
			public uint dwThreadId;
		}

		public struct STARTUPINFO {
			public uint cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public uint dwX;
			public uint dwY;
			public uint dwXSize;
			public uint dwYSize;
			public uint dwXCountChars;
			public uint dwYCountChars;
			public uint dwFillAttribute;
			public uint dwFlags;
			public short wShowWindow;
			public short cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		public struct SECURITY_ATTRIBUTES {
			public int length;
			public IntPtr lpSecurityDescriptor;
			public bool bInheritHandle;
		}

		[DllImport("kernel32.dll")]
		static extern bool CreateProcess(
			string lpApplicationName,
			string lpCommandLine,
			IntPtr lpProcessAttributes,
			IntPtr lpThreadAttributes,
			bool bInheritHandles,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation
		);

		[DllImport("kernel32.dll")]
		static extern uint GetLastError();

		private static readonly string SKYSAGA_PATH = "F:\\Program Files (x86)\\Radiant Worlds\\SkySaga Infinite Isles";
		private static readonly string GAME_PATH = SKYSAGA_PATH + "\\Client\\SkySaga.exe";
		private static readonly string BUILD = "0.10.66.36731";
		private static readonly GeoNode DEFAULT_NODE = new GeoNode {
			IP = "127.0.0.1",
			Port = 443
		};

		static void Main(string[] args) {
			Console.ForegroundColor = ConsoleColor.Green; // Only dank hackers use this color scheme :sunglasses:
			HackyUtils.InitializeHackyUtils();

			// According to what I could grab from IDA and other .NET decompilers (Don't worry, I only used IDA to look at stuff I couldn't see in the net decomps, I still have SOME sanity!) the game launches with the following info:
			/*
			 * Dictionary<string, string> with a length of 2.	-- Purpose: Auth data, username/pass -- EDIT: This may not be 2 entries long, see LauncherValues
			 * List<GeoNode> with a length of 1					-- Purpose: Points to the closest server, but doesn't seem to affect where the game connects. To do: Find out if there's any sanity checks in the game exe.
			 * List<string> with a length of 1					-- Purpose: ???
			 * List<string> with a length of 1					-- Purpose: ???
			 * string											-- Purpose: ???
			 * ProcessStartInfo									-- Don't need to specify this
			 * Process											-- Don't need to specify this.
			 * Class17											-- Purpose: ??? - This is a placeholder name. The real class name in the launcher has been HEAVILY obfuscated and doesn't even use displayable letters for its name. I've used a deobfuscator and it came out as "Class17"
			 * int32											-- Purpose: ???
			 */
			// I'm going to launch with with the first args: Dictionary<string, string>, GeoNode, List<string>, List<string>, string
			// Nothing after that will be used.

			// Wild guess: The dictionary stores username and password
			// Edit: Somehow I was right. Any other json values and it goes to the launcher. I'm baffled at how lucky I am... I'm so happy. These two values were ***EXACT.***
			// I get the feeling that this luck is a one-in-a-million thing. I'm gonna miss the luck.

			// To do: Find out how to redirect the server.

			// Note update: Class17's "String_2" value is the string "exit". Thanks, de4dotnet + dnSpy + reflection. You guys are cool. Unlike that obfuscator, that guy can eat a bag of dicks.
			// Class17 extends some extremely complicated module so I wonder if that's serialized into json since you can't pass an entire class into command line args.

			// LaunchGame does have some interesting stuff. Here's something big:
			// processStartInfo.Arguments = string.Join(Module.smethod_23<string>(2549127169u), from item in dictionary select Main.<>c__DisplayClass24.smethod_3(item.Key, Module.smethod_27<string>(759805078u), item.Value, Module.smethod_24<string>(2003834556u)));
			// smethod23(2549127169u) returns a space.
			// This is likely serializing the data into json just going through a bunch of hoops and a three-ring circus before it does.

			// From what I can tell, THE DICTIONARY MAY NOT HAVE TWO ELEMENTS IN IT.
			// The dictionary likely stores all of the config data from the launcher. This is represented in class LauncherValues.
			// Test: What if I pass this into the game exe with custom data?
			// Result: Nothing. Nothing at all. Shit.


			STARTUPINFO startInfo = new STARTUPINFO();
			PROCESS_INFORMATION processInfo = new PROCESS_INFORMATION();

			LauncherValues lValues = new LauncherValues() {
				Username = "skysaga",
				Password = "skysaga",
				client_url = "http://127.0.0.1",
				launcher_url = "http://127.0.0.1",
				ws_host = new Uri("http://127.0.0.1"),
			};

			Console.WriteLine("Testing connections...");
			if (!HackyUtils.IsHostOverwritten("pcdevelop.vm.rw")) {
				ConsoleColor oldColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Beep();
				Console.WriteLine("\nWARNING: Your hosts file does not redirect pcdevelop.vm.rw to 127.0.0.1! The game will not be able to connect to the sandboxed server.\n");
				Console.ForegroundColor = oldColor;
				Console.Write("Press any key to quit.");
				Console.ReadKey();
				return;
			}

			string launchArgs = JsonConvert.SerializeObject(lValues);
			launchArgs.Replace("\"", "\\\"");
			launchArgs = "\"" + launchArgs + "\"";

			string defNode = JsonConvert.SerializeObject(DEFAULT_NODE);
			defNode.Replace("\"", "\\\"");
			defNode = "\"[" + defNode + "]\"";
			string progArgs = launchArgs + " " + defNode;

			Console.WriteLine(progArgs);
			bool success = CreateProcess(GAME_PATH, progArgs, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startInfo, out processInfo);
			if (!success) {
				Console.Beep();
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Failure!");
				uint code = GetLastError();
				if (code == 3) {
					Console.WriteLine("Error code 3 (The System Cannot Find The Path Specified)");
				}
				else {
					Console.WriteLine("Error code " + code);
					Console.WriteLine("Google \"Windows system error codes\" to find the meaning of this code.");
				}
				Console.Write("Press any key to quit.");
				//Console.ReadKey();
			}

			Server.InitializeServer();
			
		}
    }
}

