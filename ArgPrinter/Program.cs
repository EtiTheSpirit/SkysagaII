using System;
using System.IO;

namespace ProgramArgPrinter {
	class Program {
		static void Main(string[] args) {
			string arg = "";
			foreach (string a in args) {
				arg += a + "\n";
			}
			Console.WriteLine("PROGRAM ARGS:\n" + arg);
			File.WriteAllText("./Out.txt", arg);
			Console.ReadKey();
		}
	}
}
