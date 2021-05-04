using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps
{
    public unsafe struct WorkableInfluenceMapProxy
    {
        private readonly void* Pointer;
        public readonly int Length;
        public readonly int Width;
        public readonly int Height;
        public JobHandle JobHandle;

        public WorkableInfluenceMapProxy(IInfluenceMap influenceMap)
        {
            Pointer = influenceMap.Data.GetUnsafeReadOnlyPtr();
            Length = influenceMap.Length;
            Width = influenceMap.Width;
            Height = influenceMap.Height;
            JobHandle = default;
        }

        public WorkableInfluenceMapProxy(IWorkableInfluenceMap workableInfluenceMap)
        {
            Pointer = workableInfluenceMap.Data.GetUnsafePtr();
            Length = workableInfluenceMap.Length;
            Width = workableInfluenceMap.Width;
            Height = workableInfluenceMap.Height;
            JobHandle = workableInfluenceMap.Handle;
        }

        public NativeArray<float> ToArray()
        {
            var nativeArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(Pointer, Length, Allocator.Invalid);
            return nativeArray;
        }
    }
}