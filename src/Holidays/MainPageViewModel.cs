using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Holidays
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private MonthModel _monthModel;

        public MainPageViewModel()
        {
            _ = InitializeAsync();

            ChangeMonthCommand = new Command(param =>
            {
                var direction = param?.ToString();
                if (!string.IsNullOrEmpty(direction))
                {
                    var active = direction == "Reduce"
                        ? MonthModel.ActiveMonth.AddMonths(-1)
                        : MonthModel.ActiveMonth.AddMonths(1);
                    MonthModel = new MonthModel
                    {
                        ActiveMonth = active, Holidays = GetHolidaysForMonth(active.Year, active.Month)
                    };
                }
            });
            SetThisMonthCommand = new Command(() =>
            {
                MonthModel = new MonthModel
                {
                    ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    Holidays = GetHolidaysForMonth(DateTime.Now.Year, DateTime.Now.Month)
                };
            });
        }

        private List<AnnualHolidaysModel> AnnualHolidays { get; } = new List<AnnualHolidaysModel>();

        public MonthModel MonthModel
        {
            get => _monthModel;
            set
            {
                _monthModel = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MonthModel)));
            }
        }

        public ICommand ChangeMonthCommand { get; set; }
        public ICommand SetThisMonthCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task InitializeAsync()
        {
            MonthModel = new MonthModel {ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)};

            LoadHolidaysFromCache();
            await UpdateHolidaysCacheAsync();
        }

        private async Task UpdateHolidaysCacheAsync()
        {
            var holidays = await GetAnnualHolidaysFromWeb();
            if (holidays == null)
                return;

            AnnualHolidays.Clear();
            AnnualHolidays.AddRange(holidays);

            if (Application.Current.Properties.ContainsKey("holidays"))
                Application.Current.Properties["holidays"] = JsonConvert.SerializeObject(holidays);
            else
                Application.Current.Properties.Add("holidays", JsonConvert.SerializeObject(holidays));
            await Application.Current.SavePropertiesAsync();

            MonthModel = new MonthModel
            {
                ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Holidays = GetHolidaysForMonth(DateTime.Now.Year, DateTime.Now.Month)
            };
        }

        private void LoadHolidaysFromCache()
        {
            if (Application.Current.Properties.TryGetValue("holidays", out var rawHolidays) &&
                rawHolidays is string serialHolidays)
            {
                var holidays = JsonConvert.DeserializeObject<IEnumerable<AnnualHolidaysModel>>(serialHolidays,
                    new JsonSerializerSettings
                    {
                        Culture = CultureInfo.InvariantCulture
                    });

                if (holidays == null)
                    return;

                AnnualHolidays.Clear();
                AnnualHolidays.AddRange(holidays);

                MonthModel = new MonthModel
                {
                    ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    Holidays = GetHolidaysForMonth(DateTime.Now.Year, DateTime.Now.Month)
                };
            }
        }

        private async Task<IEnumerable<AnnualHolidaysModel>> GetAnnualHolidaysFromWeb()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(
                "https://github.com/rizanzaky/HolidaysApp/blob/main/src/Holidays/GetAnnualHolidays.json?raw=true");
            if (!response.IsSuccessStatusCode) return null;

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<AnnualHolidaysModel>>(result,
                new JsonSerializerSettings
                {
                    Culture = CultureInfo.InvariantCulture
                });
        }

        private IEnumerable<MonthHolidaysModel> GetHolidaysForMonth(int year, int month)
        {
            return AnnualHolidays.FirstOrDefault(f => f.Year == year)?.Holidays
                .Where(w => w.Date.Month == month);
        }
    }
}