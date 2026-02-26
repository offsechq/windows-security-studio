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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AppControlManager.Others;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.System;

#if HARDEN_SYSTEM_SECURITY
using HardenSystemSecurity.Others;
using AppControlManager.ViewModels;
namespace HardenSystemSecurity.ViewModels;
#endif

#if APP_CONTROL_MANAGER
namespace AppControlManager.ViewModels;
#endif

#pragma warning disable IDE0063
// Do not simplify using statements, keep them scoped for proper disposal otherwise files will be in use until the method is exited

internal sealed partial class UpdateVM : ViewModelBase
{
	private static readonly string DefaultUpdateButtonContent = GlobalVars.GetStr("UpdateNavItem/ToolTipService/ToolTip");

	internal UpdateVM()
	{
		MainInfoBar = new InfoBarSettings(
			() => MainInfoBarIsOpen, value => MainInfoBarIsOpen = value,
			() => MainInfoBarMessage, value => MainInfoBarMessage = value,
			() => MainInfoBarSeverity, value => MainInfoBarSeverity = value,
			() => MainInfoBarIsClosable, value => MainInfoBarIsClosable = value,
			Dispatcher, null, null);

		AppPackageInstallerInfoBar = new InfoBarSettings(
			() => AppPackageInstallerInfoBarIsOpen, value => AppPackageInstallerInfoBarIsOpen = value,
			() => AppPackageInstallerInfoBarMessage, value => AppPackageInstallerInfoBarMessage = value,
			() => AppPackageInstallerInfoBarSeverity, value => AppPackageInstallerInfoBarSeverity = value,
			() => AppPackageInstallerInfoBarIsClosable, value => AppPackageInstallerInfoBarIsClosable = value,
			Dispatcher, null, null);
	}

	internal readonly InfoBarSettings MainInfoBar;

	#region UI-Bound Properties

	/// <summary>
	/// Whether the UI elements are enabled or disabled.
	/// </summary>
	internal bool ElementsAreEnabled
	{
		get; set
		{
			if (SP(ref field, value))
			{
				MainInfoBarIsClosable = field;
				AppPackageInstallerInfoBarIsClosable = field;
				ProgressBarVisibility = field ? Visibility.Collapsed : Visibility.Visible;
			}
		}
	} = true;

	/// <summary>
	/// Content of the main update button
	/// </summary>
	internal string UpdateButtonContent { get; set => SP(ref field, value); } = DefaultUpdateButtonContent;

	internal bool MainInfoBarIsOpen { get; set => SP(ref field, value); }
	internal string? MainInfoBarMessage { get; set => SP(ref field, value); }
	internal InfoBarSeverity MainInfoBarSeverity { get; set => SP(ref field, value); } = InfoBarSeverity.Informational;
	internal bool MainInfoBarIsClosable { get; set => SP(ref field, value); }

	internal Visibility ProgressBarVisibility { get; set => SP(ref field, value); } = Visibility.Collapsed;

	internal double ProgressBarValue { get; set => SP(ref field, value); }

	internal bool ProgressBarIsIndeterminate { get; set => SP(ref field, value); }

	internal bool WhatsNewInfoBarIsOpen { get; set => SP(ref field, value); }

	internal Visibility RatingsSectionVisibility { get; set => SP(ref field, value); } = Visibility.Collapsed;

	/// <summary>
	/// When true, Settings page should bring the Update section into view on next load.
	/// </summary>
	internal bool NavigateToUpdateSectionOnNextSettingsLoad { get; set; }

	#endregion

