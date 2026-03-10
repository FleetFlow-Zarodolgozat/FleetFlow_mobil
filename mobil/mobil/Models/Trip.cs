using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mobil.Models
{
    public class Trip
    {
        public ulong Id { get; set; }
        public string? UserEmail { get; set; }
        public string? LicensePlate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan? Long { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public decimal? DistanceKm { get; set; }
        public string? Notes { get; set; }
    }

    public class TripCreate
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? StartLocation { get; set; }
        public string? EndLocation { get; set; }
        public decimal DistanceKm { get; set; }
        public int StartOdometerKm { get; set; }
        public int EndOdometerKm { get; set; }
        public string? Notes { get; set; }
    }
}
