// MIT License
//
// Copyright (c) 2023-Present - Violet Hansen - (aka HotCakeX on GitHub) - Email Address: spynetgirl@outlook.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// See here for more information: https://github.com/OFFSECHQ/windows-security-studio/blob/main/LICENSE
//

using System.Collections.Generic;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Globalization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Dispatching;
using CommonCore.AppSettings;

using WindowsSecurityStudio.ViewModels;
using WindowsSecurityStudio.WindowComponents;
namespace WindowsSecurityStudio.ViewModels;


internal sealed partial class SettingsVM : ViewModelBase
{
	private NavigationService Nav { get; } = ViewModelProvider.NavigationService;

	internal SettingsVM()
	{
		MainInfoBar = new InfoBarSettings(
			() => MainInfoBarIsOpen, value => MainInfoBarIsOpen = value,
			() => MainInfoBarMessage, value => MainInfoBarMessage = value,
			() => MainInfoBarSeverity, value => MainInfoBarSeverity = value,
			() => MainInfoBarIsClosable, value => MainInfoBarIsClosable = value,
			Dispatcher, null, null);

		// Populate the ComboBoxes' ItemsSource collections
		LoadLanguages();


	}

	private void LoadLanguages()
	{
		LanguageOptions.Add(new("English", "ms-appx:///Assets/CountryFlags/usa-240.png"));
		LanguageOptions.Add(new("עברית", "ms-appx:///Assets/CountryFlags/israel-240.png"));
		LanguageOptions.Add(new("Ελληνικά", "ms-appx:///Assets/CountryFlags/greece-240.png"));
		LanguageOptions.Add(new("हिंदी", "ms-appx:///Assets/CountryFlags/india-240.png"));
		LanguageOptions.Add(new("Polski", "ms-appx:///Assets/CountryFlags/poland-240.png"));
		LanguageOptions.Add(new("العربية", "ms-appx:///Assets/CountryFlags/saudi-arabia-240.png"));
		LanguageOptions.Add(new("Español", "ms-appx:///Assets/CountryFlags/mexico-240.png"));
		LanguageOptions.Add(new("മലയാളം", "ms-appx:///Assets/CountryFlags/india-240.png"));
		LanguageOptions.Add(new("Deutsch", "ms-appx:///Assets/CountryFlags/germany-240.png"));
		LanguageOptions.Add(new("Français", "ms-appx:///Assets/CountryFlags/france-240.png"));
	}

	/// <summary>
	/// The main InfoBar for this VM.
	/// </summary>
	internal readonly InfoBarSettings MainInfoBar;

	internal bool MainInfoBarIsOpen { get; set => SP(ref field, value); }
	internal string? MainInfoBarMessage { get; set => SP(ref field, value); }
	internal InfoBarSeverity MainInfoBarSeverity { get; set => SP(ref field, value); } = InfoBarSeverity.Informational;
	internal bool MainInfoBarIsClosable { get; set => SP(ref field, value); }

	private MainWindowVM ViewModelMainWindow { get; } = ViewModelProvider.MainWindowVM;

	internal bool UIFlowDirectionToggleSwitch
	{
		get; set
		{
			if (SP(ref field, value))
			{
				App.Settings.ApplicationGlobalFlowDirection = field ? "LeftToRight" : "RightToLeft";
			}
		}
	} = string.Equals(App.Settings.ApplicationGlobalFlowDirection, "LeftToRight", StringComparison.OrdinalIgnoreCase);

	private enum NavViewLocation
	{
		Left = 0,
		Top = 1
	}

	internal int NavigationMenuLocationComboBoxSelectedIndex
	{
		get; set
		{
			if (SP(ref field, value))
			{
				string x = ((NavViewLocation)field).ToString();

				// Raise the global OnNavigationViewLocationChanged event
				NavigationViewLocationManager.OnNavigationViewLocationChanged(x);

				App.Settings.NavViewPaneDisplayMode = x;
			}
		}
	}

	private static readonly Dictionary<string, int> SupportedLanguages = new(StringComparer.OrdinalIgnoreCase)
	{
		{ "en-US", 0 },
		{ "he", 1 },
		{ "el", 2 },
		{ "hi", 3 },
		{ "pl", 4 },
		{ "ar", 5 },
		{ "es", 6 },
		{ "ml", 7 },
		{ "de", 8 },
		{ "fr", 9 }
	};

