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
