using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IWorkableInfluenceMap:IInfluenceMap
    {
        JobHandle Handle { get; }

        void CombineWith(JobHandle handle);

    }
}