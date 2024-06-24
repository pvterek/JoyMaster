using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Server.Models;

namespace Server.Data;

public class ConnectionConfiguration : IEntityTypeConfiguration<Connection>
{
    public void Configure(EntityTypeBuilder<Connection> builder)
    {
        builder.ToTable("Connections");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ConnectedTime)
            .HasColumnName(nameof(Connection.ConnectedTime));

        builder.Property(x => x.DisconnectedTime)
            .HasColumnName(nameof(Connection.DisconnectedTime));

        builder.HasOne(x => x.Client)
            .WithMany(x => x.Connection)
            .HasForeignKey(x => x.ClientId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
