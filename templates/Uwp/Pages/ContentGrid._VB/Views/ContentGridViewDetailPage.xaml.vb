﻿Imports Param_RootNamespace.Services
Imports Microsoft.Toolkit.Uwp.UI.Animations

Namespace Views
    Public NotInheritable Partial Class ContentGridViewDetailPage
        Inherits Page

        Public Sub New()
            InitializeComponent()
        End Sub

        Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
            MyBase.OnNavigatedTo(e)
            RegisterElementForConnectedAnimation("animationKeyContentGridView", itemHero)
            Dim orderID As Long
            orderID = CType(e.Parameter, Long)
            Await ViewModel.InitializeAsync(orderID)
        End Sub

        Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
            MyBase.OnNavigatingFrom(e)
            If e.NavigationMode = NavigationMode.Back Then
                NavigationService.Frame.SetListDataItemForNextConnectedAnimation(ViewModel.Item)
            End If
        End Sub
    End Class
End Namespace
