
namespace CapnProto.Schema
{
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 0, 2)]
    [global::CapnProto.IdAttribute(0xbfc546f6210ad7ce)]
    public partial struct CodeGeneratorRequest : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private CodeGeneratorRequest(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.CodeGeneratorRequest(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.CodeGeneratorRequest(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.CodeGeneratorRequest obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.CodeGeneratorRequest obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.CodeGeneratorRequest obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.CodeGeneratorRequest obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.CodeGeneratorRequest && (this.ѧ_ == ((global::CapnProto.Schema.CodeGeneratorRequest)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.CodeGeneratorRequest Dereference() { return (global::CapnProto.Schema.CodeGeneratorRequest)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.CodeGeneratorRequest Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.CodeGeneratorRequest)parent.Allocate(0, 2); }
        [global::CapnProto.FieldAttribute(0, pointer: 0)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Node> nodes
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Node>)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, pointer: 1)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile> requestedFiles
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile>)this.ѧ_.GetPointer(1);
            }
            set
            {
                this.ѧ_.SetPointer(1, value);
            }
        }
        [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 1, 2)]
        [global::CapnProto.IdAttribute(0xcfea0eb02e810062)]
        public partial struct RequestedFile : global::CapnProto.IPointer
        {
            private global::CapnProto.Pointer ѧ_;
            private RequestedFile(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
            public static explicit operator global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile(pointer); }
            public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile obj) { return obj.ѧ_; }
            public static bool operator true(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile obj) { return obj.ѧ_.IsValid; }
            public static bool operator false(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile obj) { return !obj.ѧ_.IsValid; }
            public static bool operator !(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile obj) { return !obj.ѧ_.IsValid; }
            public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
            partial void OnToString(ref string s);
            public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
            public override bool Equals(object obj) { return obj is global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile && (this.ѧ_ == ((global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile)obj).ѧ_); }
            global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
            public global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile Dereference() { return (global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile)this.ѧ_.Dereference(); }
            public static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile)parent.Allocate(1, 2); }
            [global::CapnProto.FieldAttribute(0, 0, 64)]
            public ulong id
            {
                get
                {
                    return this.ѧ_.GetUInt64(0);
                }
                set
                {
                    this.ѧ_.SetUInt64(0, value);
                }
            }
            [global::CapnProto.FieldAttribute(1, pointer: 0)]
            public global::CapnProto.Text filename
            {
                get
                {
                    return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
                }
                set
                {
                    this.ѧ_.SetPointer(0, value);
                }
            }
            [global::CapnProto.FieldAttribute(2, pointer: 1)]
            public global::CapnProto.FixedSizeList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import> imports
            {
                get
                {
                    return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import>)this.ѧ_.GetPointer(1);
                }
                set
                {
                    this.ѧ_.SetPointer(1, value);
                }
            }
            [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 1, 1)]
            [global::CapnProto.IdAttribute(0xae504193122357e5)]
            public partial struct Import : global::CapnProto.IPointer
            {
                private global::CapnProto.Pointer ѧ_;
                private Import(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
                public static explicit operator global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import(pointer); }
                public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import obj) { return obj.ѧ_; }
                public static bool operator true(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import obj) { return obj.ѧ_.IsValid; }
                public static bool operator false(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import obj) { return !obj.ѧ_.IsValid; }
                public static bool operator !(global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import obj) { return !obj.ѧ_.IsValid; }
                public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
                partial void OnToString(ref string s);
                public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
                public override bool Equals(object obj) { return obj is global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import && (this.ѧ_ == ((global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import)obj).ѧ_); }
                global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
                public global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import Dereference() { return (global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import)this.ѧ_.Dereference(); }
                public static global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.CodeGeneratorRequest.RequestedFile.Import)parent.Allocate(1, 1); }
                [global::CapnProto.FieldAttribute(0, 0, 64)]
                public ulong id
                {
                    get
                    {
                        return this.ѧ_.GetUInt64(0);
                    }
                    set
                    {
                        this.ѧ_.SetUInt64(0, value);
                    }
                }
                [global::CapnProto.FieldAttribute(1, pointer: 0)]
                public global::CapnProto.Text name
                {
                    get
                    {
                        return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
                    }
                    set
                    {
                        this.ѧ_.SetPointer(0, value);
                    }
                }
            }
        }
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 3, 2)]
    [global::CapnProto.IdAttribute(0x9500cce23b334d80)]
    public partial struct Method : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Method(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Method(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Method(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Method obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Method obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Method obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Method obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Method && (this.ѧ_ == ((global::CapnProto.Schema.Method)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Method Dereference() { return (global::CapnProto.Schema.Method)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Method Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.Method)parent.Allocate(3, 2); }
        [global::CapnProto.FieldAttribute(0, pointer: 0)]
        public global::CapnProto.Text name
        {
            get
            {
                return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, 0, 16)]
        public ushort codeOrder
        {
            get
            {
                return this.ѧ_.GetUInt16(0);
            }
            set
            {
                this.ѧ_.SetUInt16(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(2, 64, 128)]
        public ulong paramStructType
        {
            get
            {
                return this.ѧ_.GetUInt64(1);
            }
            set
            {
                this.ѧ_.SetUInt64(1, value);
            }
        }
        [global::CapnProto.FieldAttribute(3, 128, 192)]
        public ulong resultStructType
        {
            get
            {
                return this.ѧ_.GetUInt64(2);
            }
            set
            {
                this.ѧ_.SetUInt64(2, value);
            }
        }
        [global::CapnProto.FieldAttribute(4, pointer: 1)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation> annotations
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation>)this.ѧ_.GetPointer(1);
            }
            set
            {
                this.ѧ_.SetPointer(1, value);
            }
        }
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 1, 2)]
    [global::CapnProto.IdAttribute(0x978a7cebdc549a4d)]
    public partial struct Enumerant : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Enumerant(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Enumerant(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Enumerant(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Enumerant obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Enumerant obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Enumerant obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Enumerant obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Enumerant && (this.ѧ_ == ((global::CapnProto.Schema.Enumerant)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Enumerant Dereference() { return (global::CapnProto.Schema.Enumerant)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Enumerant Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.Enumerant)parent.Allocate(1, 2); }
        [global::CapnProto.FieldAttribute(0, pointer: 0)]
        public global::CapnProto.Text name
        {
            get
            {
                return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, 0, 16)]
        public ushort codeOrder
        {
            get
            {
                return this.ѧ_.GetUInt16(0);
            }
            set
            {
                this.ѧ_.SetUInt16(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(2, pointer: 1)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation> annotations
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation>)this.ѧ_.GetPointer(1);
            }
            set
            {
                this.ѧ_.SetPointer(1, value);
            }
        }
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 5, 5)]
    [global::CapnProto.IdAttribute(0xe682ab4cf923a417)]
    public partial struct Node : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Node(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Node(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Node(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Node obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Node obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Node obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Node obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Node && (this.ѧ_ == ((global::CapnProto.Schema.Node)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Node Dereference() { return (global::CapnProto.Schema.Node)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Node Create(global::CapnProto.Pointer parent, global::CapnProto.Schema.Node.Unions union)
        {
            var ptr = parent.Allocate(5, 5);
            ptr.SetUInt16(6, (ushort)union);
            return (global::CapnProto.Schema.Node)ptr;
        }
        [global::CapnProto.FieldAttribute(0, 0, 64)]
        public ulong id
        {
            get
            {
                return this.ѧ_.GetUInt64(0);
            }
            set
            {
                this.ѧ_.SetUInt64(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, pointer: 0)]
        public global::CapnProto.Text displayName
        {
            get
            {
                return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(2, 64, 96)]
        public uint displayNamePrefixLength
        {
            get
            {
                return this.ѧ_.GetUInt32(2);
            }
            set
            {
                this.ѧ_.SetUInt32(2, value);
            }
        }
        [global::CapnProto.FieldAttribute(3, 128, 192)]
        public ulong scopeId
        {
            get
            {
                return this.ѧ_.GetUInt64(2);
            }
            set
            {
                this.ѧ_.SetUInt64(2, value);
            }
        }
        [global::CapnProto.FieldAttribute(4, pointer: 1)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Node.NestedNode> nestedNodes
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Node.NestedNode>)this.ѧ_.GetPointer(1);
            }
            set
            {
                this.ѧ_.SetPointer(1, value);
            }
        }
        [global::CapnProto.FieldAttribute(5, pointer: 2)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation> annotations
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation>)this.ѧ_.GetPointer(2);
            }
            set
            {
                this.ѧ_.SetPointer(2, value);
            }
        }
        public global::CapnProto.Schema.Node.structGroup @struct
        {
            get
            {
                return new global::CapnProto.Schema.Node.structGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0x9ea0b19b37fb4435)]
        public partial struct structGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal structGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(7, 112, 128)]
            public ushort dataWordCount
            {
                get
                {
                    return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 7 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 7 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(8, 192, 208)]
            public ushort pointerCount
            {
                get
                {
                    return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 12 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 12 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(9, 208, 224)]
            public global::CapnProto.Schema.ElementSize preferredListEncoding
            {
                get
                {
                    return (global::CapnProto.Schema.ElementSize)this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 13 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 13 : -1, (ushort)value);
                }
            }
            [global::CapnProto.FieldAttribute(10, 224, 225)]
            public bool isGroup
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)1 ? 224 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)1 ? 224 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(11, 240, 256)]
            public ushort discriminantCount
            {
                get
                {
                    return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 15 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(6) == (ushort)1 ? 15 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(12, 256, 288)]
            public uint discriminantOffset
            {
                get
                {
                    return this.ѧ_.GetUInt32(this.ѧ_.GetUInt16(6) == (ushort)1 ? 8 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt32(this.ѧ_.GetUInt16(6) == (ushort)1 ? 8 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(13, pointer: 3)]
            public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Field> fields
            {
                get
                {
                    return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Field>)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)1 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)1 ? 3 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Node.enumGroup @enum
        {
            get
            {
                return new global::CapnProto.Schema.Node.enumGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xb54ab3364333f598)]
        public partial struct enumGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal enumGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(14, pointer: 3)]
            public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Enumerant> enumerants
            {
                get
                {
                    return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Enumerant>)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)2 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)2 ? 3 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Node.interfaceGroup @interface
        {
            get
            {
                return new global::CapnProto.Schema.Node.interfaceGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xe82753cff0c2218f)]
        public partial struct interfaceGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal interfaceGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(15, pointer: 3)]
            public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Method> methods
            {
                get
                {
                    return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Method>)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)3 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)3 ? 3 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(31, pointer: 4)]
            public global::CapnProto.FixedSizeList<ulong> extends
            {
                get
                {
                    return (global::CapnProto.FixedSizeList<ulong>)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)3 ? 4 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)3 ? 4 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Node.constGroup @const
        {
            get
            {
                return new global::CapnProto.Schema.Node.constGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xb18aa5ac7a0d9420)]
        public partial struct constGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal constGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(16, pointer: 3)]
            public global::CapnProto.Schema.Type type
            {
                get
                {
                    return (global::CapnProto.Schema.Type)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)4 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)4 ? 3 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(17, pointer: 4)]
            public global::CapnProto.Schema.Value value
            {
                get
                {
                    return (global::CapnProto.Schema.Value)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)4 ? 4 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)4 ? 4 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Node.annotationGroup annotation
        {
            get
            {
                return new global::CapnProto.Schema.Node.annotationGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xec1619d4400a0290)]
        public partial struct annotationGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal annotationGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(18, pointer: 3)]
            public global::CapnProto.Schema.Type type
            {
                get
                {
                    return (global::CapnProto.Schema.Type)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(6) == (ushort)5 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(6) == (ushort)5 ? 3 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(19, 112, 113)]
            public bool targetsFile
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 112 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 112 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(20, 113, 114)]
            public bool targetsConst
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 113 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 113 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(21, 114, 115)]
            public bool targetsEnum
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 114 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 114 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(22, 115, 116)]
            public bool targetsEnumerant
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 115 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 115 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(23, 116, 117)]
            public bool targetsStruct
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 116 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 116 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(24, 117, 118)]
            public bool targetsField
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 117 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 117 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(25, 118, 119)]
            public bool targetsUnion
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 118 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 118 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(26, 119, 120)]
            public bool targetsGroup
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 119 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 119 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(27, 120, 121)]
            public bool targetsInterface
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 120 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 120 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(28, 121, 122)]
            public bool targetsMethod
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 121 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 121 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(29, 122, 123)]
            public bool targetsParam
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 122 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 122 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(30, 123, 124)]
            public bool targetsAnnotation
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 123 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(6) == (ushort)5 ? 123 : -1, value);
                }
            }
        }
        public enum Unions
        {
            file = 0,
            @struct = 1,
            @enum = 2,
            @interface = 3,
            @const = 4,
            annotation = 5,
        }
        public global::CapnProto.Schema.Node.Unions Union
        {
            get
            {
                return (global::CapnProto.Schema.Node.Unions)this.ѧ_.GetUInt16(6);
            }
            set
            {
                this.ѧ_.SetUInt16(6, (ushort)value);
            }
        }
        [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 1, 1)]
        [global::CapnProto.IdAttribute(0xdebf55bbfa0fc242)]
        public partial struct NestedNode : global::CapnProto.IPointer
        {
            private global::CapnProto.Pointer ѧ_;
            private NestedNode(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
            public static explicit operator global::CapnProto.Schema.Node.NestedNode(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Node.NestedNode(pointer); }
            public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Node.NestedNode obj) { return obj.ѧ_; }
            public static bool operator true(global::CapnProto.Schema.Node.NestedNode obj) { return obj.ѧ_.IsValid; }
            public static bool operator false(global::CapnProto.Schema.Node.NestedNode obj) { return !obj.ѧ_.IsValid; }
            public static bool operator !(global::CapnProto.Schema.Node.NestedNode obj) { return !obj.ѧ_.IsValid; }
            public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
            partial void OnToString(ref string s);
            public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
            public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Node.NestedNode && (this.ѧ_ == ((global::CapnProto.Schema.Node.NestedNode)obj).ѧ_); }
            global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
            public global::CapnProto.Schema.Node.NestedNode Dereference() { return (global::CapnProto.Schema.Node.NestedNode)this.ѧ_.Dereference(); }
            public static global::CapnProto.Schema.Node.NestedNode Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.Node.NestedNode)parent.Allocate(1, 1); }
            [global::CapnProto.FieldAttribute(0, pointer: 0)]
            public global::CapnProto.Text name
            {
                get
                {
                    return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
                }
                set
                {
                    this.ѧ_.SetPointer(0, value);
                }
            }
            [global::CapnProto.FieldAttribute(1, 0, 64)]
            public ulong id
            {
                get
                {
                    return this.ѧ_.GetUInt64(0);
                }
                set
                {
                    this.ѧ_.SetUInt64(0, value);
                }
            }
        }
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 2, 1)]
    [global::CapnProto.IdAttribute(0xce23dcd2d7b00c9b)]
    public partial struct Value : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Value(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Value(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Value(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Value obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Value obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Value obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Value obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Value && (this.ѧ_ == ((global::CapnProto.Schema.Value)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Value Dereference() { return (global::CapnProto.Schema.Value)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Value Create(global::CapnProto.Pointer parent, global::CapnProto.Schema.Value.Unions union)
        {
            var ptr = parent.Allocate(2, 1);
            ptr.SetUInt16(0, (ushort)union);
            return (global::CapnProto.Schema.Value)ptr;
        }
        [global::CapnProto.FieldAttribute(1, 16, 17)]
        public bool @bool
        {
            get
            {
                return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(0) == (ushort)1 ? 16 : -1);
            }
            set
            {
                this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(0) == (ushort)1 ? 16 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(2, 16, 24)]
        public sbyte int8
        {
            get
            {
                return this.ѧ_.GetSByte(this.ѧ_.GetUInt16(0) == (ushort)2 ? 2 : -1);
            }
            set
            {
                this.ѧ_.SetSByte(this.ѧ_.GetUInt16(0) == (ushort)2 ? 2 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(3, 16, 32)]
        public short int16
        {
            get
            {
                return this.ѧ_.GetInt16(this.ѧ_.GetUInt16(0) == (ushort)3 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetInt16(this.ѧ_.GetUInt16(0) == (ushort)3 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(4, 32, 64)]
        public int int32
        {
            get
            {
                return this.ѧ_.GetInt32(this.ѧ_.GetUInt16(0) == (ushort)4 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetInt32(this.ѧ_.GetUInt16(0) == (ushort)4 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(5, 64, 128)]
        public long int64
        {
            get
            {
                return this.ѧ_.GetInt64(this.ѧ_.GetUInt16(0) == (ushort)5 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetInt64(this.ѧ_.GetUInt16(0) == (ushort)5 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(6, 16, 24)]
        public byte uint8
        {
            get
            {
                return this.ѧ_.GetByte(this.ѧ_.GetUInt16(0) == (ushort)6 ? 2 : -1);
            }
            set
            {
                this.ѧ_.SetByte(this.ѧ_.GetUInt16(0) == (ushort)6 ? 2 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(7, 16, 32)]
        public ushort uint16
        {
            get
            {
                return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(0) == (ushort)7 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(0) == (ushort)7 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(8, 32, 64)]
        public uint uint32
        {
            get
            {
                return this.ѧ_.GetUInt32(this.ѧ_.GetUInt16(0) == (ushort)8 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetUInt32(this.ѧ_.GetUInt16(0) == (ushort)8 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(9, 64, 128)]
        public ulong uint64
        {
            get
            {
                return this.ѧ_.GetUInt64(this.ѧ_.GetUInt16(0) == (ushort)9 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetUInt64(this.ѧ_.GetUInt16(0) == (ushort)9 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(10, 32, 64)]
        public float float32
        {
            get
            {
                return this.ѧ_.GetSingle(this.ѧ_.GetUInt16(0) == (ushort)10 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetSingle(this.ѧ_.GetUInt16(0) == (ushort)10 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(11, 64, 128)]
        public double float64
        {
            get
            {
                return this.ѧ_.GetDouble(this.ѧ_.GetUInt16(0) == (ushort)11 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetDouble(this.ѧ_.GetUInt16(0) == (ushort)11 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(12, pointer: 0)]
        public global::CapnProto.Text text
        {
            get
            {
                return (global::CapnProto.Text)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)12 ? 0 : -1);
            }
            set
            {
                this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)12 ? 0 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(13, pointer: 0)]
        public global::CapnProto.Data data
        {
            get
            {
                return (global::CapnProto.Data)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)13 ? 0 : -1);
            }
            set
            {
                this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)13 ? 0 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(14, pointer: 0)]
        public global::CapnProto.Pointer list
        {
            get
            {
                return (global::CapnProto.Pointer)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)14 ? 0 : -1);
            }
            set
            {
                this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)14 ? 0 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(15, 16, 32)]
        public ushort @enum
        {
            get
            {
                return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(0) == (ushort)15 ? 1 : -1);
            }
            set
            {
                this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(0) == (ushort)15 ? 1 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(16, pointer: 0)]
        public global::CapnProto.Pointer @struct
        {
            get
            {
                return (global::CapnProto.Pointer)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)16 ? 0 : -1);
            }
            set
            {
                this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)16 ? 0 : -1, value);
            }
        }
        [global::CapnProto.FieldAttribute(18, pointer: 0)]
        public global::CapnProto.Pointer anyPointer
        {
            get
            {
                return (global::CapnProto.Pointer)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)18 ? 0 : -1);
            }
            set
            {
                this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)18 ? 0 : -1, value);
            }
        }
        public enum Unions
        {
            @void = 0,
            @bool = 1,
            int8 = 2,
            int16 = 3,
            int32 = 4,
            int64 = 5,
            uint8 = 6,
            uint16 = 7,
            uint32 = 8,
            uint64 = 9,
            float32 = 10,
            float64 = 11,
            text = 12,
            data = 13,
            list = 14,
            @enum = 15,
            @struct = 16,
            @interface = 17,
            anyPointer = 18,
        }
        public global::CapnProto.Schema.Value.Unions Union
        {
            get
            {
                return (global::CapnProto.Schema.Value.Unions)this.ѧ_.GetUInt16(0);
            }
            set
            {
                this.ѧ_.SetUInt16(0, (ushort)value);
            }
        }
    }
    public enum ElementSize
    {
        empty = 0,
        bit = 1,
        @byte = 2,
        twoBytes = 3,
        fourBytes = 4,
        eightBytes = 5,
        pointer = 6,
        inlineComposite = 7,
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 2, 1)]
    [global::CapnProto.IdAttribute(0xd07378ede1f9cc60)]
    public partial struct Type : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Type(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Type(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Type(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Type obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Type obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Type obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Type obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Type && (this.ѧ_ == ((global::CapnProto.Schema.Type)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Type Dereference() { return (global::CapnProto.Schema.Type)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Type Create(global::CapnProto.Pointer parent, global::CapnProto.Schema.Type.Unions union)
        {
            var ptr = parent.Allocate(2, 1);
            ptr.SetUInt16(0, (ushort)union);
            return (global::CapnProto.Schema.Type)ptr;
        }
        public global::CapnProto.Schema.Type.listGroup list
        {
            get
            {
                return new global::CapnProto.Schema.Type.listGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0x87e739250a60ea97)]
        public partial struct listGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal listGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(14, pointer: 0)]
            public global::CapnProto.Schema.Type elementType
            {
                get
                {
                    return (global::CapnProto.Schema.Type)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(0) == (ushort)14 ? 0 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(0) == (ushort)14 ? 0 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Type.enumGroup @enum
        {
            get
            {
                return new global::CapnProto.Schema.Type.enumGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0x9e0e78711a7f87a9)]
        public partial struct enumGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal enumGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(15, 64, 128)]
            public ulong typeId
            {
                get
                {
                    return this.ѧ_.GetUInt64(this.ѧ_.GetUInt16(0) == (ushort)15 ? 1 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt64(this.ѧ_.GetUInt16(0) == (ushort)15 ? 1 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Type.structGroup @struct
        {
            get
            {
                return new global::CapnProto.Schema.Type.structGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xac3a6f60ef4cc6d3)]
        public partial struct structGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal structGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(16, 64, 128)]
            public ulong typeId
            {
                get
                {
                    return this.ѧ_.GetUInt64(this.ѧ_.GetUInt16(0) == (ushort)16 ? 1 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt64(this.ѧ_.GetUInt16(0) == (ushort)16 ? 1 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Type.interfaceGroup @interface
        {
            get
            {
                return new global::CapnProto.Schema.Type.interfaceGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xed8bca69f7fb0cbf)]
        public partial struct interfaceGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal interfaceGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(17, 64, 128)]
            public ulong typeId
            {
                get
                {
                    return this.ѧ_.GetUInt64(this.ѧ_.GetUInt16(0) == (ushort)17 ? 1 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt64(this.ѧ_.GetUInt16(0) == (ushort)17 ? 1 : -1, value);
                }
            }
        }
        public enum Unions
        {
            @void = 0,
            @bool = 1,
            int8 = 2,
            int16 = 3,
            int32 = 4,
            int64 = 5,
            uint8 = 6,
            uint16 = 7,
            uint32 = 8,
            uint64 = 9,
            float32 = 10,
            float64 = 11,
            text = 12,
            data = 13,
            list = 14,
            @enum = 15,
            @struct = 16,
            @interface = 17,
            anyPointer = 18,
        }
        public global::CapnProto.Schema.Type.Unions Union
        {
            get
            {
                return (global::CapnProto.Schema.Type.Unions)this.ѧ_.GetUInt16(0);
            }
            set
            {
                this.ѧ_.SetUInt16(0, (ushort)value);
            }
        }
    }
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 3, 4)]
    [global::CapnProto.IdAttribute(0x9aad50a41f4af45f)]
    public partial struct Field : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Field(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Field(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Field(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Field obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Field obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Field obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Field obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Field && (this.ѧ_ == ((global::CapnProto.Schema.Field)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Field Dereference() { return (global::CapnProto.Schema.Field)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Field Create(global::CapnProto.Pointer parent, global::CapnProto.Schema.Field.Unions union)
        {
            var ptr = parent.Allocate(3, 4);
            ptr.SetUInt16(4, (ushort)union);
            return (global::CapnProto.Schema.Field)ptr;
        }
        [global::CapnProto.FieldAttribute(0, pointer: 0)]
        public global::CapnProto.Text name
        {
            get
            {
                return (global::CapnProto.Text)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, 0, 16)]
        public ushort codeOrder
        {
            get
            {
                return this.ѧ_.GetUInt16(0);
            }
            set
            {
                this.ѧ_.SetUInt16(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(2, pointer: 1)]
        public global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation> annotations
        {
            get
            {
                return (global::CapnProto.FixedSizeList<global::CapnProto.Schema.Annotation>)this.ѧ_.GetPointer(1);
            }
            set
            {
                this.ѧ_.SetPointer(1, value);
            }
        }
        [global::CapnProto.FieldAttribute(3, 16, 32)]
        [global::System.ComponentModel.DefaultValueAttribute(65535)]
        public ushort discriminantValue
        {
            get
            {
                return (ushort)(this.ѧ_.GetUInt16(1) ^ (ushort)65535);
            }
            set
            {
                this.ѧ_.SetUInt16(1, (ushort)(value ^ (ushort)65535));
            }
        }
        public global::CapnProto.Schema.Field.slotGroup slot
        {
            get
            {
                return new global::CapnProto.Schema.Field.slotGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xc42305476bb4746f)]
        public partial struct slotGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal slotGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(4, 32, 64)]
            public uint offset
            {
                get
                {
                    return this.ѧ_.GetUInt32(this.ѧ_.GetUInt16(4) == (ushort)0 ? 1 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt32(this.ѧ_.GetUInt16(4) == (ushort)0 ? 1 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(5, pointer: 2)]
            public global::CapnProto.Schema.Type type
            {
                get
                {
                    return (global::CapnProto.Schema.Type)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(4) == (ushort)0 ? 2 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(4) == (ushort)0 ? 2 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(6, pointer: 3)]
            public global::CapnProto.Schema.Value defaultValue
            {
                get
                {
                    return (global::CapnProto.Schema.Value)this.ѧ_.GetPointer(this.ѧ_.GetUInt16(4) == (ushort)0 ? 3 : -1);
                }
                set
                {
                    this.ѧ_.SetPointer(this.ѧ_.GetUInt16(4) == (ushort)0 ? 3 : -1, value);
                }
            }
            [global::CapnProto.FieldAttribute(10, 128, 129)]
            public bool hadExplicitDefault
            {
                get
                {
                    return this.ѧ_.GetBoolean(this.ѧ_.GetUInt16(4) == (ushort)0 ? 128 : -1);
                }
                set
                {
                    this.ѧ_.SetBoolean(this.ѧ_.GetUInt16(4) == (ushort)0 ? 128 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Field.groupGroup group
        {
            get
            {
                return new global::CapnProto.Schema.Field.groupGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xcafccddb68db1d11)]
        public partial struct groupGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal groupGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(7, 128, 192)]
            public ulong typeId
            {
                get
                {
                    return this.ѧ_.GetUInt64(this.ѧ_.GetUInt16(4) == (ushort)1 ? 2 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt64(this.ѧ_.GetUInt16(4) == (ushort)1 ? 2 : -1, value);
                }
            }
        }
        public global::CapnProto.Schema.Field.ordinalGroup ordinal
        {
            get
            {
                return new global::CapnProto.Schema.Field.ordinalGroup(this.ѧ_);
            }
        }
        [global::CapnProto.Group, global::CapnProto.Id(0xbb90d5c287870be6)]
        public partial struct ordinalGroup
        {
            private global::CapnProto.Pointer ѧ_;
            internal ordinalGroup(global::CapnProto.Pointer pointer)
            {
                this.ѧ_ = pointer;
            }
            [global::CapnProto.FieldAttribute(9, 96, 112)]
            public ushort @explicit
            {
                get
                {
                    return this.ѧ_.GetUInt16(this.ѧ_.GetUInt16(5) == (ushort)1 ? 6 : -1);
                }
                set
                {
                    this.ѧ_.SetUInt16(this.ѧ_.GetUInt16(5) == (ushort)1 ? 6 : -1, value);
                }
            }
            public enum Unions
            {
                @implicit = 0,
                @explicit = 1,
            }
            public global::CapnProto.Schema.Field.ordinalGroup.Unions Union
            {
                get
                {
                    return (global::CapnProto.Schema.Field.ordinalGroup.Unions)this.ѧ_.GetUInt16(5);
                }
                set
                {
                    this.ѧ_.SetUInt16(5, (ushort)value);
                }
            }
        }
        public enum Unions
        {
            slot = 0,
            group = 1,
        }
        public global::CapnProto.Schema.Field.Unions Union
        {
            get
            {
                return (global::CapnProto.Schema.Field.Unions)this.ѧ_.GetUInt16(4);
            }
            set
            {
                this.ѧ_.SetUInt16(4, (ushort)value);
            }
        }
        public const ushort noDiscriminant = 65535;
    }
    /* Not implemented: capnp/c++.capnp:namespace (annotation)) */
    [global::CapnProto.StructAttribute(global::CapnProto.ElementSize.InlineComposite, 1, 1)]
    [global::CapnProto.IdAttribute(0xf1c8950dab257542)]
    public partial struct Annotation : global::CapnProto.IPointer
    {
        private global::CapnProto.Pointer ѧ_;
        private Annotation(global::CapnProto.Pointer pointer) { this.ѧ_ = pointer; }
        public static explicit operator global::CapnProto.Schema.Annotation(global::CapnProto.Pointer pointer) { return new global::CapnProto.Schema.Annotation(pointer); }
        public static implicit operator global::CapnProto.Pointer(global::CapnProto.Schema.Annotation obj) { return obj.ѧ_; }
        public static bool operator true(global::CapnProto.Schema.Annotation obj) { return obj.ѧ_.IsValid; }
        public static bool operator false(global::CapnProto.Schema.Annotation obj) { return !obj.ѧ_.IsValid; }
        public static bool operator !(global::CapnProto.Schema.Annotation obj) { return !obj.ѧ_.IsValid; }
        public override int GetHashCode() { return this.ѧ_.GetHashCode(); }
        partial void OnToString(ref string s);
        public override string ToString() { string s = null; this.OnToString(ref s); return s ?? this.ѧ_.ToString(); }
        public override bool Equals(object obj) { return obj is global::CapnProto.Schema.Annotation && (this.ѧ_ == ((global::CapnProto.Schema.Annotation)obj).ѧ_); }
        global::CapnProto.Pointer global::CapnProto.IPointer.Pointer { get { return this.ѧ_; } }
        public global::CapnProto.Schema.Annotation Dereference() { return (global::CapnProto.Schema.Annotation)this.ѧ_.Dereference(); }
        public static global::CapnProto.Schema.Annotation Create(global::CapnProto.Pointer parent) { return (global::CapnProto.Schema.Annotation)parent.Allocate(1, 1); }
        [global::CapnProto.FieldAttribute(0, 0, 64)]
        public ulong id
        {
            get
            {
                return this.ѧ_.GetUInt64(0);
            }
            set
            {
                this.ѧ_.SetUInt64(0, value);
            }
        }
        [global::CapnProto.FieldAttribute(1, pointer: 0)]
        public global::CapnProto.Schema.Value value
        {
            get
            {
                return (global::CapnProto.Schema.Value)this.ѧ_.GetPointer(0);
            }
            set
            {
                this.ѧ_.SetPointer(0, value);
            }
        }
    }
}