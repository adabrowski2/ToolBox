using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Praca_Inżynierska_v1.Core;
using Praca_Inżynierska_v1.MVVM.Model;
using Praca_Inżynierska_v1.Services;

namespace Praca_Inżynierska_v1.MVVM.ViewModel
{
    internal class StatisticsViewModel : ViewModelBase
    {
        private ObservableCollection<SellingsInfo> _salesResults = new();

        public ObservableCollection<SellingsInfo> SalesResults
        {
            get => _salesResults;
            set
            {
                _salesResults = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _listaProduktow = new();
        public ObservableCollection<string> ListaProduktow
        {
            get => _listaProduktow;
            set
            {
                _listaProduktow = value;
                OnPropertyChanged();
            }
        }

        private string _wybranyProdukt;
        public string WybranyProdukt
        {
            get => _wybranyProdukt;
            set
            {
                if (_wybranyProdukt != value)
                {
                    _wybranyProdukt = value;
                    OnPropertyChanged();
                    ApplyFilters(); // odświeża dane wg nowego filtra
                }
            }
        }

        private DateTime? _dateFrom = new DateTime(DateTime.Now.Year, 1, 1);
        public DateTime? DateFrom
        {
            get => _dateFrom;
            set { _dateFrom = value; OnPropertyChanged(); }
        }

        private DateTime? _dateTo = DateTime.Today;
        public DateTime? DateTo
        {
            get => _dateTo;
            set { _dateTo = value; OnPropertyChanged(); }
        }

        private decimal _lacznaWartosc;
        public decimal LacznaWartosc
        {
            get => _lacznaWartosc;
            set { _lacznaWartosc = value; OnPropertyChanged(); }
        }

        public ICommand FiltrujCommand { get; }
        public ICommand EksportujCommand { get; }
        public ICommand ResetCommand { get; }

        private List<SellingsInfo> _wszystkieDane = new(); // do filtrowania

        public StatisticsViewModel()
        {
            FiltrujCommand = new RelayCommand(_ => LoadData());
            ResetCommand = new RelayCommand(_ => ResetFilters());

            LoadData();
        }

        private void LoadData()
        {
            _wszystkieDane = SellingService.PobierzRaport(DateFrom ?? DateTime.MinValue, DateTo ?? DateTime.MaxValue);

            ListaProduktow = new ObservableCollection<string>(
                _wszystkieDane.Select(x => x.Produkt).Distinct().OrderBy(x => x)
            );

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var data = SellingService.PobierzRaport(DateFrom ?? DateTime.MinValue, DateTo ?? DateTime.MaxValue);

            // Filtrowanie po produkcie
            if (!string.IsNullOrWhiteSpace(WybranyProdukt))
                data = data.Where(d => d.Produkt == WybranyProdukt).ToList();

            // Podświetlenie top 5
            var top5 = data
                .Where(d => string.IsNullOrWhiteSpace(WybranyProdukt) || d.Produkt != WybranyProdukt)
                .OrderByDescending(d => d.WartoscDecimal)
                .Take(5)
                .Select(d => d.Produkt)
                .ToHashSet();

            foreach (var item in data)
                item.IsTopProduct = top5.Contains(item.Produkt);

            SalesResults = new ObservableCollection<SellingsInfo>(data);
            LacznaWartosc = data.Sum(d => d.WartoscDecimal);
        }


        private void ResetFilters()
        {
            DateFrom = new DateTime(DateTime.Now.Year, 1, 1);
            DateTo = DateTime.Today;
            WybranyProdukt = null;
            LoadData();
        }
    }
}