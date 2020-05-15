﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Templates.Core;
using Microsoft.Templates.Core.Gen;
using Microsoft.Templates.UI.Extensions;
using Microsoft.Templates.UI.Resources;
using Microsoft.Templates.UI.Services;

namespace Microsoft.Templates.UI.ViewModels.Common
{
    public class TemplateInfoViewModel : BasicInfoViewModel
    {
        private int _count;
        private bool _hasMoreThanOne;
        private bool _hasMoreThanTwo;
        private bool _showAddedText;
        private bool _canBeAdded;
        private bool _disabled;
        private string _disabledMessage;
        private string _emptyBackendFramework = string.Empty;

        public TemplateInfo Template { get; }

        public string Group { get; }

        public bool IsGroupExclusiveSelection { get; }

        public int GenGroup { get; }

        public TemplateType TemplateType { get; }

        public bool MultipleInstance { get; }

        public bool ItemNameEditable { get; }

        public int Count
        {
            get => _count;
            private set
            {
                HasMoreThanOne = MultipleInstance && value > 1;
                HasMoreThanTwo = MultipleInstance && value > 2;
                ShowAddedText = !MultipleInstance && value > 0;
                CanBeAdded = MultipleInstance || value == 0;
                if (MultipleInstance || value <= 1)
                {
                    SetProperty(ref _count, value);
                }
            }
        }

        public bool HasMoreThanOne
        {
            get => _hasMoreThanOne;
            private set => SetProperty(ref _hasMoreThanOne, value);
        }

        public bool HasMoreThanTwo
        {
            get => _hasMoreThanTwo;
            private set => SetProperty(ref _hasMoreThanTwo, value);
        }

        public bool ShowAddedText
        {
            get => _showAddedText;
            private set => SetProperty(ref _showAddedText, value);
        }

        public bool CanBeAdded
        {
            get => _canBeAdded;
            set => SetProperty(ref _canBeAdded, value);
        }

        public bool Disabled
        {
            get => _disabled;
            set => SetProperty(ref _disabled, value);
        }

        public string DisabledMessage
        {
            get => _disabledMessage;
            set => SetProperty(ref _disabledMessage, value);
        }

        public TemplateInfoViewModel(TemplateInfo template,  string platform, string projectType, string frameworkName)
        {
            // BasicInfo properties
            Name = template.Name;
            Identity = template.TemplateId;
            Title = template.Name;
            Summary = template.Description;
            Description = template.RichDescription;
            Author = template.Author;
            Version = template.Version;
            Icon = template.Icon;
            Order = template.DisplayOrder;
            IsHidden = template.IsHidden;
            DefaultName = template.DefaultName;
            Dependencies = template.Dependencies.Select(d => new TemplateInfoViewModel(d, platform, projectType, frameworkName));
            Requirements = template.Requirements.Select(d => new TemplateInfoViewModel(d, platform, projectType, frameworkName));
            Exclusions = template.Exclusions.Select(d => new TemplateInfoViewModel(d, platform, projectType, frameworkName));
            RequiredSdks = template.RequiredSdks.Select(sdk => Regex.Match(sdk, @"\d+(\.\d+)+").Value);
            RequiredVisualStudioWorkloads = template.RequiredVisualStudioWorkloads.Select(r => r.GetRequiredWorkloadDisplayName());
            Licenses = template.Licenses.Select(l => new LicenseViewModel(l));

            // ITemplateInfo properties
            Template = template;
            Group = template.Group;
            IsGroupExclusiveSelection = template.IsGroupExclusiveSelection;
            GenGroup = template.GenGroup;
            TemplateType = template.TemplateType;
            MultipleInstance = template.MultipleInstance;
            ItemNameEditable = template.ItemNameEditable;
            CanBeAdded = MultipleInstance || Count == 0;
            if (!DataService.HasAllVisualStudioWorkloads(template.RequiredVisualStudioWorkloads))
            {
                Disabled = true;
                DisabledMessage = StringRes.TemplateDetailsInfoUnavailableDueToMissingVSWorkload;
            }
        }

        public void IncreaseSelection()
        {
            Count++;
        }

        public void DecreaseSelection()
        {
            Count--;
        }

        public void ResetTemplateCount()
        {
            Count = 0;
        }
    }
}
