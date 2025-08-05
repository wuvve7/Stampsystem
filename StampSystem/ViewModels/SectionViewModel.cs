using Microsoft.AspNetCore.Mvc.Rendering;
using StampSystem.Data;
using System.Collections.Generic;  

public class SectionViewModel
{
    public Section Section { get; set; }
    public IEnumerable<SelectListItem> Administrations { get; set; }
}
