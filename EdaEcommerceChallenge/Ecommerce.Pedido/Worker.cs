using Ecommerce.Infrastructure.Broker.RabbitMq;
using Ecommerce.Pedido.Domain;
using Ecommerce.Pedido.Domain.Entities;
using Ecommerce.Pedido.Messages;
using System.Text.Json;

namespace Ecommerce.Pedido
{
    public class Worker : BackgroundService
    {
        private readonly MessageReceiver _messageReceiver;

        private readonly MessageSender _messageSender;

        public Worker()
        {
            _messageReceiver = new MessageReceiver("localhost", "criar-pedido");

            _messageSender = new MessageSender("localhost", "pedido-criado");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageReceiver.ReceiveMessages("criar-pedido", (message) =>
            {
                var pedido = JsonSerializer.Deserialize<PedidoCompleto>(message);
                _messageSender.SendMessage<PedidoCompleto>("pedido-criado", pedido);
                Console.WriteLine($"Pedido {pedido?.Id} criado com sucesso.");
            });

            _messageReceiver.ReceiveMessages("pedido-reservado", (message) =>
            {
                var analiseEstoqueResultado = JsonSerializer.Deserialize<ProcessamentoEstoque>(message);
                Console.WriteLine($"Pedido {analiseEstoqueResultado?.PedidoId} atualizado como 'pedido-reservado'.");
            });

            _messageReceiver.ReceiveMessages("pedido-recusado", (message) =>
            {
                var analiseEstoqueResultado = JsonSerializer.Deserialize<ProcessamentoEstoque>(message);
                Console.WriteLine($"Pedido {analiseEstoqueResultado?.PedidoId} atualizado como 'pedido-recusado'.");
            });

            // Mantém o worker rodando
            await Task.CompletedTask;
        }

        public override void Dispose()
        {
            _messageReceiver.Dispose();
            base.Dispose();
        }
    }
}
