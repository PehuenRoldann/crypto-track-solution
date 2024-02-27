using System;

namespace CryptoTrackApp.src.models
{
    public struct Currency
    {
        public string Id {get; set;}
        public int Rank {get; set;}
        public string Symbol {get; set;}
        public string Name {get; set;}
        public int Supply {get; set;}
        public int MaxSupply {get; set;}
        public double MarketCpaUsd {get; set;}
        public double VolumeUsd24Hr {get; set;}
        public double PriceUsd {get; set;}
        public double ChangePercent {get; set;}
        public double VWap24Hr {get; set;}
    }
}