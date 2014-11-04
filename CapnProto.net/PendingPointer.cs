using System;

namespace CapnProto
{
    struct PendingPointer : IEquatable<PendingPointer>, IComparable<PendingPointer>
    {
        private readonly int origin, segment, effectiveOffset, parentSegment, parentOffset;
        private readonly ulong pointer;
        public int Origin { get { return origin; } }

        public int ParentSegment { get { return parentSegment; } }
        public int ParentOffset { get { return parentOffset; } }
        public int Segment { get { return segment; } }
        public int EffectiveOffset { get { return effectiveOffset; } }
        public ulong Pointer { get { return pointer; } }

        public PendingPointer(int parentSegment, int parentOffset, int fromOrigin, ulong pointer)
        {
            this.parentSegment = parentSegment;
            this.parentOffset = parentOffset;
            this.pointer = pointer;
            switch(pointer & PointerType.Mask)
            {
                case PointerType.Struct:
                case PointerType.List:
                    origin = fromOrigin;
                    segment = parentSegment;
                    effectiveOffset = unchecked(fromOrigin + (int)(((uint)pointer) >> 2));
                    break;
                case PointerType.Far:
                    origin = 0;
                    segment = unchecked((int)(pointer >> 32));
                    effectiveOffset = unchecked((int)(((uint)pointer) >> 3));
                    break;
                default:
                    throw new InvalidOperationException("Cannot create pending pointer of this type: " + PointerType.GetName(pointer));
            }
        }
        public PendingPointer(int parentSegment, int parentOffset, int fromOrigin, ulong pointer, ulong pointerOverride)
            : this(parentSegment, parentOffset, fromOrigin, pointer)
        {
            this.pointer = pointerOverride;
        }

        public bool Equals(PendingPointer other)
        {
            return this.segment == other.segment && this.effectiveOffset == other.effectiveOffset;
        }

        public override int GetHashCode()
        {
            return unchecked(-171243 * segment + 16453 * effectiveOffset);
        }
        public override bool Equals(object obj)
        {
            return obj is PendingPointer ? Equals((PendingPointer)obj) : false;
        }

        public int CompareTo(PendingPointer other)
        {
            return this.segment == other.segment ? this.effectiveOffset.CompareTo(other.effectiveOffset) : this.segment.CompareTo(other.segment);
        }
    }
}
