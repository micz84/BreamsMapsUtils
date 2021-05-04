using System;
using pl.breams.dotsinfluancemaps.implementations.Utils;
using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps
{
    public static class IWorkableInfluenceMapSamplingExtension
    {
        public static void Sample<T>(ref this T self, NativeArray<int2> positions, NativeArray<float> values, JobHandle handle = default, int startIndex = 0) where T:struct,IWorkableInfluenceMap
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

        public static void Sample<T>(ref this T self, NativeArray<float2> positions, NativeArray<float> values, JobHandle handle = default , int startIndex = 0) where T:struct,IWorkableInfluenceMap
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

        public static void CombineFromMaps<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, IInfluenceMap mapC, IInfluenceMap mapD,
            JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void CombineDownResFromMaps<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, IInfluenceMap mapC, IInfluenceMap mapD,
            JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            if(self.Width != mapA.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapA)} {nameof(mapA.Width)}. ");
            if(self.Width != mapB.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapB)} {nameof(mapB.Width)}. ");
            if(self.Width != mapC.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapC)} {nameof(mapC.Width)}. ");
            if(self.Width != mapD.Width)
                throw new NotSupportedException($"{nameof(self.Width)} must be same size as {nameof(mapD)} {nameof(mapD.Width)}. ");
            if(self.Height != mapA.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapA)} {nameof(mapA.Height)}. ");
            if(self.Height != mapB.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapB)} {nameof(mapB.Height)}. ");
            if(self.Height != mapC.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapC)} {nameof(mapC.Height)}. ");
            if(self.Height != mapD.Height)
                throw new NotSupportedException($"{nameof(self.Height)} must be same size as {nameof(mapD)} {nameof(mapD.Height)}. ");
            handle = JobHandle.CombineDependencies(handle,self.Handle);

            var widthQuoter = self.Width / 4;
            var heightQuoter = self.Height / 4;
            var centerA = new int2(widthQuoter, heightQuoter);
            var centerB = new int2(widthQuoter * 3, heightQuoter);
            var centerC = new int2(widthQuoter, heightQuoter * 3);
            var centerD = new int2(widthQuoter * 3, heightQuoter * 3);

            self.CombineWith(handle);
            throw new System.NotImplementedException();
        }

        public static void DownResFromMap<T>(ref this T self, IInfluenceMap mapB, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
        {
            throw new System.NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            self.CombineWith(handle);
        }

        public static void MinWith<T>(ref this T self, IInfluenceMap mapA, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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
        public static void MaxWith<T>(ref this T self, IInfluenceMap mapA, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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
        public static void MinFrom<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void MaxFrom<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void Lerp<T>(ref this T self, IInfluenceMap mapA, float factor, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void Lerp<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapF, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void Lerp<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, float factor, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void Lerp<T>(ref this T self, IInfluenceMap mapA, IInfluenceMap mapB, IInfluenceMap mapF, JobHandle handle = default) where T:struct,IWorkableInfluenceMap
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

        public static void FindMaxValue<T>(ref this T self, NativeArray<int2> position, NativeArray<float> value, JobHandle handle = default, int valuesStartIndex = 0) where T:struct,IWorkableInfluenceMap
        {
            throw new NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void FindMinValue<T>(ref this T self, NativeArray<int2> position, NativeArray<float> value, JobHandle handle = default, int valuesStartIndex = 0) where T:struct,IWorkableInfluenceMap
        {
            throw new NotImplementedException();
            handle = JobHandle.CombineDependencies(handle, self.Handle);
            //
            self.CombineWith(handle);
        }

        public static void GetCenterOfGravity<T>(ref this T self, int2 center, NativeArray<float2> positionOutput, JobHandle handle = default,
            int outputPositionIndex = 0) where T:struct,IWorkableInfluenceMap
        {
            if(outputPositionIndex >= positionOutput.Length)
                throw new IndexOutOfRangeException($"{nameof(outputPositionIndex)} is outside of {nameof(positionOutput)} array.");
            handle = JobHandle.CombineDependencies(handle, self.Handle);

            handle = new CenterOfGravityJob
            {
                MapA = self.Data,
                Length = self.Length,
                MapAWidth = self.Width,
                Results = positionOutput,
                Center = center,
                ResultsIndex = outputPositionIndex
            }.Schedule(handle);

            self.CombineWith(handle);
        }
    }
}