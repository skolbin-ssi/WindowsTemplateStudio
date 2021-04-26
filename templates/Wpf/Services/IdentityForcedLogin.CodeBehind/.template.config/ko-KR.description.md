﻿Forced Login에서는 Azure AD 및 [Microsoft.Identity.Client](https://www.nuget.org/packages/Microsoft.Identity.Client) (MSAL)를 사용하여 사용자 인증을 추가합니다.
앱에 대한 액세스는 로그인하여 인증된 사용자만 가능합니다. 대화형 로그인이 필요한 경우라면 사용자는 대화형 대화 상자가 표시되기 전에 LoginPage로 리디렉션됩니다. 로그아웃하면 동일한 페이지가 표시됩니다.

이 애플리케이션에서는 Microsoft Graph로의 호출을 사용하여 NavigationPane 및 SettingsPage에서 사용자 정보 및 사진을 표시합니다.  기본 제공되는 이 기능은 모든 조직 디렉토리 및 Microsoft 개인 계정(예: Skype, Xbox, Outlook.com)을 사용하는 계정에서 작동됩니다. 또한 옵션을 제공하여 Microsoft 개인 계정을 제외하거나 특정 디렉토리에 대한 액세스 제한 또는 Windows 통합 인증 사용을 선택할 수 있습니다.

[Microsoft Identity platform에 대해 자세히 알아보십시오.](https://docs.microsoft.com/azure/active-directory/develop/v2-overview)
