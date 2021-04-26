﻿//{[{
using Param_RootNamespace.Models;
using Param_RootNamespace.Core.Contracts.Services;
//}]}

namespace Param_RootNamespace.Services
{
    public class ApplicationHostService : IApplicationHostService
    {
        private readonly IThemeSelectorService _themeSelectorService;
//{[{
        private readonly IIdentityService _identityService;
        private readonly IUserDataService _userDataService;
        private readonly AppConfig _appConfig;
//}]}
        public ApplicationHostService(/*{[{*/IIdentityService identityService, IUserDataService userDataService, AppConfig config/*}]}*/)
        {
//^^
//{[{
            _identityService = identityService;
            _userDataService = userDataService;
            _appConfig = config;
//}]}
        }

        public async Task StartAsync()
        {
            await InitializeAsync();
//{[{

            _identityService.InitializeWithAadAndPersonalMsAccounts(_appConfig.IdentityClientId, "http://localhost");
            await _identityService.AcquireTokenSilentAsync();
//}]}
        }

        private async Task InitializeAsync()
        {
            if (!_isInitialized)
            {
//^^
//{[{
                _userDataService.Initialize();
//}]}
                await Task.CompletedTask;
            }
        }
    }
}
