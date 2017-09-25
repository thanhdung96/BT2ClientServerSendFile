namespace BT2SendFileServer
{
	class Program
	{
		private static Server server;

		static void Main(string[] args)
		{
			server = new Server();
			server.startListen();
		}
	}
}
