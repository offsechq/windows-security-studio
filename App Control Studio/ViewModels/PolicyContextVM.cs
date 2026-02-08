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
// See here for more information: https://github.com/HotCakeX/Harden-Windows-Security/blob/main/LICENSE
//

using System.Diagnostics;
using AppControlManager.Others;
using Microsoft.UI.Xaml.Controls;

namespace AppControlManager.ViewModels;

/// <summary>
/// ViewModel for the PolicyContext page
/// </summary>
internal sealed partial class PolicyContextVM : ViewModelBase
{
	// InfoBar properties
	internal bool MainInfoBarIsOpen { get; set => SP(ref field, value); }
	internal string? MainInfoBarMessage { get; set => SP(ref field, value); }
	internal InfoBarSeverity MainInfoBarSeverity { get; set => SP(ref field, value); } = InfoBarSeverity.Informational;
	internal bool MainInfoBarIsClosable { get; set => SP(ref field, value); } = true;

	/// <summary>
	/// Gets or sets the Persistent Library toggle state.
	/// Mirrors the App.Settings value and persists it when changed.
	/// </summary>
	internal bool PersistentLibraryToggleState
	{
		get; set => SP(ref field, value);
	} = App.Settings.PersistentLibrary;

	/// <summary>
	/// Gets or sets the Encrypt Persistent Policies Library toggle state.
	/// </summary>
	internal bool EncryptPersistentPoliciesLibraryToggleState
	{
		get; set => SP(ref field, value);
	} = App.Settings.EncryptPersistentPoliciesLibrary;

	/// <summary>
	/// Gets or sets the Encryption Scope toggle state.
	/// True = User Scope, False = Machine Scope.
	/// </summary>
	internal bool EncryptionScopeIsUserToggleState
	{
		get; set => SP(ref field, value);
	} = App.Settings.EncryptionScopeIsUser;

	/// <summary>
	/// Handles the Persistent Library toggle being toggled.
	/// When turned off, clears the persistent cache from disk.
	/// </summary>
	internal void PersistentLibraryToggle_Toggled()
	{
		App.Settings.PersistentLibrary = PersistentLibraryToggleState;

		// When the persistent library is disabled, clear the cache
		if (!PersistentLibraryToggleState)
		{
			PolicyLibraryCache.Clear();
		}
	}

	/// <summary>
	/// Handles the Encrypt Persistent Policies Library toggle being toggled.
	/// </summary>
	internal void EncryptPersistentPoliciesLibraryToggle_Toggled()
	{
		App.Settings.EncryptPersistentPoliciesLibrary = EncryptPersistentPoliciesLibraryToggleState;
	}

	/// <summary>
	/// Handles the Encryption Scope toggle being toggled.
	/// </summary>
	internal void EncryptionScopeToggle_Toggled()
	{
		App.Settings.EncryptionScopeIsUser = EncryptionScopeIsUserToggleState;
	}

	/// <summary>
	/// Opens the application configuration directory in File Explorer
	/// </summary>
	internal void OpenConfigDirectory()
	{
		try
		{
			string configPath = GlobalVars.UserConfigDir;

			if (System.IO.Directory.Exists(configPath))
			{
				_ = Process.Start(new ProcessStartInfo
				{
					FileName = configPath,
					UseShellExecute = true
				});
			}
			else
			{
				MainInfoBarMessage = "Configuration directory does not exist.";
				MainInfoBarSeverity = InfoBarSeverity.Warning;
				MainInfoBarIsOpen = true;
			}
		}
		catch (System.Exception ex)
		{
			MainInfoBarMessage = $"Failed to open config directory: {ex.Message}";
			MainInfoBarSeverity = InfoBarSeverity.Error;
			MainInfoBarIsOpen = true;
		}
	}

	/// <summary>
	/// Manually triggers garbage collection to free up memory
	/// </summary>
	internal void OptimizeMemory()
	{
		try
		{
			long before = System.GC.GetTotalMemory(false);

			System.GC.Collect();
			System.GC.WaitForPendingFinalizers();
			System.GC.Collect();

			long after = System.GC.GetTotalMemory(true);

			long freedMB = (before - after) / (1024 * 1024);

			MainInfoBarMessage = $"Memory optimized. Freed approximately {freedMB} MB.";
			MainInfoBarSeverity = InfoBarSeverity.Success;
			MainInfoBarIsOpen = true;
		}
		catch (System.Exception ex)
		{
			MainInfoBarMessage = $"Failed to optimize memory: {ex.Message}";
			MainInfoBarSeverity = InfoBarSeverity.Error;
			MainInfoBarIsOpen = true;
		}
	}
}
