using System.Collections.ObjectModel;

namespace QulixTest.Core.Domain
{
    public class Tag
    {
        public Tag()
        {
            Images = new ObservableCollection<ImageEntity>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public virtual ObservableCollection<ImageEntity> Images{ get; set; }
    }
}
