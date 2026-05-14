namespace Praca_Inżynierska_v1.MVVM.Model
{
    class SellingsInfo
    {
        public string Produkt { get; set; }
        public int Ilosc { get; set; }
        public string Wartosc { get; set; }
        public decimal CenaJednostkowa { get; set; }
        public decimal? Rabat { get; set; }
        public int LiczbaZamowien { get; set; }
        public decimal WartoscDecimal { get; set; }
        public bool IsTopProduct { get; set; }
    }
}
