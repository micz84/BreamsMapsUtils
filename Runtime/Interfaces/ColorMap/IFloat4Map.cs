using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IFloat4Map
    {
        bool IsCreated { get; }
        int Width { get; }

        int Height { get; }
        int Length { get; }
        NativeArray<float4> Data { get; }
        void Dispose(JobHandle handle);
    }
}