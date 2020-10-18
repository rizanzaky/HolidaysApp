using System;
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
            if (sender is MainPageViewModel vm && e.PropertyName == nameof(MainPageViewModel.MonthModel)) UpdateMonth(vm);
        }

        private void UpdateMonth(MainPageViewModel vm)
        {
            var days = calendarView.Children.Skip(7).OfType<Label>().ToList();

            var dayOfWeek = vm.MonthModel.ActiveMonth.DayOfWeek == 0 ? 7 : (int) vm.MonthModel.ActiveMonth.DayOfWeek;
            var count = 2 - dayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(vm.MonthModel.ActiveMonth.Year, vm.MonthModel.ActiveMonth.Month);
            foreach (var day in days)
            {
                var dayHolidays = vm.MonthModel.Holidays?.Where(w => w.Date.Day == count).OrderBy(o => o.Type).FirstOrDefault();

                day.BackgroundColor = dayHolidays == null ? Color.Blue : Color.ForestGreen;
                
                day.Text = count > 0 && count <= daysInMonth ? $"{count}" : string.Empty;
                count++;
            }

            // if (vm.ActiveMonth.Year == DateTime.Now.Year && vm.ActiveMonth.Month == DateTime.Now.Month)
            // {
            //     calendarView.Children[0] = new Frame
            //     {
            //         HasShadow = false,
            //         BorderColor = Color.ForestGreen,
            //         Content = calendarView.Children[0]
            //     };
            // }
        }
    }
}