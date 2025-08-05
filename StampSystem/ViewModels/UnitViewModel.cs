using Microsoft.AspNetCore.Mvc.Rendering;
using StampSystem.Data;
using System.Collections.Generic;


public class UnitViewModel
{
    public Unit Unit { get; set; }
    public IEnumerable<SelectListItem> Section { get; set; }
}
