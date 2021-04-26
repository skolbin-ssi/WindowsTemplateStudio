﻿//{[{
using System.Runtime.CompilerServices;
using System.ComponentModel;
//}]}

namespace Param_RootNamespace.Views
{
//{??{
    public partial class wts.ItemNamePage : Page, INotifyPropertyChanged
//}??}
    {
//{??{
        public wts.ItemNamePage()
//}??}
        {
            InitializeComponent();
//^^
//{[{
            DataContext = this;
//}]}
        }
//^^

//{[{
        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return;
            }

            storage = value;
            OnPropertyChanged(propertyName);
        }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//}]}
    }
}
