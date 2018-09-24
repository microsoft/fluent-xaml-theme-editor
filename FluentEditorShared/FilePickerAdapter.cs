// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;

namespace FluentEditorShared
{
    public static class FilePickerAdapters
    {
        public static async Task<StorageFile> ShowLoadFilePicker(string[] filters = null, PickerLocationId startLocation = PickerLocationId.PicturesLibrary, PickerViewMode viewMode = PickerViewMode.Thumbnail, bool addToRecent = false, bool addToFuture = false)
        {
            var picker = new FileOpenPicker();
            picker.SuggestedStartLocation = startLocation;
            picker.ViewMode = viewMode;
            if (filters != null)
            {
                foreach (string filter in filters)
                {
                    picker.FileTypeFilter.Add(filter);
                }
            }
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                if (addToRecent)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
                if (addToFuture)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(file);
                }
                return file;
            }
            else
            {
                return null;
            }
        }

        public static async Task<StorageFile> ShowSaveFilePicker(string suggestedFileName = null, string defaultFileExtension = null, Tuple<string, IList<string>>[] fileExtensions = null, string enterpriseId = null, PickerLocationId startLocation = PickerLocationId.PicturesLibrary, bool addToRecent = false, bool addToFuture = false, string commmitButtonText = null)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = startLocation;
            if (commmitButtonText != null)
            {
                picker.CommitButtonText = commmitButtonText;
            }
            if (defaultFileExtension != null)
            {
                picker.DefaultFileExtension = defaultFileExtension;
            }
            if (enterpriseId != null)
            {
                picker.EnterpriseId = enterpriseId;
            }
            if (suggestedFileName != null)
            {
                picker.SuggestedFileName = suggestedFileName;
            }
            if (fileExtensions != null)
            {
                foreach (var ext in fileExtensions)
                {
                    picker.FileTypeChoices.Add(ext.Item1, ext.Item2);
                }
            }

            StorageFile file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                if (addToRecent)
                {
                    StorageApplicationPermissions.MostRecentlyUsedList.Add(file);
                }
                if (addToFuture)
                {
                    StorageApplicationPermissions.FutureAccessList.Add(file);
                }
                return file;
            }
            else
            {
                return null;
            }
        }
    }
}
