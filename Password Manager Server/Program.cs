using System;

namespace Password_Manager_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.StartServer();
            System.Threading.Thread.Sleep(int.MaxValue);
        }
    }
}
