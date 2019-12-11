// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Gen;
using Microsoft.TemplateEngine.Abstractions;

using Xunit;
using Microsoft.Templates.Fakes;
using Microsoft.Templates.Core.Helpers;
using Microsoft.Templates.Core.Extensions;
using Microsoft.Templates.Core.Naming;
using Microsoft.Templates.UI.Services;

namespace Microsoft.Templates.Test
{
    public class BaseGenAndBuildTests
    {
        protected BaseGenAndBuildFixture _fixture;
        private readonly string _emptyBackendFramework = string.Empty;
        protected const string All = "all";

        protected List<string> excludedTemplatesGroup1 = new List<string>() { "wts.Service.IdentityOptionalLogin", "wts.Feat.MultiInstanceAdvanced", "wts.Feat.MultiInstance" };
        protected List<string> excludedTemplatesGroup2 = new List<string>() { "wts.Service.IdentityForcedLogin", "wts.Feat.BackgroundTask" };
        protected List<string> excludedTemplatesGroup1VB = new List<string>() { "wts.Service.IdentityOptionalLogin.VB", "wts.Feat.MultiInstanceAdvanced.VB", "wts.Feat.MultiInstance.VB" };
        protected List<string> excludedTemplatesGroup2VB = new List<string>() { "wts.Service.IdentityForcedLogin.VB", "wts.Feat.BackgroundTask.VB" };

        public BaseGenAndBuildTests(BaseGenAndBuildFixture fixture, IContextProvider contextProvider = null, string framework = "")
        {
            _fixture = fixture;
            _fixture.InitializeFixture(contextProvider ?? new FakeContextProvider(), framework);
        }

        protected static string ShortLanguageName(string language)
        {
            return language == ProgrammingLanguages.CSharp ? "CS" : "VB";
        }

        // Used to create names that include a number of characters that are valid in project names but have the potential to cause issues
        protected static string CharactersThatMayCauseProjectNameIssues()
        {
            // $ is technically valid in a project name but cannot be used with WTS as it is used as an identifier in global post action file names.
            // ^ is technically valid in project names but Visual Studio cannot open files with this in the path
            // ' is technically valid in project names but breaks test projects if used in the name so don't test for it
            return " -_.,@! (£)+=";
        }

        protected static string ShortProjectType(string projectType)
        {
            switch (projectType)
            {
                case "Blank":
                    return "B";
                case "SplitView":
                    return "SV";
                case "SplitViewWpf":
                    return "SVWpf";
                case "TabbedNav":
                    return "TN";
                case "MenuBar":
                    return "MB";
                case "MenuBarWpf":
                    return "MBWpf";
                case "Ribbon":
                    return "RB";
                default:
                    return projectType;
            }
        }

        protected static string GetProjectExtension(string language)
        {
            return language == ProgrammingLanguages.CSharp ? "csproj" : "vbproj";
        }

        protected async Task<(string projectName, string projectPath)> GenerateEmptyProjectAsync(string projectType, string framework, string platform, string language)
        {

            var projectName = $"{ShortProjectType(projectType)}";

            var projectPath = await AssertGenerateProjectAsync(projectName, projectType, framework, platform, language, null, null);

            return (projectName, projectPath);
        }

        protected async Task<(string projectName, string projectPath)> GenerateAllPagesAndFeaturesAsync(string projectType, string framework, string platform, string language)
        {
            // get first item from each exclusive selection group
            var exclusiveSelectionGroups = GenContext.ToolBox.Repo.GetAll().Where(t =>
                t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && t.GetIsGroupExclusiveSelection()).GroupBy(t => t.GetGroup(), (key, g) => g.First());

            // this selector excludes templates with exclusions
            Func<ITemplateInfo, bool> templateSelector =
                t => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && t.GetExclusionsList().Count() == 0
                && (!t.GetIsGroupExclusiveSelection() || (t.GetIsGroupExclusiveSelection() && exclusiveSelectionGroups.Contains(t)))
                && !t.GetIsHidden();

            var projectName = $"{ShortProjectType(projectType)}All{ShortLanguageName(language)}";

            var projectPath = await AssertGenerateProjectAsync(projectName, projectType, framework, platform, language, templateSelector, BaseGenAndBuildFixture.GetDefaultName);

            return (projectName, projectPath);
        }

