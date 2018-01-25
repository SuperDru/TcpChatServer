using System;

namespace TcpChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                Server.Listen("25.31.100.46");
            else
                Server.Listen(args[0]);

            Console.ReadKey();
        }
    }
}
