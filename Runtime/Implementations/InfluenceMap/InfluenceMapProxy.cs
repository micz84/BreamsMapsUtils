using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections.LowLevel.Unsafe;

namespace pl.breams.dotsinfluancemaps
{
    public unsafe struct InfluenceMapProxy
    {
        private readonly void* _ReadOnlyPointer;
        private readonly void* _Pointer;
        public readonly int Length;
        public readonly int Width;
        public readonly int Height;

        public float this [int index]
        {
            get => UnsafeUtility.ReadArrayElement<float>(_ReadOnlyPointer, index);
            set => UnsafeUtility.WriteArrayElement(_Pointer, index, value);
        }

        public InfluenceMapProxy(IInfluenceMap influenceMap)
        {
            _ReadOnlyPointer = influenceMap.Data.GetUnsafeReadOnlyPtr();
            _Pointer = influenceMap.Data.GetUnsafePtr();
            Length = influenceMap.Length;
            Width = influenceMap.Width;
            Height = influenceMap.Height;
        }

        /*public NativeArray<float> ToArray()
        {
            var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(Pointer, Length, Allocator.Invalid);
            return nativeArray;
        }*/
    }
}