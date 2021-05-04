using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps.tests
{
    public struct TestMap : IInfluenceMap
    {
        public bool IsCreated => Data.IsCreated;
        public int Width { get; }

        public int Height { get; }
        public int Length { get; }
        public NativeArray<float> Data { get; }

        public TestMap(int width, Allocator allocator = Allocator.TempJob)
        {
            Width = width;
            Height = width;
            Data = new NativeArray<float>(width*width, allocator);
            Length = width * width;
        }

        public void Dispose()
        {
            Data.Dispose();
        }

        public void Dispose(JobHandle handle)
        {
            Data.Dispose(handle);
        }
    }
}