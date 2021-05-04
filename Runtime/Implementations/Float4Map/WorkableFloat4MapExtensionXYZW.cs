using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps;
using Unity.Jobs;

namespace pl.breams.dotsinfluancemaps.implementations
{
    public static class WorkableFloat4MapExtensionXYZW
    {
        public static void CopyMapXYZW<TSelf, TMapX, TMapY, TMapZ,TMapW>(ref this TSelf self, TMapX mapX, TMapY mapY,
            TMapZ mapZ, TMapW mapW, JobHandle handle = default)
            where TSelf : struct, IWorkableFloat4Map
            where TMapX : struct, IInfluenceMap
            where TMapY : struct, IInfluenceMap
            where TMapZ : struct, IInfluenceMap
            where TMapW : struct, IInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyMapXToXMapYToYToMapZToZMapWToWOfMapDJob
            {
                MapX = mapX.Data,
                MapY = mapY.Data,
                MapZ = mapZ.Data,
                MapW = mapW.Data,
                MapD = self.Data
            }.ScheduleBatch(self.Length, self.Width, handle);
            self.CombineWith(handle);
        }

        public static void WddMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float w = 1, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void WddWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, float w = 1, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float w = 1,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, float w = 1, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float w = 1,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, float weightX, float weightY,
            float weightZ, float a = 1, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void CopyMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void WddMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void WddWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW, float weightX,
            float weightY, float weightZ, float weightW, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            float weightX, float weightY, float weightZ, float weightW, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MultiplyZyWeightedMapXYZW<T>(ref this T self, IInfluenceMap mapX, IInfluenceMap mapY, IInfluenceMap mapZ, IInfluenceMap mapW,
            float weightX, float weightY, float weightZ, float weightW, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }
    }
}