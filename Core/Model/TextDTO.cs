using System.ComponentModel.DataAnnotations;

namespace QulixTest.Core.Model;
public class CreateTextDTO
{
    [Required]
    [StringLength(maximumLength: 30, ErrorMessage = "Text name is too long")]
    public string Name { get; set; }

    [Required]
    [StringLength(maximumLength: 10000, ErrorMessage = "TextField name is too long")]
    public string TextField { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public double Cost { get; set; }
}
public class TextDTO : CreateTextDTO
{
    public long Id { get; set; }

}

public class UpdateTextDTO : CreateTextDTO
{

}
