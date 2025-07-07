using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class UsoDeEspacio
{
    public int IdEspacioUso { get; set; }

    public string? DescripcionUso { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<VentasEspacioParque> VentasEspacioParques { get; set; } = new List<VentasEspacioParque>();
}
