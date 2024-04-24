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
        ServerUDP sudp = new ServerUDP();
        sudp.Start();
    }
}

class ServerUDP
{
    private UdpClient udpServer;
    private const int ServerPort = 32000;
    private const int PacketSize = 1024;
    private bool thresholdReached = false;

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
                Console.WriteLine($"Received from {clientEndpoint}: {receivedMessage.Content}");

                HandleReceivedMessage(receivedMessage, clientEndpoint);
            }
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException occurred: {ex.Message}");
        }
        finally
        {
            udpServer.Close();
        }
    }

    private void HandleReceivedMessage(Message receivedMessage, IPEndPoint clientEndpoint)
    {
        Message responseMessage = new Message();

        switch (receivedMessage.Type)
        {
            case MessageType.Hello:
                responseMessage.Type = MessageType.Welcome;
                responseMessage.Content = "Welcome from server";
                break;
            case MessageType.RequestData:
                responseMessage.Type = MessageType.Data;
                SendFileDataPackets("hamlet.txt", clientEndpoint);
                if (thresholdReached)
                {
                    SendEndMessage(clientEndpoint);
                }
                break;
            default:
                // Handle unsupported message type
                break;
        }

        byte[] responseData = Serialize(responseMessage);
        udpServer.Send(responseData, responseData.Length, clientEndpoint);
    }

    private void SendFileDataPackets(string filePath, IPEndPoint clientEndpoint)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        int totalPackets = (int)Math.Ceiling((double)fileData.Length / PacketSize);

        for (int sequenceNumber = 0; sequenceNumber < totalPackets; sequenceNumber++)
        {
            int offset = sequenceNumber * PacketSize;
            int remainingBytes = Math.Min(PacketSize, fileData.Length - offset);
            byte[] packetData = new byte[remainingBytes];
            Array.Copy(fileData, offset, packetData, 0, remainingBytes);

            Message fileMessage = new Message();
            fileMessage.Type = MessageType.Data;
            fileMessage.Content = Convert.ToBase64String(packetData);

            byte[] serializedMessage = Serialize(fileMessage);
            udpServer.Send(serializedMessage, serializedMessage.Length, clientEndpoint);

            Console.WriteLine($"Sent packet {sequenceNumber + 1}/{totalPackets}");

            // Simulate threshold reached (for demonstration purposes)
            if (sequenceNumber == 15)
            {
                thresholdReached = true;
            }
        }
    }

    private void SendEndMessage(IPEndPoint clientEndpoint)
    {
        Message endMessage = new Message();
        endMessage.Type = MessageType.End;
        endMessage.Content = "End of file transmission";

        byte[] endData = Serialize(endMessage);
        udpServer.Send(endData, endData.Length, clientEndpoint);
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