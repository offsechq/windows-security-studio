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
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;

namespace AppControlManager.Others;

/// <summary>
/// Provides persistent caching and optional DPAPI encryption for the policy library.
/// When the Persistent Library setting is enabled, policies are saved to a JSON file on disk
/// so they survive app restarts. When encryption is enabled, the cached file is protected
/// using the Windows Data Protection API with either User or Machine scope.
/// </summary>
internal static class PolicyLibraryCache
{
	/// <summary>
	/// The directory path where the persistent policy library cache is stored.
	/// </summary>
	private static readonly string CacheDirectory = Path.Combine(GlobalVars.UserConfigDir, "PolicyLibraryCache");

	/// <summary>
	/// The file name for the plain-text cache.
	/// </summary>
	private const string CacheFileName = "policies.json";

	/// <summary>
	/// The file name for the encrypted cache.
	/// </summary>
	private const string EncryptedCacheFileName = "policies.dat";

	/// <summary>
	/// Gets the full path to the plain-text cache file.
	/// </summary>
	private static string CacheFilePath => Path.Combine(CacheDirectory, CacheFileName);

	/// <summary>
	/// Gets the full path to the encrypted cache file.
	/// </summary>
	private static string EncryptedCacheFilePath => Path.Combine(CacheDirectory, EncryptedCacheFileName);

	/// <summary>
	/// Saves a collection of policies to the persistent cache on disk.
	/// If encryption is enabled in settings, the cache will be encrypted using DPAPI.
	/// </summary>
	/// <param name="policies">The collection of policies to persist.</param>
	internal static async Task SaveAsync(IReadOnlyList<CiPolicyInfo> policies)
	{
		try
		{
			// Ensure the cache directory exists
			if (!Directory.Exists(CacheDirectory))
			{
				_ = Directory.CreateDirectory(CacheDirectory);
			}

			// Serialize the policies to JSON using source-generated context for AOT compatibility
			string json = JsonSerializer.Serialize(policies, PolicyLibraryCacheJsonContext.Default.IReadOnlyListCiPolicyInfo);

			if (App.Settings.EncryptPersistentPoliciesLibrary)
			{
				// Encrypt and write the data using DPAPI
				await EncryptAndWriteAsync(json);

				// Remove the plain-text cache file if it exists
				if (File.Exists(CacheFilePath))
				{
					File.Delete(CacheFilePath);
				}
			}
			else
			{
				// Write plain-text JSON to disk
				await File.WriteAllTextAsync(CacheFilePath, json);

				// Remove the encrypted cache file if it exists
				if (File.Exists(EncryptedCacheFilePath))
				{
					File.Delete(EncryptedCacheFilePath);
				}
			}

			Logger.Write($"Policy library cache saved successfully with {policies.Count} policies.");
		}
		catch (Exception ex)
		{
			Logger.Write($"Failed to save policy library cache: {ex.Message}");
		}
	}

	/// <summary>
	/// Loads policies from the persistent cache on disk.
	/// If the cache was encrypted, it will be decrypted using DPAPI.
	/// </summary>
	/// <returns>A list of cached policies, or an empty list if no cache exists or loading fails.</returns>
	internal static async Task<List<CiPolicyInfo>> LoadAsync()
	{
		try
		{
			string? json = null;

			if (File.Exists(EncryptedCacheFilePath))
			{
				// Read and decrypt the encrypted cache
				json = await ReadAndDecryptAsync();
			}
			else if (File.Exists(CacheFilePath))
			{
				// Read the plain-text cache
				json = await File.ReadAllTextAsync(CacheFilePath);
			}

			if (!string.IsNullOrWhiteSpace(json))
			{
				List<CiPolicyInfo>? policies = JsonSerializer.Deserialize(json, PolicyLibraryCacheJsonContext.Default.ListCiPolicyInfo);

				if (policies is not null)
				{
					Logger.Write($"Policy library cache loaded successfully with {policies.Count} policies.");
					return policies;
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Write($"Failed to load policy library cache: {ex.Message}");
		}

		return [];
	}

	/// <summary>
	/// Deletes the persistent policy library cache from disk.
	/// Removes both encrypted and plain-text cache files.
	/// </summary>
	internal static void Clear()
	{
		try
		{
			if (File.Exists(CacheFilePath))
			{
				File.Delete(CacheFilePath);
			}

			if (File.Exists(EncryptedCacheFilePath))
			{
				File.Delete(EncryptedCacheFilePath);
			}

			Logger.Write("Policy library cache cleared.");
		}
		catch (Exception ex)
		{
			Logger.Write($"Failed to clear policy library cache: {ex.Message}");
		}
	}

	/// <summary>
	/// Encrypts the given JSON string using Windows Data Protection API and writes it to the encrypted cache file.
	/// The encryption scope (User or Machine) is determined by the current settings.
	/// </summary>
	/// <param name="json">The JSON string to encrypt.</param>
	private static async Task EncryptAndWriteAsync(string json)
	{
		// Determine the DPAPI protection descriptor based on the encryption scope setting
		// LOCAL=user uses the current user's credentials
		// LOCAL=machine uses the machine's credentials
		string protectionDescriptor = App.Settings.EncryptionScopeIsUser ? "LOCAL=user" : "LOCAL=machine";

		DataProtectionProvider provider = new(protectionDescriptor);

		// Convert the JSON string to an IBuffer
		IBuffer plainBuffer = CryptographicBuffer.ConvertStringToBinary(json, BinaryStringEncoding.Utf8);

		// Encrypt the data
		IBuffer encryptedBuffer = await provider.ProtectAsync(plainBuffer);

		// Write the encrypted bytes to disk
		CryptographicBuffer.CopyToByteArray(encryptedBuffer, out byte[] encryptedBytes);
		await File.WriteAllBytesAsync(EncryptedCacheFilePath, encryptedBytes);
	}

	/// <summary>
	/// Reads the encrypted cache file from disk and decrypts it using Windows Data Protection API.
	/// DPAPI automatically determines the correct decryption parameters from the encrypted data header.
	/// </summary>
	/// <returns>The decrypted JSON string.</returns>
	private static async Task<string> ReadAndDecryptAsync()
	{
		byte[] encryptedBytes = await File.ReadAllBytesAsync(EncryptedCacheFilePath);

		// Create a provider without a descriptor for decryption (DPAPI knows how to decrypt from the data)
		DataProtectionProvider provider = new();

		// Convert the bytes to an IBuffer
		IBuffer encryptedBuffer = CryptographicBuffer.CreateFromByteArray(encryptedBytes);

		// Decrypt the data
		IBuffer decryptedBuffer = await provider.UnprotectAsync(encryptedBuffer);

		// Convert back to string
		return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedBuffer);
	}
}

/// <summary>
/// JSON serialization context for the policy library cache, supporting AOT compilation.
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(IReadOnlyList<Others.CiPolicyInfo>))]
[JsonSerializable(typeof(List<Others.CiPolicyInfo>))]
internal sealed partial class PolicyLibraryCacheJsonContext : JsonSerializerContext
{
}