	/// <summary>
	/// Event handler for check for update button - checks for updates via GitHub
	/// </summary>
	internal async void CheckForUpdateButton_Click()
	{
		try
		{
			ElementsAreEnabled = false;
			ProgressBarValue = 0;
			ProgressBarIsIndeterminate = true;
			WhatsNewInfoBarIsOpen = false;

			MainInfoBar.WriteInfo(GlobalVars.GetStr("CheckingForUpdate"));

			// Check for update asynchronously via GitHub
			UpdateCheckResponse updateCheckResult = await Task.Run(AppUpdate.CheckGitHub);

			// If a new version is available
			if (updateCheckResult.IsNewVersionAvailable)
			{
				MainInfoBar.WriteInfo(GlobalVars.GetStr("VersionComparison") + App.currentAppVersion + GlobalVars.GetStr("WhileOnlineVersion") + updateCheckResult.OnlineVersion + GlobalVars.GetStr("UpdatingApplication"));

				WhatsNewInfoBarIsOpen = true;

				try
				{
					string stagingArea = StagingArea.NewStagingArea("AppUpdate").ToString();

					// Store the latest MSIXBundle version download link after retrieving it from GitHub text file
					string onlineDownloadURLRaw = await SecHttpClient.Instance.GetStringAsync(GlobalVars.AppUpdateDownloadLinkURL);
					Uri onlineDownloadURL = new(onlineDownloadURLRaw.Trim(), UriKind.Absolute);

					// Location of the MSIXBundle package where it will be saved after downloading it from GitHub
					string packageSavePath = Path.Combine(stagingArea, "update.msixbundle");

					MainInfoBar.WriteInfo(GlobalVars.GetStr("DownloadingPackage"));

					// Send an Async get request to the url and specify to stop reading after headers are received for better efficiently
					using (HttpResponseMessage response = await SecHttpClient.Instance.GetAsync(onlineDownloadURL, HttpCompletionOption.ResponseHeadersRead))
					{
						// Ensure that the response is successful (status code 2xx); otherwise, throw an exception
						_ = response.EnsureSuccessStatusCode();

						// Retrieve the total file size from the Content-Length header (if available)
						long? totalBytes = response.Content.Headers.ContentLength;

						// Open a stream to read the response content asynchronously
						await using (Stream contentStream = await response.Content.ReadAsStreamAsync())
						{
							ProgressBarIsIndeterminate = !totalBytes.HasValue;

							// Open a file stream to save the downloaded data locally
							await using (FileStream fileStream = new(
								packageSavePath,                 // Path to save the file
								FileMode.Create,                 // Create a new file or overwrite if it exists
								FileAccess.Write,                // Write-only access
								FileShare.None,                  // Do not allow other processes to access the file
								bufferSize: 8192,                // Set buffer size to 8 KB
								useAsync: true))                 // Enable asynchronous operations for the file stream
							{
								// Define a buffer to hold data chunks as they are read
								byte[] buffer = new byte[8192];
								long totalReadBytes = 0;         // Track the total number of bytes read
								int readBytes;                   // Holds the count of bytes read in each iteration
								double lastReportedProgress = 0; // Tracks the last reported download progress

								// Loop to read from the content stream in chunks until no more data is available
								while ((readBytes = await contentStream.ReadAsync(buffer)) > 0)
								{
									// Write the buffer to the file stream
									await fileStream.WriteAsync(buffer.AsMemory(0, readBytes));
									totalReadBytes += readBytes;  // Update the total bytes read so far

									// If the total file size is known, calculate and report progress
									if (totalBytes.HasValue)
									{
										// Calculate the current download progress as a percentage
										double progressPercentage = (double)totalReadBytes / totalBytes.Value * 100;

										// Only update the ProgressBar if progress has increased by at least 1% to avoid constantly interacting with the UI thread
										if (progressPercentage - lastReportedProgress >= 1)
										{
											// Update the last reported progress
											lastReportedProgress = progressPercentage;

											// Update the UI ProgressBar value on the dispatcher thread
											_ = Dispatcher.TryEnqueue(() =>
											{
												ProgressBarValue = progressPercentage;
											});
										}
									}
								}
							}
						}
					}

					MainInfoBar.WriteInfo(GlobalVars.GetStr("DownloadSuccess") + packageSavePath);

					ProgressBarIsIndeterminate = true;

					// Download and trust the signing certificate from the same release
					MainInfoBar.WriteInfo("Downloading signing certificate...");

					// Derive certificate URL from the bundle URL (same release, different filename)
					string bundleUrlString = onlineDownloadURL.ToString();
					int lastSlashIndex = bundleUrlString.LastIndexOf('/');
					string certDownloadUrlString = string.Concat(bundleUrlString.AsSpan(0, lastSlashIndex + 1), "OFFSECHQ_CodeSigning.cer");
					Uri certDownloadUrl = new(certDownloadUrlString);

					string certSavePath = Path.Combine(stagingArea, "OFFSECHQ_CodeSigning.cer");

					// Download the certificate file
					using (HttpResponseMessage certResponse = await SecHttpClient.Instance.GetAsync(certDownloadUrl))
					{
						_ = certResponse.EnsureSuccessStatusCode();
						await using (FileStream certFileStream = new(certSavePath, FileMode.Create, FileAccess.Write, FileShare.None))
						{
							await certResponse.Content.CopyToAsync(certFileStream);
						}
					}

					MainInfoBar.WriteInfo("Installing signing certificate to trusted store...");

					// Import the certificate to LocalMachine\TrustedPeople store
					using (X509Certificate2 signingCert = X509CertificateLoader.LoadCertificateFromFile(certSavePath))
					{
						const string expectedPublisher = "CN=520167C9-C63F-4572-841C-0538368FD2C2";
						if (!string.Equals(signingCert.Subject, expectedPublisher, StringComparison.OrdinalIgnoreCase))
						{
							throw new InvalidOperationException($"Unexpected signing certificate subject '{signingCert.Subject}'.");
						}

						using X509Store trustedPeopleStore = new(StoreName.TrustedPeople, StoreLocation.LocalMachine);
						trustedPeopleStore.Open(OpenFlags.ReadWrite);

						bool alreadyTrusted = trustedPeopleStore.Certificates
							.Cast<X509Certificate2>()
							.Any(cert => string.Equals(cert.Thumbprint, signingCert.Thumbprint, StringComparison.OrdinalIgnoreCase));

						if (!alreadyTrusted)
						{
							trustedPeopleStore.Add(signingCert);
							MainInfoBar.WriteInfo("Signing certificate installed successfully.");
						}
						else
						{
							MainInfoBar.WriteInfo("Signing certificate already trusted. Skipping import.");
						}

						// Keep only the currently used OFFSECHQ signing certificate to avoid store growth.
						List<X509Certificate2> staleCerts = trustedPeopleStore.Certificates
							.Cast<X509Certificate2>()
							.Where(cert => string.Equals(cert.Subject, signingCert.Subject, StringComparison.OrdinalIgnoreCase) &&
								!string.Equals(cert.Thumbprint, signingCert.Thumbprint, StringComparison.OrdinalIgnoreCase))
							.ToList();

						foreach (X509Certificate2 staleCert in staleCerts)
						{
							trustedPeopleStore.Remove(staleCert);
							staleCert.Dispose();
						}

						if (staleCerts.Count > 0)
						{
							MainInfoBar.WriteInfo($"Removed {staleCerts.Count} old signing certificate(s) from TrustedPeople.");
						}

						trustedPeopleStore.Close();
					}

					MainInfoBar.WriteInfo(GlobalVars.GetStr("DownloadsFinished"));

					await InstallAppPackage(packageSavePath, UseHardenedInstallationProcess, MainInfoBar);

					MainInfoBar.WriteSuccess(GlobalVars.GetStr("UpdateSuccess"));

					UpdateButtonContent = GlobalVars.GetStr("UpdatesInstalled");
				}
				catch
				{
					WhatsNewInfoBarIsOpen = false;
					throw;
				}
			}
			else
			{
				UpdateButtonContent = DefaultUpdateButtonContent;
				MainInfoBar.WriteSuccess(GlobalVars.GetStr("AlreadyUpdated"));
			}
		}
		catch (Exception ex)
		{
			UpdateButtonContent = DefaultUpdateButtonContent;
			MainInfoBar.WriteError(ex, GlobalVars.GetStr("UpdateCheckError"));
		}
		finally
		{
			ProgressBarValue = 0;
			ElementsAreEnabled = true;
			ProgressBarIsIndeterminate = true;
		}
	}

