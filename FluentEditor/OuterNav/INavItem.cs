// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.ComponentModel;

namespace FluentEditor.OuterNav
{
    public interface INavItem : INotifyPropertyChanged
    {
        string Id { get; }
        string Title { get; }
        string Glyph { get; }
    }
}
