// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace FluentEditorShared.ColorPalette
{
    public class ContrastColorWrapper
    {
        public ContrastColorWrapper(IColorPaletteEntry color, bool showInContrastList, bool showContrastErrors)
        {
            if(color == null)
            {
                throw new ArgumentNullException("color");
            }
            Color = color;
            ShowInContrastList = showInContrastList;
            ShowContrastErrors = showContrastErrors;
        }

        public IColorPaletteEntry Color { get; }

        public bool ShowInContrastList { get; }

        public bool ShowContrastErrors { get; }
    }
}
