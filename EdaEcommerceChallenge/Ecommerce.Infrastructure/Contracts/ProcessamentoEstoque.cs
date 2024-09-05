using Ecommerce.Domain.Enums;

namespace Ecommerce.Pedido.Messages
{
    public sealed record ProcessamentoEstoque(
        Guid PedidoId, 
        StatusPedido Status);
}