//namespace CapnProto.Schema
//{
//    [global::CapnProto.Id(0xbfc546f6210ad7ce)]
//    public partial class CodeGeneratorRequest : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.CodeGeneratorRequest obj);
//        internal static global::CapnProto.Schema.CodeGeneratorRequest ѧ_ctor()
//        {
//            global::CapnProto.Schema.CodeGeneratorRequest tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.CodeGeneratorRequest();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[2];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 2);
//            if (raw[0] != 0)
//            {
//                // CodeGeneratorRequest.nodes
//                this.ѧ_p_0 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Node>(ctx, segment, origin + 1, raw[0]);
//            }
//            if (raw[1] != 0)
//            {
//                // CodeGeneratorRequest.requestedFiles
//                this.ѧ_p_1 = ctx.Reader.ReadStructList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile>(ctx, segment, origin + 2, raw[1]);
//            }
//        }
//        static CodeGeneratorRequest()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.Node> nodes
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.Node>)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile> requestedFiles
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile>)this.ѧ_p_1;
//            }
//            set
//            {
//                this.ѧ_p_1 = value;
//            }
//        }
//        // body words: 0; pointers: 2
//        private object ѧ_p_0, ѧ_p_1;
//        [global::CapnProto.Id(0xcfea0eb02e810062)]
//        public partial class RequestedFile : global::CapnProto.IBlittable
//        {
//            static partial void OnCreate(ref global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile obj);
//            internal static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile ѧ_ctor()
//            {
//                global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile tmp = null;
//                OnCreate(ref tmp);
//                // if you are providing custom construction, please also provide a private
//                // parameterless constructor (it can just throw an exception if you like)
//                return tmp ?? new global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile();
//            }
//            unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                ulong* raw = stackalloc ulong[2];
//                ctx.Reader.ReadData(segment, origin, pointer, raw, 1);
//                ѧ_w_0 = raw[0];
//                origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 2);
//                if (raw[0] != 0)
//                {
//                    // RequestedFile.filename
//                    this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//                }
//                if (raw[1] != 0)
//                {
//                    // RequestedFile.imports
//                    this.ѧ_p_1 = ctx.Reader.ReadStructList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import>(ctx, segment, origin + 2, raw[1]);
//                }
//            }
//            static RequestedFile()
//            {
//                global::CapnProto.TypeModel.AssertLittleEndian();
//            }
//            [global::CapnProto.FieldAttribute(0)]
//            public ulong id
//            {
//                get
//                {
//                    return unchecked((ulong)this.ѧ_w_0);
//                }
//                set
//                {
//                    this.ѧ_w_0 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(1)]
//            public string filename
//            {
//                get
//                {
//                    return (string)this.ѧ_p_0;
//                }
//                set
//                {
//                    this.ѧ_p_0 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(2)]
//            public global::System.Collections.Generic.List<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import> imports
//            {
//                get
//                {
//                    return (global::System.Collections.Generic.List<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import>)this.ѧ_p_1;
//                }
//                set
//                {
//                    this.ѧ_p_1 = value;
//                }
//            }
//            // body words: 1; pointers: 2
//            private ulong ѧ_w_0;
//            private object ѧ_p_0, ѧ_p_1;
//            [global::CapnProto.Id(0xae504193122357e5)]
//            public partial class Import : global::CapnProto.IBlittable
//            {
//                static partial void OnCreate(ref global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import obj);
//                internal static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import ѧ_ctor()
//                {
//                    global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import tmp = null;
//                    OnCreate(ref tmp);
//                    // if you are providing custom construction, please also provide a private
//                    // parameterless constructor (it can just throw an exception if you like)
//                    return tmp ?? new global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import();
//                }
//                unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//                {
//                    ulong* raw = stackalloc ulong[1];
//                    ctx.Reader.ReadData(segment, origin, pointer, raw, 1);
//                    ѧ_w_0 = raw[0];
//                    origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 1);
//                    if (raw[0] != 0)
//                    {
//                        // Import.name
//                        this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//                    }
//                }
//                static Import()
//                {
//                    global::CapnProto.TypeModel.AssertLittleEndian();
//                }
//                [global::CapnProto.FieldAttribute(0)]
//                public uint id
//                {
//                    get
//                    {
//                        return unchecked((uint)this.ѧ_w_0);
//                    }
//                    set
//                    {
//                        this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff00000000) | unchecked(((ulong)(value)));
//                    }
//                }
//                [global::CapnProto.FieldAttribute(1)]
//                public string name
//                {
//                    get
//                    {
//                        return (string)this.ѧ_p_0;
//                    }
//                    set
//                    {
//                        this.ѧ_p_0 = value;
//                    }
//                }
//                // body words: 1; pointers: 1
//                private ulong ѧ_w_0;
//                private object ѧ_p_0;
//            }
//        }
//    }
//    [global::CapnProto.Id(0xe682ab4cf923a417)]
//    public partial class Node : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Node obj);
//        internal static global::CapnProto.Schema.Node ѧ_ctor()
//        {
//            global::CapnProto.Schema.Node tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Node();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[5];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 5);
//            ѧ_w_0 = raw[0];
//            ѧ_w_1 = raw[1];
//            ѧ_w_2 = raw[2];
//            ѧ_w_3 = raw[3];
//            ѧ_w_4 = raw[4];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 5);
//            if (raw[0] != 0)
//            {
//                // Node.displayName
//                this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//            }
//            if (raw[1] != 0)
//            {
//                // Node.nestedNodes
//                this.ѧ_p_1 = ctx.Reader.ReadStructList<global::CapnProto.Schema.NestedNode>(ctx, segment, origin + 2, raw[1]);
//            }
//            if (raw[2] != 0)
//            {
//                // Node.annotations
//                this.ѧ_p_2 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Annotation>(ctx, segment, origin + 3, raw[2]);
//            }
//            if (raw[3] != 0)
//            {
//                // structGroup.fields
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x100000000) this.ѧ_p_3 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Field>(ctx, segment, origin + 4, raw[3]);
//                // enumGroup.enumerants
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x200000000) this.ѧ_p_3 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Enumerant>(ctx, segment, origin + 4, raw[3]);
//                // interfaceGroup.methods
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x300000000) this.ѧ_p_3 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Method>(ctx, segment, origin + 4, raw[3]);
//                // constGroup.type
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x400000000) this.ѧ_p_3 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_d07378ede1f9cc60(segment, origin + 4, ctx, raw[3]);
//                // annotationGroup.type
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x500000000) this.ѧ_p_3 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_d07378ede1f9cc60(segment, origin + 4, ctx, raw[3]);
//            }
//            if (raw[4] != 0)
//            {
//                // interfaceGroup.extends
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x300000000) this.ѧ_p_4 = ctx.Reader.ReadInt64List(segment, origin, raw[4]);
//                // constGroup.value
//                if ((this.ѧ_w_1 & 0xffff00000000) == 0x400000000) this.ѧ_p_4 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_ce23dcd2d7b00c9b(segment, origin + 5, ctx, raw[4]);
//            }
//        }
//        static Node()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public ulong id
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public string displayName
//        {
//            get
//            {
//                return (string)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public uint displayNamePrefixLength
//        {
//            get
//            {
//                return unchecked((uint)this.ѧ_w_1);
//            }
//            set
//            {
//                this.ѧ_w_1 = (this.ѧ_w_1 & 0xffffffff00000000) | unchecked(((ulong)(value)));
//            }
//        }
//        [global::CapnProto.FieldAttribute(3)]
//        public ulong scopeId
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_2);
//            }
//            set
//            {
//                this.ѧ_w_2 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(4)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.NestedNode> nestedNodes
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.NestedNode>)this.ѧ_p_1;
//            }
//            set
//            {
//                this.ѧ_p_1 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(5)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation> annotations
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation>)this.ѧ_p_2;
//            }
//            set
//            {
//                this.ѧ_p_2 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(6)]
//        public global::CapnProto.Void file
//        {
//            get
//            {
//                return (this.ѧ_w_1 & 0xffff00000000) == 0x0 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        public global::CapnProto.Schema.Node.structGroup @struct
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Node.structGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0x9ea0b19b37fb4435)]
//        public struct structGroup
//        {
//            private readonly global::CapnProto.Schema.Node parent;
//            internal structGroup(global::CapnProto.Schema.Node parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(7)]
//            public ushort? dataWordCount
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        return unchecked((ushort)(this.parent.ѧ_w_1 >> 48));
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_1 = (this.parent.ѧ_w_1 & 0xffffffffffff) | unchecked(((ulong)(value.Value) << 48));
//                }
//            }
//            [global::CapnProto.FieldAttribute(8)]
//            public ushort? pointerCount
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        return unchecked((ushort)this.parent.ѧ_w_3);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_3 = (this.parent.ѧ_w_3 & 0xffffffffffff0000) | unchecked(((ulong)(value.Value)));
//                }
//            }
//            [global::CapnProto.FieldAttribute(9)]
//            public global::CapnProto.Schema.ElementSize? preferredListEncoding
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        switch (this.parent.ѧ_w_3 & 0xffff0000)
//                        {
//                            case 0x0: return global::CapnProto.Schema.ElementSize.empty;
//                            case 0x10000: return global::CapnProto.Schema.ElementSize.bit;
//                            case 0x20000: return global::CapnProto.Schema.ElementSize.@byte;
//                            case 0x30000: return global::CapnProto.Schema.ElementSize.twoBytes;
//                            case 0x40000: return global::CapnProto.Schema.ElementSize.fourBytes;
//                            case 0x50000: return global::CapnProto.Schema.ElementSize.eightBytes;
//                            case 0x60000: return global::CapnProto.Schema.ElementSize.pointer;
//                            case 0x70000: return global::CapnProto.Schema.ElementSize.inlineComposite;
//                            default: throw new global::System.InvalidOperationException("unexpected enum value: " + unchecked((ushort)(this.parent.ѧ_w_3 >> 16)));
//                        }
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    throw new global::System.NotImplementedException();
//                }
//            }
//            [global::CapnProto.FieldAttribute(10)]
//            public bool? isGroup
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x10000000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x10000000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffefffffff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(11)]
//            public ushort? discriminantCount
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        return unchecked((ushort)(this.parent.ѧ_w_3 >> 48));
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_3 = (this.parent.ѧ_w_3 & 0xffffffffffff) | unchecked(((ulong)(value.Value) << 48));
//                }
//            }
//            [global::CapnProto.FieldAttribute(12)]
//            public uint? discriminantOffset
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)
//                    {
//                        return unchecked((uint)this.parent.ѧ_w_4);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_4 = (this.parent.ѧ_w_4 & 0xffffffff00000000) | unchecked(((ulong)(value.Value)));
//                }
//            }
//            [global::CapnProto.FieldAttribute(13)]
//            public global::System.Collections.Generic.List<global::CapnProto.Schema.Field> fields
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000 ? (global::System.Collections.Generic.List<global::CapnProto.Schema.Field>)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x100000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Node.enumGroup @enum
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Node.enumGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xb54ab3364333f598)]
//        public struct enumGroup
//        {
//            private readonly global::CapnProto.Schema.Node parent;
//            internal enumGroup(global::CapnProto.Schema.Node parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(14)]
//            public global::System.Collections.Generic.List<global::CapnProto.Schema.Enumerant> enumerants
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x200000000 ? (global::System.Collections.Generic.List<global::CapnProto.Schema.Enumerant>)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x200000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Node.interfaceGroup @interface
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Node.interfaceGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xed8bca69f7fb0cbf)]
//        public struct interfaceGroup
//        {
//            private readonly global::CapnProto.Schema.Node parent;
//            internal interfaceGroup(global::CapnProto.Schema.Node parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(15)]
//            public global::System.Collections.Generic.List<global::CapnProto.Schema.Method> methods
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x300000000 ? (global::System.Collections.Generic.List<global::CapnProto.Schema.Method>)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x300000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(31)]
//            public global::System.Collections.Generic.List<long> extends
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x300000000 ? (global::System.Collections.Generic.List<long>)this.parent.ѧ_p_4 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x300000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_4 = value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Node.constGroup @const
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Node.constGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xb18aa5ac7a0d9420)]
//        public struct constGroup
//        {
//            private readonly global::CapnProto.Schema.Node parent;
//            internal constGroup(global::CapnProto.Schema.Node parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(16)]
//            public global::CapnProto.Schema.Type type
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x400000000 ? (global::CapnProto.Schema.Type)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x400000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(17)]
//            public global::CapnProto.Schema.Value value
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x400000000 ? (global::CapnProto.Schema.Value)this.parent.ѧ_p_4 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x400000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_4 = value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Node.annotationGroup annotation
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Node.annotationGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xec1619d4400a0290)]
//        public struct annotationGroup
//        {
//            private readonly global::CapnProto.Schema.Node parent;
//            internal annotationGroup(global::CapnProto.Schema.Node parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(17)]
//            public global::CapnProto.Schema.Type type
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000 ? (global::CapnProto.Schema.Type)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(19)]
//            public bool? targetsFile
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(20)]
//            public bool? targetsConst
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(21)]
//            public bool? targetsEnum
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(22)]
//            public bool? targetsEnumerant
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(23)]
//            public bool? targetsStruct
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(24)]
//            public bool? targetsField
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(25)]
//            public bool? targetsUnion
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(26)]
//            public bool? targetsGroup
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x4000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x4000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffffbfff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(27)]
//            public bool? targetsInterface
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x8000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x8000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffff7fff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(28)]
//            public bool? targetsMethod
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x8000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x8000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffff7fff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(29)]
//            public bool? targetsParam
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x8000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x8000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffff7fff;
//                    }
//                }
//            }
//            [global::CapnProto.FieldAttribute(30)]
//            public bool? targetsAnnotation
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x8000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff00000000) == 0x500000000)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x8000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xffffffffffff7fff;
//                    }
//                }
//            }
//        }
//        public enum Unions
//        {
//            file = 0,
//            @struct = 1,
//            @enum = 2,
//            @interface = 3,
//            @const = 4,
//            annotation = 5,
//        }
//        public global::CapnProto.Schema.Node.Unions Union
//        {
//            get
//            {
//                return (global::CapnProto.Schema.Node.Unions)((this.ѧ_w_1 >> 32) & 0xFFFF);
//            }
//            set
//            {
//                this.ѧ_w_1 = (this.ѧ_w_1 & 0xffff0000ffffffff) | ((ulong)value << 32);
//            }
//        }
//        // body words: 5; pointers: 5
//        private ulong ѧ_w_0, ѧ_w_1, ѧ_w_2, ѧ_w_3, ѧ_w_4;
//        private object ѧ_p_0, ѧ_p_1, ѧ_p_2, ѧ_p_3, ѧ_p_4;
//    }
//    [global::CapnProto.Id(0xdebf55bbfa0fc242)]
//    public partial class NestedNode : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.NestedNode obj);
//        internal static global::CapnProto.Schema.NestedNode ѧ_ctor()
//        {
//            global::CapnProto.Schema.NestedNode tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.NestedNode();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[1];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 1);
//            ѧ_w_0 = raw[0];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 1);
//            if (raw[0] != 0)
//            {
//                // NestedNode.name
//                this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//            }
//        }
//        static NestedNode()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public string name
//        {
//            get
//            {
//                return (string)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public ulong id
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = value;
//            }
//        }
//        // body words: 1; pointers: 1
//        private ulong ѧ_w_0;
//        private object ѧ_p_0;
//    }
//    [global::CapnProto.Id(0xf1c8950dab257542)]
//    public partial class Annotation : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Annotation obj);
//        internal static global::CapnProto.Schema.Annotation ѧ_ctor()
//        {
//            global::CapnProto.Schema.Annotation tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Annotation();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[2];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 1);
//            ѧ_w_0 = raw[0];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 2);
//            if (raw[1] != 0)
//            {
//                // Annotation.value
//                this.ѧ_p_1 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_ce23dcd2d7b00c9b(segment, origin + 2, ctx, raw[1]);
//            }
//        }
//        static Annotation()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public ulong id
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public global::CapnProto.Schema.Value value
//        {
//            get
//            {
//                return (global::CapnProto.Schema.Value)this.ѧ_p_1;
//            }
//            set
//            {
//                this.ѧ_p_1 = value;
//            }
//        }
//        // body words: 1; pointers: 2
//        private ulong ѧ_w_0;
//        private object ѧ_p_0, ѧ_p_1;
//    }
//    public enum ElementSize
//    {
//        empty = 0,
//        bit = 1,
//        @byte = 2,
//        twoBytes = 3,
//        fourBytes = 4,
//        eightBytes = 5,
//        pointer = 6,
//        inlineComposite = 7,
//    }
//    [global::CapnProto.Id(0x9aad50a41f4af45f)]
//    public partial class Field : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Field obj);
//        internal static global::CapnProto.Schema.Field ѧ_ctor()
//        {
//            global::CapnProto.Schema.Field tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Field();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[4];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 3);
//            ѧ_w_0 = raw[0];
//            ѧ_w_1 = raw[1];
//            ѧ_w_2 = raw[2];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 4);
//            if (raw[0] != 0)
//            {
//                // Field.name
//                this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//            }
//            if (raw[1] != 0)
//            {
//                // Field.annotations
//                this.ѧ_p_1 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Annotation>(ctx, segment, origin + 2, raw[1]);
//            }
//            if (raw[2] != 0)
//            {
//                // slotGroup.type
//                if ((this.ѧ_w_1 & 0xffff) == 0x0) this.ѧ_p_2 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_d07378ede1f9cc60(segment, origin + 3, ctx, raw[2]);
//            }
//            if (raw[3] != 0)
//            {
//                // slotGroup.defaultValue
//                if ((this.ѧ_w_1 & 0xffff) == 0x0) this.ѧ_p_3 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_ce23dcd2d7b00c9b(segment, origin + 4, ctx, raw[3]);
//            }
//        }
//        static Field()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public string name
//        {
//            get
//            {
//                return (string)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public ushort codeOrder
//        {
//            get
//            {
//                return unchecked((ushort)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff0000) | unchecked(((ulong)(value)));
//            }
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation> annotations
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation>)this.ѧ_p_1;
//            }
//            set
//            {
//                this.ѧ_p_1 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(3)]
//        public ushort discriminantValue
//        {
//            get
//            {
//                return unchecked((ushort)(this.ѧ_w_0 >> 16));
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff0000ffff) | unchecked(((ulong)(value) << 16));
//            }
//        }
//        public global::CapnProto.Schema.Field.slotGroup slot
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Field.slotGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xc42305476bb4746f)]
//        public struct slotGroup
//        {
//            private readonly global::CapnProto.Schema.Field parent;
//            internal slotGroup(global::CapnProto.Schema.Field parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(4)]
//            public uint? offset
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff) == 0x0)
//                    {
//                        return unchecked((uint)(this.parent.ѧ_w_0 >> 32));
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff) == 0x0)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_0 = (this.parent.ѧ_w_0 & 0xffffffff) | unchecked(((ulong)(value.Value) << 32));
//                }
//            }
//            [global::CapnProto.FieldAttribute(5)]
//            public global::CapnProto.Schema.Type type
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff) == 0x0 ? (global::CapnProto.Schema.Type)this.parent.ѧ_p_2 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff) == 0x0)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_2 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(6)]
//            public global::CapnProto.Schema.Value defaultValue
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_1 & 0xffff) == 0x0 ? (global::CapnProto.Schema.Value)this.parent.ѧ_p_3 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff) == 0x0)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_3 = value;
//                }
//            }
//            [global::CapnProto.FieldAttribute(10)]
//            public bool? hadExplicitDefault
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff) == 0x0)
//                    {
//                        return (this.parent.ѧ_w_0 & 0x10000) != 0;
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff) == 0x0)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    if (value.Value)
//                    {
//                        this.parent.ѧ_w_0 |= 0x10000;
//                    }
//                    else
//                    {
//                        this.parent.ѧ_w_0 &= 0xfffffffffffeffff;
//                    }
//                }
//            }
//        }
//        public global::CapnProto.Schema.Field.groupGroup group
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Field.groupGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xcafccddb68db1d11)]
//        public struct groupGroup
//        {
//            private readonly global::CapnProto.Schema.Field parent;
//            internal groupGroup(global::CapnProto.Schema.Field parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(7)]
//            public ulong? typeId
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_1 & 0xffff) == 0x1)
//                    {
//                        return unchecked((ulong)this.parent.ѧ_w_2);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_1 & 0xffff) == 0x1)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_2 = value.Value;
//                }
//            }
//        }
//        public enum Unions
//        {
//            slot = 0,
//            group = 1,
//        }
//        public global::CapnProto.Schema.Field.Unions Union
//        {
//            get
//            {
//                return (global::CapnProto.Schema.Field.Unions)((this.ѧ_w_1) & 0xFFFF);
//            }
//            set
//            {
//                this.ѧ_w_1 = (this.ѧ_w_1 & 0xffffffffffff0000) | (ulong)value;
//            }
//        }
//        // body words: 3; pointers: 4
//        private ulong ѧ_w_0, ѧ_w_1, ѧ_w_2;
//        private object ѧ_p_0, ѧ_p_1, ѧ_p_2, ѧ_p_3;
//    }
//    [global::CapnProto.Id(0x978a7cebdc549a4d)]
//    public partial class Enumerant : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Enumerant obj);
//        internal static global::CapnProto.Schema.Enumerant ѧ_ctor()
//        {
//            global::CapnProto.Schema.Enumerant tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Enumerant();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[2];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 1);
//            ѧ_w_0 = raw[0];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 2);
//            if (raw[0] != 0)
//            {
//                // Enumerant.name
//                this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//            }
//            if (raw[1] != 0)
//            {
//                // Enumerant.annotations
//                this.ѧ_p_1 = ctx.Reader.ReadStructList<global::CapnProto.Schema.Annotation>(ctx, segment, origin + 2, raw[1]);
//            }
//        }
//        static Enumerant()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation> annotations
//        {
//            get
//            {
//                return (global::System.Collections.Generic.List<global::CapnProto.Schema.Annotation>)this.ѧ_p_1;
//            }
//            set
//            {
//                this.ѧ_p_1 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public string name
//        {
//            get
//            {
//                return (string)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public ushort codeOrder
//        {
//            get
//            {
//                return unchecked((ushort)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff0000) | unchecked(((ulong)(value)));
//            }
//        }
//        // body words: 1; pointers: 2
//        private ulong ѧ_w_0;
//        private object ѧ_p_0, ѧ_p_1;
//    }
//    [global::CapnProto.Id(0x9500cce23b334d80)]
//    public partial class Method : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Method obj);
//        internal static global::CapnProto.Schema.Method ѧ_ctor()
//        {
//            global::CapnProto.Schema.Method tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Method();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[3];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 3);
//            ѧ_w_0 = raw[0];
//            ѧ_w_1 = raw[1];
//            ѧ_w_2 = raw[2];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 1);
//            if (raw[0] != 0)
//            {
//                // Method.name
//                this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//            }
//        }
//        static Method()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public string name
//        {
//            get
//            {
//                return (string)this.ѧ_p_0;
//            }
//            set
//            {
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public ushort codeOrder
//        {
//            get
//            {
//                return unchecked((ushort)this.ѧ_w_0);
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff0000) | unchecked(((ulong)(value)));
//            }
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public ulong paramStructType
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_1);
//            }
//            set
//            {
//                this.ѧ_w_1 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(3)]
//        public ulong resultStructType
//        {
//            get
//            {
//                return unchecked((ulong)this.ѧ_w_2);
//            }
//            set
//            {
//                this.ѧ_w_2 = value;
//            }
//        }
//        // body words: 3; pointers: 1
//        private ulong ѧ_w_0, ѧ_w_1, ѧ_w_2;
//        private object ѧ_p_0;
//    }
//    [global::CapnProto.Id(0xd07378ede1f9cc60)]
//    public partial class Type : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Type obj);
//        internal static global::CapnProto.Schema.Type ѧ_ctor()
//        {
//            global::CapnProto.Schema.Type tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Type();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[2];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 2);
//            ѧ_w_0 = raw[0];
//            ѧ_w_1 = raw[1];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 1);
//            if (raw[0] != 0)
//            {
//                // listGroup.elementType
//                if ((this.ѧ_w_0 & 0xffff) == 0xe) this.ѧ_p_0 = global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase.ѧ_r_d07378ede1f9cc60(segment, origin + 1, ctx, raw[0]);
//            }
//        }
//        static Type()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        [global::CapnProto.FieldAttribute(0)]
//        public global::CapnProto.Void @void
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x0 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public global::CapnProto.Void @bool
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x1 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public global::CapnProto.Void int8
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x2 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(3)]
//        public global::CapnProto.Void int16
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x3 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(4)]
//        public global::CapnProto.Void int32
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x4 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(5)]
//        public global::CapnProto.Void int64
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x5 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(6)]
//        public global::CapnProto.Void uint8
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x6 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(7)]
//        public global::CapnProto.Void uint16
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x7 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(8)]
//        public global::CapnProto.Void uint32
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x8 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(9)]
//        public global::CapnProto.Void uint64
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x9 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(10)]
//        public global::CapnProto.Void float32
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xa ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(11)]
//        public global::CapnProto.Void float64
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xb ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(12)]
//        public global::CapnProto.Void text
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xc ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(13)]
//        public global::CapnProto.Void data
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xd ? global::CapnProto.Void.Value : null;
//            }
//        }
//        public global::CapnProto.Schema.Type.listGroup list
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Type.listGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0x87e739250a60ea97)]
//        public struct listGroup
//        {
//            private readonly global::CapnProto.Schema.Type parent;
//            internal listGroup(global::CapnProto.Schema.Type parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(14)]
//            public global::CapnProto.Schema.Type elementType
//            {
//                get
//                {
//                    return (this.parent.ѧ_w_0 & 0xffff) == 0xe ? (global::CapnProto.Schema.Type)this.parent.ѧ_p_0 : null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_0 & 0xffff) == 0xe)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_p_0 = value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Type.enumGroup @enum
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Type.enumGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xb54ab3364333f597)]
//        public struct enumGroup
//        {
//            private readonly global::CapnProto.Schema.Type parent;
//            internal enumGroup(global::CapnProto.Schema.Type parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(15)]
//            public ulong? typeId
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_0 & 0xffff) == 0xf)
//                    {
//                        return unchecked((ulong)this.parent.ѧ_w_1);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_0 & 0xffff) == 0xf)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_1 = value.Value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Type.structGroup @struct
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Type.structGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xac3a6f60ef4cc6d3)]
//        public struct structGroup
//        {
//            private readonly global::CapnProto.Schema.Type parent;
//            internal structGroup(global::CapnProto.Schema.Type parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(16)]
//            public ulong? typeId
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_0 & 0xffff) == 0x10)
//                    {
//                        return unchecked((ulong)this.parent.ѧ_w_1);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_0 & 0xffff) == 0x10)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_1 = value.Value;
//                }
//            }
//        }
//        public global::CapnProto.Schema.Type.interfaceGroup @interface
//        {
//            get
//            {
//                return new global::CapnProto.Schema.Type.interfaceGroup(this);
//            }
//        }
//        [global::CapnProto.Group, global::CapnProto.Id(0xe82753cff0c2218f)]
//        public struct interfaceGroup
//        {
//            private readonly global::CapnProto.Schema.Type parent;
//            internal interfaceGroup(global::CapnProto.Schema.Type parent)
//            {
//                this.parent = parent;
//            }
//            [global::CapnProto.FieldAttribute(17)]
//            public ulong? typeId
//            {
//                get
//                {
//                    if ((this.parent.ѧ_w_0 & 0xffff) == 0x11)
//                    {
//                        return unchecked((ulong)this.parent.ѧ_w_1);
//                    }
//                    return null;
//                }
//                set
//                {
//                    if (!((this.parent.ѧ_w_0 & 0xffff) == 0x11)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                    this.parent.ѧ_w_1 = value.Value;
//                }
//            }
//        }
//        [global::CapnProto.FieldAttribute(18)]
//        public global::CapnProto.Void anyPointer
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x12 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        public enum Unions
//        {
//            @void = 0,
//            @bool = 1,
//            int8 = 2,
//            int16 = 3,
//            int32 = 4,
//            int64 = 5,
//            uint8 = 6,
//            uint16 = 7,
//            uint32 = 8,
//            uint64 = 9,
//            float32 = 10,
//            float64 = 11,
//            text = 12,
//            data = 13,
//            list = 14,
//            @enum = 15,
//            @struct = 16,
//            @interface = 17,
//            anyPointer = 18,
//        }
//        public global::CapnProto.Schema.Type.Unions Union
//        {
//            get
//            {
//                return (global::CapnProto.Schema.Type.Unions)((this.ѧ_w_0) & 0xFFFF);
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff0000) | (ulong)value;
//            }
//        }
//        // body words: 2; pointers: 1
//        private ulong ѧ_w_0, ѧ_w_1;
//        private object ѧ_p_0;
//    }
//    [global::CapnProto.Id(0xce23dcd2d7b00c9b)]
//    public partial class Value : global::CapnProto.IBlittable
//    {
//        static partial void OnCreate(ref global::CapnProto.Schema.Value obj);
//        internal static global::CapnProto.Schema.Value ѧ_ctor()
//        {
//            global::CapnProto.Schema.Value tmp = null;
//            OnCreate(ref tmp);
//            // if you are providing custom construction, please also provide a private
//            // parameterless constructor (it can just throw an exception if you like)
//            return tmp ?? new global::CapnProto.Schema.Value();
//        }
//        unsafe void global::CapnProto.IBlittable.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//        {
//            ulong* raw = stackalloc ulong[2];
//            ctx.Reader.ReadData(segment, origin, pointer, raw, 2);
//            ѧ_w_0 = raw[0];
//            ѧ_w_1 = raw[1];
//            origin = ctx.Reader.ReadPointers(segment, origin, pointer, raw, 1);
//            if (raw[0] != 0)
//            {
//                // Value.Text
//                if ((this.ѧ_w_0 & 0xffff) == 0xc) this.ѧ_p_0 = ctx.Reader.ReadStringFromPointer(segment, origin + 1, raw[0]);
//                // Value.data
//                if ((this.ѧ_w_0 & 0xffff) == 0xd) this.ѧ_p_0 = ctx.Reader.ReadBytesFromPointer(segment, origin + 1, raw[0]);
//                // Value.list
//                if ((this.ѧ_w_0 & 0xffff) == 0xe) this.ѧ_p_0 = null;
//#warning any-pointer not yet implemented
//                // Value.struct
//                if ((this.ѧ_w_0 & 0xffff) == 0x10) this.ѧ_p_0 = null;
//#warning any-pointer not yet implemented
//                // Value.anyPointer
//                if ((this.ѧ_w_0 & 0xffff) == 0x12) this.ѧ_p_0 = null;
//#warning any-pointer not yet implemented
//            }
//        }
//        static Value()
//        {
//            global::CapnProto.TypeModel.AssertLittleEndian();
//        }
//        public global::CapnProto.Void @void
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x0 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(1)]
//        public bool? @bool
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x1)
//                {
//                    return (this.ѧ_w_0 & 0x4) != 0;
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x1)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                if (value.Value)
//                {
//                    this.ѧ_w_0 |= 0x4;
//                }
//                else
//                {
//                    this.ѧ_w_0 &= 0xfffffffffffffffb;
//                }
//            }
//        }
//        [global::CapnProto.FieldAttribute(2)]
//        public sbyte? int8
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x2)
//                {
//                    return unchecked((sbyte)(this.ѧ_w_0 >> 16));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x2)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffff00ffff) | unchecked(((ulong)(value.Value) << 16));
//            }
//        }
//        [global::CapnProto.FieldAttribute(3)]
//        public short? int16
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x3)
//                {
//                    return unchecked((short)(this.ѧ_w_0 >> 16));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x3)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff0000ffff) | unchecked(((ulong)(value.Value) << 16));
//            }
//        }
//        [global::CapnProto.FieldAttribute(4)]
//        public int? int32
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x4)
//                {
//                    return unchecked((int)(this.ѧ_w_0 >> 32));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x4)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff) | unchecked(((ulong)(value.Value) << 32));
//            }
//        }
//        [global::CapnProto.FieldAttribute(5)]
//        public long? int64
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x5)
//                {
//                    return unchecked((long)this.ѧ_w_1);
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x5)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_1 = unchecked((ulong)value.Value);
//            }
//        }
//        [global::CapnProto.FieldAttribute(5)]
//        public byte? uint8
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x6)
//                {
//                    return unchecked((byte)(this.ѧ_w_0 >> 8));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x6)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff00ff) | unchecked(((ulong)(value.Value) << 8));
//            }
//        }
//        [global::CapnProto.FieldAttribute(7)]
//        public ushort? uint16
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x7)
//                {
//                    return unchecked((ushort)(this.ѧ_w_0 >> 16));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x7)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff0000ffff) | unchecked(((ulong)(value.Value) << 16));
//            }
//        }
//        [global::CapnProto.FieldAttribute(8)]
//        public uint? uint32
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x8)
//                {
//                    return unchecked((uint)(this.ѧ_w_0 >> 32));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x8)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff) | unchecked(((ulong)(value.Value) << 32));
//            }
//        }
//        [global::CapnProto.FieldAttribute(9)]
//        public ulong? uint64
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0x9)
//                {
//                    return unchecked((ulong)this.ѧ_w_1);
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x9)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_1 = value.Value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(10)]
//        public unsafe float? float32
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0xa)
//                {
//                    ulong tmp = this.ѧ_w_0 >> 32;
//                    return *((float*)(&tmp));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xa)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                throw new global::System.NotImplementedException();
//            }
//        }
//        [global::CapnProto.FieldAttribute(11)]
//        public unsafe double? float64
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0xb)
//                {
//                    ulong tmp = this.ѧ_w_1;
//                    return *((double*)(&tmp));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xb)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                throw new global::System.NotImplementedException();
//            }
//        }
//        [global::CapnProto.FieldAttribute(12)]
//        public string Text
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xc ? (string)this.ѧ_p_0 : null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xc)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(13)]
//        public byte[] data
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xd ? (byte[])this.ѧ_p_0 : null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xd)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(14)]
//        public object list
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0xe ? (object)this.ѧ_p_0 : null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xe)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(15)]
//        public ushort? @enum
//        {
//            get
//            {
//                if ((this.ѧ_w_0 & 0xffff) == 0xf)
//                {
//                    return unchecked((ushort)(this.ѧ_w_0 >> 16));
//                }
//                return null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0xf)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffff0000ffff) | unchecked(((ulong)(value.Value) << 16));
//            }
//        }
//        [global::CapnProto.FieldAttribute(16)]
//        public object @struct
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x10 ? (object)this.ѧ_p_0 : null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x10)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_p_0 = value;
//            }
//        }
//        [global::CapnProto.FieldAttribute(17)]
//        public global::CapnProto.Void @interface
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x11 ? global::CapnProto.Void.Value : null;
//            }
//        }
//        [global::CapnProto.FieldAttribute(18)]
//        public object anyPointer
//        {
//            get
//            {
//                return (this.ѧ_w_0 & 0xffff) == 0x12 ? (object)this.ѧ_p_0 : null;
//            }
//            set
//            {
//                if (!((this.ѧ_w_0 & 0xffff) == 0x12)) throw new global::CapnProto.InvalidUnionDiscriminatorException();
//                this.ѧ_p_0 = value;
//            }
//        }
//        public enum Unions
//        {
//            @void = 0,
//            @bool = 1,
//            int8 = 2,
//            int16 = 3,
//            int32 = 4,
//            int64 = 5,
//            uint8 = 6,
//            uint16 = 7,
//            uint32 = 8,
//            uint64 = 9,
//            float32 = 10,
//            float64 = 11,
//            Text = 12,
//            data = 13,
//            list = 14,
//            @enum = 15,
//            @struct = 16,
//            @interface = 17,
//            anyPointer = 18,
//        }
//        public global::CapnProto.Schema.Value.Unions Union
//        {
//            get
//            {
//                return (global::CapnProto.Schema.Value.Unions)((this.ѧ_w_0) & 0xFFFF);
//            }
//            set
//            {
//                this.ѧ_w_0 = (this.ѧ_w_0 & 0xffffffffffff0000) | (ulong)value;
//            }
//        }
//        // body words: 2; pointers: 1
//        private ulong ѧ_w_0, ѧ_w_1;
//        private object ѧ_p_0;
//    }
//    public class SchemaSerializer : global::CapnProto.TypeModel
//    {
//        private global::CapnProto.ITypeSerializer ѧ_f_bfc546f6210ad7ce, ѧ_f_cfea0eb02e810062, ѧ_f_e682ab4cf923a417, ѧ_f_ae504193122357e5, ѧ_f_debf55bbfa0fc242, ѧ_f_f1c8950dab257542, ѧ_f_9aad50a41f4af45f, ѧ_f_978a7cebdc549a4d, ѧ_f_9500cce23b334d80, ѧ_f_d07378ede1f9cc60, ѧ_f_ce23dcd2d7b00c9b;
//        public override global::CapnProto.ITypeSerializer GetSerializer(global::System.Type type)
//        {
//            if (type == typeof(global::CapnProto.Schema.CodeGeneratorRequest)) return ѧ_f_bfc546f6210ad7ce ?? (ѧ_f_bfc546f6210ad7ce = new ѧ_s_bfc546f6210ad7ce(this));
//            if (type == typeof(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile)) return ѧ_f_cfea0eb02e810062 ?? (ѧ_f_cfea0eb02e810062 = new ѧ_s_cfea0eb02e810062(this));
//            if (type == typeof(global::CapnProto.Schema.Node)) return ѧ_f_e682ab4cf923a417 ?? (ѧ_f_e682ab4cf923a417 = new ѧ_s_e682ab4cf923a417(this));
//            if (type == typeof(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import)) return ѧ_f_ae504193122357e5 ?? (ѧ_f_ae504193122357e5 = new ѧ_s_ae504193122357e5(this));
//            if (type == typeof(global::CapnProto.Schema.NestedNode)) return ѧ_f_debf55bbfa0fc242 ?? (ѧ_f_debf55bbfa0fc242 = new ѧ_s_debf55bbfa0fc242(this));
//            if (type == typeof(global::CapnProto.Schema.Annotation)) return ѧ_f_f1c8950dab257542 ?? (ѧ_f_f1c8950dab257542 = new ѧ_s_f1c8950dab257542(this));
//            if (type == typeof(global::CapnProto.Schema.Field)) return ѧ_f_9aad50a41f4af45f ?? (ѧ_f_9aad50a41f4af45f = new ѧ_s_9aad50a41f4af45f(this));
//            if (type == typeof(global::CapnProto.Schema.Enumerant)) return ѧ_f_978a7cebdc549a4d ?? (ѧ_f_978a7cebdc549a4d = new ѧ_s_978a7cebdc549a4d(this));
//            if (type == typeof(global::CapnProto.Schema.Method)) return ѧ_f_9500cce23b334d80 ?? (ѧ_f_9500cce23b334d80 = new ѧ_s_9500cce23b334d80(this));
//            if (type == typeof(global::CapnProto.Schema.Type)) return ѧ_f_d07378ede1f9cc60 ?? (ѧ_f_d07378ede1f9cc60 = new ѧ_s_d07378ede1f9cc60(this));
//            if (type == typeof(global::CapnProto.Schema.Value)) return ѧ_f_ce23dcd2d7b00c9b ?? (ѧ_f_ce23dcd2d7b00c9b = new ѧ_s_ce23dcd2d7b00c9b(this));
//            return base.GetSerializer(type);
//        }
//        class ѧ_s_bfc546f6210ad7ce : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.CodeGeneratorRequest>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_bfc546f6210ad7ce(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_bfc546f6210ad7ce(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.CodeGeneratorRequest Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_bfc546f6210ad7ce(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_cfea0eb02e810062 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_cfea0eb02e810062(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_cfea0eb02e810062(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_cfea0eb02e810062(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_e682ab4cf923a417 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Node>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_e682ab4cf923a417(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_e682ab4cf923a417(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Node Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_e682ab4cf923a417(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_ae504193122357e5 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_ae504193122357e5(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_ae504193122357e5(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_ae504193122357e5(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_debf55bbfa0fc242 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.NestedNode>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_debf55bbfa0fc242(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_debf55bbfa0fc242(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.NestedNode Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_debf55bbfa0fc242(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_f1c8950dab257542 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Annotation>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_f1c8950dab257542(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_f1c8950dab257542(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Annotation Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_f1c8950dab257542(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_9aad50a41f4af45f : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Field>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_9aad50a41f4af45f(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_9aad50a41f4af45f(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Field Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_9aad50a41f4af45f(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_978a7cebdc549a4d : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Enumerant>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_978a7cebdc549a4d(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_978a7cebdc549a4d(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Enumerant Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_978a7cebdc549a4d(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_9500cce23b334d80 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Method>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_9500cce23b334d80(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_9500cce23b334d80(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Method Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_9500cce23b334d80(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_d07378ede1f9cc60 : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Type>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_d07378ede1f9cc60(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_d07378ede1f9cc60(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Type Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_d07378ede1f9cc60(segment, origin, ctx, pointer);
//            }
//        }
//        class ѧ_s_ce23dcd2d7b00c9b : global::CapnProto.Schema.SchemaSerializer.ѧ_SerializerBase, global::CapnProto.ITypeSerializer<global::CapnProto.Schema.Value>
//        {
//            private readonly global::CapnProto.TypeModel model;
//            internal ѧ_s_ce23dcd2d7b00c9b(global::CapnProto.TypeModel model)
//            {
//                this.model = model;
//            }
//            global::CapnProto.TypeModel global::CapnProto.ITypeSerializer.Model { get { return this.model; } }
//            object global::CapnProto.ITypeSerializer.Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_ce23dcd2d7b00c9b(segment, origin, ctx, pointer);
//            }
//            public global::CapnProto.Schema.Value Deserialize(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                return ѧ_r_ce23dcd2d7b00c9b(segment, origin, ctx, pointer);
//            }
//        }
//        internal class ѧ_SerializerBase
//        {
//            internal static global::CapnProto.Schema.CodeGeneratorRequest ѧ_r_bfc546f6210ad7ce(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.CodeGeneratorRequest.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.CodeGeneratorRequest>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile ѧ_r_cfea0eb02e810062(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Node ѧ_r_e682ab4cf923a417(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Node.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Node>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import ѧ_r_ae504193122357e5(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.NestedNode ѧ_r_debf55bbfa0fc242(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.NestedNode.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.NestedNode>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Annotation ѧ_r_f1c8950dab257542(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Annotation.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Annotation>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Field ѧ_r_9aad50a41f4af45f(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Field.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Field>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Enumerant ѧ_r_978a7cebdc549a4d(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Enumerant.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Enumerant>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Method ѧ_r_9500cce23b334d80(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Method.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Method>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Type ѧ_r_d07378ede1f9cc60(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Type.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Type>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//            internal static global::CapnProto.Schema.Value ѧ_r_ce23dcd2d7b00c9b(int segment, int origin, global::CapnProto.DeserializationContext ctx, ulong pointer)
//            {
//                var obj = global::CapnProto.Schema.Value.ѧ_ctor();
//#pragma warning disable 0618
//                global::CapnProto.TypeSerializer.Deserialize<global::CapnProto.Schema.Value>(ref obj, segment, origin, ctx, pointer);
//#pragma warning restore 0618
//                return obj;
//            }
//        }
//    }
//}