using System;
using System.Text;

namespace Miqo.Config.CreateKeys {
	class Program {
		static void Main(string[] args) {
			OutputHeader();
			OutputPrivateKey();
		}

		private static void OutputHeader() {
			Console.OutputEncoding = Encoding.UTF8;

			Console.Title = "Miqo.Config: Key Creation Tool";
			Console.Clear();
			Console.WriteLine();
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Key Creation Tool");
			Console.ForegroundColor = ConsoleColor.DarkGray;
			Console.WriteLine("Generates a new key pair for use with Miqo.Config");
			Console.WriteLine();
			Console.WriteLine();
		}

		private static void OutputPrivateKey() {
			Console.WriteLine($"Private key:");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine($"{Miqo.Config.StringCipher.CreateRandomKey()}");
			Console.WriteLine();
			Console.WriteLine();
		}
	}
}
