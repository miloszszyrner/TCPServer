using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace TCPServer
{
	class Program
	{
		static void Main(string[] args)
		{
			string host = "localhost";
			int port = 7;
			int tempPort;
			string message = String.Empty;
			Socket socket = null;
			Socket client = null;

			Console.WriteLine("Siema, default port = 7, Would you like to change it? If yes press y, otherwise click any key");
			if (Console.ReadLine().Equals("y"))
			{
				do
				{
					Console.WriteLine("Give me the port");
					if (int.TryParse(Console.ReadLine(), out tempPort))
					{
						port = tempPort;
					}
				} while (port < 0);
			}
			try
			{
				socket = CreateServer(host, port);
				client = socket.Accept();
				while (true)
				{
					
					Console.WriteLine("Połączenie z {0}", client.RemoteEndPoint.ToString());
					message = GetMessage(client);
					SendMessage(message, client);
					client.Close();
				}
				socket.Close();
			}
			catch (SocketException e)
			{
				Console.WriteLine("Niepoowodzenie dostępu do gniazda");
			}
			catch (ObjectDisposedException e)
			{
				Console.WriteLine("Gniazdo zostało zamkniete");
			}
			catch (InvalidOperationException e)
			{
				Console.WriteLine("Gniazdo akceptujace nie jest gniazdem sluchajacym");
			}
			Console.Read();
		}
		public static Socket CreateServer(string host, int port)
		{
			IPAddress[] IPs = Dns.GetHostAddresses(host);

			Socket s = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream,
				ProtocolType.Unspecified);

			Console.WriteLine("Creating server on {0}",
				host);
			foreach (IPAddress address in IPs)
			{
				if (IPAddress.Parse(address.ToString()).AddressFamily == AddressFamily.InterNetwork)
				{
					try
					{
						s.Bind(new IPEndPoint(address, port));
					}
					catch (ArgumentNullException e)
					{
						Console.WriteLine("localEP jest null");
					}
					catch (SecurityException e)
					{
						Console.WriteLine("Nie posiada uprawnien do wykonywania tej operacji");
					}
					s.Listen(10);
					break;
				}
			}
			Console.WriteLine("Server Created");
			return s;
		}
		public static string GetMessage(Socket s)
		{
			byte[] bytes = new byte[512];
			try
			{
				s.Receive(bytes);
				Console.WriteLine("Otrzymane dane od {0} :", s.RemoteEndPoint.ToString());
				Console.WriteLine(Encoding.ASCII.GetString(bytes).TrimEnd('\0'));
			}
			catch (SecurityException e)
			{
				Console.WriteLine("Brak uprawnień przy wykonywaniu metody");
				s.Shutdown(SocketShutdown.Both);
				s.Close();
			}
			return Encoding.ASCII.GetString(bytes).TrimEnd('\0');
		}
		public static void SendMessage(string textToSend, Socket s)
		{
			byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(textToSend);
			s.Send(bytesToSend);
		}
	}
}
