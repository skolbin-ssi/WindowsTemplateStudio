## WinUI 3 Apps

We're looking into adding WinUI 3 App templates to Windows Template Studio.

Our templates for WinUI 3 Apps are currently in preview. You can try them out in our [dev-nightly version](./../getting-started-extension.md#nightly--pre-release-feeds-for-windows-template-studio).

We currently provide the following project templates:

C# templates:
- App (WinUI 3 Desktop) 
- Class Library (WinUI 3 in Desktop) 

- App (WinUI 3 UWP) PREVIEW
- Class Library (WinUI 3 in UWP) PREVIEW
- Windows Runtime Component (WinUI 3 in UWP) PREVIEW

C++ Templates:
- App (WinUI 3 Desktop) 
- Windows Runtime Component (WinUI 3) 

- App (WinUI 3 UWP) PREVIEW

We also offer the following item templates:
- Blank page
- Blank Window (desktop)
- Custom Control
- Resource Dictionary
- Resources File
- User Control

We would love to get your feedback on our [tracking issues for WinUI3 templates](https://github.com/microsoft/WindowsTemplateStudio/issues?q=is%3Aopen+is%3Aissue+label%3Afeature+milestone%3AWinUI3).

If you miss anything or find an issue that is not mentioned in the known issues section please file a new issue.

### Prerequisites

1. Ensure that your development computer has Windows 10, version 1809 (build 17763), or a later OS version installed.

2. Install [Visual Studio 2019, version 16.10 Preview](https://visualstudio.microsoft.com/vs/preview/) (or later) if you haven't done so already. 
   
   You must include the following components when installing Visual Studio:

   - On the **Workloads** tab, make sure **Universal Windows Platform development** is selected.
   - On the **Individual components** tab, make sure **Windows 10 SDK (10.0.19041.0)** is selected in the **SDKs, libraries, and frameworks** section.

   To build .NET apps, you must also include the following components:

   - **.NET Desktop Development** workload.

   To build C++ apps, you must also include the following components:

   - **Desktop development with C++** workload.
   - The **C++ (v142) Universal Windows Platform tools** optional component for the **Universal Windows Platform development** workload.
   
3. If you previously installed the WinUI 3 Preview extension from an earlier WinUI 3 preview release, uninstall the extension. For more information about how to uninstall an extension, see Manage extensions for Visual Studio.

4. Make sure your system has a NuGet package source enabled for **nuget.org**. For more information, see [Common NuGet configurations](https://docs.microsoft.com/en-us/nuget/consume-packages/configuring-nuget-behavior)

### Known issues:
- [Dark/Light theme issue](https://github.com/microsoft/WindowsTemplateStudio/issues/4087)
- [VisualStateManager initialization issue](https://github.com/microsoft/WindowsTemplateStudio/issues/4072)

### Additional docs:
- [Windows UI Library 3 - Project Reunion 0.5 (March 2021)](https://docs.microsoft.com/windows/apps/winui/winui3/)
- [Windows UI Library on GitHub](https://github.com/Microsoft/microsoft-ui-xaml)
- [Project Reunion on GitHub](https://github.com/microsoft/ProjectReunion)
- [Windows Community Toolkit for Project Reunion 0.5](https://devblogs.microsoft.com/ifdef-windows/windows-community-toolkit-for-project-reunion-0-5/)
- [Update existing projects to the latest release of Project Reunion](https://docs.microsoft.com/windows/apps/project-reunion/update-existing-projects-to-the-latest-release)
