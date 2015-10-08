using System;
using Slicedbread.AzureServiceBus.Client;

namespace Slicedbread.AzureServiceBus.Demo.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter connectionstring: ");
            var connectionString = Console.ReadLine();
            Console.Write("Enter queue name: ");
            var queueName = Console.ReadLine();

            var client = new ServiceBusClient();

            client.Connect(connectionString, queueName);

            while (true)
            {
                Console.WriteLine();
                Console.Write("Enter message type: ");
                var messageType = Console.ReadLine();
                Console.Write("Enter value for foo: ");
                var foo = Console.ReadLine();
                Console.Write("Enter value for bar: ");
                var bar = Console.ReadLine();
                client.Send(messageType, new { Foo = foo, Bar = bar }).Wait();
                Console.WriteLine();
            }
        }
    }
}
