using System.Collections.Generic;
using Newtonsoft.Json;

namespace TradeWave.Models
{
    public class CoinDetailViewModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("market_data")]
        public MarketData MarketData { get; set; }
    }

    public class MarketData
    {
        [JsonProperty("current_price")]
        public Dictionary<string, decimal> CurrentPrice { get; set; }

        [JsonProperty("price_change_percentage_24h")]
        public decimal PriceChangePercentage24h { get; set; }
    }
}
