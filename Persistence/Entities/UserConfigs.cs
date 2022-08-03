using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QulixTest.Core.Domain;

namespace QulixTest.Persistence.Entities;

public class UserConfigs : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasData(
            new Author
            {
                FirstName = "Jack",
                LastName = "Joe",
                UserName = "_jack",
                PasswordHash = "Password123",
                Email = "jackjoe@gmail.com",
                /// UserStatus = true
            },
            new Author
            {
                FirstName = "Broun",
                LastName = "James",
                UserName = "_broun",
                PasswordHash = "Password123",
                Email = "brouns@gmail.com",
                // UserStatus = true
            },
            new Author
            {
                FirstName = "Guest",
                LastName = "Guest",
                UserName = "SuperUser",
                PasswordHash = "Guest123",
                Email = "noEmail@gmail.com",
                /// UserStatus = true,
            }

            );

    }
}
