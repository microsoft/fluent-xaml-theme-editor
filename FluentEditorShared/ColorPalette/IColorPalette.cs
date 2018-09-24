// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;

namespace FluentEditorShared.ColorPalette
{
    // These classes are not intended to be viewmodels.
    // They deal with the data about an editable palette and are passed to special purpose controls for editing
    public interface IColorPalette
    {
        IColorPaletteEntry BaseColor { get; }
        int Steps { get; }
        IReadOnlyList<EditableColorPaletteEntry> Palette { get; }
        IReadOnlyList<ContrastColorWrapper> ContrastColors { get; set; }
    }
}
