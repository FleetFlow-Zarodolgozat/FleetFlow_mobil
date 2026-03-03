using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace mobil.Models
{
    public class Driver
    {
        [JsonPropertyName("id")]
        public ulong? Id { get; set; }
        [JsonPropertyName("fullname")]
        public string? FullName { get; set; }
        [JsonPropertyName("email")]
        public string? Email { get; set; }
        [JsonPropertyName("phone")]
        public string? Phone { get; set; }
        [JsonPropertyName("licenseNumber")]
        public string? LicenseNumber { get; set; }
        [JsonPropertyName("licenseExpiryDate")]
        public DateTime LicenseExpiryDate { get; set; }
        [JsonPropertyName("profileImgFileId")]
        public ulong? ProfileImgFileId { get; set; }
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }

    public class Vehicle
    {
        [JsonPropertyName("brandModel")]
        public string? BrandModel { get; set; }
        [JsonPropertyName("licensePlate")]
        public string? LicensePlate { get; set; }
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonPropertyName("currentMileageKm")]
        public int CurrentMileageKm { get; set; }
        [JsonPropertyName("vin")]
        public string? Vin { get; set; }
        [JsonPropertyName("status")]
        public string? Status { get; set; }
    }

    public class Stats
    {
        [JsonPropertyName("totalTrips")]
        public int TotalTrips { get; set; }
        [JsonPropertyName("totalDistance")]
        public decimal TotalDistance { get; set; }
        [JsonPropertyName("totalServices")]
        public int TotalServices { get; set; }
        [JsonPropertyName("totalServicesCost")]
        public decimal TotalServicesCost { get; set; }
        [JsonPropertyName("totalFuels")]
        public int TotalFuels { get; set; }
        [JsonPropertyName("totalFuelCost")]
        public decimal TotalFuelCost { get; set; }
    }
}