        protected async Task<string> AssertGenerateProjectAsync(string projectName, string projectType, string framework, string platform, string language, Func<ITemplateInfo, bool> itemTemplatesSelector = null, Func<TemplateInfo, string> getName = null)
        {
            BaseGenAndBuildFixture.SetCurrentLanguage(language);
            BaseGenAndBuildFixture.SetCurrentPlatform(platform);

            var destinationPath = Path.Combine(_fixture.TestProjectsPath, projectName, projectName);

            GenContext.Current = new FakeContextProvider
            {
                ProjectName = projectName,
                DestinationPath = destinationPath,
                GenerationOutputPath = destinationPath,
            };

            var userSelection = _fixture.SetupProject(projectType, framework, platform, language, getName);

            if (getName != null && itemTemplatesSelector != null)
            {
                var itemTemplates = _fixture.Templates().Where(itemTemplatesSelector);
                var itemsTemplateInfo = GenContext.ToolBox.Repo.GetTemplatesInfo(itemTemplates, platform, projectType, framework, _emptyBackendFramework);
                _fixture.AddItems(userSelection, itemsTemplateInfo, getName);
            }

            await NewProjectGenController.Instance.UnsafeGenerateProjectAsync(userSelection);

            var resultPath = Path.Combine(_fixture.TestProjectsPath, projectName);

            // Assert
            Assert.True(Directory.Exists(resultPath));
            Assert.True(Directory.GetFiles(resultPath, "*.*", SearchOption.AllDirectories).Count() > 2);

            if (platform == Platforms.Uwp)
            {
                AssertCorrectProjectConfigInfo(projectType, framework, platform);
            }

            return resultPath;
        }

        protected void EnsureCanInferConfigInfo(string projectType, string framework, string platform, string projectPath)
        {
            RemoveProjectConfigInfoFromProject();

            AssertCorrectProjectConfigInfo(projectType, framework, platform);
            AssertProjectConfigInfoRecreated(projectType, framework, platform);

            Fs.SafeDeleteDirectory(projectPath);
        }

        protected void RemoveProjectConfigInfoFromProject()
        {
            var manifest = Path.Combine(GenContext.Current.DestinationPath, "Package.appxmanifest");
            var lines = File.ReadAllLines(manifest);
            var sb = new StringBuilder();
            var fx = $"genTemplate:Item Name=\"framework\"";
            var pt = $"genTemplate:Item Name=\"projectType\"";
            foreach (var line in lines)
            {
                if (!line.Contains(fx) && !line.Contains(pt))
                {
                    sb.AppendLine(line);
                }
            }

            File.Delete(manifest);
            File.WriteAllText(manifest, sb.ToString(), Encoding.UTF8);
        }

        protected void AssertCorrectProjectConfigInfo(string expectedProjectType, string expectedFramework, string expectedPlatform)
        {
            var info = ProjectConfigInfoService.ReadProjectConfiguration();

            Assert.Equal(expectedProjectType, info.ProjectType);
            Assert.Equal(expectedFramework, info.Framework);
            Assert.Equal(expectedPlatform, info.Platform);
        }

