using RabbitMQ.Client;
using System.Text;
public class Sender
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory(){HostName = "localhost"}; //hostname -> where rabbitmq is running
        //using is used for disposal and awaiting on that
        await using var connection = await factory.CreateConnectionAsync(); //open connection
        //in 7.x createModel() replace with CreateChannelAsync()
        await using var channel = await connection.CreateChannelAsync(); //channel to create queue , pubkush message and close
        
        //queue name is Basci Test
        await channel.QueueDeclareAsync("BasicTest", true, false, false, null);

        string message = "Getting started with .NET Core RabbitMQ";
        var body = Encoding.UTF8.GetBytes(message);

        await channel.BasicPublishAsync("", "BasicTest", body);
        Console.WriteLine("Sent messagw {0}...",message);

        Console.WriteLine("Press [enter] to exit the Sender App..");
        Console.ReadLine();
    }
}

// QueueDeclare(
//     "hello",
//     false,
//     false,
//     false,
//     null);

// That worked in RabbitMQ 3.x.

// You're using:

// RabbitMQ Server 4.x
// RabbitMQ.Client 7.2.1

// RabbitMQ 4.x introduced several breaking changes and deprecated transient non-exclusive queues.