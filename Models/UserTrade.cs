namespace TradeWave.Models
{
    public class UserTrade
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public string CoinName { get; set; }
        public string CoinSymbol { get; set; }
        public DateTime AddedTime { get; set; }
        public double PriceWhenAdded { get; set; }
        public double CurrentPrice { get; set; }
        public double PriceChangePercentage24h { get; set; }
        public double TotalInvestment { get; set; }
        public double CurrentValue { get; set; }
        public double ProfitLoss { get; set; }
        public string TradeType { get; set; }
    }
}