        protected void AssertProjectConfigInfoRecreated(string projectType, string framework, string platform)
        {
            var content = File.ReadAllText(Path.Combine(GenContext.Current.DestinationPath, "Package.appxmanifest"));
            var expectedFxText = $"Name=\"framework\" Value=\"{framework}\"";
            var expectedPtText = $"Name=\"projectType\" Value=\"{projectType}\"";
            var expectedPfText = $"Name=\"platform\" Value=\"{platform}\"";

            Assert.Contains(expectedFxText, content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(expectedPtText, content, StringComparison.OrdinalIgnoreCase);
            Assert.Contains(expectedPfText, content, StringComparison.OrdinalIgnoreCase);
        }

        protected void AssertBuildProjectAsync(string projectPath, string projectName, string platform, bool deleteAfterBuild = true)
        {
            // Build solution
            var result = _fixture.BuildSolution(projectName, projectPath, platform);

            // Assert
            Assert.True(result.exitCode.Equals(0), $"Solution {projectName} was not built successfully. {Environment.NewLine}Errors found: {_fixture.GetErrorLines(result.outputFile)}.{Environment.NewLine}Please see {Path.GetFullPath(result.outputFile)} for more details.");

            // Clean
            if (deleteAfterBuild)
            {
                Fs.SafeDeleteDirectory(projectPath);
            }
        }

        protected void AssertBuildProjectThenRunTestsAsync(string projectPath, string projectName, string platform)
        {
            var (buildExitCode, buildOutputFile) = _fixture.BuildSolution(projectName, projectPath, platform);

            if (buildExitCode.Equals(0))
            {
                var (testExitCode, testOutputFile) = _fixture.RunTests(projectName, projectPath);

                var summary = _fixture.GetTestSummary(testOutputFile);

                Assert.True(
                    summary.Contains("Failed: 0."),
                    $"Tests failed. {Environment.NewLine}{summary}{Environment.NewLine}Please see {Path.GetFullPath(buildOutputFile)} for more details.");
            }
            else
            {
                Assert.True(buildExitCode.Equals(0), $"Solution {projectName} was not built successfully. {Environment.NewLine}Errors found: {_fixture.GetErrorLines(buildOutputFile)}.{Environment.NewLine}Please see {Path.GetFullPath(buildOutputFile)} for more details.");
            }

            // Tidy up if all tests passed
            Fs.SafeDeleteDirectory(projectPath);
        }

        protected async Task<string> AssertGenerateRightClickAsync(string projectName, string projectType, string framework, string platform, string language, bool emptyProject, List<string> excludedGroupIdentity = null)
        {
            BaseGenAndBuildFixture.SetCurrentLanguage(language);
            BaseGenAndBuildFixture.SetCurrentPlatform(platform);
            var path = Path.Combine(_fixture.TestNewItemPath, projectName, projectName);

            GenContext.Current = new FakeContextProvider
            {
                ProjectName = projectName,
                DestinationPath = path,
                GenerationOutputPath = path,
            };

            var userSelection = _fixture.SetupProject(projectType, framework, platform, language);

            if (!emptyProject)
            {
                var templates = _fixture.Templates().Where(
                    t => t.GetTemplateType().IsItemTemplate()
                    && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                    && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                    && t.GetPlatform() == platform
                    && (excludedGroupIdentity == null || (!excludedGroupIdentity.Contains(t.GroupIdentity)))
                    && !t.GetIsHidden());

                var templatesInfo = GenContext.ToolBox.Repo.GetTemplatesInfo(templates, platform, projectType, framework, _emptyBackendFramework);

                _fixture.AddItems(userSelection, templatesInfo, BaseGenAndBuildFixture.GetDefaultName);
            }

            await NewProjectGenController.Instance.UnsafeGenerateProjectAsync(userSelection);

            var project = Path.Combine(_fixture.TestNewItemPath, projectName);

            // Assert on project
            Assert.True(Directory.Exists(project));

            int emptyProjecFileCount = Directory.GetFiles(project, "*.*", SearchOption.AllDirectories).Count();
            Assert.True(emptyProjecFileCount > 2);

            var rightClickTemplates = _fixture.Templates().Where(
                t => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !t.GetIsHidden()
                && (excludedGroupIdentity == null || (!excludedGroupIdentity.Contains(t.GroupIdentity)))
                && t.GetRightClickEnabled());

            await AddRightClickTemplatesAsync(path, rightClickTemplates, projectName, projectType, framework, platform, language);

            var finalProjectPath = Path.Combine(_fixture.TestNewItemPath, projectName);
            int finalProjectFileCount = Directory.GetFiles(finalProjectPath, "*.*", SearchOption.AllDirectories).Count();

            if (emptyProject)
            {
                Assert.True(finalProjectFileCount > emptyProjecFileCount);
            }
            else
            {
                Assert.True(finalProjectFileCount == emptyProjecFileCount);
            }

            return finalProjectPath;
        }

        protected async Task AddRightClickTemplatesAsync(string destinationPath, IEnumerable<ITemplateInfo> rightClickTemplates, string projectName, string projectType, string framework, string platform, string language)
        {
            // Add new items
            foreach (var item in rightClickTemplates)
            {
                GenContext.Current = new FakeContextProvider
                {
                    ProjectName = projectName,
                    DestinationPath = destinationPath,
                    GenerationOutputPath = GenContext.GetTempGenerationPath(projectName),
                };

                var newUserSelection = new UserSelection(projectType, framework, _emptyBackendFramework, platform, language)
                {
                    HomeName = string.Empty,
                    ItemGenerationType = ItemGenerationType.GenerateAndMerge,
                };

                var templateInfo = GenContext.ToolBox.Repo.GetTemplateInfo(item, platform, projectType, framework, _emptyBackendFramework);

                _fixture.AddItem(newUserSelection, templateInfo, BaseGenAndBuildFixture.GetDefaultName);

                await NewItemGenController.Instance.UnsafeGenerateNewItemAsync(item.GetTemplateType(), newUserSelection);

                NewItemGenController.Instance.UnsafeFinishGeneration(newUserSelection);
            }
        }

        protected async Task<(string ProjectPath, string ProjecName)> AssertGenerationOneByOneAsync(string itemName, string projectType, string framework, string platform, string itemId, string language, bool cleanGeneration = true)
        {
            BaseGenAndBuildFixture.SetCurrentLanguage(language);
            BaseGenAndBuildFixture.SetCurrentPlatform(platform);

            var itemTemplate = _fixture.Templates().FirstOrDefault(t => t.Identity == itemId);
            var finalName = itemTemplate.GetDefaultName();

            if (itemTemplate.GetItemNameEditable())
            {
                var nameValidationService = new ItemNameService(GenContext.ToolBox.Repo.ItemNameValidationConfig, () => new string[] { });
                finalName = nameValidationService.Infer(finalName);
            }

            var projectName = $"{ShortProjectType(projectType)}{finalName}{ShortLanguageName(language)}";
            var destinationPath = Path.Combine(_fixture.TestProjectsPath, projectName, projectName);

            GenContext.Current = new FakeContextProvider
            {
                ProjectName = projectName,
                DestinationPath = destinationPath,
                GenerationOutputPath = destinationPath,
            };

            var userSelection = _fixture.SetupProject(projectType, framework, platform, language);
            var templateInfo = GenContext.ToolBox.Repo.GetTemplateInfo(itemTemplate, platform, projectType, framework, _emptyBackendFramework);

            _fixture.AddItem(userSelection, templateInfo, BaseGenAndBuildFixture.GetDefaultName);

            await NewProjectGenController.Instance.UnsafeGenerateProjectAsync(userSelection);

            var resultPath = Path.Combine(_fixture.TestProjectsPath, projectName);

            // Assert
            Assert.True(Directory.Exists(resultPath));
            Assert.True(Directory.GetFiles(resultPath, "*.*", SearchOption.AllDirectories).Count() > 2);

            // Clean
            if (cleanGeneration)
            {
                Fs.SafeDeleteDirectory(resultPath);
            }

            return (resultPath, projectName);
        }

        public static IEnumerable<object[]> GetProjectTemplatesForGenerationAsync()
        {
            return GenerationFixture.GetProjectTemplates();
        }

        public static IEnumerable<object[]> GetCSharpUwpProjectTemplatesForGenerationAsync()
        {
            var result = GenerationFixture.GetProjectTemplates();

            foreach (var item in result)
            {
                if (item[2].ToString() == Platforms.Uwp && item[3].ToString() == ProgrammingLanguages.CSharp)
                {
                    yield return item;
                }
            }
        }

        protected async Task<(string ProjectPath, string ProjectName)> SetUpComparisonProjectAsync(string language, string projectType, string framework, IEnumerable<string> genIdentities, bool lastPageIsHome = false, bool includeMultipleInstances = true)
        {
            BaseGenAndBuildFixture.SetCurrentLanguage(language);
            BaseGenAndBuildFixture.SetCurrentPlatform(Platforms.Uwp);

            var singlePageName = string.Empty;

            var genIdentitiesList = genIdentities.ToList();

            if (genIdentitiesList.Count == 1)
            {
                singlePageName = genIdentitiesList.Last().Split('.').Last();
            }

            var projectName = $"{projectType}{framework}{singlePageName}{ShortLanguageName(language)}";
            var destinationPath = Path.Combine(_fixture.TestProjectsPath, projectName, projectName);

            GenContext.Current = new FakeContextProvider
            {
                ProjectName = projectName,
                DestinationPath = destinationPath,
                GenerationOutputPath = destinationPath,
            };

            var userSelection = _fixture.SetupProject(projectType, framework, Platforms.Uwp, language);

            foreach (var identity in genIdentitiesList)
            {
                var itemTemplate = _fixture.Templates().FirstOrDefault(t
                    => (t.Identity.StartsWith($"{identity}.") || t.Identity.Equals(identity))
                    && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                    && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All)));

                var templateInfo = GenContext.ToolBox.Repo.GetTemplateInfo(itemTemplate, Platforms.Uwp, projectType, framework, _emptyBackendFramework);
                _fixture.AddItem(userSelection, templateInfo, BaseGenAndBuildFixture.GetDefaultName);

                // Add multiple pages if supported to check these are handled the same
                if (includeMultipleInstances && templateInfo.MultipleInstance)
                {
                    _fixture.AddItem(userSelection, templateInfo, BaseGenAndBuildFixture.GetDefaultName);
                }
            }

