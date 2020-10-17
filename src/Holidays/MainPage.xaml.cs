using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace Holidays
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            var vm = new MainPageViewModel();
            UpdateMonth(vm);
            vm.PropertyChanged += OnPropertyChanged;
            BindingContext = vm;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is MainPageViewModel vm) UpdateMonth(vm);
        }

        private void UpdateMonth(MainPageViewModel vm)
        {
            var days = calendarView.Children.Skip(7).OfType<Label>().ToList();

            var count = 1;
            var dayOfWeek = vm.ActiveMonth.DayOfWeek == 0 ? 7 : (int) vm.ActiveMonth.DayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(vm.ActiveMonth.Year, vm.ActiveMonth.Month) + dayOfWeek - 1;
            for (var i = dayOfWeek - 1; i < daysInMonth; i++)
            {
                days[i].Text = $"{count}";
                count++;
            }
        }
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        private DateTime _activeMonth;

        public MainPageViewModel()
        {
            ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            ChangeMonthCommand = new Command(param =>
            {
                var direction = param?.ToString();
                if (!string.IsNullOrEmpty(direction))
                    ActiveMonth = direction == "Reduce" ? ActiveMonth.AddMonths(-1) : ActiveMonth.AddMonths(1);
            });
            SetThisMonthCommand = new Command(() =>
            {
                ActiveMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            });
        }

        public DateTime ActiveMonth
        {
            get => _activeMonth;
            set
            {
                _activeMonth = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveMonth)));
            }
        }

        public ICommand ChangeMonthCommand { get; set; }
        public ICommand SetThisMonthCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}