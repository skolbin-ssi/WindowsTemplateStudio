﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

using Microsoft.TemplateEngine.Abstractions;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Extensions;
using Microsoft.Templates.Core.Gen;
using Xunit;

namespace Microsoft.Templates.Test.Build.Wpf
{
    [Collection("BuildTemplateTestCollection")]
    public class BuildPrismProjectTests : BaseGenAndBuildTests
    {
        public BuildPrismProjectTests(BuildTemplatesTestFixture fixture)
            : base(fixture, null, Frameworks.Prism)
        {
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, "", Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "BuildProjects")]
        public async Task Build_EmptyProject_WpfAsync(string projectType, string framework, string platform, string language)
        {
            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var (projectName, projectPath) = await GenerateEmptyProjectAsync(context);

            AssertBuildProject(projectPath, projectName, platform, true);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, "", Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "BuildAllPagesAndFeaturesWpf")]
        [Trait("Type", "BuildRandomNamesWpf")]
        public async Task Build_All_G2_ProjectNameValidation_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group1.Contains(t.GroupIdentity)
                && t.Identity != "wts.Wpf.Feat.MSIXPackaging"
                && !t.GetIsHidden();

            var projectName = $"{ShortProjectType(projectType)}{CharactersThatMayCauseProjectNameIssues()}G1{ShortLanguageName(language)}";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetRandomName);

            AssertBuildProject(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, "", Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "BuildAllPagesAndFeaturesWpf")]
        [Trait("Type", "BuildRandomNamesWpf")]
        public async Task Build_All_G1_ProjectNameValidation_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group2.Contains(t.GroupIdentity)
                && t.Identity != "wts.Wpf.Feat.MSIXPackaging"
                && !t.GetIsHidden();

            var projectName = $"{ShortProjectType(projectType)}{CharactersThatMayCauseProjectNameIssues()}G1{ShortLanguageName(language)}";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetRandomName);

            AssertBuildProject(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, ProgrammingLanguages.CSharp, Platforms.Wpf)]
        [Trait("ExecutionSet", "MinimumWpf")]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "CodeStyleWpf")]
        public async Task Build_All_CheckWithStyleCop_G1_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group2.Contains(t.GroupIdentity)
                && !t.GetIsHidden()
                && t.Identity != "wts.Wpf.Feat.MSIXPackaging"
                || t.Identity == "wts.Wpf.Feat.StyleCop";

            var projectName = $"{projectType}{framework}AllG1";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetDefaultName, false);

            AssertBuildProject(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, ProgrammingLanguages.CSharp, Platforms.Wpf)]
        [Trait("ExecutionSet", "MinimumWpf")]
        [Trait("ExecutionSet", "MinimumPrismWpf")]
        [Trait("ExecutionSet", "_CIBuild")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "CodeStyleWpf")]
        public async Task Build_All_CheckWithStyleCop_G2_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group1.Contains(t.GroupIdentity)
                && !t.GetIsHidden()
                && t.Identity != "wts.Wpf.Feat.MSIXPackaging"
                || t.Identity == "wts.Wpf.Feat.StyleCop";

            var projectName = $"{projectType}{framework}AllG2";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetDefaultName, true);

            AssertBuildProject(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, ProgrammingLanguages.CSharp, Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        public async Task Build_AllWithMsix_G1_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group2.Contains(t.GroupIdentity)
                && !t.GetIsHidden()
                || t.Identity == "wts.Wpf.Feat.StyleCop";

            var projectName = $"{projectType}{framework}AllMsix";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };


            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetDefaultName);

            AssertBuildProjectWpfWithMsix(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, ProgrammingLanguages.CSharp, Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        public async Task Build_AllWithMsix_G2_WpfAsync(string projectType, string framework, string platform, string language)
        {
            bool templateSelector(ITemplateInfo t) => t.GetTemplateType().IsItemTemplate()
                && (t.GetProjectTypeList().Contains(projectType) || t.GetProjectTypeList().Contains(All))
                && (t.GetFrontEndFrameworkList().Contains(framework) || t.GetFrontEndFrameworkList().Contains(All))
                && t.GetPlatform() == platform
                && !excludedTemplates_Wpf_Group1.Contains(t.GroupIdentity)
                && !t.GetIsHidden()
                || t.Identity == "wts.Wpf.Feat.StyleCop";

            var projectName = $"{projectType}{framework}AllMsix";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateProjectAsync(projectName, context, templateSelector, BaseGenAndBuildFixture.GetDefaultName);

            AssertBuildProjectWpfWithMsix(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetProjectTemplatesForBuild), Frameworks.Prism, "", Platforms.Wpf)]
        [Trait("ExecutionSet", "BuildPrismWpf")]
        [Trait("ExecutionSet", "_Full")]
        [Trait("Type", "BuildRightClickWpf")]
        public async Task Build_Empty_AddRightClick_WpfAsync(string projectType, string framework, string platform, string language)
        {
            var projectName = $"{ShortProjectType(projectType)}AllRC{ShortLanguageName(language)}";

            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var projectPath = await AssertGenerateRightClickAsync(projectName, context, true);

            AssertBuildProject(projectPath, projectName, platform);
        }

        [Theory]
        [MemberData(nameof(BaseGenAndBuildTests.GetPageAndFeatureTemplatesForBuild), Frameworks.Prism, ProgrammingLanguages.CSharp, Platforms.Wpf, "wts.Wpf.Feat.MSIXPackaging")]
        [Trait("ExecutionSet", "BuildOneByOnePrismWpf")]
        [Trait("ExecutionSet", "_OneByOne")]
        [Trait("Type", "BuildOneByOnePrismWpf")]
        public async Task Build_Prism_OneByOneItems_WpfAsync(string itemName, string projectType, string framework, string platform, string itemId, string language)
        {
            var context = new UserSelectionContext(language, platform)
            {
                ProjectType = projectType,
                FrontEndFramework = framework
            };

            var (ProjectPath, ProjecName) = await AssertGenerationOneByOneAsync(itemName, context, itemId, false);

            AssertBuildProject(ProjectPath, ProjecName, platform);
        }
    }
}
