namespace TradeWave.Models
{
    public class Watchlistuser
    {
        public int ID { get; set; }

        // Foreign Key
        public int UserID { get; set; }
        public User? User { get; set; }

        // Coin Details
        public string CoinId { get; set; } = string.Empty; // CoinGecko'dan alınan eşsiz ID
        public string CoinSymbol { get; set; } = string.Empty;
        public string CoinName { get; set; } = string.Empty;
    }
}