            if (lastPageIsHome)
            {
                // Useful if creating a blank project type and want to change the start page
                userSelection.HomeName = userSelection.Pages.Last().Name;

                if (projectType == "TabbedNav")
                {
                    userSelection.Pages.Reverse();
                }
            }

            await NewProjectGenController.Instance.UnsafeGenerateProjectAsync(userSelection);

            var resultPath = Path.Combine(_fixture.TestProjectsPath, GenContext.Current.ProjectName);

            return (resultPath, GenContext.Current.ProjectName);
        }

        public static IEnumerable<object[]> GetPageAndFeatureTemplatesForGeneration(string framework)
        {
            var result = GenerationFixture.GetPageAndFeatureTemplatesForGeneration(framework);
            return result;
        }

        private const string NavigationPanel = "SplitView";
        private const string Blank = "Blank";
        private const string TabbedNav = "TabbedNav";
        private const string MenuBar = "MenuBar";
        private const string MvvmBasic = "MVVMBasic";
        private const string MvvmLight = "MVVMLight";
        private const string CodeBehind = "CodeBehind";

        // This returns a list of project types and frameworks supported by BOTH C# and VB
        public static IEnumerable<object[]> GetMultiLanguageProjectsAndFrameworks()
        {
            yield return new object[] { NavigationPanel, CodeBehind };
            yield return new object[] { NavigationPanel, MvvmBasic };
            yield return new object[] { NavigationPanel, MvvmLight };
            yield return new object[] { Blank, CodeBehind };
            yield return new object[] { Blank, MvvmBasic };
            yield return new object[] { Blank, MvvmLight };
            yield return new object[] { TabbedNav, CodeBehind };
            yield return new object[] { TabbedNav, MvvmBasic };
            yield return new object[] { TabbedNav, MvvmLight };
            yield return new object[] { MenuBar, CodeBehind };
            yield return new object[] { MenuBar, MvvmBasic };
            yield return new object[] { MenuBar, MvvmLight };
        }

