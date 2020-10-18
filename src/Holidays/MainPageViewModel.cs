using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Holidays
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private DateTime _activeMonth;
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

                // Reload(GetHolidaysForMonth());
            });
            SetThisMonthCommand = new Command(() =>
            {
                // ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                // Reload(GetHolidaysForMonth());
                MonthModel = new MonthModel
                {
                    ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                    Holidays = GetHolidaysForMonth(DateTime.Now.Year, DateTime.Now.Month)
                };
            });
        }

        private List<AnnualHolidaysModel> AnnualHolidays { get; } = new List<AnnualHolidaysModel>();

        // public DateTime ActiveMonth
        // {
        //     get => _activeMonth;
        //     set
        //     {
        //         _activeMonth = value;
        //         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveMonth)));
        //     }
        // }

        // public ObservableCollection<MonthHolidaysModel> Holidays { get; } =
        //     new ObservableCollection<MonthHolidaysModel>();

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
            //
            MonthModel = new MonthModel {ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)};

            await LoadAnnualHolidaysAsync();
            // Reload(GetHolidaysForMonth());
            MonthModel = new MonthModel
            {
                ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                Holidays = GetHolidaysForMonth(DateTime.Now.Year, DateTime.Now.Month)
            };
        }

        private async Task LoadAnnualHolidaysFromWeb()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(
                "https://github.com/rizanzaky/HolidaysApp/blob/main/src/Holidays/GetAnnualHolidays.json?raw=true");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                var holidays = JsonConvert.DeserializeObject<IEnumerable<AnnualHolidaysModel>>(result,
                    new JsonSerializerSettings
                    {
                        Culture = CultureInfo.InvariantCulture
                    });
                AnnualHolidays.AddRange(holidays);
            }
        }

        private async Task LoadAnnualHolidaysAsync()
        {
            await Task.Delay(3000);

            var assembly = Assembly.GetExecutingAssembly();
            const string resourceName = "Holidays.GetAnnualHolidays.json";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                var result = reader.ReadToEnd();
                var holidays = JsonConvert.DeserializeObject<IEnumerable<AnnualHolidaysModel>>(result,
                    new JsonSerializerSettings
                    {
                        Culture = CultureInfo.InvariantCulture
                    });
                AnnualHolidays.AddRange(holidays);
            }
        }

        private IEnumerable<MonthHolidaysModel> GetHolidaysForMonth(int year, int month)
        {
            return AnnualHolidays.FirstOrDefault(f => f.Year == year)?.Holidays
                .Where(w => w.Date.Month == month);
        }

        // private void Reload(IEnumerable<MonthHolidaysModel> monthHolidays)
        // {
        //     Holidays.Clear();
        //     if (monthHolidays == null)
        //     {
        //         return;
        //     }
        //
        //     monthHolidays = monthHolidays.OrderBy(o => o.Date);
        //     foreach (var monthHoliday in monthHolidays) Holidays.Add(monthHoliday);
        // }
    }

    public class MonthModel
    {
        public DateTime ActiveMonth { get; set; }
        public IEnumerable<MonthHolidaysModel> Holidays { get; set; }
    }
}