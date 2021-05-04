using System;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IWorkableFloat4Map:IFloat4Map, IDisposable
    {
        JobHandle Handle { get; }

        void CombineWith(JobHandle handle);
    }
}