using QulixTest.Core.Domain;
using QulixTest.Persistence.Repositories;

namespace QulixTest.Core.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        GenericRepository<Tag> Tags{ get; }
        GenericRepository<ImageEntity> Files { get; }
        GenericRepository<Author> Authors { get; }
        GenericRepository<Text> Texts{ get; }

        Task SaveAsync();

    }
}
