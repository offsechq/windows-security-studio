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

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace AppControlManager.Pages;

internal sealed partial class Home : Page, IDisposable, CommonCore.UI.IInvisibleCrumbar
{
#if HARDEN_SYSTEM_SECURITY
	private ViewModels.HomeVM ViewModel { get; } = HardenSystemSecurity.ViewModels.ViewModelProvider.HomeVM;
#else
	private ViewModels.HomeVM ViewModel { get; } = ViewModels.ViewModelProvider.HomeVM;
#endif

	internal Home()
	{
		InitializeComponent();
		NavigationCacheMode = NavigationCacheMode.Disabled;
		DataContext = ViewModel;
	}

	private void OnInitialLoaded(object sender, RoutedEventArgs e)
	{
		// Run the code that needs to run in ViewModel class when page is loaded.
		ViewModel.OnHomePageLoaded(sender);
	}

	private void OnUnloadedDisposeResources(object sender, RoutedEventArgs e)
	{
		// Run the code that needs to run in ViewModel class when page is unloaded.
		ViewModel.OnHomePageUnLoaded();
	}

	// Disposal guard to ensure owned resources are released exactly once
	private bool _disposed;

	// Safe to call multiple times, and also safe to call in addition to Unloaded cleanup.
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}
		_disposed = true;

		// This runs cleanup code when disposed.
		OnUnloadedDisposeResources(this, new RoutedEventArgs());
	}
}
