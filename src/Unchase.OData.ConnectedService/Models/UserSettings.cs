// Copyright (c) 2018 Unchase (https://github.com/unchase).  All rights reserved.
// Licensed under the Apache License 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Design;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.ConnectedServices;
using Unchase.OData.ConnectedService.Common;

namespace Unchase.OData.ConnectedService.Models
{
    [DataContract]
    internal class UserSettings
    {
        #region Properties and fields

        #region Private
        private const string Name = "Settings";

        private const int MaxMruEntries = 10;

        private ConnectedServiceLogger logger;
        #endregion

        [DataMember]
        public ObservableCollection<string> MruEndpoints { get; private set; }

        [DataMember]
        public string Endpoint { get; set; }

        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public bool OpenGeneratedFilesOnComplete { get; set; } = false;

        [DataMember]
        public LanguageOption LanguageOption { get; set; }
        #endregion

        #region Constructors
        private UserSettings()
        {
            this.MruEndpoints = new ObservableCollection<string>();
        }
        #endregion

        #region Methods
        public void Save()
        {
            UserSettingsPersistenceHelper.Save(this, Constants.ProviderId, UserSettings.Name, null, this.logger);
        }

        public static UserSettings Load(ConnectedServiceLogger logger)
        {
            var userSettings = UserSettingsPersistenceHelper.Load<UserSettings>(
                Constants.ProviderId, UserSettings.Name, null, logger) ?? new UserSettings();
            userSettings.logger = logger;

            return userSettings;
        }

        public static void AddToTopOfMruList<T>(ObservableCollection<T> mruList, T item)
        {
            int index = mruList.IndexOf(item);
            if (index >= 0)
            {
                // Ensure there aren't any duplicates in the list.
                for (var i = mruList.Count - 1; i > index; i--)
                {
                    if (EqualityComparer<T>.Default.Equals(mruList[i], item))
                    {
                        mruList.RemoveAt(i);
                    }
                }

                if (index > 0)
                {
                    // The item is in the MRU list but it is not at the top.
                    mruList.Move(index, 0);
                }
            }
            else
            {
                // The item is not in the MRU list, make room for it by clearing out the oldest item.
                while (mruList.Count >= UserSettings.MaxMruEntries)
                {
                    mruList.RemoveAt(mruList.Count - 1);
                }

                mruList.Insert(0, item);
            }
        }
        #endregion
    }
}
