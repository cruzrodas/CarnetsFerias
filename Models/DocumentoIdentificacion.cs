using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class DocumentoIdentificacion
{
    public int Id { get; set; }

    public string? TipoIdentificacion { get; set; }

    public bool? Estatus { get; set; }

    public virtual ICollection<FeriaCarnet> FeriaCarnets { get; set; } = new List<FeriaCarnet>();
}
