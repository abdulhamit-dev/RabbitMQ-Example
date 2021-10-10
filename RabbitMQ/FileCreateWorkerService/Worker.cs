using ClosedXML.Excel;
using FileCreateWorkerService.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileCreateWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RabbitMQClientService _rabbitMQClientService;

        private readonly IServiceProvider _serviceProvider;

        private IModel _channel;

        public Worker(ILogger<Worker> logger, RabbitMQClientService rabbitMQClientService, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
            _serviceProvider = serviceProvider;
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel = _rabbitMQClientService.Connect();
            _channel.BasicQos(0, 1, false);
            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            _channel.BasicConsume(RabbitMQClientService.QueueName, false, consumer);
            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            await Task.Delay(1000);
            try
            {
                var sonuc = Encoding.UTF8.GetString(@event.Body.ToArray());
                using var ms = new MemoryStream();
                var wb = new XLWorkbook();
                var ds = new DataSet();
                ds.Tables.Add(GetTable(sonuc));
                wb.Worksheets.Add(ds);
                wb.SaveAs(@"d:\sonuc.xlsx");
                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception ex)
            {

                
            }
            

        }
        private DataTable GetTable(string tableName)
        {
            

            DataTable table = new DataTable { TableName = tableName };

            table.Columns.Add("ProductId", typeof(int));
            table.Columns.Add("Name", typeof(String));
            table.Columns.Add("ProductNumber", typeof(string));

            for (int i = 0; i < 100; i++)
            {
                table.Rows.Add(i, "product_"+i.ToString(),i.ToString());

            }

            return table;


        }
    }
}
