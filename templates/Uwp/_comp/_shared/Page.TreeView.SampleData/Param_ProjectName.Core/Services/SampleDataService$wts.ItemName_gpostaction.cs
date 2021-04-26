﻿//{**
// This code block adds the method `GetTreeViewDataAsync()` to the SampleDataService of your project.
//**}
namespace Param_RootNamespace.Core.Services
{
    public static class SampleDataService
    {
//^^
//{[{

        // Remove this once your TreeView page is displaying real data.
        public static async Task<IEnumerable<SampleCompany>> GetTreeViewDataAsync()
        {
            await Task.CompletedTask;
            return AllCompanies();
        }
//}]}
    }
}
