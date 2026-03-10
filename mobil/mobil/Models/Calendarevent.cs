using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace mobil.Models
{
    public class Calendarevent
    {
        public ulong Id { get; set; }
        public string? EventType { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public ulong? RelatedServiceRequestId { get; set; }
    }
}
