using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace mobil.Models
{
    public class Calendarevent
    {
        [JsonPropertyName("id")]
        public ulong Id { get; set; }
        [JsonPropertyName("eventType")]
        public string? EventType { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("startAt")]
        public DateTime StartAt { get; set; }
        [JsonPropertyName("endAt")]
        public DateTime? EndAt { get; set; }
        [JsonPropertyName("relatedServiceRequestId")]
        public ulong? RelatedServiceRequestId { get; set; }
    }
}
