using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class FeriaArea
{
    public int Id { get; set; }

    public string? DescripcionArea { get; set; }

    public string? CantidadStand { get; set; }

    public int? IdFeria { get; set; }

    public int? IdEspacio { get; set; }

    public bool? Estatus { get; set; }

    public bool? Eliminada { get; set; }

    public virtual ICollection<FeriaCarnet> FeriaCarnets { get; set; } = new List<FeriaCarnet>();

    public virtual EspacioParque? IdEspacioNavigation { get; set; }

    public virtual Ferium? IdFeriaNavigation { get; set; }
}
