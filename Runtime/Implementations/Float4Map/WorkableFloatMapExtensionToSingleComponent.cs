using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.implementations
{
    public static class WorkableColorMapExtensionToSingleComponent
    {
        public static void CopyMap<T, TMap>(ref this T self, Float4Component component, TMap map, JobHandle handle = default)
            where T:struct,IWorkableFloat4Map
            where TMap : struct, IInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyMapAToMapBSingleComponentJob
            {
                MapA = map.Data,
                MapB = self.Data,
                ComponentIndex = (int) component
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddMap<T>(ref this T self, Float4Component component, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {

            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddMapAToMapBSingleComponentJob
            {
                MapA = map.Data,
                MapB = self.Data,
                ComponentIndex = (int) component
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractMap<T>(ref this T self, Float4Component component, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyByMap<T>(ref this T self, Float4Component component, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyByWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SplatMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void LiftMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void LiftAdditivelyMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SplatWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void LiftWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void LiftAdditivelyWeightedMap<T>(ref this T self, Float4Component component, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            self.CombineWith(handle);
        }


    }
    public enum Float4Component
    {
        X,
        Y,
        Z,
        W
    }
}