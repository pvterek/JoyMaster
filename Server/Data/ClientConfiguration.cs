using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Models;

namespace Server.Data;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("Clients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName(nameof(Client.Id));

        builder.Property(x => x.Name)
            .HasColumnName(nameof(Client.Name))
            .IsRequired();

        builder.Property(x => x.IpAddress)
            .HasColumnName(nameof(Client.IpAddress))
            .IsRequired();

        builder.Property(x => x.LastConnectionDate)
            .HasColumnName(nameof(Client.LastConnectionDate))
            .IsRequired();
    }
}