	/// <summary>
	/// Opens GitHub releases page for rating/feedback.
	/// </summary>
	internal async void LaunchRating()
	{
		try
		{
			// Open GitHub releases page for feedback
			_ = await Launcher.LaunchUriAsync(new Uri("https://github.com/OFFSECHQ/windows-security-studio/releases"));
		}
		catch (Exception ex)
		{
			MainInfoBar.WriteError(ex);
		}
	}


	#region App Package Installer's Page

	[JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
	[JsonSerializable(typeof(string[]))] // Used to deserialize MS Defender results
	private sealed partial class MSDefenderJsonContext : JsonSerializerContext
	{
	}

	/// <summary>
	/// Removes any existing ASR rule exclusions that belong to the GitHub-distributed App Control Studio package.
	/// </summary>
	private static void RemoveExistingAppControlManagerASRExclusions()
	{
		string? ASROutput = null;

		const string comCommand = "get ROOT\\Microsoft\\Windows\\Defender MSFT_MpPreference AttackSurfaceReductionOnlyExclusions";

		try
		{
			ASROutput = ProcessStarter.RunCommand(GlobalVars.ComManagerProcessPath, comCommand);

			// If there are ASR rule exclusions, find ones that belong to App Control Studio and remove them
			if (!string.IsNullOrWhiteSpace(ASROutput))
			{
				// Deserialize the JSON string
				string[]? ASROutputArrayCleaned = JsonSerializer.Deserialize(ASROutput, MSDefenderJsonContext.Default.StringArray) as string[];

				// If there were ASR rules exceptions
				if (ASROutputArrayCleaned is not null && ASROutputArrayCleaned.Length > 0)
				{
					List<string> asrRulesToRemove = [];

					// Find all the rules that belong to the App Control Studio
					foreach (string item in ASROutputArrayCleaned)
					{
						if (item.Contains("__52ydnp6c4w3g1", StringComparison.OrdinalIgnoreCase))
						{
							asrRulesToRemove.Add(item);
						}
					}

					// If any of the rules belong to the App Control Studio
					if (asrRulesToRemove.Count > 0)
					{
						// Wrap them with double quotes and separate them with a space
						string asrRulesToRemoveFinal = string.Join(" ", asrRulesToRemove.Select(item => $"\"{item}\""));

						_ = ProcessStarter.RunCommand(GlobalVars.ComManagerProcessPath, $@"wmi stringarray ROOT\Microsoft\Windows\Defender MSFT_MpPreference remove AttackSurfaceReductionOnlyExclusions {asrRulesToRemoveFinal}");
					}
				}
			}
		}
		catch (JsonException Jex)
		{
			Logger.Write(string.Format(GlobalVars.GetStr("ASRRulesDeserializationFailedMessage"), ASROutput, Jex.Message));
		}
		catch (Exception ex)
		{
			Logger.Write(GlobalVars.GetStr("ASRError") + ex.Message);
		}
	}

	private static void AddASRExclusionsForAppControlManager(PackageManager packageManager)
	{
		try
		{
			// This correctly lists all packages for all users and gets the latest version package which we just installed which will be in staged state.
			Package? AppControlManagerPackage = packageManager.FindPackages("OFFSECHQ.AppControlStudio_52ydnp6c4w3g1")
				.OrderByDescending(p => new Version(p.Id.Version.Major, p.Id.Version.Minor, p.Id.Version.Build, p.Id.Version.Revision))
				.FirstOrDefault();

			if (AppControlManagerPackage is null)
				return;

			string AppControlInstallFolder = AppControlManagerPackage.EffectivePath;

			// Construct the paths to the .exe and .dll files of the App Control Studio
			string path1 = Path.Combine(AppControlInstallFolder, "AppControlManager.exe");
			string path2 = Path.Combine(AppControlInstallFolder, "AppControlManager.dll");
			string path3 = Path.Combine(AppControlInstallFolder, "CppInterop", "ComManager.exe");

			// Adding the extra executables included in the package so they will be allowed to run as well
			_ = ProcessStarter.RunCommand(GlobalVars.ComManagerProcessPath, $"wmi stringarray ROOT\\Microsoft\\Windows\\Defender MSFT_MpPreference add AttackSurfaceReductionOnlyExclusions \"{path1}\" \"{path2}\" \"{path3}\" ");
		}
		catch (Exception ex)
		{
			Logger.Write(GlobalVars.GetStr("ASRAddError") + ex.Message);
		}
	}

	/// <summary>
	/// Navigate to the Package Installer sub-page.
	/// </summary>
	internal void NavigateToAppPackageInstallerPage_Click() =>
		ViewModelProvider.NavigationService.Navigate(typeof(Pages.UpdatePageCustomMSIXPath), null);

	/// <summary>
	/// Whether the installation process must use hardened procedures.
	/// </summary>
	internal bool UseHardenedInstallationProcess { get; set => SP(ref field, value); } = true;

	/// <summary>
	/// Common name of the on-device generated certificate used to sign the App Control Studio MSIXBundle package.
	/// </summary>
	private const string CertCommonName = "OFFSECHQ";

	internal readonly InfoBarSettings AppPackageInstallerInfoBar;
	internal bool AppPackageInstallerInfoBarIsOpen { get; set => SP(ref field, value); }
	internal string? AppPackageInstallerInfoBarMessage { get; set => SP(ref field, value); }
	internal InfoBarSeverity AppPackageInstallerInfoBarSeverity { get; set => SP(ref field, value); } = InfoBarSeverity.Informational;
	internal bool AppPackageInstallerInfoBarIsClosable { get; set => SP(ref field, value); }

	/// <summary>
	/// The package path that the user supplied.
	/// </summary>
	internal string? LocalPackageFilePath { get; set => SP(ref field, value); }

	/// <summary>
	/// Opens a file picker to select a MSIX/MSIXBundle package file.
	/// </summary>
	internal void BrowseForCustomMSIXPathButton_Click() =>
		LocalPackageFilePath = FileDialogHelper.ShowFilePickerDialog("MSIX/MSIXBundle files|*.msixbundle;*.msix");

	/// <summary>
	/// Event handler to clear the selected file path.
	/// </summary>
	internal void ClearSelectedFilePath() => LocalPackageFilePath = null;

	/// <summary>
	/// Event handler for the UI button.
	/// </summary>
	internal async void InstallButton_Click()
	{
		try
		{
			ElementsAreEnabled = false;
			await InstallAppPackage(LocalPackageFilePath, UseHardenedInstallationProcess, AppPackageInstallerInfoBar);
		}
		catch (Exception ex)
		{
			AppPackageInstallerInfoBar.WriteError(ex);
		}
		finally
		{
			ElementsAreEnabled = true;
		}
	}

	/// <summary>
	/// Installs an app package from a user-supplied path.
	/// It can automatically detect if the package is signed or unsigned and perform signing if needed.
	/// </summary>
	/// <param name="packagePath"></param>
	/// <exception cref="InvalidOperationException"></exception>
	private static async Task InstallAppPackage(
		string? packagePath,
		bool UseHardenedInstallationProcess,
		InfoBarSettings infoBar)
	{
		if (packagePath is null)
		{
			throw new InvalidOperationException("You must provide a valid package path.");
		}

		await Task.Run(() =>
		{
			bool isNonStoreACM = false;

			infoBar.WriteInfo($"Getting the package details for: '{packagePath}'");

			List<AllFileSigners> possibleExistingSigners = AllCertificatesGrabber.GetAllFileSigners(packagePath);

			// Only attempt signing if the package doesn't already have signatures
			if (possibleExistingSigners.Count == 0)
			{
				LLPackageReader.PackageDetails packageDits = LLPackageReader.GetPackageDetails(packagePath);

				// Determine whether this is the App Control Studio app package provided in GitHub releases that the user is trying to install
				isNonStoreACM = string.Equals(packageDits.CertCN, CertCommonName, StringComparison.Ordinal);

				infoBar.WriteInfo($"Package details retrieved. Package Publisher: '{packageDits.CertCN}', Package Hashing Algorithm: '{packageDits.HashAlgorithm}'.");

				// Remove any certificates with the specified common name that may already exist on the system form previous attempts
				CertificateGenerator.DeleteCertificateByCN(packageDits.CertCN);

				// Generate a new certificate
				using X509Certificate2 generatedCert = CertificateGenerator.GenerateSelfSignedCertificate(
				subjectName: packageDits.CertCN,
				validityInYears: 100,
				keySize: 4096,
				hashAlgorithm: packageDits.HashAlgorithm,
				storeLocation: CertificateGenerator.CertificateStoreLocation.Machine,
				cerExportFilePath: null,
				friendlyName: packageDits.CertCN,
				UserProtectedPrivateKey: UseHardenedInstallationProcess,
				ExportablePrivateKey: false);

				// Sign the package
				CommonCore.Signing.Main.SignAppPackage(packagePath, generatedCert);

				// Remove any certificates with the specified common name again
				// Because the existing one contains private keys and we don't want that
				CertificateGenerator.DeleteCertificateByCN(packageDits.CertCN);

				// Adding the certificate to the 'Local Machine/Trusted Root Certification Authorities' store with public key only.
				// This safely stores the certificate on your device, ensuring its private key does not exist so cannot be used to sign anything else.
				CertificateGenerator.StoreCertificateInStore(generatedCert, CertificateGenerator.CertificateStoreLocation.Machine, true);
			}
			else
			{
				infoBar.WriteInfo("The package is already signed. Proceeding with installation.");
			}

			PackageManager packageManager = new();

			infoBar.WriteInfo($"Installing '{packagePath}'");

			// https://learn.microsoft.com/uwp/api/windows.management.deployment.addpackageoptions
			AddPackageOptions options = new()
			{
				DeferRegistrationWhenPackagesAreInUse = true,
				ForceUpdateFromAnyVersion = true
			};

			IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> deploymentOperation = packageManager.AddPackageByUriAsync(new Uri(packagePath), options);

			// This event is signaled when the operation completes
			using ManualResetEvent opCompletedEvent = new(false);

			// The delegate
			deploymentOperation.Completed = (depProgress, status) => { _ = opCompletedEvent.Set(); };

			// Wait until the operation completes
			_ = opCompletedEvent.WaitOne();

			// Check the status of the operation
			if (deploymentOperation.Status == AsyncStatus.Error)
			{
				DeploymentResult deploymentResult = deploymentOperation.GetResults();
				throw new InvalidOperationException($"There was a problem installing '{packagePath}': {deploymentOperation.ErrorCode} - {deploymentResult.ErrorText}");
			}
			else if (deploymentOperation.Status == AsyncStatus.Canceled)
			{
				infoBar.WriteWarning("App installation was cancelled.");
			}
			else if (deploymentOperation.Status == AsyncStatus.Completed)
			{
				infoBar.WriteSuccess($"Successfully installed '{packagePath}'");
			}
			else
			{
				throw new InvalidOperationException($"There was an unknown problem installing '{packagePath}'");
			}

			if (isNonStoreACM)
			{
				RemoveExistingAppControlManagerASRExclusions();
				AddASRExclusionsForAppControlManager(packageManager);
			}
		});
	}

	#endregion

}
