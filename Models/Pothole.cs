using System;
using System.Collections.Generic;

namespace PotholesApp.Models
{
    public partial class Pothole
    {
        public int Id { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Location { get; set; }
        public byte[]? Img { get; set; }
        public string? Note { get; set; }
        public DateTime DetectedDate { get; set; }
    }
}
