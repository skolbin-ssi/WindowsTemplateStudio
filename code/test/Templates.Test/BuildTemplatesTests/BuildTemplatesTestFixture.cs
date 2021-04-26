﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.Core.Locations;
using Microsoft.Templates.Fakes;
using Microsoft.Templates.UI;

namespace Microsoft.Templates.Test
{
    public sealed class BuildTemplatesTestFixture : BaseGenAndBuildFixture, IDisposable
    {
        private string _testExecutionTimeStamp = DateTime.Now.FormatAsDateHoursMinutes();

        private string _framework;

        private static Dictionary<string, bool> syncExecuted = new Dictionary<string, bool>();

        public override string GetTestRunPath() => $"{Path.GetPathRoot(Environment.CurrentDirectory)}\\UIT\\{ShortFrameworkName(_framework)}\\{_testExecutionTimeStamp}\\";

        public TemplatesSource Source => new LocalTemplatesSource(null, ShortFrameworkName(_framework));

        public static IEnumerable<object[]> GetProjectTemplates(string frameworkFilter, string programmingLanguage, string selectedPlatform)
        {
            InitializeTemplates(new LocalTemplatesSource(null, ShortFrameworkName(frameworkFilter)));

            List<object[]> result = new List<object[]>();

            var languagesOfInterest = ProgrammingLanguages.GetAllLanguages().ToList();

            if (!string.IsNullOrWhiteSpace(programmingLanguage))
            {
                languagesOfInterest.Clear();
                languagesOfInterest.Add(programmingLanguage);
            }

            var platformsOfInterest = Platforms.GetAllPlatforms().ToList();

            if (!string.IsNullOrWhiteSpace(selectedPlatform))
            {
                platformsOfInterest.Clear();
                platformsOfInterest.Add(selectedPlatform);
            }

            foreach (var language in languagesOfInterest)
            {
                SetCurrentLanguage(language);
                foreach (var platform in platformsOfInterest)
                {
                    SetCurrentPlatform(platform);

                    if (platform == Platforms.WinUI)
                    {
                        var appModels = AppModels.GetAllAppModels().ToList();
                        foreach (var appModel in appModels)
                        {
                            result.AddRange(GetContextOptions(frameworkFilter, language, platform, appModel));
                        }
                    }
                    else
                    {
                        result.AddRange(GetContextOptions(frameworkFilter, language, platform, string.Empty));

                    }
                }
            }
            return result;
        }

        private static List<object[]> GetContextOptions(string frameworkFilter, string language, string platform, string appModel)
        {
            List<object[]> result = new List<object[]>();

            var context = new UserSelectionContext(language, platform);
            if (!string.IsNullOrEmpty(appModel))
            {
                context.AddAppModel(appModel);
            }

            var projectTypes = GenContext.ToolBox.Repo.GetProjectTypes(context)
                    .Where(m => !string.IsNullOrEmpty(m.Description))
                    .Select(m => m.Name);



            foreach (var projectType in projectTypes)
            {
                context.ProjectType = projectType;
                var targetFrameworks = GenContext.ToolBox.Repo.GetFrontEndFrameworks(context)
                                            .Where(m => m.Name == frameworkFilter)
                                            .Select(m => m.Name)
                                            .ToList();

                foreach (var framework in targetFrameworks)
                {
                    if (!string.IsNullOrEmpty(appModel))
                    {
                        result.Add(new object[] { projectType, framework, platform, language, appModel });
                    }
                    else
                    {
                        result.Add(new object[] { projectType, framework, platform, language });
                    }
                    
                }
            }
            return result;
        }

        public static IEnumerable<object[]> GetPageAndFeatureTemplatesForBuild(string frameworkFilter, string language = ProgrammingLanguages.CSharp, string platform = Platforms.Uwp, string excludedItem = "")
        {
            InitializeTemplates(new LocalTemplatesSource(null, ShortFrameworkName(frameworkFilter)));

            return BaseGenAndBuildFixture.GetPageAndFeatureTemplates(frameworkFilter, language, platform, excludedItem);
        }

        [SuppressMessage(
         "Usage",
         "VSTHRD002:Synchronously waiting on tasks or awaiters may cause deadlocks",
         Justification = "Required for unit testing.")]
        private static void InitializeTemplates(TemplatesSource source)
        {

            if (syncExecuted.ContainsKey(source.Id) && syncExecuted[ShortFrameworkName(source.Id)] == true)
            {
                return;
            }

            GenContext.Bootstrap(source, new FakeGenShell(Platforms.Uwp, ProgrammingLanguages.CSharp), Platforms.Uwp, ProgrammingLanguages.CSharp);

            GenContext.ToolBox.Repo.SynchronizeAsync(true).Wait();

            syncExecuted.Add(source.Id, true);
        }

        public override void InitializeFixture(IContextProvider contextProvider, string framework)
        {
            GenContext.Current = contextProvider;
            _framework = framework;

            InitializeTemplates(Source);
        }

        public static string ShortFrameworkName(string framework)
        {
            switch (framework)
            {
                case Frameworks.CaliburnMicro:
                    return "CM";
                case Frameworks.Prism:
                    return "P";
                case Frameworks.CodeBehind:
                    return "CB";
                case Frameworks.MVVMLight:
                    return "ML";
                case Frameworks.MVVMBasic:
                    return "MB";
                case Frameworks.MVVMToolkit:
                    return "MTM";
                default:
                    return framework;
            }
        }
    }
}
