﻿//{[{
using System.Threading.Tasks;
using Param_RootNamespace.Core.Services;
//}]}

namespace Param_RootNamespace.Core.Tests.MSTest
{
    public class Tests
    {
        //^^
        //{[{

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsGridDataAsync()
        {
            var dataService = new SampleDataService();
            var actual = await dataService.GetGridDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }
        //}]}
    }
}
