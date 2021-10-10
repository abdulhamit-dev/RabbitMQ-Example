using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace RabbitMQ.subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://ajcptffj:Gv4tAAKgyXHbzJ_QFgtp7TOXE6QwFt2u@cattle.rmq2.cloudamqp.com/ajcptffj");

            using var connection = factory.CreateConnection();

            var channel = connection.CreateModel();
            var randomQueueName = channel.QueueDeclare().QueueName;
            channel.QueueBind(randomQueueName, "logs-fanout", "", null);

            channel.BasicQos(0, 1, false);
            var consumer = new EventingBasicConsumer(channel);

            channel.BasicConsume(randomQueueName, false, consumer);

            Console.WriteLine("Loglar dinleniyor");
            consumer.Received += (object sender, BasicDeliverEventArgs e)=>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine("gelen mesaj: "+message);
                channel.BasicAck(e.DeliveryTag,false);
            };

            Console.ReadLine();
        }

        
    }
}
