using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
            if (sender is MainPageViewModel vm && e.PropertyName == nameof(MainPageViewModel.MonthModel))
                UpdateMonth(vm);
        }

        private void UpdateMonth(MainPageViewModel vm)
        {
            var days = calendarView.Children.Skip(7).OfType<Label>().ToList();

            var dayOfWeek = vm.MonthModel.ActiveMonth.DayOfWeek == 0 ? 7 : (int) vm.MonthModel.ActiveMonth.DayOfWeek;
            var count = 2 - dayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(vm.MonthModel.ActiveMonth.Year, vm.MonthModel.ActiveMonth.Month);
            foreach (var day in days)
            {
                var dayHolidays = vm.MonthModel.Holidays?.Where(w => w.Date.Day == count).ToList();

                day.BackgroundColor = dayHolidays?.Any() ?? false
                    ? GetHolidayColor(dayHolidays)
                    : GetColor(dayOfWeek, count);

                day.Text = count > 0 && count <= daysInMonth ? $"{count}" : string.Empty;
                if (vm.MonthModel.ActiveMonth.Year == DateTime.Now.Year &&
                    vm.MonthModel.ActiveMonth.Month == DateTime.Now.Month && DateTime.Now.Day == count)
                {
                    day.FontAttributes = FontAttributes.Bold;
                }
                else
                {
                    day.FontAttributes = FontAttributes.None;
                }
                count++;
            }
        }

        private Color GetHolidayColor(IEnumerable<MonthHolidaysModel> dayHolidays)
        {
            var isPoya = dayHolidays.OrderBy(o => o.Type).First().Type.StartsWith("F");
            return isPoya ? Color.Yellow : Color.PaleVioletRed;
        }

        private Color GetColor(int dayOfWeek, int day)
        {
            var sunday = dayOfWeek == 1 ? 0 : 7 - dayOfWeek + 1;
            var saturday = dayOfWeek == 7 ? 0 : 7 - dayOfWeek;
            if (day % 7 == sunday) return Color.LightPink;
            if (day % 7 == saturday) return Color.LightSkyBlue;
            return Color.LightGray;
        }
    }
}