        // Gets a list of partial identities for page and feature templates supported by C# and VB
        protected static IEnumerable<string> GetTemplatesThatDoNotSupportVB()
        {
            return new[]
            {
                "wts.Service.WebApi",
                "wts.Service.SecuredWebApi",
                "wts.Service.SecuredWebApi.CodeBehind",
            };
        }

        // Need overload with different number of params to work with XUnit.MemberData
        public static IEnumerable<object[]> GetProjectTemplatesForBuild(string framework)
        {
            return GetProjectTemplatesForBuild(framework, string.Empty, string.Empty);
        }

        // Set a single programming language to stop the fixture using all languages available to it
        public static IEnumerable<object[]> GetProjectTemplatesForBuild(string framework, string programmingLanguage, string platform)
        {
            IEnumerable<object[]> result = new List<object[]>();

            switch (framework)
            {
                case "CodeBehind":
                    result = BuildTemplatesTestFixture.GetProjectTemplates(framework, programmingLanguage, platform);
                    break;

                case "MVVMBasic":
                    result = BuildTemplatesTestFixture.GetProjectTemplates(framework, programmingLanguage, platform);
                    break;

                case "MVVMLight":
                    result = BuildTemplatesTestFixture.GetProjectTemplates(framework, programmingLanguage, platform);
                    break;

                case "CaliburnMicro":
                    result = BuildTemplatesTestFixture.GetProjectTemplates(framework, programmingLanguage, platform);
                    break;

                case "LegacyFrameworks":
                    result = BuildRightClickWithLegacyFixture.GetProjectTemplates();
                    break;

                case "Prism":
                    result = BuildTemplatesTestFixture.GetProjectTemplates(framework, programmingLanguage, platform);
                    break;

                default:
                    result = BuildFixture.GetProjectTemplates();
                    break;
            }

            return result;
        }

        public static IEnumerable<object[]> GetPageAndFeatureTemplatesForBuild(string framework, string language = ProgrammingLanguages.CSharp, string platform = Platforms.Uwp, string excludedItem = "")
        {
            IEnumerable<object[]> result = new List<object[]>();

            switch (framework)
            {
                case "CodeBehind":
                    result = BuildTemplatesTestFixture.GetPageAndFeatureTemplatesForBuild(framework, language, platform, excludedItem);
                    break;

                case "MVVMBasic":
                    result = BuildTemplatesTestFixture.GetPageAndFeatureTemplatesForBuild(framework, language, platform, excludedItem);
                    break;

                case "MVVMLight":
                    result = BuildTemplatesTestFixture.GetPageAndFeatureTemplatesForBuild(framework, language, platform, excludedItem);
                    break;

                case "CaliburnMicro":
                    result = BuildTemplatesTestFixture.GetPageAndFeatureTemplatesForBuild(framework, language, platform, excludedItem);
                    break;

                case "Prism":
                    result = BuildTemplatesTestFixture.GetPageAndFeatureTemplatesForBuild(framework, language, platform, excludedItem);
                    break;
            }

            return result;
        }
    }
}
