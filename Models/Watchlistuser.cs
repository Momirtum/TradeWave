namespace TradeWave.Models
{
    public class Watchlistuser
    {
        public int ID { get; set; }
        public int UserID { get; set; }  // Kullanýcý kimliði
        public string CoinName { get; set; }
        public string CoinSymbol { get; set; }
    }
}