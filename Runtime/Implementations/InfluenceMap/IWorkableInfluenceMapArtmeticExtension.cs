using System;
using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps
{
    public static class IWorkableInfluenceMapArtmeticExtension
    {
        public static void CopyMap<T, TMap>(ref this T self, TMap map, JobHandle handle = default)
            where T:struct,IWorkableInfluenceMap
            where TMap:struct,IInfluenceMap
        {
            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data
            }.Schedule(handle);
            self.CombineWith(handle);
        }

        public static void CopyWeightedMap<T>(ref this T self, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyWeightedMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddMap<T>(ref this T self, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddWeightedMap<T>(ref this T self, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {

            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddWeightedMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SubtractMap<T>(ref this T self, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {

            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SubtractMapAFromMapBJob
            {
                MapA = map.Data,
                MapB = self.Data
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMap<T>(ref this T self, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {

            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SubtractWeightedMapAFromMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByMap<T>(ref this T self, IInfluenceMap map, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyMapAByMapBJob
            {
                MapA = map.Data,
                MapB = self.Data
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByWeightedMap<T>(ref this T self, IInfluenceMap map, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            if (map.Width != self.Width)
                throw new NotSupportedException("Maps must be of same size to copy");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyWeightedMapAByMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByWeight<T>(ref this T self, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyMapByWeightJob
            {
                MapA = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }


    }
}