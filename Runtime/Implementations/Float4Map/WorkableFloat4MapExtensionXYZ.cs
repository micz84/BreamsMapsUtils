using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.implementations
{
    public static class WorkableFloat4MapExtensionXYZ
    {
        public static void CopyMapXYZ<TSelf,TMapX,TMapY,TMapZ>(ref this TSelf self, TMapX mapX, TMapY mapY, TMapZ mapZ, JobHandle handle = default)
            where TSelf:struct,IWorkableFloat4Map
            where TMapX:struct,IInfluenceMap
            where TMapY:struct,IInfluenceMap
            where TMapZ:struct,IInfluenceMap
        {

            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyMapXToXMapYToYToMapZToZOfMapDJob
            {
                MapX = mapX.Data,
                MapY = mapY.Data,
                MapZ = mapZ.Data,
                MapD = self.Data
            }.ScheduleBatch(self.Length, self.Width, handle);
            self.CombineWith(handle);
        }

        public static void AddMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddMapXToXMapYToYMapZToZOfMapDJob
            {
                MapX = mapX.Data,
                MapY = mapY.Data,
                MapZ = mapZ.Data,
                MapD = self.Data
            }.ScheduleBatch(self.Length, self.Width, handle);
            self.CombineWith(handle);
        }

        public static void AddWeightedMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyWeightedMapXYZ<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }
    }
}