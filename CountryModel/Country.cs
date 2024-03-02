using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CountryModel;

[Table("Country")]
public partial class Country
{
    [Key]
    public int CountryId { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(2)]
    [Unicode(false)]
    public string Iso2 { get; set; } = null!;

    [StringLength(3)]
    [Unicode(false)]
    public string Iso3 { get; set; } = null!;

    [InverseProperty("CityNavigation")]
    public virtual City? City { get; set; }
}
