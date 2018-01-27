using System;
using System.Text;
using System.Net.Sockets;

namespace TcpChatServer
{
    public class ServerClient
    {
        public string name { get; private set; } 
        string message;
        NetworkStream stream;


        public ServerClient(TcpClient client)
        {
            stream = client.GetStream();
        }
//
//---------------------------------------------------------------------------------------------------------------------------------------
//
        //Начинает асинхронно ожидать данные от клиента и после обработки отсылает их остальным подключённым клиентам.
        //   /#/ - знак того, что данные информационные (после этого знака идут две цифры, сообщающие тип информации)
        //   /#/00 - имя пользователя
        async public void StartReceiving()
        {
            byte[] buff = new byte[256];
            try
            {
                while (true)
                {
                    int count = await stream.ReadAsync(buff, 0, buff.Length);
                    message = Encoding.GetEncoding(1251).GetString(buff, 0, count);

                    if (!message.StartsWith("/#/00")) {
                        Console.WriteLine(name + ": " + message);
                        Server.BroadcastMessage(name + ": " + message);
                    }
                    else if (Server.isUniqueName(message.Substring(5)))
                    {
                        AddUser();
                    }
                    else
                    {
                        SendMessage("/#/00");
                        return;
                    }
                }
            }
            catch
            {
                RemoveUser();
            }
        }
        //Отправляет асинхронно сообщение клиенту.
        async public void SendMessage(string message)
        {
            try
            {
                byte[] buff = Encoding.GetEncoding(1251).GetBytes(message);
                await stream.WriteAsync(buff, 0, message.Length);
            }
            catch
            {
                RemoveUser();
            }
        }
//
//---------------------------------------------------------------------------------------------------------------------------------------
//
        //Добавляет пользователя в список
        private void AddUser()
        {
            lock (Server.State)
            {
                name = message.Substring(5);
                Server.clients.Add(this);
                Console.WriteLine("The user " + name + " is connected to the server.");
                Server.users += (name + "\n");
                Server.BroadcastMessage("/#/01The user " + name + " has joined to the chat./#/01" + Server.users);
            }
        }
        //Удаляет пользователя из списка
        private void RemoveUser()
        {
            lock (Server.State)
            {
                Console.WriteLine("The user " + name + " disconnected from the server.");
                Server.clients.Remove(this);
                Server.users = Server.users.Replace(name + "\n", "");
                Server.BroadcastMessage("/#/01The user " + name + " left the chat./#/01" + Server.users);
            }
        }
    }
}
