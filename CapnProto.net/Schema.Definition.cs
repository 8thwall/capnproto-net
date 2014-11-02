
using System.ComponentModel;
namespace CapnProto
{
    [Id(0xa93fc509624c72d9)]
    static partial class Schema
    {
        //# schema.capnp
        //@0xa93fc509624c72d9;
        //[module: Id(0xa93fc509624c72d9)]        
        //$import "/capnp/c++.capnp".namespace("capnp::schema");

        //struct Node @0xe682ab4cf923a417 {  # 40 bytes, 5 ptrs
        [Id(0xe682ab4cf923a417)]
        public partial class Node
        {
            //  id @0 :UInt64;  # bits[0, 64)
            [Field(0, 0, 64)]
            public ulong id { get; set; }

            //  displayName @1 :Text;  # ptr[0]
            [Field(1, pointer: 0)]
            public string displayName { get; set; }

            //  displayNamePrefixLength @2 :UInt32;  # bits[64, 96)
            [Field(2, 64, 96)]
            public uint displayNamePrefixLength { get; set; }

            //  scopeId @3 :UInt64;  # bits[128, 192)
            [Field(3, 128, 192)]
            public ulong scopeId { get; set; }

            //  nestedNodes @4 :List(NestedNode);  # ptr[1]
            [Field(4, pointer: 1)]
            public global::System.Collections.Generic.List<NestedNode> nestedNodes { get; set; }

            //  annotations @5 :List(Annotation);  # ptr[2]
            [Field(5, pointer: 2)]
            public global::System.Collections.Generic.List<Annotation> annotations { get; set; }


            //  union {  # tag bits [96, 112)
            //    file @6 :Void;  # bits[0, 0), union tag = 0
            [Field(6, 0, 0), Union(0, 96, 112)]
            public Void file { get; set; }

            //    struct :group {  # union tag = 1
            [Union(1)]
            public structGroup @struct { get; set; }

            [Group, Id(11430331134483579957)]
            public partial class structGroup
            {
                //      dataWordCount @7 :UInt16;  # bits[112, 128)
                [Field(7, 112, 128)]
                public ushort dataWordCount { get; set; }

                //      pointerCount @8 :UInt16;  # bits[192, 208)
                [Field(8, 192, 208)]
                public ushort pointerCount { get; set; }

                //      preferredListEncoding @9 :ElementSize;  # bits[208, 224)
                [Field(9, 208, 224)]
                public ElementSize preferredListEncoding { get; set; }

                //      isGroup @10 :Bool;  # bits[224, 225)
                [Field(10, 224, 225)]
                public bool isGroup { get; set; }

                //      discriminantCount @11 :UInt16;  # bits[240, 256)
                [Field(11, 240, 256)]
                public ushort discriminantCount { get; set; }

                //      discriminantOffset @12 :UInt32;  # bits[256, 288)
                [Field(12, 256, 288)]
                public uint discriminantOffset { get; set; }

                //      fields @13 :List(Field);  # ptr[3]
                [Field(13, pointer: 3)]
                public global::System.Collections.Generic.List<Field> fields { get; set; }
                //    }
            }

            //    enum :group {  # union tag = 2
            [Union(2)]
            public enumGroup @enum { get; set; }

            [Group, Id(13063450714778629528)]
            public partial class enumGroup
            {
                //      enumerants @14 :List(Enumerant);  # ptr[3]
                [Field(14, pointer: 3)]
                public global::System.Collections.Generic.List<Enumerant> enumerants { get; set; }
                //    }
            }

            //    interface :group {  # union tag = 3
            [Union(3)]
            public interfaceGroup @interface { get; set; }

            [Group, Id(17116997365232503999)]
            public partial class interfaceGroup
            {
                //      methods @15 :List(Method);  # ptr[3]
                [Field(15, pointer: 3)]
                public global::System.Collections.Generic.List<Method> methods { get; set; }

                //      extends @31 :List(UInt64);  # ptr[4]
                [Field(31, pointer: 4)]
                public global::System.Collections.Generic.List<long> extends { get; set; }
                //    }
            }

            //    const :group {  # union tag = 4
            [Union(4)]
            public constGroup @const { get; set; }

            [Group, Id(12793219851699983392)]
            public partial class constGroup
            {
                //      type @16 :Type;  # ptr[3]
                [Field(16, pointer: 3)]
                public Type type { get; set; }

