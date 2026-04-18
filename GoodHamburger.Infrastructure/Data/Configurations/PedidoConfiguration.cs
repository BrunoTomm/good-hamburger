using GoodHamburger.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Data.Configurations;

public sealed class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.UsuarioId).IsRequired();
        builder.Property(p => p.CriadoEm).IsRequired();
        builder.Property(p => p.AtualizadoEm);
        builder.Property(p => p.DeletadoEm);

        builder.HasQueryFilter(p => p.DeletadoEm == null);

        builder.OwnsMany(p => p.Itens, itens =>
        {
            itens.ToTable("ItensPedido");
            itens.WithOwner().HasForeignKey("PedidoId");
            itens.Property(i => i.Tipo).IsRequired();
            itens.Property(i => i.Nome).IsRequired().HasMaxLength(100);
            itens.Property(i => i.Preco).HasPrecision(10, 2).IsRequired();
        });

        builder.Ignore(p => p.Subtotal);
        builder.Ignore(p => p.PercentualDesconto);
        builder.Ignore(p => p.Desconto);
        builder.Ignore(p => p.Total);
        builder.Ignore(p => p.DomainEvents);
    }
}
