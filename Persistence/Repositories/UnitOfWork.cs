using QulixTest.Core.Domain;
using QulixTest.Core.IRepositories;

namespace QulixTest.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly DatabaseDbContext _context;

        private GenericRepository<Tag> _tags;

        private GenericRepository<ImageEntity> _files;

        private GenericRepository<Author> _users;

        private GenericRepository<Text> _texts;

        public GenericRepository<Tag> Tags => _tags ??= new GenericRepository<Tag>(_context);

        public GenericRepository<ImageEntity> Files => _files ??= new GenericRepository<ImageEntity>(_context);

        public GenericRepository<Author> Authors => _users ??= new GenericRepository<Author>(_context);

        public GenericRepository<Text> Texts => _texts ??= new GenericRepository<Text>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
