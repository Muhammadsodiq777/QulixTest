using System.ComponentModel.DataAnnotations.Schema;

namespace QulixTest.Core.Domain
{
    public class Text
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string TextField { get; set; }

        public DateTime CreatedDate { get; set; }

        public double Cost { get; set; }

        public long NumberOfSales { get; set; } = 0;

        public double Rating { get; set; }

        public Author Author { get; set; }

        [ForeignKey(nameof(Author))]

        public string AuthorId { get; set; }


    }
}
