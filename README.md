
Fluent XAML Theme Editor 
===

![using component overrides](XamlThemeEditor_screenshot.png)

This repo contains the full solution and source code to the Fluent XAML Theme Editor - a tool that helps demonstrate the flexibility of the [Fluent Design System](https://www.microsoft.com/design/fluent/) as well as supports the app development process by generating XAML markup for our ResourceDictionary framework used in Universal Windows Platform (UWP) applications.

The source code located in this repo was created with the Universal Windows Platform available in Visual Studio and is designed to run on desktop, mobile and future devices that support the Universal Windows Platform.

> **Note:** If you are unfamiliar with XAML, the Universal Windows Platform, Fluent Design or resources and ResourceDictionaries, it is highly recommended that you visit the [Design Universal Windows Platform](https://developer.microsoft.com/en-us/windows/apps/design) site and familiarize yourself with the language and framework that this tool is intended for.

Universal Windows Platform development
---

This application requires Visual Studio 2017 Update 4 or higher and the Windows Software Development Kit (SDK) version 17744 or higher for Windows 10.

   [Get a free copy of Visual Studio 2017 Community Edition with support for building Universal Windows Platform apps](http://go.microsoft.com/fwlink/p/?LinkID=280676)

Additionally, to stay on top of the latest updates to Windows and the development tools, become a Windows Insider by joining the Windows Insider Program.

   [Become a Windows Insider](https://insider.windows.com/)

Using the tool
---
If you wish to get access to the source code without using Git you can download the zip file directly by clicking on the "Clone or Download" dropdown button at the top right of the repo landing page and selecting the last option labeled "Download Zip".

Theming for Downlevel
---
Although the API used in the exported code for this tool is for version 17744 or greater, it's not too complicated to get your theme to work on earlier SDK versions.

When you export your theme, you'll see a ResourceDictionary markup with a ColorPaletteResources definition similar to this:

```xaml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Default">
        <ResourceDictionary.MergedDictionaries>
            <ColorPaletteResources Accent="#FF8961CC" AltHigh="#FF000000" AltLow="#FF000000" AltMedium="#FF000000" AltMediumHigh="#FF000000" AltMediumLow="#FF000000" BaseHigh="#FFFFFFFF" BaseLow="#FF64576B" BaseMedium="#FFB6AABC" BaseMediumHigh="#FFCBBFD0" BaseMediumLow="#FF8D8193" ChromeAltLow="#FFCBBFD0" ChromeBlackHigh="#FF000000" ChromeBlackLow="#FFCBBFD0" ChromeBlackMedium="#FF000000" ChromeBlackMediumLow="#FF000000" ChromeDisabledHigh="#FF64576B" ChromeDisabledLow="#FFB6AABC" ChromeGray="#FFA295A8" ChromeHigh="#FFA295A8" ChromeLow="#FF332041" ChromeMedium="#FF3F2E4B" ChromeMediumLow="#FF584960" ChromeWhite="#FFFFFFFF" ListLow="#FF3F2E4B" ListMedium="#FF64576B" />
        <ResourceDictionary>
            <Color x:Key="RegionColor">#FF262738</Color>
            <SolidColorBrush x:Key="RegionBrush" Color="{StaticResource RegionColor}" />
        </ResourceDictionary>
    </ResourceDictionary.MergedDictionaries>
</ResourceDictionary>
```

ColorPaletteResources is a friendly API for our SystemColors that sit within generic.xaml and allows for those SystemColors to be scoped at any level. 

If you wanted to enable this same theme to work downlevel, you would have to define each SystemColor individually with each color from your theme:

```xaml
<ResourceDictionary>
    <ResourceDictionary.ThemeDictionaries>
        <ResourceDictionary x:Key="Default">
            <Color x:Key="SystemAltHighColor">#FF000000</Color>
            <Color x:Key="SystemAltLowColor">#FF000000</Color>
            <Color x:Key="SystemAltMediumColor">#FF000000</Color>
            <Color x:Key="SystemAltMediumHighColor">#FF000000</Color>
            <Color x:Key="SystemAltMediumLowColor">#FF000000</Color>
            <Color x:Key="SystemBaseHighColor">#FFFFFFFF</Color>
            <Color x:Key="SystemBaseLowColor">#FF64576B</Color>
            <Color x:Key="SystemBaseMediumColor">#FFB6AABC</Color>
            <Color x:Key="SystemBaseMediumHighColor">#FFCBBFD0</Color>
            <Color x:Key="SystemBaseMediumLowColor">#FF8D8193</Color>
            <Color x:Key="SystemChromeAltLowColor">#FFCBBFD0</Color>
            <Color x:Key="SystemChromeBlackHighColor">#FF000000</Color>
            <Color x:Key="SystemChromeBlackLowColor">#FFCBBFD0</Color>
            <Color x:Key="SystemChromeBlackMediumLowColor">#FF000000</Color>
            <Color x:Key="SystemChromeBlackMediumColor">#FF000000</Color>
            <Color x:Key="SystemChromeDisabledHighColor">#FF64576B</Color>
            <Color x:Key="SystemChromeDisabledLowColor">#FFB6AABC</Color>
            <Color x:Key="SystemChromeHighColor">#FFA295A8</Color>
            <Color x:Key="SystemChromeLowColor">#FF332041</Color>
            <Color x:Key="SystemChromeMediumColor">#FF3F2E4B</Color>
            <Color x:Key="SystemChromeMediumLowColor">#FF584960</Color>
            <Color x:Key="SystemChromeWhiteColor">#FFFFFFFF</Color>
            <Color x:Key="SystemChromeGrayColor">#FFA295A8</Color>
            <Color x:Key="SystemListLowColor">#FF3F2E4B</Color>
            <Color x:Key="SystemListMediumColor">#FF64576B</Color>                
            <Color x:Key="SystemAccentColor">#FF8961CC</Color>
            
            <Color x:Key="RegionColor">#FF262738</Color>
            <SolidColorBrush x:Key="RegionBrush" Color="{StaticResource RegionColor}"/>
         </ResourceDictionary>
    </ResourceDictionary.ThemeDictionaries>
</ResourceDictionary>
```
In this case we're using the Lavendar theme to show this downlevel transition.

> **Warning:** Although this markup format change will enable your theme to be applied across controls in earlier SDK versions, it **will not** work on a page, container or control scoped level. ColorPaletteResources is the API that allows **scoping behavior**. This markup format will only work at the app.xaml level for earlier SDKs.

Contributions
---
This tool was created directly from the feature team and although we welcome your input and suggestions for improvements to the tool, we are not accepting any features or changes from the public at this time. Please check back regularly as we may evolve our contribution model in the future.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/). For more information, see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

See also
---

For additional Windows source code, tools and samples, please see [Windows on GitHub](http://microsoft.github.io/windows/). 
