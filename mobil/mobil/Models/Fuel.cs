using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mobil.Models
{
    public class Fuel
    {
        public ulong Id { get; set; }
        public DateTime Date { get; set; }
        public string? TotalCostCur { get; set; }
        public decimal Liters { get; set; }
        public string? StationName { get; set; }
        public ulong? ReceiptFileId { get; set; }
        public string? UserEmail { get; set; }
        public string? LicensePlate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class FuelCreate
    {
        public DateTime Date { get; set; }
        public int OdometerKm { get; set; }
        public decimal Liters { get; set; }
        public decimal TotalCost { get; set; }
        public string? StationName { get; set; }
        public string? LocationText { get; set; }
        public FileResult? File { get; set; }
        public ulong? ReceiptFileId { get; set; }
    }
}
