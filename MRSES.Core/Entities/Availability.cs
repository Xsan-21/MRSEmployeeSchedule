﻿namespace MRSES.Core.Entities
{
    public class Availability : IAvailability
    {
        public string ObjectId { get; set; }
        public string Employee { get; set; }
        public string Monday { get; set; }
        public string Tuesday { get; set; }
        public string Wednesday { get; set; }
        public string Thursday { get; set; }
        public string Friday { get; set; }
        public string Saturday { get; set; }
        public string Sunday { get; set; }
    }

    public interface IAvailability
    {
        string ObjectId { get; set; }
        string Employee { get; set; }
        string Monday { get; set; }
        string Tuesday { get; set; }
        string Wednesday { get; set; }
        string Thursday { get; set; }
        string Friday { get; set; }
        string Saturday { get; set; }
        string Sunday { get; set; }
    }
}
