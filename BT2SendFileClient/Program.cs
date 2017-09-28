namespace BT2SendFileClient
{
	class Program
	{
		private static Client client;
		static void Main(string[] args)
		{
			client = new Client();
			client.receiveFile();
		}
	}
}
