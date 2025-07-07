using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class EspacioParque
{
    public int Id { get; set; }

    public string? DescripcionEspacio { get; set; }

    public int? CapacidadPersonas { get; set; }

    public int? CapacidadVehiculos { get; set; }

    public double? MetrosAncho { get; set; }

    public double? MetrosLargo { get; set; }

    public double? TotalMetros { get; set; }

    public string? Informacion { get; set; }

    public bool? FeriaUso { get; set; }

    public bool? Estatus { get; set; }

    public bool? Arrendamineto { get; set; }

    public virtual ICollection<FeriaArea> FeriaAreas { get; set; } = new List<FeriaArea>();
}
