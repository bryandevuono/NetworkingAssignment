using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MessageNS;

class Program
{
    static void Main(string[] args)
    {
        ClientUDP udpClient = new ClientUDP();
        udpClient.Start();
    }
}
class ClientUDP
{
    private const string ServerAddress = "127.0.0.1";
    private const int ServerPort = 32000;
    private UdpClient udpClient;

    public int threshold = 20;
    public void Start()
    {
        try
        {
            udpClient = new UdpClient();
            IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse(ServerAddress), ServerPort);

            SendMessage(MessageType.Hello, "Hello from the client" + threshold.ToString());

            ReceiveMessages();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
        }
        finally
        {
            if (udpClient != null)
            {
                udpClient.Close();
            }
        }
    }
    private void SendMessage(MessageType messageType, string content)
    {
        string messageContent = messageType == MessageType.Hello ? $"{content} {threshold}" : content;
        Message message = new Message { Type = messageType, Content = messageContent };
        byte[] data = Serialize(message);
        udpClient.Send(data, data.Length, ServerAddress, ServerPort);
    }



    private void ReceiveMessages()
    {
        List<string> receivedLines = new List<string>();
        int messageCount = 0;
        try
        {
            while (true)
            {
                IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] receivedData = udpClient.Receive(ref serverEndpoint);
                Message receivedMessage = Deserialize(receivedData);

                if (receivedMessage.Type == MessageType.Welcome)
                {
                    Console.WriteLine($"Server said: {receivedMessage.Type} {receivedMessage.Content}");
                    SendMessage(MessageType.RequestData, "Requesting data from server");
                }
                else if (receivedMessage.Type == MessageType.Data)
                {
                    if (!string.IsNullOrWhiteSpace(receivedMessage.Content))
                    {
                        Console.WriteLine($"Received data from server: {receivedMessage.Content}");
                        SendMessage(MessageType.Ack, "Received data");
                        receivedLines.Add(receivedMessage.Content);
                        messageCount++;
                    }

                }
                else if (receivedMessage.Type == MessageType.End)
                {
                    Console.WriteLine("End of file transmission.");
                    break;
                }
                if (messageCount >= threshold)
                {
                    Console.WriteLine($"Threshold of {threshold} reached. Stopping.");
                    break;
                }
            }
            string fileName = "received.txt";
            File.WriteAllLines(fileName, receivedLines);
            Console.WriteLine($"Receibed lines written to {fileName}");
        }
        catch (SocketException ex)
        {
            Console.WriteLine($"SocketException occurred: {ex.Message}");
        }

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