using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json.Serialization;

namespace mobil.Models
{
    public class Driver
    {
        public ulong? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? LicenseNumber { get; set; }
        public DateTime LicenseExpiryDate { get; set; }
        public ulong? ProfileImgFileId { get; set; }
        public string? Role { get; set; }
    }

    public class Vehicle
    {
        public string? BrandModel { get; set; }
        public string? LicensePlate { get; set; }
        public int Year { get; set; }
        public int CurrentMileageKm { get; set; }
        public string? Vin { get; set; }
        public string? Status { get; set; }
    }

    public class Stats
    {
        public int TotalTrips { get; set; }
        public decimal TotalDistance { get; set; }
        public int TotalServices { get; set; }
        public decimal TotalServicesCost { get; set; }
        public int TotalFuels { get; set; }
        public decimal TotalFuelCost { get; set; }
    }

    public class EditProfileData
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Password { get; set; }
        public string? PasswordAgain { get; set; }
        public FileResult? File { get; set; }
    }
}
