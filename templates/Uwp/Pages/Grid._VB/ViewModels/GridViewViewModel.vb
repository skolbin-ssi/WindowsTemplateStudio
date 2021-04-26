﻿Imports Param_RootNamespace.Core.Models
Imports Param_RootNamespace.Core.Services

Namespace ViewModels
    Public Class GridViewViewModel
        Inherits System.ComponentModel.INotifyPropertyChanged

        Public Property Source As ObservableCollection(Of SampleOrder) = New ObservableCollection(Of SampleOrder)

        Public Sub New()
        End Sub

        Public Async Function LoadDataAsync() As Task
            Source.Clear()

            ' Replace this with your actual data
            Dim data = Await SampleDataService.GetGridDataAsync()
            For Each item In data
                Source.Add(item)
            Next
        End Function
    End Class
End Namespace
