//namespace CapnProto
//{
//    internal static class PointerType
//    {
//        public const ulong
//            Struct = 0,
//            List = 1,
//            Far = 2,
//            Other = 3,
//            Mask = 3;

//        public static string GetName(ulong pointer)
//        {
//            switch (pointer & PointerType.Mask)
//            {
//                case PointerType.Struct: return "struct";
//                case PointerType.List: return "list";
//                case PointerType.Far: return "far";
//                case PointerType.Other: return "other";
//                default: return "unknown";
//            }
//        }
//    }
//}
