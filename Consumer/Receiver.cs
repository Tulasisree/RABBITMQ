using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Consumer;

public class Receiver
{
    public static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory(){HostName = "localhost"}; 
        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync("BasicTest", true, false, false,null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (channel, a) =>
        {
            var body = a.Body.ToArray();//gestring expects a byte[]
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine("Receiver message {0}",message);
            await Task.CompletedTask;
        };

        //consuming message
        //"true" -> auto acknowledge
        await channel.BasicConsumeAsync("BasicTest",true,consumer);

        Console.WriteLine("Press [enter] to exit the Consumer App..");
        Console.ReadLine();
    }
}

