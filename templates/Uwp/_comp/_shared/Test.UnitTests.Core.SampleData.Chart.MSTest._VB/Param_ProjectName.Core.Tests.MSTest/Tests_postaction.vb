﻿'{[{
Imports Param_RootNamespace.Core.Services
'}]}

Public Class Tests
    '^^
    '{[{

    ' Remove or update this once your app is using real data and not the SampleDataService.
    ' This test serves only as a demonstration of testing functionality in the Core library.
    <TestMethod>
    Public Async Function EnsureSampleDataServiceReturnsChartDataAsync() As Task
        Dim actual = Await SampleDataService.GetChartDataAsync()

        Assert.AreNotEqual(0, actual.Count)
    End Function
    '}]}
End Class
