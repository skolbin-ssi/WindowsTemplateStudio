﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Templates.UI.ViewModels.NewItem;

namespace Microsoft.Templates.UI.Views.NewItem
{
    public partial class MainPage : Page
    {
        public MainPage()
        {
            DataContext = MainViewModel.Instance;
            InitializeComponent();
        }

        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (stepFrame.Content == null)
            {
                Services.NavigationService.InitializeSecondaryFrame(stepFrame, new TemplateSelectionPage());
            }

            Services.NavigationService.SubscribeEventHandlers();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Services.NavigationService.UnsubscribeEventHandlers();
        }
    }
}
