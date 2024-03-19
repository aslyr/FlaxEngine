// Copyright (c) 2012-2024 Wojciech Figat. All rights reserved.

using System.Collections.Generic;
using System.IO;
using Flax.Build;
using Flax.Build.NativeCpp;

/// <summary>
/// https://github.com/zeux/volk
/// </summary>
public class volk : ThirdPartyModule
{
    private bool _missingSDKError;

    /// <inheritdoc />
    public override void Init()
    {
        base.Init();

        LicenseType = LicenseTypes.MIT;
        LicenseFilePath = "LICENSE.txt";

        // Merge third-party modules into engine binary
        BinaryModuleName = "FlaxEngine";
    }

    /// <inheritdoc />
    public override void Setup(BuildOptions options)
    {
        base.Setup(options);

        switch (options.Platform.Target)
        {
        case TargetPlatform.Windows:
            options.PublicDefinitions.Add("VK_USE_PLATFORM_WIN32_KHR");
            break;
        case TargetPlatform.Linux:
            options.PrivateDefinitions.Add("VK_USE_PLATFORM_XLIB_KHR");
            break;
        case TargetPlatform.Android:
            options.PublicDefinitions.Add("VK_USE_PLATFORM_ANDROID_KHR");
            break;
        case TargetPlatform.Mac:
            options.PublicDefinitions.Add("VK_USE_PLATFORM_MACOS_MVK");
            options.DependencyFiles.Add(Path.Combine(VulkanSdk.Instance.RootPath, "../MoltenVK/dylib/macOS/libMoltenVK.dylib"));
            options.DependencyFiles.Add(Path.Combine(VulkanSdk.Instance.RootPath, "../MoltenVK/dylib/macOS/MoltenVK_icd.json"));
            break;
        case TargetPlatform.iOS:
            options.PublicDefinitions.Add("VK_USE_PLATFORM_IOS_MVK");
            options.DependencyFiles.Add(Path.Combine(VulkanSdk.Instance.RootPath, "../MoltenVK/dylib/iOS/libMoltenVK.dylib"));
            options.DependencyFiles.Add(Path.Combine(VulkanSdk.Instance.RootPath, "../MoltenVK/dylib/iOS/MoltenVK_icd.json"));
            break;
        default: throw new InvalidPlatformException(options.Platform.Target);
        }

        string includesFolderPath;
        if (VulkanSdk.Instance.TryGetIncludePath(options.Platform.Target, out includesFolderPath))
        {
            options.PublicIncludePaths.Add(includesFolderPath);
        }
        else
        {
            Log.ErrorOnce("Missing VulkanSDK.", ref _missingSDKError);
        }
    }

    /// <inheritdoc />
    public override void GetFilesToDeploy(List<string> files)
    {
        base.GetFilesToDeploy(files);

        files.Add(Path.Combine(FolderPath, "volk.h"));
    }
}