                //      value @17 :Value;  # ptr[4]
                [Field(17, pointer: 4)]
                public Value value { get; set; }
                //    }
            }

            //    annotation :group {  # union tag = 5
            [Union(5)]
            public annotationGroup annotation { get; set; }

            [Group, Id(17011813041836786320)]
            public partial class annotationGroup
            {
                //      type @18 :Type;  # ptr[3]
                [Field(17, pointer: 3)]
                public Type type { get; set; }
                //      targetsFile @19 :Bool;  # bits[112, 113)
                [Field(19, 112, 113)]
                public bool targetsFile { get; set; }
                //      targetsConst @20 :Bool;  # bits[113, 114)
                [Field(20, 113, 114)]
                public bool targetsConst { get; set; }
                //      targetsEnum @21 :Bool;  # bits[114, 115)
                [Field(21, 114, 115)]
                public bool targetsEnum { get; set; }
                //      targetsEnumerant @22 :Bool;  # bits[115, 116)
                [Field(22, 115, 116)]
                public bool targetsEnumerant { get; set; }
                //      targetsStruct @23 :Bool;  # bits[116, 117)
                [Field(23, 116, 117)]
                public bool targetsStruct { get; set; }
                //      targetsField @24 :Bool;  # bits[117, 118)
                [Field(24, 117, 118)]
                public bool targetsField { get; set; }
                //      targetsUnion @25 :Bool;  # bits[118, 119)
                [Field(25, 118, 119)]
                public bool targetsUnion { get; set; }
                //      targetsGroup @26 :Bool;  # bits[119, 120)
                [Field(26, 119, 120)]
                public bool targetsGroup { get; set; }
                //      targetsInterface @27 :Bool;  # bits[120, 121)
                [Field(27, 120, 121)]
                public bool targetsInterface { get; set; }
                //      targetsMethod @28 :Bool;  # bits[121, 122)
                [Field(28, 121, 122)]
                public bool targetsMethod { get; set; }
                //      targetsParam @29 :Bool;  # bits[122, 123)
                [Field(29, 122, 123)]
                public bool targetsParam { get; set; }
                //      targetsAnnotation @30 :Bool;  # bits[123, 124)
                [Field(30, 123, 124)]
                public bool targetsAnnotation { get; set; }
                //    }
            }

            //  }
        }

        //  struct NestedNode @0xdebf55bbfa0fc242 {  # 8 bytes, 1 ptrs
        [Id(0xdebf55bbfa0fc242)]
        public partial class NestedNode
        {
            //    name @0 :Text;  # ptr[0]
            [Field(0, pointer: 0)]
            public string name { get; set; }
            //    id @1 :UInt64;  # bits[0, 64)
            [Field(1, 0, 64)]
            public ulong id { get; set; }
            //  }
        }
        //struct Field @0x9aad50a41f4af45f {  # 24 bytes, 4 ptrs
        [Id(0x9aad50a41f4af45f)]
        public partial class Field
        {
            //  name @0 :Text;  # ptr[0]
            [Field(0, pointer: 0)]
            public string name { get; set; }

            //  codeOrder @1 :UInt16;  # bits[0, 16)
            [Field(1, 0, 16)]
            public ushort codeOrder { get; set; }

            //  annotations @2 :List(Annotation);  # ptr[1]
            [Field(2, pointer: 1)]
            public global::System.Collections.Generic.List<Annotation> annotations { get; set; }

            //  discriminantValue @3 :UInt16 = 65535;  # bits[16, 32)
            [Field(3, 16, 32), DefaultValue(65535)]
            public ushort discriminantValue { get; set; }


            //  union {  # tag bits [64, 80)
            //    slot :group {  # union tag = 0
            [Union(0, 64, 80)]
            public slotGroup slot { get; set; }

            [Group, Id(14133145859926553711)]
            public class slotGroup
            {
                //      offset @4 :UInt32;  # bits[32, 64)
                [Field(4,32,64)]
                public uint offset { get; set; }
                //      type @5 :Type;  # ptr[2]
                [Field(5, pointer: 2)]
                public Type type {get;set;}
                //      defaultValue @6 :Value;  # ptr[3]
                [Field(6, pointer: 3)]
                public Value defaultValue { get; set; }

                //      hadExplicitDefault @10 :Bool;  # bits[128, 129)
                [Field(10, 128, 129)]
                public bool hadExplicitDefault { get; set; }
                
