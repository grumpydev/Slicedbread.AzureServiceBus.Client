using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Slicedbread.AzureServiceBus.Client;
using Slicedbread.AzureServiceBus.Client.ServiceBus;

namespace Slicedbread.AzureServiceBus.Demo.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter connectionstring: ");
            var connectionString = Console.ReadLine();
            Console.Write("Enter queue name: ");
            var queueName = Console.ReadLine();

            var listener = new ServiceBusListener(new[] {new BusSubscriber()});

            listener.Connect(connectionString, queueName);

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }

    internal class BusSubscriber : IDynamicSubscriber
    {
        public bool CanProcess(IServiceBusMessage metadata)
        {
            return true;
        }

        public Task Process(IServiceBusMessage metadata, dynamic body)
        {
            Console.WriteLine("++++++");
            Console.WriteLine("Message type: " + metadata.MessageType);
            Console.WriteLine("Foo: " + body.Foo);
            Console.WriteLine("Bar: " + body.Bar);
            Console.WriteLine("++++++");
            Console.WriteLine();

            return Task.FromResult(0);
        }
    }
}
