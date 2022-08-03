using System.ComponentModel.DataAnnotations;

namespace QulixTest.Core.Model
{
    public class LoginUserDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Your Password is limited to {2} to {1} characters", MinimumLength = 8)]
        public string Password { get; set; }
    }
    public class UserDTO:LoginUserDTO
    {
        [Required(ErrorMessage = "Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]

        public string LastName { get; set; }

        public DateTime BirthDate { get; set; }


        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }

    public class UpdateUserDTO : UserDTO
    {

    }
}
