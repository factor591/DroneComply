using System;
using Microsoft.Build.Logging.StructuredLogger;

class Program
{
    static void Main()
    {
        var build = BinaryLog.ReadBuild("c:/Programming/Dronecomply/msbuild.binlog");
        foreach (var error in build.FindChildrenRecursive<Error>(_ => true))
        {
            Console.WriteLine($"ERROR: {error.Text}");
        }
    }
}
