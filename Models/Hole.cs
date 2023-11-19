using System;
using System.Collections.Generic;

namespace PotholesApp.Models
{
    public partial class Hole
    {
        public int Id { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Location { get; set; }
        public string? Img { get; set; }
        public DateTime DetectDate { get; set; }
    }
}
