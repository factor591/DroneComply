using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.Windows.ApplicationModel.DynamicDependency;

namespace DroneComply.App;

public static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        InitializeWindowsAppRuntime();

        Application.Start(_ =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            new App();
        });
    }

    private static void InitializeWindowsAppRuntime()
    {
        const uint majorMinor = Microsoft.WindowsAppSDK.Release.MajorMinor;
        string versionTag = Microsoft.WindowsAppSDK.Release.VersionTag;
        var minVersion = new PackageVersion(Microsoft.WindowsAppSDK.Runtime.Version.UInt64);
        var options = Bootstrap.InitializeOptions.OnNoMatch_ShowUI | Bootstrap.InitializeOptions.OnError_DebugBreak;

        if (Bootstrap.TryInitialize(majorMinor, versionTag, minVersion, options, out int hr))
        {
            return;
        }

        throw new COMException($"Windows App SDK runtime initialization failed (HRESULT: 0x{hr:X8}).", hr);
    }
}
