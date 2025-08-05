using System.ComponentModel.DataAnnotations;

public class Unit
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    // رئيس الوحدة
    public string HeadName { get; set; }

    // الربط بالقسم
    public int SectionId { get; set; }
    public Section Section { get; set; }
}
