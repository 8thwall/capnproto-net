using CapnProto;
using CapnProto.Schema;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Tests
{
    [TestFixture]
    public class Bootstrap
    {

        public IEnumerable<string> GetSchemaFiles()
        {
            return Directory.EnumerateFiles("Schema", "*.bin");
        }

        [Test]
        [TestCaseSource("GetSchemaFiles")]
        public void CanCrawl(string source)
        {
            string destination = Path.ChangeExtension(source, ".crawl.txt");
            using (var reader = Message.Load(source))
            using (var outFile = File.CreateText(destination))
            {
                while (reader.ReadNext())
                {
                    reader.Crawl(outFile, true);
                }
            }
            Console.WriteLine("Written: " + destination);
        }

        [Test]
        [TestCaseSource("GetSchemaFiles")]
        public void HexDump(string source)
        {
            string destination = Path.ChangeExtension(source, ".dump.txt");

            using (var outFile = File.CreateText(destination))
            using (var reader = Message.Load(source))
            {
                int messageIndex = 0;
                while (reader.ReadNext())
                {
                    if (messageIndex != 0)
                    {
                        outFile.WriteLine();
                        outFile.WriteLine("<< next message >>");
                        outFile.WriteLine();
                    }
                    outFile.WriteLine("message {0}, segments: {0}", messageIndex, reader.SegmentCount);
                    long offset = 0;
                    for (int segmentIndex = 0; segmentIndex < reader.SegmentCount; segmentIndex++)
                    {
                        var segment = reader[segmentIndex];
                        outFile.WriteLine();
                        outFile.WriteLine("message: {0}; segment: {0}, offset: {1}, length: {2}", messageIndex, segmentIndex, offset, segment.Length);
                        outFile.WriteLine();
                        for (int word = 0; word < segment.Length; word++)
                        {
                            ulong value = segment[word];
                            outFile.WriteLine("{10:00}/{9:00}/{0:000000}:\t{1:X2} {2:X2} {3:X2} {4:X2} {5:X2} {6:X2} {7:X2} {8:X2}",
                                word,
                                (byte)value, (byte)(value >> 8), (byte)(value >> 16), (byte)(value >> 24),
                                (byte)(value >> 32), (byte)(value >> 40), (byte)(value >> 48), (byte)(value >> 56),
                                segmentIndex, messageIndex);
                        }
                        offset += segment.Length;
                    }
                    messageIndex++;
                }
            }
            Console.WriteLine("Written: " + destination);
        }

        [Test]
        [TestCaseSource("GetSchemaFiles")]
        public void CanGenerateFromSchemas(string source)
        {
            string destination = Path.ChangeExtension(source, ".designer.cs");

            var @namespace = PickNamespace(source);

            using (var msg = Message.Load(source))
            {
                if (!msg.ReadNext()) throw new EndOfStreamException();
                var req = (CodeGeneratorRequest)msg.Root;
                using (var sw = new StringWriter())
                {
                    var codeWriter = new CSharpStructWriter(sw, req.nodes, @namespace);
                    req.GenerateCustomModel(codeWriter);
                    File.WriteAllText(destination, sw.ToString());
                    Console.WriteLine("File generated: {0}", destination);
                }
            }
        }

#if UNSAFE
        [Test]
        [TestCaseSource("GetSchemaFiles")]
        public void CanGenerateFromSchemasUsingMemoryMappedFiles(string source)
        {
            string destination = Path.ChangeExtension(source, ".mmf.designer.cs");

            var @namespace = PickNamespace(source);


            using (var msg = Message.Load(MemoryMappedFileSegmentFactory.Open(source)))
            {
                if (!msg.ReadNext()) throw new EndOfStreamException();
                var req = (CodeGeneratorRequest)msg.Root;
                using (var sw = new StringWriter())
                {
                    var codeWriter = new CSharpStructWriter(sw, req.nodes, @namespace);
                    req.GenerateCustomModel(codeWriter);
                    File.WriteAllText(destination, sw.ToString());
                    Console.WriteLine("File generated: {0}", destination);
                }
            }
        }
#endif

        private string PickNamespace(string source)
        {
            source = Path.GetFileNameWithoutExtension(source);
            switch(source)
            {
                case "schema":
                    return "CapnProto.Schema";
                // add more special cases here
                default:
                    return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(source);
            }
        }

        private static void ShowSchemaInfo(CodeGeneratorRequest req)
        {
            if (req.requestedFiles.IsValid())
            {
                if (req.requestedFiles.Count() != 0)
                {
                    Console.WriteLine("Requested files:");
                    foreach (var file in req.requestedFiles)
                    {
                        Console.WriteLine("{0}: {1}", file.id, file.filename);
                        if (file.imports.IsValid())
                        {
                            foreach (var imp in file.imports)
                            {
                                Console.WriteLine("\t{0}: {1}", imp.id, imp.name);
                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
            if (req.nodes.IsValid())
            {
                if (req.nodes.Count() != 0)
                {
                    Console.WriteLine("Nodes:");
                    foreach (var node in req.nodes)
                    {
                        string name = node.displayName.ToString();
                        Console.WriteLine("  {2}: {0} ({1})", name, name.Substring((int)node.displayNamePrefixLength), node.Union);
                    }
                    Console.WriteLine();
                }
            }
        }

    }
}
