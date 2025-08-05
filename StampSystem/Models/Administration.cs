using System.ComponentModel.DataAnnotations;

public class Administration
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    
    public string ManagerName { get; set; }

    
    public ICollection<Section> Sections { get; set; }
}
