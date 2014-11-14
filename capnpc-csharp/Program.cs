
using System;
using System.IO;
namespace CapnProto.Schema
{
    /// <summary>
    /// Command-line plugin tool for capnp
    /// </summary>
    public static class CapnpPlugin
    {
        public static int Process(Stream source, TextWriter destination, TextWriter errors)
        {
            try
            {
                using (var msg = Message.Load(source))
                {
                    if (!msg.ReadNext()) throw new InvalidOperationException("Message on stdin not detected");
                    var req = (CodeGeneratorRequest)msg.Root;

                    var codeWriter = new CSharpStructWriter(destination, req.nodes, "YourNamespace");
                    req.GenerateCustomModel(codeWriter);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                errors.WriteLine(ex.Message);
                return -1;
            }
        }
        static int Main()
        {
            try
            {
                using(var stdin = Console.OpenStandardInput())
                {
                    return Process(stdin, Console.Out, Console.Error);
                }                
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
