﻿'{[{
Imports Param_RootNamespace.Core.Services
'}]}

Public class Tests
    '^^
    '{[{

    ' Remove or update this once your app is using real data and not the SampleDataService.
    ' This test serves only as a demonstration of testing functionality in the Core library.
    <Test>
    Public Async Function EnsureSampleDataServiceReturnsContentGridDataAsync() As Task
        Dim actual = Await SampleDataService.GetContentGridDataAsync()

        Assert.AreNotEqual(0, actual.Count)
    End Function
    '}]}
End Class
