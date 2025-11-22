using System;

namespace JN_ProyectoPrograAvanzadaWeb_G1.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string ActionType { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }

        public string BeforeState { get; set; }
        public string AfterState { get; set; }
        public string Meta { get; set; }

        public string IpAddress { get; set; }
        public string Source { get; set; }

        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
