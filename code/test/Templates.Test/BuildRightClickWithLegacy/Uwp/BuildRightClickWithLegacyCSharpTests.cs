// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.TemplateEngine.Abstractions;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Extensions;
using Microsoft.Templates.Core.Gen;
using Xunit;

namespace Microsoft.Templates.Test.BuildWithLegacy.Uwp
{      
    [Collection("BuildRightClickWithLegacyCSharpCollection")]
    public class BuildRightClickWithLegacyCSharpTests : BaseGenAndBuildTests
    {
        private readonly string _emptyBackendFramework = string.Empty;
        private string[] excludedTemplates = { };

        public BuildRightClickWithLegacyCSharpTests(BuildRightClickWithLegacyCSharpFixture fixture)
            : base(fixture)
        {
        }

        [Theory(Skip = "Disabled due to moving catalog")]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), "LegacyFrameworks", ProgrammingLanguages.CSharp, Platforms.Uwp)]
        [Trait("ExecutionSet", "BuildRightClickWithLegacy")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "BuildRightClickLegacy")]
        public async Task Build_Empty_Legacy_AddRightClick_Uwp(string projectType, string framework, string platform, string language)
        {
            var fixture = _fixture as BuildRightClickWithLegacyCSharpFixture;

            var projectName = $"{projectType}{framework}Legacy{ShortLanguageName(language)}";

            var projectPath = await AssertGenerateProjectAsync(projectName, projectType, framework, Platforms.Uwp, language, null, null);

            await fixture.ChangeToLocalTemplatesSource();

            var rightClickTemplates = _fixture.Templates().Where(
                t => t.GetTemplateType().IsItemTemplate()
                && t.GetFrontEndFrameworkList().Contains(framework)
                && !excludedTemplates.Contains(t.GroupIdentity)
                && t.GetPlatform() == platform
                && !t.GetIsHidden()
                && t.GetRightClickEnabled());

            await AddRightClickTemplatesAsync(GenContext.Current.DestinationPath, rightClickTemplates, projectName, projectType, framework, platform, language);

            AssertBuildProjectAsync(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), "LegacyFrameworks", ProgrammingLanguages.CSharp, Platforms.Uwp)]
        [Trait("ExecutionSet", "ManualOnly")]
        ////This test sets up projects for further manual tests. It generates legacy projects with all pages and features.
#pragma warning disable xUnit1026 // Theory methods should use all of their parameters
        public async Task Build_All_Legacy_Uwp(string projectType, string framework, string platform, string language)
#pragma warning restore xUnit1026 // Theory methods should use all of their parameters
        {
            var fixture = _fixture as BuildRightClickWithLegacyCSharpFixture;

            var projectName = $"{ProgrammingLanguages.GetShortProgrammingLanguage(language)}{ShortProjectType(projectType)}{framework}AllLegacy";

            Func<ITemplateInfo, bool> templateSelector =
                t => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !t.GetIsHidden();

            var projectPath = await AssertGenerateProjectAsync(projectName, projectType, framework, platform, language, templateSelector, BaseGenAndBuildFixture.GetDefaultName);
        }
    }
}
