using System;
using System.Collections.Generic;
using System.Text;

namespace mobil.Models
{
    public class Notification
    {
        public ulong Id { get; set; }

        public ulong UserId { get; set; }

        public string? Type { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public bool IsRead { get; set; }

        public ulong? RelatedServiceRequestId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
