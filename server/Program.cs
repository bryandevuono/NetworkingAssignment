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
    int PacketSize = 1024;
    private const int InitialCongestionWindowSize = 1;
    private const int Threshold = 92;
    bool thresholdreached = false;
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
                if (receivedMessage.Type == MessageType.RequestData)
                {
                    responseMessage.Type = MessageType.Data;
                    byte[] fileData = File.ReadAllBytes("hamlet.txt");
                    // Send file data in packets
                    SendFileDataPackets(fileData, clientEndpoint);
                    if(thresholdreached)
                    {
                        Message endmessage = new Message();
                        endmessage.Type = MessageType.End;
                        endmessage.Content = "end";
                        byte[] enddata = Serialize(endmessage);
                        udpServer.Send(enddata, enddata.Length, clientEndpoint);
                    }
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

    private void SendFileDataPackets(byte[] fileData, IPEndPoint clientEndpoint)
    {
        int sequenceNumber = 0;
        int congestionWindowSize = InitialCongestionWindowSize;
        int threshold = Threshold;

        while (sequenceNumber * PacketSize < fileData.Length)
        {
            int remainingBytes = Math.Min(PacketSize, fileData.Length - sequenceNumber * PacketSize);
            byte[] packetData = new byte[remainingBytes];
            Array.Copy(fileData, sequenceNumber * PacketSize, packetData, 0, remainingBytes);

            // Send packet with data
            Message fileMessage = new Message();
            fileMessage.Type = MessageType.Data;
            fileMessage.Content = Convert.ToBase64String(packetData);
            udpServer.Send(Serialize(fileMessage), packetData.Length, clientEndpoint);
            Console.WriteLine($"Sent packet {sequenceNumber + 1}/{(int)Math.Ceiling((double)fileData.Length / PacketSize)}");

            sequenceNumber++;

            // Wait for ACK or handle timeouts
            WaitForAck(clientEndpoint);

            // Update congestion window size
            if (congestionWindowSize < threshold)
            {
                congestionWindowSize *= 2; // Exponential increase
            }
            if(congestionWindowSize == threshold)
            {
                thresholdreached = true;
            }
            else
            {
                congestionWindowSize++; // Linear increase
            }
        }
    }

    private void WaitForAck(IPEndPoint clientEndpoint)
    {
        bool ackReceived = false;
        int timeout = 1000; // 1 second timeout
        DateTime startTime = DateTime.Now;

        while (!ackReceived && (DateTime.Now - startTime).TotalMilliseconds < timeout)
        {
            try
            {
                byte[] ackData = udpServer.Receive(ref clientEndpoint);
                Message ackMessage = Deserialize(ackData);

                if (ackMessage.Type == MessageType.Ack)
                {
                    ackReceived = true;
                    Console.WriteLine($"Received ACK for packet");
                }
            }
            catch (SocketException ex)
            {
                // Handle timeout or other socket errors
                Console.WriteLine($"SocketException occurred: {ex.Message}");
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


