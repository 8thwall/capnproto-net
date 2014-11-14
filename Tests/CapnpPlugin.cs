using CapnProto;
using CapnProto.Schema;
using Microsoft.CSharp;
using NUnit.Framework;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestFixture]
    public class CapnpPluginTests
    {
        public IEnumerable<string> GetSchemaFiles()
        {
            return Directory.EnumerateFiles("Schema", "*.bin");
        }

        [Test]
        [TestCaseSource("GetSchemaFiles")]
        public void TestAsReference(string path)
        {
            string code;
            using(var file = File.OpenRead(path))
            using(var csharp = new StringWriter())
            using(var errors = new StringWriter())
            {
                int exitCode = CapnpPlugin.Process(file, csharp, errors);
                Assert.AreEqual(0, exitCode);
                Assert.AreEqual("", errors.ToString());
                code = csharp.ToString();
            }
            File.WriteAllText(Path.ChangeExtension(path, ".plugin.cs"), code);
            Compile(code);
        }
        static void Compile(string code)
        {
            using(var csc = new CSharpCodeProvider())
            {
                var options = new CompilerParameters();
                options.ReferencedAssemblies.Add("System.dll");
                options.ReferencedAssemblies.Add("CapnProto-net.dll");
                var results = csc.CompileAssemblyFromSource(options, code);
                foreach(var error in results.Errors)
                {
                    Console.WriteLine(error);
                }
                Assert.AreEqual(0, results.Errors.Count);
            }
        }

        [Test, Ignore("Failing; I think due to stdin oddness; looking")]
        [TestCaseSource("GetSchemaFiles")]
        public void TestAsConsole(string path)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "capnpc-csharp";
            psi.UseShellExecute = false;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardOutput = true;
            psi.WindowStyle = ProcessWindowStyle.Hidden;
            psi.CreateNoWindow = true;
            string code = null;
            using(var proc = Process.Start(psi))
            {
                ThreadPool.QueueUserWorkItem(delegate
                {
                    using (var stdin = proc.StandardInput)
                    using (var source = File.OpenRead(path))
                    {
                        source.CopyTo(stdin.BaseStream);
                        stdin.Close();
                    }
                    using (var stdout = proc.StandardOutput)
                    {
                        var output = stdout.ReadToEnd();
                        Interlocked.Exchange(ref code, output);
                    }
                });
                if (!proc.WaitForExit(5000)) throw new TimeoutException();
            }
            var tmp = Interlocked.CompareExchange(ref code, null, null);
            File.WriteAllText(Path.ChangeExtension(path, ".console.cs"), tmp);
            Compile(tmp);
        }
    }
}
