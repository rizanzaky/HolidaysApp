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
            if (sender is MainPageViewModel vm) UpdateMonth(vm);
        }

        private void UpdateMonth(MainPageViewModel vm)
        {
            var days = calendarView.Children.Skip(7).OfType<Label>().ToList();

            var dayOfWeek = vm.ActiveMonth.DayOfWeek == 0 ? 7 : (int) vm.ActiveMonth.DayOfWeek;
            var count = 2 - dayOfWeek;
            var daysInMonth = DateTime.DaysInMonth(vm.ActiveMonth.Year, vm.ActiveMonth.Month);
            foreach (var day in days)
            {
                day.Text = count > 0 && count <= daysInMonth ? $"{count}" : string.Empty;
                count++;
            }
        }
    }
}