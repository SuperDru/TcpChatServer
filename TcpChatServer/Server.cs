using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace TcpChatServer
{
    public static class Server
    {
        //Список всех пользователей, подключённых к серверу.
        public static List<ServerClient> clients = new List<ServerClient>();
        //Строка с именами для отправки клиентам при добавлении(удалении) пользователей.
        public static string users = "";

        //Начинает ожидать клиентов по заданному ip и константно заданному порту.
        public static void Listen(string ip)
        {
            TcpListener server;
            try
            {
                server = new TcpListener(IPAddress.Parse(ip), 3329);
            } catch 
            {
                Console.WriteLine("Invalid ip address.");
                return;
            }
            server.Start();
            Console.WriteLine("Server is started.");
            while (true)
            {
                ServerClient client = new ServerClient(server.AcceptTcpClient());
                clients.Add(client);
                client.StartReceiving();
            }
        }

        //Отправляет сообщение всем клиентам.
        public static void BroadcastMessage(string message)
        {
            clients.ForEach((c) => c.SendMessage(message));
        }

        //Проверяет имя на уникальность.
        public static bool isUniqueName(string name)
        {
            return clients.TrueForAll((c) => c.Name != name);
        }
    }
}
