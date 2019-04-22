using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SkysagaServerEmu.LauncherClasses {

	/// <summary>
	/// This references a custom exe that has been deobfuscated (partially) using de4dotnet, and then post-processed with dnSpy to make some internal stuff public.
	/// </summary>
	class HackyUtils {
		public static readonly string SKYSAGA_PATH = "F:\\Program Files (x86)\\Radiant Worlds\\SkySaga Infinite Isles";
		public static readonly string CUSTOM_EXE = SKYSAGA_PATH + "\\SkySagaLauncher-cleaned-2.exe";
		public static Assembly SkySaga { get; private set; } = null;

		private static Type LauncherMainType = null;
		private static Type ModuleType = null;

		private static void ErrorIfInvalid() {
			//This gets annoying, to be honest, but hey.
			if (Debugger.IsAttached)
				throw new InvalidOperationException("You're running this with the debugger attached!\nThis will cause grabbing data from the assembly to error. Launch with CTRL + F5 instead.");
		}

		public static string GetHexString(string str) {
			byte[] ba = Encoding.Default.GetBytes(str);
			StringBuilder hex = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba) hex.AppendFormat("0x{0:x2}", b);
			return hex.ToString();
		}

		public static void PrintStringAndHex(string output) {
			ConsoleColor oldColor = Console.ForegroundColor;
			Console.Write("String value: ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(output);
			Console.ForegroundColor = oldColor;
			Console.Write("As hex: ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(GetHexString(output));
			Console.ForegroundColor = oldColor;
		}

		public static byte[] GetByte0Value() {
			ErrorIfInvalid();

			FieldInfo byte0field = null;
			//Likewise, moduleType.GetRuntimeField("byte_0") also fails, so I have to iterate to snag that too.
			foreach (FieldInfo t in ModuleType.GetRuntimeFields()) {
				if (t.Name == "byte_0") {
					byte0field = t;
					break;
				}
			}
			RuntimeHelpers.RunClassConstructor(ModuleType.TypeHandle); //byte_0 is initialized in the static constructor of Module so we need to run that.
			object obj = byte0field.GetValue(null); //Static value, no object to get it from.
			return (byte[])obj;
		}

		public static string CallThatStupidStringFunction(uint arg, string methodName = "smethod_24") {
			ErrorIfInvalid();
			MethodInfo method = null;
			foreach (MethodInfo m in ModuleType.GetMethods()) {
				if (m.Name == methodName) {
					method = m;
					break;
				}
			}
			if (method == null) {
				throw new InvalidOperationException("Cannot call " + methodName + "! Did you forget to expose it (set its type to public rather than internal)?");
			}

			method = method.MakeGenericMethod(typeof(string)); //yes
			return method.Invoke(null, new object[] { arg }) as string; //Thanks, obfuscation!
		}

		public static bool IsHostOverwritten(string url) {
			try {
				//Given that the servers no longer exist, this will error in the event that it's not redirected. We need to have a catch case.
				IPAddress[] addresses = Dns.GetHostEntry(url).AddressList;
				if (addresses.Length == 0) { return false; }
				foreach (IPAddress addr in addresses) {
					if (addr.Equals(IPAddress.Loopback)) {
						return true;
					}
				}
				return false;
			} catch (SocketException) {
				return false;
			}
		}

		public static void InitializeHackyUtils() {
			if (!File.Exists(CUSTOM_EXE)) {
				ConsoleColor oldColor = Console.ForegroundColor;
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine("Debugging for the launcher exe (extracting values and such) has been disabled since you don't have the altered exe. If you aren't Xan, this is expected.");
				Console.ForegroundColor = oldColor;
				return;
			}
			SkySaga = Assembly.LoadFrom(CUSTOM_EXE);

			//Now before you go apeshit, for some reason that is entirely beyond me, directly using GetType("Module") fails. Why? No idea.
			//It bothers the living shit out of me regardless.
			//I found that Struct0 is shown so I have to (ab)use that to my advantage by searching for it (since, you guessed it, GetType("Module+Struct0") also fails!) via a loop.
			Type struct0 = null;
			foreach (Type t in SkySaga.GetTypes()) {
				if (t.FullName == "Module+Struct0") {
					struct0 = t;
				} else if (t.FullName == "Launcher.Main") {
					LauncherMainType = t;
				}
			}

			//Get the declaring type which is Module.
			ModuleType = struct0.DeclaringType;

			Console.WriteLine("Got Module? " + (ModuleType != null ? "YES" : "NO"));
			Console.WriteLine("Got Main? " + (LauncherMainType != null ? "YES" : "NO"));
		}

	}
}
