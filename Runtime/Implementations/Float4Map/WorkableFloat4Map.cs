using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.implementations
{
    public struct WorkableFloat4Map : IWorkableFloat4Map
    {
        public WorkableFloat4Map(int width, Allocator allocator) : this(width, new JobHandle(), allocator) { }

        public WorkableFloat4Map(int width, JobHandle handle = new JobHandle(), Allocator allocator = Allocator.TempJob)
        {
            Width = width;
            Height = width;
            Length = width * width;
            Data = new NativeArray<float4>(Length, allocator);
            Handle = handle;
        }

        public bool IsCreated => Data.IsCreated;
        public int Width { get; }
        public int Height { get; }
        public int Length { get; }
        public NativeArray<float4> Data { get; }
        public JobHandle Handle { get; private set; }

        public void Dispose(JobHandle handle) => Data.Dispose(JobHandle.CombineDependencies(handle, Handle));

        public void Dispose() => Data.Dispose(Handle);
        public void CombineWith(JobHandle handle) => Handle = JobHandle.CombineDependencies(Handle, handle);
    }
}