                //    }
            }
            

           
            //    group :group {  # union tag = 1
            [Union(1)]
            public groupGroup group { get; set; }

            [Group, Id(14626792032033250577)]
            public class groupGroup
            {
                //      typeId @7 :UInt64;  # bits[128, 192)
                [Field(7, 128, 192)]
                public ulong typeId { get; set; }
                //    }
            }


            public ordinalGroup ordinal { get; set; }

            //  ordinal :group {
            [Group, Id(13515537513213004774)]
            public class ordinalGroup
            {
                //    union {  # tag bits [80, 96)
                //      implicit @8 :Void;  # bits[0, 0), union tag = 0
                [Field(8, 0, 0), Union(0, 80, 96)]
                public Void @implicit { get; set; }

                //      explicit @9 :UInt16;  # bits[96, 112), union tag = 1
                [Field(9, 96, 112), Union(1)]
                public ushort @explicit { get; set; }

                //  }
            }
            



            //  const noDiscriminant @0x97b14cbe7cfec712 :UInt16 = 65535;
            [Id(0x97b14cbe7cfec712)]
            public const ushort noDiscriminant = 65535;
            //}
        }

        //struct Enumerant @0x978a7cebdc549a4d {  # 8 bytes, 2 ptrs
        [Id(0x978a7cebdc549a4d)]
        public partial class Enumerant
        {
            //  name @0 :Text;  # ptr[0]
            [Field(0, pointer: 0)]
            public string name { get; set; }
            //  codeOrder @1 :UInt16;  # bits[0, 16)
            [Field(1, 0, 16)]
            public ushort codeOrder { get; set; }

            //  annotations @2 :List(Annotation);  # ptr[1]
            [Field(2, pointer: 1)]
            public global::System.Collections.Generic.List<Annotation> annotations = new global::System.Collections.Generic.List<Annotation>();
            
            //}
        }
        //struct Method @0x9500cce23b334d80 {  # 24 bytes, 2 ptrs
        [Id(0x9500cce23b334d80)]
        public partial class Method
        {
            //  name @0 :Text;  # ptr[0]
            [Field(0, pointer: 0)]
            public string name {get;set;}
            //  codeOrder @1 :UInt16;  # bits[0, 16)
            [Field(1, 0, 16)]
            public ushort codeOrder { get; set; }
            //  paramStructType @2 :UInt64;  # bits[64, 128)
            [Field(2, 64,128)]
            public ulong paramStructType { get; set; }
            //  resultStructType @3 :UInt64;  # bits[128, 192)
            [Field(3, 128,192)]
            public ulong resultStructType {get;set;}
            //  annotations @4 :List(Annotation);  # ptr[1]
            //}
        }
        //struct Type @0xd07378ede1f9cc60 {  # 16 bytes, 1 ptrs
        [Id(0xd07378ede1f9cc60)]
        public partial class Type
        {
            //  union {  # tag bits [0, 16)
            //    void @0 :Void;  # bits[0, 0), union tag = 0
            [Field(0, 0, 0), Union(0, 0, 16)]
            public Void @void { get; set; }

            //    bool @1 :Void;  # bits[0, 0), union tag = 1
            [Field(1, 0, 0), Union(1)]
            public Void @bool { get; set; }
            //    int8 @2 :Void;  # bits[0, 0), union tag = 2
            [Field(2, 0, 0), Union(2)]
            public Void int8 { get; set; }
            //    int16 @3 :Void;  # bits[0, 0), union tag = 3
            [Field(3, 0, 0), Union(3)]
            public Void int16 { get; set; }
            //    int32 @4 :Void;  # bits[0, 0), union tag = 4
            [Field(4, 0, 0), Union(4)]
            public Void int32 { get; set; }
            //    int64 @5 :Void;  # bits[0, 0), union tag = 5
            [Field(5, 0, 0), Union(5)]
            public Void int64 { get; set; }
            //    uint8 @6 :Void;  # bits[0, 0), union tag = 6
            [Field(6, 0, 0), Union(6)]
            public Void uint8 { get; set; }
            //    uint16 @7 :Void;  # bits[0, 0), union tag = 7
            [Field(7, 0, 0), Union(7)]
            public Void uint16 { get; set; }
            //    uint32 @8 :Void;  # bits[0, 0), union tag = 8
            [Field(8, 0, 0), Union(8)]
            public Void uint32 { get; set; }
            //    uint64 @9 :Void;  # bits[0, 0), union tag = 9
            [Field(9, 0, 0), Union(9)]
            public Void uint64 { get; set; }
            //    float32 @10 :Void;  # bits[0, 0), union tag = 10
            [Field(10, 0, 0), Union(10)]
            public Void float32 { get; set; }
            //    float64 @11 :Void;  # bits[0, 0), union tag = 11
            [Field(11, 0, 0), Union(11)]
            public Void float64 { get; set; }
            //    text @12 :Void;  # bits[0, 0), union tag = 12
            [Field(12, 0, 0), Union(12)]
            public Void text { get; set; }
            //    data @13 :Void;  # bits[0, 0), union tag = 13
            [Field(13, 0, 0), Union(13)]
            public Void data { get; set; }

