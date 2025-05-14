namespace TradeWave.Models
{
    public class Watchlistuser
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string CoinSymbol { get; set; } = string.Empty;
        public string CoinName { get; set; } = string.Empty;
        public DateTime AddedTime { get; set; }
        public double PriceWhenAdded { get; set; }
    }
}
