using System;
using System.Collections.Generic;

namespace CarnetsdeFeria.Models;

public partial class FeriaCarnet
{
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public int? IdParticipacion { get; set; }

    public int IdDocIdentificacion { get; set; }

    public int NoIdenitficacion { get; set; }

    public int? NoBoleta { get; set; }

    public bool BoletaCortesia { get; set; }

    public string? Foto { get; set; }

    public DateOnly? FechaCreacion { get; set; }

    public DateOnly? FechaModificacion { get; set; }

    public bool? Impresion { get; set; }

    public int? IdFeriaArea { get; set; }

    public int? NoStand { get; set; }

    public bool? Estatus { get; set; }

    public virtual DocumentoIdentificacion IdDocIdentificacionNavigation { get; set; } = null!;

    public virtual FeriaArea? IdFeriaAreaNavigation { get; set; }

    public virtual Participacion? IdParticipacionNavigation { get; set; }
}
