using System;
using Unity.Collections;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IInfluenceMap:IDisposable
    {
        bool IsCreated { get; }
        int Width { get; }

        int Height { get; }
        int Length { get; }
        NativeArray<float> Data { get; }

        void Dispose(JobHandle handle);
    }
}