	private static readonly string[] SupportedLanguagesReverse = [
		 "en-US",
		 "he",
		 "el",
		 "hi",
		 "pl",
		 "ar",
		 "es",
		 "ml",
		 "de",
		 "fr"
	];

	internal int LanguageComboBoxSelectedIndex
	{
		get; set
		{
			if (SP(ref field, value))
			{
				string x = SupportedLanguagesReverse[field];

				ApplicationLanguages.PrimaryLanguageOverride = x;
				App.Settings.ApplicationGlobalLanguage = x;

				// Get reference to the MainWindow and refresh the localized content
				if (App.MainWindow is MainWindow mainWindow)
				{
					mainWindow.RefreshLocalizedContent();
				}

				// Refresh this page.
				Nav.RefreshSettingsPage();
			}
		}
	} = SupportedLanguages.TryGetValue(App.Settings.ApplicationGlobalLanguage, out int x) ? x : 0;

	/// <summary>
	/// Language Selection ComboBox ItemsSource
	/// </summary>
	internal readonly List<LanguageOption> LanguageOptions = [];

	private static readonly Dictionary<string, int> AppThemes = new(StringComparer.OrdinalIgnoreCase)
	{
		{"Use System Setting", 0 },
		{"Dark", 1 },
		{"Light", 2 }
	};
	private static readonly string[] AppThemesReverse = [
		"Use System Setting",
		"Dark" ,
		"Light"
	];

	internal int AppThemeComboBoxSelectedIndex
	{
		get; set
		{
			if (SP(ref field, value))
			{
				// Raise the global BackgroundChanged event
				AppThemeManager.OnAppThemeChanged(AppThemesReverse[field]);

				App.Settings.AppTheme = AppThemesReverse[field];
			}
		}
	} = AppThemes.TryGetValue(App.Settings.AppTheme, out int x) ? x : 0;

	private static readonly Dictionary<string, int> IconsStyles = new(StringComparer.OrdinalIgnoreCase)
	{
		{"Animated", 0 },
		{"Windows Accent", 1 },
		{"Monochromatic" , 2 }
	};
	private static readonly Dictionary<int, string> IconsStylesReverse = new()
	{
		{ 0, "Animated" },
		{ 1, "Windows Accent"},
		{ 2, "Monochromatic" }
	};

	internal int IconsStylesComboBoxSelectedIndex
	{
		get; set
		{
			if (SP(ref field, value))
			{
				if (IconsStylesReverse.TryGetValue(field, out string? x))
				{
					ViewModelMainWindow.OnIconsStylesChanged(x);

					App.Settings.IconsStyle = x;
				}
				else
				{
					Logger.Write($"Unknown Icons Style Index: {field}");
				}
			}
		}
	} = IconsStyles.TryGetValue(App.Settings.IconsStyle, out int x) ? x : 2;

	/// <summary>
	/// Set the version in the settings card to the current app version
	/// </summary>
	internal readonly string VersionTextBlockText = $"Version {App.currentAppVersion}";

	/// <summary>
	/// Set the year for the copyright section
	/// </summary>
	internal readonly string CopyRightSettingsExpanderDescription = $"© {DateTime.Now.Year}. All rights reserved.";

	/// <summary>
	/// Executed when flow direction toggle is changed.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	internal void FlowDirectionToggleSwitch_Toggled(object sender, RoutedEventArgs e)
	{
		MainWindowVM.SetCaptionButtonsFlowDirection(((ToggleSwitch)sender).IsOn ? FlowDirection.LeftToRight : FlowDirection.RightToLeft);

		// Needs to run via Dispatcher, otherwise the 1st double-click on the UI elements register as pass-through, meaning they will resize the window as if we clicked on an empty area on the TitleBar.
		_ = Dispatcher.TryEnqueue(DispatcherQueuePriority.Normal, () =>
		{
			// Get reference to the MainWindow and refresh the localized content
			if (App.MainWindow is MainWindow mainWindow)
			{
				mainWindow.SetRegionsForCustomTitleBar();
			}
		});
	}


}
