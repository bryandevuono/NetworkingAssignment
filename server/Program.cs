using System;
using System.Data.SqlTypes;
using System.Net;
using System.IO;
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
        ServerUDP sUDP = new ServerUDP();
        sUDP.Start();
    }
}

class ServerUDP
{
    private UdpClient udpServer;
    private const int ServerPort = 32000;
    private const int PacketSize = 3024;
    private bool thresholdReached = false;
    public int InitialWindowSize = 1;
    public int Threshold = 0;
    public int acked = 0;
    private HashSet<string> requestDataMessages = new HashSet<string>();

    public ServerUDP()
    {
        udpServer = new UdpClient(ServerPort);
    }

    public void Start()
    {
        Console.WriteLine("UDP server is running. Waiting for messages...");

        try
        {
            while (true)
            {
                IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpServer.Receive(ref clientEndpoint);
                Message receivedMessage = Deserialize(receivedData);

                HandleReceivedMessage(receivedMessage, clientEndpoint);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException occurred: {ex.Message}");
            Console.WriteLine("Restarting server...");
            Start();
            Console.WriteLine("UDP server restarted. Waiting for messages...");
        }
        finally
        {
            udpServer.Close();
        }
    }

    private void HandleReceivedMessage(Message receivedMessage, IPEndPoint clientEndpoint)
    {
        Message responseMessage = new Message();

        if (receivedMessage.Type == MessageType.Hello)
        {

            string[] parts = receivedMessage.Content.Split(' ');
            if (parts.Length > 1)
            {

                if (int.TryParse(parts[parts.Length - 1], out int clientThreshold))
                {
                    Threshold = clientThreshold;
                }
                else
                {
                    Console.WriteLine("Invalid threshold value in Hello message.");
                }
            }
            else
            {
                Console.WriteLine("Threshold not found in Hello message.");
            }

            responseMessage.Type = MessageType.Welcome;
            responseMessage.Content = "Welcome from server";
            Console.WriteLine($"Received from Client - Type: {receivedMessage.Type}\n");
        }

        else if (receivedMessage.Type == MessageType.RequestData)
        {
            if (IsValidRequestDataMessage(receivedMessage.Content))
            {
                if (!requestDataMessages.Contains(receivedMessage.Content))
                {
                    requestDataMessages.Add(receivedMessage.Content);
                    responseMessage.Type = MessageType.Data;

                    SendFileDataPackets("hamlet.txt", clientEndpoint);
                    if (thresholdReached)
                    {
                        SendEndMessage(clientEndpoint);
                    }
                }


                else
                {
                    Console.WriteLine($"Duplicate RequestData message received.");
                }
            }
            else
            {
                Console.WriteLine($"Invalid format for RequestData message.");
            }
        }
        else if (receivedMessage.Type == MessageType.Ack)
        {
            acked++;
            Console.WriteLine($"Wating for ack from client..");
            Console.WriteLine($"Received {receivedMessage.Type} from client - {receivedMessage.Content}");
        }
        else
        {
            Console.WriteLine($"Received from Client - Type: {receivedMessage.Type} - Content: {receivedMessage.Content}\n");
        }
        byte[] responseData = Serialize(responseMessage);
        udpServer.Send(responseData, responseData.Length, clientEndpoint);
    }

    public void SendFileDataPackets(string filePath, IPEndPoint clientEndpoint)
    {
        int currentWindowSize = InitialWindowSize;
        string[] fileData = File.ReadAllLines(filePath);
        int totalLines = fileData.Length;
        int linesSent = 0;

        while (linesSent <= Threshold)
        {
            int linesToSend = Math.Min(currentWindowSize, totalLines - linesSent);
            int ID = 1;
            StringBuilder sb = new StringBuilder();
            for (int i = linesSent; i < linesSent + linesToSend; i++)
            {
                sb.AppendLine($"ID:{ID:D4} - {fileData[i]}");
                ID++;
            }

            Message fileMessage = new Message();
            fileMessage.Type = MessageType.Data;
            fileMessage.Content = sb.ToString();
            byte[] serializedMessage = Serialize(fileMessage);
            udpServer.Send(serializedMessage, serializedMessage.Length, clientEndpoint);
            Console.WriteLine($"Sent {linesToSend} lines from {linesSent + 1} to {linesSent + linesToSend}\n");

            linesSent += linesToSend;

            byte[] receiveddata = udpServer.Receive(ref clientEndpoint);
            Message receivedmessage = Deserialize(receiveddata);
            HandleReceivedMessage(receivedmessage, clientEndpoint);

            if (currentWindowSize < Threshold && acked == currentWindowSize)
            {
                currentWindowSize = Math.Min(currentWindowSize * 2, totalLines - linesSent);
            }
            else
            {
                currentWindowSize = Threshold;
                System.Threading.Thread.Sleep(50);
            }
        }
        thresholdReached = true;
    }
    private void SendEndMessage(IPEndPoint clientEndpoint)
    {
        Message endMessage = new Message();
        endMessage.Type = MessageType.End;
        endMessage.Content = "End of file transmission";

        byte[] endData = Serialize(endMessage);
        udpServer.Send(endData, endData.Length, clientEndpoint);
    }
    private bool IsValidRequestDataMessage(string content)
    {
        return !string.IsNullOrWhiteSpace(content);
    }
    private static byte[] Serialize(Message message)
    {
        string messageString = $"{message.Type}|{message.Content}";
        return Encoding.UTF8.GetBytes(messageString);
    }

    private static Message Deserialize(byte[] data)
    {
        try
        {
            string messageString = Encoding.UTF8.GetString(data);
            string[] parts = messageString.Split('|');

            if (parts.Length != 2 || !Enum.TryParse(parts[0], out MessageType messageType))
            {
                throw new ArgumentException("Invalid message format");
            }

            return new Message { Type = messageType, Content = parts[1] };
        }
        catch (Exception ex)
        {
            throw new ArgumentException("Error decoding message", ex);
        }
    }

}
//TODO: keep receiving messages from clients
// you can call a dedicated method to handle each received type of messages

//TODO: [Receive Hello]

//TODO: [Send Welcome]

//TODO: [Receive RequestData]

//TODO: [Send Data]

//TODO: [Implement your slow-start algorithm considering the threshold] 

//TODO: [End sending data to client]

//TODO: [Handle Errors]

//TODO: create all needed methods to handle incoming messages