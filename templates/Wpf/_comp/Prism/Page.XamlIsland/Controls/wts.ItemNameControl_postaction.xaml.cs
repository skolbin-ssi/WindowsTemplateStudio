﻿namespace Param_RootNamespace.Controls
{
    public partial class wts.ItemNameControl : UserControl
    {
        public wts.ItemNameControl()
        {
//^^
//{[{
            _themeSelectorService = ((App)Application.Current).Container.Resolve(typeof(IThemeSelectorService)) as IThemeSelectorService;
//}]}
            _themeSelectorService.ThemeChanged += OnThemeChanged;
        }
    }
}