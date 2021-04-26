﻿Imports WinUI = Microsoft.UI.Xaml.Controls
Imports Windows.System
Imports Param_RootNamespace.Helpers
Imports Param_RootNamespace.Services

Namespace Views
    ' TODO WTS: Change the icons and titles for all NavigationViewItems in ShellPage.xaml.
    Public NotInheritable Partial Class ShellPage
        Inherits Page
        Implements INotifyPropertyChanged

        Private ReadOnly _altLeftKeyboardAccelerator As KeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu)
        Private ReadOnly _backKeyboardAccelerator As KeyboardAccelerator = BuildKeyboardAccelerator(VirtualKey.GoBack)

        Private _isBackEnabled As Boolean
        Private _selected As WinUI.NavigationViewItem

        Public Property IsBackEnabled As Boolean
            Get
                Return _isBackEnabled
            End Get

            Set(value As Boolean)
                [Set](_isBackEnabled, value)
            End Set
        End Property

        Public Property Selected As WinUI.NavigationViewItem
            Get
                Return _selected
            End Get

            Set(value As WinUI.NavigationViewItem)
                [Set](_selected, value)
            End Set
        End Property

        Public Sub New()
            InitializeComponent()
            DataContext = Me
            Initialize()
        End Sub

        Private Sub Initialize()
            NavigationService.Frame = shellFrame
            AddHandler NavigationService.NavigationFailed, Function(sender, e)
                                                                Throw e.Exception
                                                            End Function
            AddHandler NavigationService.Navigated, AddressOf Frame_Navigated
            AddHandler navigationView.BackRequested, AddressOf OnBackRequested
        End Sub

        Private Async Sub OnLoaded(sender As Object, e As RoutedEventArgs)
            ' Keyboard accelerators are added here to avoid showing 'Alt + left' tooltip on the page.
            ' More info on tracking issue https://github.com/Microsoft/microsoft-ui-xaml/issues/8
            keyboardAccelerators.Add(_altLeftKeyboardAccelerator)
            keyboardAccelerators.Add(_backKeyboardAccelerator)
            Await Task.CompletedTask
        End Sub

        Private Sub OnBackRequested(sender As WinUI.NavigationView, args As WinUI.NavigationViewBackRequestedEventArgs)
            NavigationService.GoBack()
        End Sub

        Private Sub Frame_NavigationFailed(sender As Object, e As NavigationFailedEventArgs)
            Throw e.Exception
        End Sub

        Public Sub Frame_Navigated(sender As Object, e As NavigationEventArgs)
            IsBackEnabled = NavigationService.CanGoBack
            Dim selectedItem = GetSelectedItem(navigationView.MenuItems, e.SourcePageType)
            If selectedItem IsNot Nothing Then
                Selected = selectedItem
            End If
        End Sub

        Private Function GetSelectedItem(menuItems As IEnumerable(Of Object), pageType As Type) As WinUI.NavigationViewItem
            For Each item In menuItems.OfType(Of WinUI.NavigationViewItem)()

                If IsMenuItemForPageType(item, pageType) Then
                    Return item
                End If

                Dim selectedChild = GetSelectedItem(item.MenuItems, pageType)

                If selectedChild IsNot Nothing Then
                    Return selectedChild
                End If
            Next

            Return Nothing
        End Function

        Private Function IsMenuItemForPageType(menuItem As WinUI.NavigationViewItem, sourcePageType As Type) As Boolean
            Dim pageType = TryCast(menuItem.GetValue(NavHelper.NavigateToProperty), Type)
            Return pageType = sourcePageType
        End Function

        Private Sub OnItemInvoked(sender As WinUI.NavigationView, args As WinUI.NavigationViewItemInvokedEventArgs)
            If args.IsSettingsInvoked Then
                ' Navigate to the settings page - implement as appropriate if needed
            Else
                Dim selectedItem As WinUI.NavigationViewItem = TryCast(args.InvokedItemContainer, WinUI.NavigationViewItem)
                Dim pageType = TryCast(selectedItem.GetValue(NavHelper.NavigateToProperty), Type)
                If pageType IsNot Nothing Then
                    NavigationService.Navigate(pageType, Nothing, args.RecommendedNavigationTransitionInfo)
                End If
            End If
        End Sub

        Private Function BuildKeyboardAccelerator(key As VirtualKey, Optional modifiers As VirtualKeyModifiers? = Nothing) As KeyboardAccelerator
            Dim keyboardAccelerator = New KeyboardAccelerator() With {
                .Key = key
            }

            If modifiers.HasValue Then
                keyboardAccelerator.Modifiers = modifiers.Value
            End If

            AddHandler keyboardAccelerator.Invoked, AddressOf OnKeyboardAcceleratorInvoked
            Return keyboardAccelerator
        End Function

        Private Overloads Sub OnKeyboardAcceleratorInvoked(sender As KeyboardAccelerator, args As KeyboardAcceleratorInvokedEventArgs)
            Dim result = NavigationService.GoBack()
            args.Handled = result
        End Sub

        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        Private Sub [Set](Of T)(ByRef storage As T, value As T, <CallerMemberName> Optional propertyName As String = Nothing)
            If Equals(storage, value) Then
                Return
            End If

            storage = value
            OnPropertyChanged(propertyName)
        End Sub

        Private Sub OnPropertyChanged(propertyName As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
