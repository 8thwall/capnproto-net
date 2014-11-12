`CapnProto-net` is a .NET implementation of the [Cap'n Proto](http://kentonv.github.io/capnproto/) data format. It is a binary format suitable for transport, exchange, storage and live in-memory usage.

Rather than being a *serialization* format as such, it instead describes a mapping a memory space to objects directly. This means that *there is no serialization step*: you simply work with the objects, and the in-memory representation is already serialized - it just needs to be copied to the destination. Likewise, instead of *deserializing*, you simply load a message, and access the root node.

Motivations... why would I use this?
-

Firstly, because there is no serialization/deserialization step, it is *really, really fast*. Of course, nothing is free, and you end up paying a bit more in terms of accessing each data field, but:

1. this is heavily optimized
2. you often don't access all of the fields or read all of the rows... so *why deserialize them?*

But there's another big point here:

Zero alloactions
-

CanpProto-net has been specifically written to work *with almost zero allocations*. When you're traversing data, behind the scene's each instance is actually a `struct`; you just read and write to the `struct`, and it all *just works*

> but Marc, mutable structs are evil! think of the kittens!

Yes, but you're *not actually mutating the `struct`*; the accessors are just proxies to the underlying memory space. I'm not a comp-sci person, but it probably maps as a "flyweight". Behind the scenes, you are manipulating memory formatted as defined by the Cap'n Proto specification, but you don't usually need to worry about any of those details.


Installation
-

CapnProto-net is available to [build from source](https://github.com/mgravell/capnproto-net/), but for convenience is [available on NuGet](https://www.nuget.org/packages/CapnProto-net/); to install the core library:

    PM> Install-Package CapnProto-net

If you also need the schema-processing tools:

    PM> Install-Package CapnProto-net.Schema

Schemas
-

Cap'n Proto data is often described via a schema file; the syntax for this file is [described in the Cap'n Proto documentation](http://kentonv.github.io/capnproto/language.htm). At the current time, `CapnProto-net` does not have a "managed" parser (although I intend writing one); it can, however, process pre-compiled schemas that have been compiled with [the `capnp` tool](http://kentonv.github.io/capnproto/capnp-tool.html). To compile a schema ***on linux***, you might use:

    capnp compile -o/bin/cat {source.capnp} > {target.bin}

The tooling here will be improved shortly, but for now, a `.designer.cs` can be generated from this pre-compiled schema by referencing `CapnProto-net.Schema`, and using:

    using (var msg = Message.Load(source))
    {
        msg.ReadNext();
        var req = (CodeGeneratorRequest)msg.Root;
        using (var sw = new StringWriter())
        {
            var codeWriter = new CSharpStructWriter(sw, req.nodes, @namespace);
            req.GenerateCustomModel(codeWriter);
            File.WriteAllText(destination, sw.ToString());
            Console.WriteLine("File generated: {0}", destination);
        }
    }

Working with data
---

The core object for working with `CapnProto-net` is the `Message` object, which represents a memory workspace. We've already seen this working above, with `Message.Load`; the important thing is simply how you cast the `.Root`. In our example above, we're casting the `.Root` to a `CodeGeneratorRequest` instance, because the `capnp compile` tool *generates* a `CodeGeneratorRequest`. From there, we have access to all the properties and collections throughout the model, for example, since our model contains information about schema objects, we can list the types in our schema:

    foreach (var node in req.nodes)
    {
        string name = node.displayName.ToString();
        Console.WriteLine("  {2}: {0} ({1})", name,
             name.Substring((int)node.displayNamePrefixLength), node.Union);
    }
 
But if your `.Root` object was a `Customer` or an `Order`, then you would have all the members from your own domain model.

Note that there are sometimes multiple message instances in a single file / stream / socket / etc. To move to the next message, just call `msg.ReadNext()` - it returns `true` if a message was found, or `false` if the stream terminated. Hence a simple pattern is:

    using(var msg = Message.Load(source))
    {
        while(msg.ReadNext())
        {
            ProcessData((YourDataType)msg.Root);
        }
    }

Creating new data
-

*Creating* new objects works a bit differently; you can't just `new` them, because they need to be mapped into our `Message`'s memory-space. However, just about every object in the model implicitly knows about the `Message`, so we don't need to pass it around everywhere. Any object in your model will do; for example:

    var newAnnotation = Annotation.Create(req);

or just:

    var newAnnotation = msg.Allocate<Annotation>();

We can then assign this instance to property members of other objects, or as our `.Root` object, or add it to a list:

    annotations[4] = newAnnotation;

When you want to save the data:

    msg.Write(destination);

or:

    await msg.WriteAsync(destination);