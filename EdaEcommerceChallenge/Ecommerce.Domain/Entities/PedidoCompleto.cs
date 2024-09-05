using Ecommerce.Domain.Enums;

namespace Ecommerce.Pedido.Domain.Entities;

public sealed record PedidoCompleto(
    Guid Id,
    Endereco Endereco,
    Pagamento Pagamento,
    StatusPedido Status);