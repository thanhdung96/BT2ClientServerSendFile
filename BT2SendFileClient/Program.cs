using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BT2SendFileClient
{
	class Program
	{
		private Socket clientSock;
		byte[] dataToSend;
		byte[] dataToReceive;

		public Program()
		{
			clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	//create socket using IPv4, type stream, TCP
			clientSock.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1724));
			clientSock.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1724));
		}

		static void Main(string[] args)
		{
		}
	}
}
