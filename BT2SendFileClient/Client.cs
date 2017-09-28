using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

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
 *	2 (0000 0010)		server inform about finished sending file
**/
namespace BT2SendFileClient
{
	class Client
	{
		private Socket clientSock;
		private byte[] dataToSend;
		private byte[] dataReceive;
		private int dataBlockSize;

		public Client()
		{
			//client init
			Console.WriteLine("Client Initializing...");
			clientSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	//create socket using IPv4, type stream, TCP

			dataToSend = new byte[1];		//signal send to server, 1 byte
			dataReceive = new byte[4096];		//data receive from server, 4Kb buffer
		}

		private void connect()
		{
			Console.WriteLine("Client connecting to Server...");
			clientSock.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1724));	//connect to server at 127.0.0.1 (Loopback), port 1724
			Console.WriteLine("Client connected to Server...");

			dataToSend[0] = 1;		//first signal to server
			clientSock.Send(dataToSend, 1, SocketFlags.None);		//inform server that client is ready, send 1 byte of value 1

			clientSock.Receive(dataReceive, 1, SocketFlags.None);	//receive first signal from server
			byte signalFromServer = dataReceive[0];
			if (signalFromServer == 1)		//if server acknowledge first signal
			{
				receiveFile();
			}
		}

		private void disconnect()
		{
			//disconnect from server
			dataToSend[0] = 8;		//first signal to server
			clientSock.Send(dataToSend, 1, SocketFlags.None);		//inform server that client is ready, send 1 byte of value 1
			//clientSock.Close();
			Console.WriteLine("Client disconnected from server...");
		}

		public void receiveFile()
		{
			clientSock.Receive(dataReceive, 1, SocketFlags.None);	//receive first signal from server
			byte signalFromServer = dataReceive[0];

			while (signalFromServer == 3/* && signalFromServer != 2*/)
			{
				dataBlockSize = clientSock.Receive(dataReceive);		//receive stream of data from server
				string line = Encoding.ASCII.GetString(dataReceive, 0, dataBlockSize);	//convert to string

				//write to file
				using (StreamWriter sw = new StreamWriter("receive.txt"))
				{
					sw.WriteLine(line);
				}

				dataToSend[0] = 1;		//first signal to server
				clientSock.Send(dataToSend, 1, SocketFlags.None);		//inform server that client is ready, send 1 byte of value 1

				clientSock.Receive(dataReceive, 1, SocketFlags.None);	//receive next signal from server
				signalFromServer = dataReceive[0];		//if signal is not 2, continue receiving
			}

			disconnect();
		}
	}
}
