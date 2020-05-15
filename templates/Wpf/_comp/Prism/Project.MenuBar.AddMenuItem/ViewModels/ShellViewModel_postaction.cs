﻿namespace Param_RootNamespace.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private DelegateCommand _goBackCommand;
//{[{
        private ICommand _menuViewswts.ItemNameCommand;
//}]}
        private ICommand _loadedCommand;
        public DelegateCommand GoBackCommand => _goBackCommand ?? (_goBackCommand = new DelegateCommand(OnGoBack, CanGoBack));
//{[{

        public ICommand MenuViewswts.ItemNameCommand => _menuViewswts.ItemNameCommand ?? (_menuViewswts.ItemNameCommand = new DelegateCommand(OnMenuViewswts.ItemName));
//}]}
        private void OnGoBack()
            => _navigationService.Journal.GoBack();
//^^
//{[{

        private void OnMenuViewswts.ItemName()
            => RequestNavigateAndCleanJournal(PageKeys.wts.ItemName);
//}]}
    }
}
