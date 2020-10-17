using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public MainPageViewModel()
        {
            _ = InitializeAsync();

            ChangeMonthCommand = new Command(param =>
            {
                var direction = param?.ToString();
                if (!string.IsNullOrEmpty(direction))
                    ActiveMonth = direction == "Reduce" ? ActiveMonth.AddMonths(-1) : ActiveMonth.AddMonths(1);
                Reload(GetHolidaysForMonth());
            });
            SetThisMonthCommand = new Command(() =>
            {
                ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                Reload(GetHolidaysForMonth());
            });
        }

        private List<AnnualHolidaysModel> AnnualHolidays { get; } = new List<AnnualHolidaysModel>();

        public DateTime ActiveMonth
        {
            get => _activeMonth;
            set
            {
                _activeMonth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveMonth)));
            }
        }

        public ObservableCollection<MonthHolidaysModel> Holidays { get; } =
            new ObservableCollection<MonthHolidaysModel>();

        public ICommand ChangeMonthCommand { get; set; }
        public ICommand SetThisMonthCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private async Task InitializeAsync()
        {
            ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            await LoadAnnualHolidaysAsync();
            Reload(GetHolidaysForMonth());
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
                var holidays = JsonConvert.DeserializeObject<IEnumerable<AnnualHolidaysModel>>(result, new JsonSerializerSettings
                {
                    Culture = CultureInfo.InvariantCulture
                });
                AnnualHolidays.AddRange(holidays);
            }
        }

        private IEnumerable<MonthHolidaysModel> GetHolidaysForMonth()
        {
            return AnnualHolidays.FirstOrDefault(f => f.Year == ActiveMonth.Year)?.Holidays
                .Where(w => w.Date.Month == ActiveMonth.Month);
        }

        private void Reload(IEnumerable<MonthHolidaysModel> monthHolidays)
        {
            Holidays.Clear();
            foreach (var monthHoliday in monthHolidays) Holidays.Add(monthHoliday);
        }
    }
}