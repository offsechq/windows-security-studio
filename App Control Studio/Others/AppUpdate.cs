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

using System.Threading.Tasks;

#if HARDEN_SYSTEM_SECURITY
using AppControlManager.Others;
using HardenSystemSecurity.ViewModels;
using UpdateVM = HardenSystemSecurity.ViewModels.UpdateVM;
namespace HardenSystemSecurity.Others;
#endif

#if APP_CONTROL_MANAGER
using AppControlManager.ViewModels;
namespace AppControlManager.Others;
#endif

/// <summary>
/// AppUpdate class is responsible for checking for application updates via GitHub.
/// </summary>
internal static class AppUpdate
{
	private static readonly string DefaultUpdateButtonContent = GlobalVars.GetStr("UpdateNavItem/ToolTipService/ToolTip");

	/// <summary>
	/// Event triggered when an update is available.
	/// Includes details about the availability status and the version.
	/// </summary>
	internal static event EventHandler<UpdateAvailableEventArgs>? UpdateAvailable;

	private static UpdateVM UpdateVM { get; } = ViewModelProvider.UpdateVM;

	/// <summary>
	/// Downloads the version file from GitHub,
	/// Checks the online version against the current app version,
	/// and raises the UpdateAvailable event if an update is found.
	/// </summary>
	internal static UpdateCheckResponse CheckGitHub()
	{
		string versionsResponse = SecHttpClient.Instance.GetStringAsync(GlobalVars.AppVersionLinkURL).GetAwaiter().GetResult().Trim();

		if (versionsResponse.Length > 0 && (versionsResponse[0] == 'v' || versionsResponse[0] == 'V'))
		{
			versionsResponse = versionsResponse[1..];
		}

		if (!Version.TryParse(versionsResponse, out Version? onlineAvailableVersion))
		{
			throw new InvalidOperationException($"Invalid online version format: '{versionsResponse}'");
		}

		bool isUpdateAvailable = onlineAvailableVersion > App.currentAppVersion;

		// Raise the UpdateAvailable event if there are subscribers
		UpdateAvailable?.Invoke(
			null,
			new UpdateAvailableEventArgs(isUpdateAvailable, onlineAvailableVersion)
		);

		// If a new version is available
		if (isUpdateAvailable)
		{
			// Set the text for the button in the update page
			UpdateVM.UpdateButtonContent = string.Format(
				GlobalVars.GetStr("InstallVersionMessage"),
				onlineAvailableVersion);
		}
		else
		{
			UpdateVM.UpdateButtonContent = DefaultUpdateButtonContent;
			Logger.Write(GlobalVars.GetStr("TheAppIsUpToDate"));
		}

		return new UpdateCheckResponse(
			isUpdateAvailable,
			onlineAvailableVersion
		);
	}

	/// <summary>
	/// Runs at startup to perform update check via GitHub.
	/// </summary>
	internal static void CheckAtStartup()
	{
		_ = Task.Run(() =>
		{
			try
			{
				if (App.Settings.AutoCheckForUpdateAtStartup)
				{
					_ = CheckGitHub();
				}
			}
			catch (Exception ex)
			{
				Logger.Write(ex);
			}
		});
	}

}
