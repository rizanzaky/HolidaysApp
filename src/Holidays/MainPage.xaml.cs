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
            BindingContext = new MainPageViewModel();
            //SetBinding(BindingContextProperty, new Binding());
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (BindingContext is MainPageViewModel vm)
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
    }

    public class MainPageViewModel : INotifyPropertyChanged
    {
        public DateTime ActiveMonth { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        
        public event PropertyChangedEventHandler PropertyChanged;
    }
}