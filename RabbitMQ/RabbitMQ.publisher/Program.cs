using RabbitMQ.Client;
using System;
using System.Linq;
using System.Text;

namespace RabbitMQ.publisher
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri=new Uri("amqps://ajcptffj:Gv4tAAKgyXHbzJ_QFgtp7TOXE6QwFt2u@cattle.rmq2.cloudamqp.com/ajcptffj");

            using var connection=factory.CreateConnection();

            var channel = connection.CreateModel();

            //channel.QueueDeclare("hello-queue", true, false, false);
            channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout);

            Enumerable.Range(1, 20).ToList().ForEach(x =>
            {
                string message = $"log {x}";
                var messageBody = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish("logs-fanout", "", null, messageBody);
                Console.WriteLine($"Mesaj gönderildi :{x}");
            });
            Console.ReadLine();
            
        }
    }
}