            //    list :group {  # union tag = 14
            [Union(14)]
            public listGroup list { get; set; }

            [Group, Id(9792858745991129751)]
            public partial class listGroup
            {
                //      elementType @14 :Type;  # ptr[0]
                [Field(14, pointer: 0)]
                public Type elementType { get; set; }
                //    }
            }
            
            //    enum :group {  # union tag = 15
            [Union(15)]
            public enumGroup @enum { get;set;}

            [Group, Id(13063450714778629527)]
            public class enumGroup {
                //      typeId @15 :UInt64;  # bits[64, 128)
                [Field(15, 64,128)]
                public ulong typeId {get;set;}
                //    }
            }

            //    struct :group {  # union tag = 16
            [Union(16)]
            public structGroup @struct { get; set; }

            [Group, Id(12410354185295152851)]
            public class structGroup
            {
                //      typeId @16 :UInt64;  # bits[64, 128)
                [Field(16, 64, 128)]
                public ulong typeId { get; set; }
                //    }
            }

            //    interface :group {  # union tag = 17
            [Union(17)]
            public interfaceGroup @interface { get; set; }

            [Group, Id(16728431493453586831)]
            public class interfaceGroup
            {
                //      typeId @17 :UInt64;  # bits[64, 128)
                [Field(17, 64,128)]
                public ulong typeId { get; set; }
                //    }
            }

            //    anyPointer @18 :Void;  # bits[0, 0), union tag = 18
            [Field(18, 0, 0), Union(18)]
            public Void anyPointer { get; set; }
            
            //  }
            //}
        }
        //struct Value @0xce23dcd2d7b00c9b {  # 16 bytes, 1 ptrs
        [Id(0xce23dcd2d7b00c9b)]
        public partial class Value
        {
            //  union {  # tag bits [0, 16)
            //    void @0 :Void;  # bits[0, 0), union tag = 0
            [Union(0, 0, 16)]
            public Void @void {get;set;}
            
            //    bool @1 :Bool;  # bits[16, 17), union tag = 1
            [Field(1, 16, 17), Union(1)]
            public bool @bool { get; set; }

            //    int8 @2 :Int8;  # bits[16, 24), union tag = 2
            [Field(2, 16,24), Union(2)]
            public sbyte int8 { get; set; }
            //    int16 @3 :Int16;  # bits[16, 32), union tag = 3
            [Field(3, 16, 32), Union(3)]
            public short int16 { get; set; }

            //    int32 @4 :Int32;  # bits[32, 64), union tag = 4
            [Field(4, 32, 64), Union(4)]
            public int int32 { get; set; }

            //    int64 @5 :Int64;  # bits[64, 128), union tag = 5
            [Field(5, 64, 128), Union(5)]
            public long int64 { get; set; }

            //    uint8 @6 :UInt8;  # bits[16, 24), union tag = 6
            [Field(5, 16,24), Union(6)]
            public byte uint8 { get; set; }

            //    uint16 @7 :UInt16;  # bits[16, 32), union tag = 7
            [Field(7, 16,32), Union(7)]
            public ushort uint16 { get; set; }

            //    uint32 @8 :UInt32;  # bits[32, 64), union tag = 8
            [Field(8, 32, 64), Union(8)]
            public uint uint32 { get; set; }

            //    uint64 @9 :UInt64;  # bits[64, 128), union tag = 9
            [Field(9, 64, 128), Union(9)]
            public ulong uint64 { get; set; }

