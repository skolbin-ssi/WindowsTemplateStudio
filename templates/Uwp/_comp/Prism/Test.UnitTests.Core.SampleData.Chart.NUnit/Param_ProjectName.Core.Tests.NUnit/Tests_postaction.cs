﻿//{[{
using System.Threading.Tasks;
using Param_RootNamespace.Core.Services;
//}]}

namespace Param_RootNamespace.Core.Tests.NUnit
{
    public class Tests
    {
        //^^
        //{[{

        // Remove or update this once your app is using real data and not the SampleDataService
        // This test serves only as a demonstration of testing functionality in the Core library
        [Test]
        public async Task EnsureSampleDataServiceReturnsChartDataAsync()
        {
            var dataService = new SampleDataService();
            var actual = await dataService.GetChartDataAsync();

            Assert.IsNotEmpty(actual);
        }
        //}]}
    }
}
