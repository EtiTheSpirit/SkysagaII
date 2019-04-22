using NetworkingShared.NetworkData;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;

namespace NetworkingShared.Network {
	public class Server {

		private static readonly int GAME_PORT = 443;

		public static void InitializeServer() {
			//This is a shot in the dark, except I'm also blind, piss drunk, and my target is the size of a quarter.
			//Whenever I can find out how to redirect traffic to localhost, I'll use this to see what the packets are.
			TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), GAME_PORT);

			server.Start();
			Console.WriteLine("Server has started on 127.0.0.1:" + GAME_PORT + ".");
			Console.WriteLine("Waiting for a connection...");

			TcpClient client = server.AcceptTcpClient();

			Console.WriteLine("A client connected! Visualizing network stream.\n");

			NetworkStream stream = client.GetStream();

			//int packet = 0;
			while (true) {
				while (!stream.DataAvailable) ;

				byte[] bytes = new byte[client.Available];
				stream.Read(bytes, 0, bytes.Length);

				Console.WriteLine(JsonConvert.SerializeObject(bytes));

				//File.WriteAllBytes("./Out" + packet + ".dat", bytes);
			}
		}

	}
}
