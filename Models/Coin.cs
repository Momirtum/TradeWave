namespace TradeWave.Models
{
    public class Coin
    {
        public string CoinName { get; set; }
        public string CoinSymbol { get; set; }
        public DateTime AddedTime { get; set; }
        public double PriceWhenAdded { get; set; }
    }
}
