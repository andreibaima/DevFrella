using DevFreela.Core.Services;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Infrastructure.MessageBus
{
    public class MessageBusService : IMessageBusService
    {
        private readonly ConnectionFactory _factory; // CONEXÃO COM RABBitMQ
        public MessageBusService(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = "localhost"
            };
        }
        public void Publish(string queue, byte[] message)
        {
            // inicializar conexão
            using (var connection = _factory.CreateConnection())
            {
                // criar um canal para comunicação
                using(var channel = connection.CreateModel())
                {
                    // garantir que a fila esteja criada
                    channel.QueueDeclare(
                        //informação da fila
                        queue: queue,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null
                        );

                    // publicar a mensagem
                    channel.BasicPublish(
                        exchange: "", // agente responsavel por rotear a msg, nesse caso é o padrão
                        routingKey: queue, // qual fila vai retornar
                        basicProperties: null,
                        body: message // corpo da msg
                        );
                    
                }
            }
        }
    }
}
