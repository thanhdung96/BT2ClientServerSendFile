using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

/**
 * Signals send from client:
 *	1 (0000 0001)		client ready to receive data
 *	0 (0000 0000)		client received fragment of data successfully and ready to receive next fragment
 *	2 (0000 0010)		client received all of data successfully
 *	8 (1111 1111)		client wants to close connection
**/

/**
 * Signals send from server:
 *	1 (0000 0001)		server acknowledged signal from client
**/
namespace BT2SendFileServer
{
	class Server
	{
		private Socket serverSock;		//socket bind to server
		private Socket clientSock;		//socket represent client
		private byte[] dataToSend;
		private byte[] dataToReceive;
		private byte signalFromDataReceived;

		public Server()
		{
			Console.WriteLine("Server Initializing...");

			//init socket
			//server
			serverSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	//create socket using IPv4, type stream, TCP
			serverSock.Bind(new IPEndPoint(IPAddress.Any, 1724));	//listen to any network interface at port 1724
			//init data stream
			dataToSend = new byte[1024];		//data send to client
			dataToReceive = new byte[1];		//signal receive from client 1 byte
		}

		public void startListen()
		{
			//start listening and accepting
			serverSock.Listen(10);
			Console.WriteLine("Server listening...");
			clientSock = serverSock.Accept();
			Console.WriteLine("Server accepted a client...");

			//receive first signal from client
			clientSock.Receive(dataToReceive);
			signalFromDataReceived = dataToReceive[0];		//get first byte from data stream

			if (signalFromDataReceived == 1)	//if client is ready
			{
				clientSock.Send(new byte[1]{1});		//send 1 byte of value 1 to client to ACK
				Console.WriteLine("Server sending file to client...");
				sendFile();		//begin transferring file
			}
		}

		private void sendFile()
		{
			//throw new NotImplementedException();

			// Create an instance of StreamReader to read from a file.
			using (StreamReader sr = new StreamReader("text.txt"))
			{
				string line;
				// Read and display lines from the file until the end of the file is reached. 
				while ((line = sr.ReadLine()) != null)
				{
					clientSock.Receive(dataToReceive);
					signalFromDataReceived = dataToReceive[0];		//get first byte from data stream

					if (signalFromDataReceived == 0)
					{
						dataToSend = Encoding.ASCII.GetBytes(line);
						clientSock.Send(dataToSend, line.Length, SocketFlags.None);
					}
				}
			}
		}
	}
}