            //    float32 @10 :Float32;  # bits[32, 64), union tag = 10
            [Field(10, 32, 64), Union(10)]
            public float float32 { get; set; }

            //    float64 @11 :Float64;  # bits[64, 128), union tag = 11
            [Field(11, 64, 128), Union(11)]
            public double float64 { get; set; }

            //    text @12 :Text;  # ptr[0], union tag = 12
            [Field(12, pointer: 0), Union(12)]
            public string Text { get; set; }
            
            //    data @13 :Data;  # ptr[0], union tag = 13
            [Field(13, pointer: 0), Union(13)]
            public byte[] data { get; set; }
            //    list @14 :AnyPointer;  # ptr[0], union tag = 14
            [Field(14, pointer: 0), Union(14)]
            public global::System.Collections.IList list { get; set; }
            //    enum @15 :UInt16;  # bits[16, 32), union tag = 15
            [Field(15, 16, 32), Union(15)]
            public ushort @enum { get; set; }
            //    struct @16 :AnyPointer;  # ptr[0], union tag = 16
            [Field(16, pointer: 0), Union(16)]
            public object @struct { get; set; }

            //    interface @17 :Void;  # bits[0, 0), union tag = 17
            [Field(17, 0, 0), Union(17)]
            public Void @interface { get; set; }
            //    anyPointer @18 :AnyPointer;  # ptr[0], union tag = 18
            [Field(18, pointer: 0), Union(18)]
            public object anyPointer { get; set; }

            //  }
            //}
        }
        //struct Annotation @0xf1c8950dab257542 {  # 8 bytes, 1 ptrs
        [Id(0xf1c8950dab257542)]
        public partial class Annotation
        {
            //  id @0 :UInt64;  # bits[0, 64)
            [Field(0, 0, 64)]
            public ulong id { get; set; }
            //  value @1 :Value;  # ptr[0]
            [Field(1, pointer: 1)]
            public Value value { get; set; }
            //}
        }
        //enum ElementSize @0xd1958f7dba521926 {
        [Id(0xd1958f7dba521926)]
        public enum ElementSize
        {
            //  empty @0;
            [Field(0)]
            empty,
            //  bit @1;
            [Field(1)]
            bit,
            //  byte @2;
            [Field(2)]
            @byte,
            //  twoBytes @3;
            [Field(3)]
            twoBytes,
            //  fourBytes @4;
            [Field(4)]
            fourBytes,
            //  eightBytes @5;
            [Field(5)]
            eightBytes,
            //  pointer @6;
            [Field(6)]
            pointer,
            //  inlineComposite @7;
            [Field(7)]
            inlineComposite,
            //}
        }
        //struct CodeGeneratorRequest @0xbfc546f6210ad7ce {  # 0 bytes, 2 ptrs
        [Id(0xbfc546f6210ad7ce)]
        public partial class CodeGeneratorRequest
        {
            //  nodes @0 :List(Node);  # ptr[0]
            [Field(0, pointer: 0)]
            public global::System.Collections.Generic.List<Node> nodes { get; set; }

            //  requestedFiles @1 :List(RequestedFile);  # ptr[1]
            [Field(1, pointer: 1)]
            public global::System.Collections.Generic.List<RequestedFile> requestedFiles { get; set; }

            //  struct RequestedFile @0xcfea0eb02e810062 {  # 8 bytes, 2 ptrs
            [Id(0xcfea0eb02e810062)]
            public partial class RequestedFile
            {
                //    id @0 :UInt64;  # bits[0, 64)
                [Field(0, 0, 64)]
                public ulong id { get; set; }

                //    filename @1 :Text;  # ptr[0]
                [Field(1, pointer: 0)]
                public string filename { get; set; }

                //    imports @2 :List(Import);  # ptr[1]
                [Field(2, pointer: 1)]
                public global::System.Collections.Generic.List<Import> imports { get; set; }

                //    struct Import @0xae504193122357e5 {  # 8 bytes, 1 ptrs
                [Id(0xae504193122357e5)]
                public partial class Import
                {
                    //      id @0 :UInt64;  # bits[0, 64)
                    [Field(0, 0, 64)]
                    public uint id { get; set; }

                    //      name @1 :Text;  # ptr[0]
                    [Field(1, pointer: 0)]
                    public string name { get; set; }
                    //  }
                }
                //}
            }
            //}
        }
    }
}
