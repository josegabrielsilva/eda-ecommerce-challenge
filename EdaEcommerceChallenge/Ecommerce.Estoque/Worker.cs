using Ecommerce.Domain.Enums;
using Ecommerce.Infrastructure.Broker.RabbitMq;
using Ecommerce.Pedido.Domain.Entities;
using Ecommerce.Pedido.Messages;
using System.Text.Json;

namespace Ecommerce.Estoque
{
    public class Worker : BackgroundService
    {
        private readonly MessageReceiver _messageReceiver;

        private readonly MessageSender _messageSenderPedidoReservado;

        private readonly MessageSender _messageSenderPedidoRecusado;

        public Worker()
        {
            _messageReceiver = new MessageReceiver("localhost", "pedido-criado");

            _messageSenderPedidoReservado = new MessageSender("localhost", "pedido-reservado");

            _messageSenderPedidoRecusado = new MessageSender("localhost", "pedido-recusado"); _messageSenderPedidoReservado = new MessageSender("localhost", "pedido-reservado");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _messageReceiver.ReceiveMessages("pedido-criado", (message) =>
            {
                var pedido = JsonSerializer.Deserialize<PedidoCompleto>(message);

                if(pedido.Id.Equals("7e75c88d-9781-4692-ab8e-e22650aafe1e"))
                    _messageSenderPedidoReservado.SendMessage(
                        "pedido-reservado", 
                        new ProcessamentoEstoque(pedido.Id, StatusPedido.Reservado));
                else
                    _messageSenderPedidoReservado.SendMessage(
                        "pedido-recusado",
                        new ProcessamentoEstoque(pedido.Id, StatusPedido.Recusado));

                Console.WriteLine($"Verificação estoque p/ pedido {pedido?.Id} finalizada.");
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
