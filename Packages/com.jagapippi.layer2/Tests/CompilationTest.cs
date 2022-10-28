using System.IO;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build.Player;

namespace Jagapippi.Layer2
{
    public class CompilationTest
    {
        private static readonly string OutputPath = $"Temp/{nameof(CompilationTest)}";

        [Test] public void StandaloneOSX() => Assert.IsTrue(Compile(BuildTarget.StandaloneOSX));
        [Test] public void StandaloneWindows() => Assert.IsTrue(Compile(BuildTarget.StandaloneWindows));
        [Test] public void iOS() => Assert.IsTrue(Compile(BuildTarget.iOS));
        [Test] public void Android() => Assert.IsTrue(Compile(BuildTarget.Android));
        [Test] public void StandaloneWindows64() => Assert.IsTrue(Compile(BuildTarget.StandaloneWindows64));
        [Test] public void WebGL() => Assert.IsTrue(Compile(BuildTarget.WebGL));
        [Test] public void WSAPlayer() => Assert.IsTrue(Compile(BuildTarget.WSAPlayer));
        [Test] public void StandaloneLinux64() => Assert.IsTrue(Compile(BuildTarget.StandaloneLinux64));
        [Test] public void PS4() => Assert.IsTrue(Compile(BuildTarget.PS4));
        [Test] public void XboxOne() => Assert.IsTrue(Compile(BuildTarget.XboxOne));
        [Test] public void tvOS() => Assert.IsTrue(Compile(BuildTarget.tvOS));
        [Test] public void Switch() => Assert.IsTrue(Compile(BuildTarget.Switch));

        private static bool Compile(BuildTarget target)
        {
            var input = new ScriptCompilationSettings
            {
                target = target,
                group = BuildPipeline.GetBuildTargetGroup(target),
            };

            var result = PlayerBuildInterface.CompilePlayerScripts(input, OutputPath);
            var assemblies = result.assemblies;
            var passed = (assemblies is { Count: > 0 } && (result.typeDB != null));

            if (Directory.Exists(OutputPath)) Directory.Delete(OutputPath, true);

            return passed;
        }
    }
}
