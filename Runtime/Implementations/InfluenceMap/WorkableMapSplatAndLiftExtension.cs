using pl.breams.dotsinfluancemaps.implementations.Utils;
using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps
{
    public static class WorkableMapSplatAndLiftExtension
    {
        public static void SplatMultipleMaps<T>(ref this T self, NativeArray<InfluenceMapProxy> maps, NativeArray<int2> positions, JobHandle handle = default)
            where T : struct, IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SplatMultipleMaps
            {
                Width = self.Width,
                Height = self.Height,
                InfluenceMapProxies = maps,
                Positions = positions,

                Map = self.Data
            }.Schedule(handle);
            self.CombineWith(handle);
        }
        public static void SplatMultipleMaps<T>(ref this T self, NativeArray<InfluenceMapProxy> maps, int2 globalPosition,NativeArray<int2> positions, JobHandle handle = default)
            where T : struct, IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SplatMultipleMapsGlobalPosition
            {
                Width = self.Width,
                Height = self.Height,
                InfluenceMapProxies = maps,
                Positions = positions,
                GlobalPositionOffset = globalPosition * self.Width,
                Map = self.Data
            }.Schedule(handle);
            self.CombineWith(handle);
        }
        public static void SplatMultipleMaps2<T>(ref this T self, NativeArray<InfluenceMapProxy> maps, NativeArray<int2> positions, JobHandle handle = default)
            where T : struct, IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SplatMultipleMapsGlobalPositionIndexed
            {
                Width = self.Width,
                Height = self.Height,
                InfluenceMapProxies = maps,
                Positions = positions,
                Map = self.Data
            }.Schedule(handle);
            self.CombineWith(handle);
        }
        public static JobHandle SplatMap<T>(ref this T self, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(self.Width, self.Height, map.Width, map.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return handle;
            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new AddMapAToMapBJob
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.ScheduleBatch(map.Length, math.min(2048, self.Length), handle);
            else
                handle = new FitAddMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = smallMapStartIndex,
                    MapBStartIndex = biggerMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth
                }.Schedule(cellsCount, math.min(2048, map.Length), handle);
            self.CombineWith(handle);
            return self.Handle;
        }

        public static void LiftMap<T>(ref this T self, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(map.Width, map.Height, self.Width, self.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return;
            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new CopyMapAToMapBJob()
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.Schedule( handle);
            else
                handle = new FitCopyMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = biggerMapStartIndex,
                    MapBStartIndex = smallMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth
                }.Schedule(cellsCount, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void LiftAdditivelyMap<T>(ref this T self, IInfluenceMap map, int2 center, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(map.Width, map.Height, self.Width, self.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return;
            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new AddMapAToMapBJob
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.ScheduleBatch(map.Length, math.min(2048, self.Length), handle);
            else
                handle = new FitAddMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = biggerMapStartIndex,
                    MapBStartIndex = smallMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth
                }.Schedule(cellsCount, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SplatWeightedMap<T>(ref this T self, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(self.Width,self.Height, map.Width, map.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return;
            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new AddWeightedMapAToMapBJob
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.ScheduleBatch(map.Length, math.min(2048, self.Length), handle);
            else
                handle = new FitAddWeightedMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = smallMapStartIndex,
                    MapBStartIndex = biggerMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth,
                    Weight = weight
                }.Schedule(cellsCount, math.min(2048, map.Length), handle);
            self.CombineWith(handle);
        }

        public static void LiftWeightedMap<T>(ref this T self, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(map.Width, map.Height, self.Width, self.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return;

            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new CopyWeightedMapAToMapBJob
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.ScheduleBatch(map.Length, math.min(2048, self.Length), handle);
            else
                handle = new FitCopyWeightedMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = biggerMapStartIndex,
                    MapBStartIndex = smallMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth,
                    Weight = weight
                }.Schedule(cellsCount, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void LiftAdditivelyWeightedMap<T>(ref this T self, IInfluenceMap map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(map.Width, map.Height, self.Width, self.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
                out var workingWidth))
                return;

            if (smallMapStartIndex == 0 && biggerMapStartIndex == 0 && map.Height == self.Height && map.Width == self.Width)
                handle = new AddWeightedMapAToMapBJob
                {
                    MapA = map.Data,
                    MapB = self.Data,
                }.ScheduleBatch(map.Length, math.min(2048, self.Length), handle);
            else
                handle = new FitAddWeightedMapAToMapB
                {
                    MapA = map.Data,
                    MapB = self.Data,
                    MapAStartIndex = biggerMapStartIndex,
                    MapBStartIndex = smallMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth,
                    Weight = weight
                }.Schedule(cellsCount, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }
    }
}