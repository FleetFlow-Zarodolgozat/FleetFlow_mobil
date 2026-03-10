using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace mobil.Models
{
    public class Service
    {
        public ulong Id { get; set; }
        public string? UserEmail { get; set; }
        public string? LicensePlate { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public DateTime? ScheduledStart { get; set; }
        public decimal? DriverReportCost { get; set; }
        public ulong? InvoiceFileId { get; set; }
        public DateTime? ClosedAt { get; set; }
    }

    public class ServiceCreate
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
    }

    public class ServiceDetailUpload
    {
        public decimal DriverReportCost { get; set; }
        public string? DriverCloseNote { get; set; }
        public FileResult? File { get; set; }
    }
}
