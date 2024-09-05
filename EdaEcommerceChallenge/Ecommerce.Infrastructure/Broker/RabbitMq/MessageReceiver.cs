using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Ecommerce.Infrastructure.Broker.RabbitMq;

public sealed class MessageReceiver : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public MessageReceiver(string hostname, string queueName)
    {
        var factory = new ConnectionFactory() { HostName = hostname };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        // Declara a fila
        _channel.QueueDeclare(queue: queueName,
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
    }

    public void ReceiveMessages(string queueName, Action<string> onMessageReceived)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            onMessageReceived(message);
        };

        _channel.BasicConsume(queue: queueName,
                             autoAck: true,
                             consumer: consumer);
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}