// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Models;
using Unchase.OData.ConnectedService.Views;

namespace Unchase.OData.ConnectedService.ViewModels
{
    internal class FunctionImportsViewModel : ConnectedServiceWizardPage
    {
        #region Properties and fields
        public UserSettings UserSettings { get; }

        public List<FunctionImportModel> FunctionImports { get; set; }

        public List<OperationImportModel> OperationImports { get; set; }

        public int FunctionImportsCount { get; set; } = 0;

        public int OperationImportsCount { get; set; } = 0;

        public Wizard InternalWizard;
        #endregion

        #region Constructors
        public FunctionImportsViewModel(UserSettings userSettings, Wizard wizard) : base()
        {
            this.Title = "Operation Imports Selection";
            this.Description = "Operation Imports Settings for select the necessary methods that will be added after generation";
            this.Legend = "Operation Imports Selection";
            this.FunctionImports = new List<FunctionImportModel>();
            this.FunctionImportsCount = FunctionImports.Count;
            this.OperationImports = new List<OperationImportModel>();
            this.OperationImportsCount = OperationImports.Count;
            this.UserSettings = userSettings;
            this.InternalWizard = wizard;
        }
        #endregion

        #region Methods
        public event EventHandler<EventArgs> PageEntering;

        public override async Task OnPageEnteringAsync(WizardEnteringArgs args)
        {
            await base.OnPageEnteringAsync(args);

            this.View = new FunctionImports(this.InternalWizard) {DataContext = this};
            this.FunctionImportsCount = this.FunctionImports.Count;
            this.OperationImportsCount = this.OperationImports.Count;
            PageEntering?.Invoke(this, EventArgs.Empty);
        }

        public override Task<PageNavigationResult> OnPageLeavingAsync(WizardLeavingArgs args)
        {
            return base.OnPageLeavingAsync(args);
        }
        #endregion
    }
}
