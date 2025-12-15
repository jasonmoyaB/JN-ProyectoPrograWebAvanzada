


namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    public class AuditoriaLog
    {
        public int AuditoriaID { get; set; }
        public string Usuario { get; set; }
        public string Accion { get; set; }
        public string Entidad { get; set; }
        public int EntidadID { get; set; }
        public string IP { get; set; }
        public DateTime Fecha { get; set; }
    }
}
