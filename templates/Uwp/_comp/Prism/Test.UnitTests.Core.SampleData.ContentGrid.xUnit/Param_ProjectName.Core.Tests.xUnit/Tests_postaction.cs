﻿//{[{
using System.Threading.Tasks;
using Param_RootNamespace.Core.Services;
//}]}

namespace Param_RootNamespace.Core.Tests.XUnit
{
    public class Tests
    {
        //^^
        //{[{

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [Fact]
        public async Task EnsureSampleDataServiceReturnsContentGridDataAsync()
        {
            var dataService = new SampleDataService();
            var actual = await dataService.GetContentGridDataAsync();

            Assert.NotEmpty(actual);
        }
        //}]}
    }
}
