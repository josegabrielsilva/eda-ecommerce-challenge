namespace Ecommerce.Pedido.Domain.Entities;

public sealed record Pagamento(
    string NumeroCartao,
    string Bandeira,
    decimal ValorCompra);