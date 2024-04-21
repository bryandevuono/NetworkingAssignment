using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using MessageNS;

// Julina Mercera | 1055662
// Bryan de Vuono | 1061043
// Do not modify this class
class Program
{
    static void Main(string[] args)
    {
        ServerUDP sudp = new ServerUDP();
        sudp.Start();
    }
}

class ServerUDP
{

    //TODO: implement all necessary logic to create sockets and handle incoming messages
    // Do not put all the logic into one method. Create multiple methods to handle different tasks.
    public void Start()
    {
        ServerUDP udpServer = new ServerUDP();
        receiveMessage();
    }
    //TODO: create all needed objects for your sockets 
    public ServerUDP()
    {
        udpServer = new UdpClient();
    }
    private UdpClient udpServer;
    private const int serverPort = 32000;
    //TODO: keep receiving messages from clients
    // you can call a dedicated method to handle each received type of messages
    //TODO: [Receive Hello]
    // Which should print hello!
    public void receiveMessage()
    {
        Console.WriteLine("UDP server is running. Waiting for messages...");

        try
        {
            udpServer.Client.Bind(new IPEndPoint(IPAddress.Any, serverPort));
            while (true)
            {
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpServer.Receive(ref clientEndpoint);

                Message receivedMessage = Deserialize(receivedData);

                Console.WriteLine($"Received from {clientEndpoint}: {receivedMessage.Content}");
                Message responseMessage = new Message();
                if (receivedMessage.Type == MessageType.Hello)
                {
                    responseMessage.Type = MessageType.Welcome;
                    responseMessage.Content = "Welcome from server";
                }
                byte[] responseData = Serialize(responseMessage);

                udpServer.Send(responseData, responseData.Length, clientEndpoint);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            if (udpServer != null)
            {
                udpServer.Close();
            }
        }
    }


    static Message Deserialize(byte[] data)
    {
        string messageString = Encoding.UTF8.GetString(data);
        string[] parts = messageString.Split('|');

        if (parts.Length != 2 || !Enum.TryParse(parts[0], out MessageType messageType))
        {
            throw new ArgumentException("Invalid message format");
        }
        Message decodedMessage = new Message();
        decodedMessage.Type = messageType;
        decodedMessage.Content = parts[1];
        return decodedMessage;
    }

    static byte[] Serialize(Message message)
    {
        string messageString = $"{message.Type}|{message.Content}";
        return Encoding.UTF8.GetBytes(messageString);
    }
}

    //TODO: [Send Welcome]

    //TODO: [Receive RequestData]

    //TODO: [Send Data]

    //TODO: [Implement your slow-start algorithm considering the threshold] 

    //TODO: [End sending data to client]

    //TODO: [Handle Errors]

    //TODO: create all needed methods to handle incoming messages


