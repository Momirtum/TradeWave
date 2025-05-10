namespace TradeWave.Models
{
    public class Watchlistuser
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string CoinSymbol { get; set; } = string.Empty;
        public string CoinName { get; set; } = string.Empty;
        // Add other fields as needed (e.g., current price, etc.)
        public User? User { get; set; }
    }
}