using System;
using System.Collections.Generic;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models.ViewModels
{
    public class UsuarioKpiDto
    {
        public int UsuarioID { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
        public string RolNombre { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
    }

    public class ReportesKPIViewModel
    {
        public IEnumerable<UsuarioKpiDto> Usuarios { get; set; }
    }
}
