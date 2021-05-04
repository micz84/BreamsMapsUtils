using System;
using pl.breams.dotsinfluancemaps.implementations.Utils;
using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.implementations
{
    public static class WorkableColorMapArtmetics
    {
        public static void CopyMap<T>(ref this T self, IFloat4Map map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data
            }.Schedule(handle);
            self.CombineWith(handle);
        }

        public static void CopyWeightedMap<T>(ref this T self, IFloat4Map map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new CopyWeightedMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddMap<T>(ref this T self, IFloat4Map map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void AddWeightedMap<T>(ref this T self, IFloat4Map map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new AddWeightedMapAToMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SubtractMap<T>(ref this T self, IFloat4Map map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SubtractMapAFromMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SubtractWeightedMap<T>(ref this T self, IFloat4Map map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SubtractWeightedMapAFromMapBJob()
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByWeight<T>(ref this T self, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyMapByWeightJob
            {
                MapA = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByMap<T>(ref this T self, IFloat4Map map, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyMapAByMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void MultiplyByWeightedMap<T>(ref this T self, IFloat4Map map, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MultiplyWeightedMapAByMapBJob
            {
                MapA = map.Data,
                MapB = self.Data,
                AWeight = weight
            }.ScheduleBatch(self.Length, math.min(2048, self.Length), handle);
            self.CombineWith(handle);
        }

        public static void SplatMap<T>(ref this T self, IFloat4Map map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {

            handle = JobHandle.CombineDependencies(handle, self.Handle);
            if (!MapUtils.GetJobParams(self.Width, self.Height, map.Width, map.Height, center, out var cellsCount, out var smallMapStartIndex, out var biggerMapStartIndex,
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
                    MapAStartIndex = smallMapStartIndex,
                    MapBStartIndex = biggerMapStartIndex,
                    MapATotalWidth = map.Width,
                    MapBTotalWidth = self.Width,
                    WorkingWidth = workingWidth
                }.Schedule(cellsCount, math.min(2048, map.Length), handle);
            self.CombineWith(handle);
            self.CombineWith(handle);
        }

        public static void LiftMap<T>(ref this T self, IFloat4Map map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
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

        public static void LiftAdditivelyMap<T>(ref this T self, IFloat4Map map, int2 center, JobHandle handle = default) where T:struct,IWorkableFloat4Map
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
            self.CombineWith(handle);
        }

        public static void SplatWeightedMap<T>(ref this T self, IFloat4Map map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
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

        public static void LiftWeightedMap<T>(ref this T self, IFloat4Map map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
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

        public static void LiftAdditivelyWeightedMap<T>(ref this T self, IFloat4Map map, int2 center, float weight, JobHandle handle = default) where T:struct,IWorkableFloat4Map
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
        public static void Sample<T>(ref this T self, NativeArray<int2> positions, NativeArray<float4> values, JobHandle handle = default, int startIndex = 0) where T:struct,IWorkableFloat4Map
        {
            if (positions.Length > values.Length - startIndex)
                throw new NotSupportedException("Positions array must be same size or smaller then values array");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SampleDiscreet
            {
                Width = self.Width,
                ValuesStartIndex = startIndex,
                Map = self.Data,
                Positions = positions,
                Values = values
            }.Schedule(positions.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void Sample<T>(ref this T self, NativeArray<float2> positions, NativeArray<float4> values, JobHandle handle = default , int startIndex = 0) where T:struct,IWorkableFloat4Map
        {
            if (positions.Length > values.Length - startIndex)
                throw new NotSupportedException("Positions array must be same size or smaller then values array");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new SampleLerp
            {
                Width = self.Width,
                ValuesStartIndex = startIndex,
                Map = self.Data,
                Positions = positions,
                Values = values
            }.Schedule(positions.Length, 64, handle);
            self.CombineWith(handle);
        }
        public static void CombineFromMaps<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, IFloat4Map mapC, IFloat4Map mapD, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width * 2)
                throw new NotSupportedException($"{nameof(self.Width)} must be exactly 2x larger then {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Width != mapB.Width * 2)
                throw new NotSupportedException($"{nameof(self.Width)} must be exactly 2x larger then {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Width != mapC.Width * 2)
                throw new NotSupportedException($"{nameof(self.Width)} must be exactly 2x larger then {nameof(mapC)} {nameof(mapC.Width)}. ");
            if(self.Width != mapD.Width * 2)
                throw new NotSupportedException($"{nameof(self.Width)} must be exactly 2x larger then {nameof(mapD)} {nameof(mapD.Width)}. ");
            if(self.Height != mapA.Height * 2)
                throw new NotSupportedException($"{nameof(self.Height)} must be exactly 2x larger then {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Height != mapB.Height * 2)
                throw new NotSupportedException($"{nameof(self.Height)} must be exactly 2x larger then {nameof(mapB)} {nameof(mapB.Height)}. ");
            if(self.Height != mapC.Height * 2)
                throw new NotSupportedException($"{nameof(self.Height)} must be exactly 2x larger then {nameof(mapC)} {nameof(mapC.Height)}. ");
            if(self.Height != mapD.Height * 2)
                throw new NotSupportedException($"{nameof(self.Height)} must be exactly 2x larger then {nameof(mapD)} {nameof(mapD.Height)}. ");
            handle = JobHandle.CombineDependencies(handle,self.Handle);

            var widthQuoter = self.Width / 4;
            var heightQuoter = self.Height / 4;
            var centerA = new int2(widthQuoter, heightQuoter);
            var centerB = new int2(widthQuoter * 3, heightQuoter);
            var centerC = new int2(widthQuoter, heightQuoter * 3);
            var centerD = new int2(widthQuoter * 3, heightQuoter * 3);

            // TODO: Maybe it is worth not to use FitMapAToMapB
            if (!MapUtils.GetJobParams(self.Width, self.Height, mapA.Width, mapA.Height, centerA, out var cellsCountA, out var smallMapStartIndexA, out var biggerMapStartIndexA,
                out var workingWidthA))
                return;
            if (!MapUtils.GetJobParams(self.Width, self.Height, mapB.Width, mapB.Height, centerB, out var cellsCountB, out var smallMapStartIndexB, out var biggerMapStartIndexB,
                out var workingWidthB))
                return;
            if (!MapUtils.GetJobParams(self.Width, self.Height, mapC.Width, mapC.Height, centerC, out var cellsCountC, out var smallMapStartIndexC, out var biggerMapStartIndexC,
                out var workingWidthC))
                return;
            if (!MapUtils.GetJobParams(self.Width, self.Height, mapD.Width, mapD.Height, centerD, out var cellsCountD, out var smallMapStartIndexD, out var biggerMapStartIndexD,
                out var workingWidthD))
                return;
            var mapAHandle = new FitAddMapAToMapB
            {
                MapA = mapA.Data,
                MapB = self.Data,
                MapAStartIndex = smallMapStartIndexA,
                MapBStartIndex = biggerMapStartIndexA,
                MapATotalWidth = mapA.Width,
                MapBTotalWidth = self.Width,
                WorkingWidth = workingWidthA
            }.Schedule(cellsCountA, math.min(2048, mapA.Length), handle);
            var mapBHandle = new FitAddMapAToMapB
            {
                MapA = mapB.Data,
                MapB = self.Data,
                MapAStartIndex = smallMapStartIndexB,
                MapBStartIndex = biggerMapStartIndexB,
                MapATotalWidth = mapB.Width,
                MapBTotalWidth = self.Width,
                WorkingWidth = workingWidthB
            }.Schedule(cellsCountB, math.min(2048, mapB.Length), handle);
            var mapCHandle = new FitAddMapAToMapB
            {
                MapA = mapC.Data,
                MapB = self.Data,
                MapAStartIndex = smallMapStartIndexC,
                MapBStartIndex = biggerMapStartIndexC,
                MapATotalWidth = mapC.Width,
                MapBTotalWidth = self.Width,
                WorkingWidth = workingWidthC
            }.Schedule(cellsCountC, math.min(2048, mapC.Length), handle);
            var mapDHandle = new FitAddMapAToMapB
            {
                MapA = mapD.Data,
                MapB = self.Data,
                MapAStartIndex = smallMapStartIndexD,
                MapBStartIndex = biggerMapStartIndexD,
                MapATotalWidth = mapD.Width,
                MapBTotalWidth = self.Width,
                WorkingWidth = workingWidthD
            }.Schedule(cellsCountD, math.min(2048, mapD.Length), handle);
            handle = JobHandle.CombineDependencies(mapAHandle, mapBHandle);
            handle = JobHandle.CombineDependencies(mapCHandle, mapDHandle, handle);
            self.CombineWith(handle);
        }

        public static void CombineDownResFromMaps<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, IFloat4Map mapC, IFloat4Map mapD, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void DownResFromMap<T>(ref this T self, IFloat4Map mapA, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void MinWith<T>(ref this T self, IFloat4Map mapA, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}.");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}.");
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            handle = new MinBetweenTwoMaps
            {
                MapA = mapA.Data,
                MapB = self.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void MaxWith<T>(ref this T self, IFloat4Map mapA, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            handle = new MaxBetweenTwoMaps
            {
                MapA = mapA.Data,
                MapB = self.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void MinFrom<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Width != mapB.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Height != mapB.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapB)} {nameof(mapB.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            handle = new MinBetweenTwoMapsNonDestructive
            {
                MapA = mapA.Data,
                MapB = mapB.Data,
                MapC = self.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void MaxFrom<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Width != mapB.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Height != mapB.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapB)} {nameof(mapB.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new MaxBetweenTwoMapsNonDestructive
            {
                MapA = mapA.Data,
                MapB = mapB.Data,
                MapC = self.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void Lerp<T>(ref this T self, IFloat4Map mapA, float factor, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            handle = new LerpBetweenTwoMapsFactor
            {
                MapA = mapA.Data,
                MapB = self.Data,
                Factor = factor
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void Lerp<T>(ref this T self, IFloat4Map mapA, IInfluenceMap mapF, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Width != mapF.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapF)} {nameof(mapF.Width)}. ");
            if(self.Height != mapF.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapF)} {nameof(mapF.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new LerpBetweenTwoMapsMapFactor
            {
                MapA = mapA.Data,
                MapB = self.Data,
                MapF = mapF.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void Lerp<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, float factor, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Width != mapB.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Height != mapB.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapB)} {nameof(mapB.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new LerpBetweenTwoMapsNonDestructiveFactor
            {
                MapA = mapA.Data,
                MapB = mapB.Data,
                MapC = self.Data,
                Factor = factor
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }

        public static void Lerp<T>(ref this T self, IFloat4Map mapA, IFloat4Map mapB, IInfluenceMap mapF, JobHandle handle = default) where T:struct,IWorkableFloat4Map
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Width != mapB.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Height != mapB.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapB)} {nameof(mapB.Height)}. ");
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapA.Width)}. ");
            if(self.Height != mapF.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapF)} {nameof(mapF.Height)}. ");
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            handle = new LerpBetweenTwoMapsNonDestructiveMapFactor
            {
                MapA = mapA.Data,
                MapB = mapB.Data,
                MapC = self.Data,
                MapF = mapF.Data
            }.Schedule(self.Length, 64, handle);
            self.CombineWith(handle);
        }
    }
}