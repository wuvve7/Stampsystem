using System.ComponentModel.DataAnnotations;

public class Section
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    
    public string HeadName { get; set; }

    // الربط بالإدارة
    public int AdministrationId { get; set; }
    public Administration Administration { get; set; }

    // علاقات: تحتوي على وحدات
    public ICollection<Unit> Units { get; set; }
}
