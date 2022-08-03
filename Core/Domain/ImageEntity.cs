using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace QulixTest.Core.Domain
{
    public class ImageEntity
    {
        public ImageEntity()
        {
            Tags = new ObservableCollection<Tag>();
        }
        public long Id { get; set; }
        public string imageUrl { get; set; }
        public double OriginalSize { get; set; }
        public DateTime CreatedDate { get; set; }
        public double Cost { get; set; }
        public long NumberOfSales { get; set; } = 0;
        public double Rating { get; set; }
        public Author Author { get; set; }

        [ForeignKey(nameof(Author))]
        public string AuthorId { get; set; }
        public virtual ObservableCollection<Tag> Tags { get; set; }

        IApplicationBuilder applicationBuilder { get; set; }

    }
}
