using System.Collections.Generic;
using Blog.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Data.Mappings{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("User");
            
            //Mapeamento
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

            builder.Property(x => x.Name)
            .IsRequired()
            .HasColumnName("Name")
            .HasColumnType("NVARCHAR")
            .HasMaxLength(80);

            builder.Property(x => x.Bio)
            .IsRequired(false);

            builder.Property(x => x.Email)
            .IsRequired()
            .HasColumnName("Email")
            .HasColumnType("VARCHAR")
            .HasMaxLength(160);

            builder.Property(x => x.Image)
            .IsRequired(false);

            builder.Property(x => x.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasColumnType("VARCHAR")
            .HasMaxLength(255);

            builder.Property(x => x.Slug)
            .IsRequired()
            .HasColumnName("Slug")
            .HasColumnType("VARCHAR")
            .HasMaxLength(80);

            //�ndices
            builder.HasIndex(x => x.Slug, "IX_User_Slug").IsUnique();

            //Relacionamentos
            builder.HasMany(x=> x.Roles)
            .WithMany(x=> x.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserRole",
                user => user.HasOne<Role>()
                .WithMany()
                .HasForeignKey("RoleId")
                .HasConstraintName("FK_UserRole_RoleId")
                .OnDelete(DeleteBehavior.Cascade),
                role => role.HasOne<User>()
                .WithMany()
                .HasForeignKey("UserId")
                .HasConstraintName("FK_UserRole_UserId")
                .OnDelete(DeleteBehavior.Cascade)
            );
        }
    }
}