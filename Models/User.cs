﻿namespace TradeWave.Models
{
    public class User
    {
        public  int ID { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Password { get; set; }
        public string? Email { get; set; }
        public required DateTime CreationDate { get; set; }
    }
}
