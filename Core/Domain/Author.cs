using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace QulixTest.Core.Domain
{
    public class Author : IdentityUser
    {
        public Author()
        {
            Images = new ObservableCollection<ImageEntity>();
            Texts = new ObservableCollection<Text>();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisteredDate { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual ObservableCollection<ImageEntity> Images { get; set; }
        public virtual ObservableCollection<Text> Texts{ get; set; }
    }
}
