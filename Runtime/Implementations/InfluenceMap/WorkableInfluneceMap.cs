using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps
{
    public struct WorkableInfluneceMap :IWorkableInfluenceMap
    {
        public WorkableInfluneceMap(int width, Allocator allocator) : this(width, new JobHandle(), allocator)
        {
        }

        public WorkableInfluneceMap(int width, JobHandle handle = new JobHandle(), Allocator allocator = Allocator.TempJob)
        {
            Width = width;
            Height = width;
            Length = width * width;
            Data = new NativeArray<float>(Length, allocator);
            Handle = handle;
        }

        public bool IsCreated => Data.IsCreated;
        public int Width { get; }

        public int Height { get; }
        public int Length { get; }
        public NativeArray<float> Data { get; }
        public JobHandle Handle { get; private set; }

        public void Dispose()
        {
            Data.Dispose(Handle);
        }

        public void Dispose(JobHandle handle)
        {
            Handle = JobHandle.CombineDependencies(handle, Handle);
            Data.Dispose(Handle);
        }

        public void CombineWith(JobHandle handle)
        {
            Handle = JobHandle.CombineDependencies(handle, Handle);
        }


    }
}