using pl.breams.dotsinfluancemaps.implementations.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps
{
    [BurstCompile]
    public struct SplatMultipleMaps:IJob
    {
        [ReadOnly]
        public NativeArray<int2> Positions;
        [ReadOnly]
        public NativeArray<InfluenceMapProxy> InfluenceMapProxies;
        [ReadOnly]
        public int Width;
        [ReadOnly]
        public int Height;
        public NativeArray<float> Map;
        public void Execute()
        {
            for (var i = 0; i < InfluenceMapProxies.Length; i++)
            {
                var proxy = InfluenceMapProxies[i];
                MapUtils.GetJobParamsFast(Width, Height, proxy.Width, proxy.Height, Positions[i], out var cellsCount,
                    out var smallerMapStartIndex, out var biggerMapStartIndex, out var workingWidth, out var workingHeight);
                /*for (var index = 0; index < cellsCount; index++)
                {
                    var mapAIndex = smallerMapStartIndex + index / workingWidth * proxy.Width + index % workingWidth;
                    var mapBIndex = biggerMapStartIndex + index / workingWidth * Width + index % workingWidth;
                    Map[mapBIndex] = proxy[mapAIndex];
                }*/

                for (var h = 0; h < workingHeight && h < cellsCount ; h++)
                {
                    var hOffsetA = smallerMapStartIndex + h * proxy.Width;
                    var hOffsetB = biggerMapStartIndex + h * Width;
                    for (var w = 0; w < workingWidth; w++)
                    {
                        var mapAIndex = hOffsetA + w;
                        var mapBIndex = hOffsetB + w;
                        Map[mapBIndex] = proxy[mapAIndex];
                    }
                }
            }
        }
    }
    [BurstCompile]
    public struct SplatMultipleMapsGlobalPosition:IJob
    {
        [ReadOnly]
        public NativeArray<int2> Positions;
        [ReadOnly]
        public NativeArray<InfluenceMapProxy> InfluenceMapProxies;
        [ReadOnly]
        public int Width;

        [ReadOnly] public int2 GlobalPositionOffset;
        [ReadOnly]
        public int Height;
        public NativeArray<float> Map;
        public void Execute()
        {
            for (var i = 0; i < InfluenceMapProxies.Length; i++)
            {
                var proxy = InfluenceMapProxies[i];
                var position = Positions[i] - GlobalPositionOffset;
                MapUtils.GetJobParamsFast(Width, Height, proxy.Width, proxy.Height, position, out var cellsCount,
                    out var smallerMapStartIndex, out var biggerMapStartIndex, out var workingWidth, out var workingHeight);

                for (var h = 0; h < workingHeight && h < cellsCount ; h++)
                {
                    var hOffsetA = smallerMapStartIndex + h * proxy.Width;
                    var hOffsetB = biggerMapStartIndex + h * Width;
                    for (var w = 0; w < workingWidth; w++)
                    {
                        var mapAIndex = hOffsetA + w;
                        var mapBIndex = hOffsetB + w;
                        Map[mapBIndex] = proxy[mapAIndex];
                    }
                }
            }
        }

    }

    [BurstCompile]
    public struct SplatMultipleMapsGlobalPositionIndexed : IJob
    {
        [ReadOnly] public NativeArray<int2> Positions;
        [ReadOnly] public NativeArray<InfluenceMapProxy> InfluenceMapProxies;
        [ReadOnly] public int Width;

        [ReadOnly] public int Height;
        public NativeArray<float> Map;

        public void Execute()
        {
            for (var i = 0; i < Positions.Length; i++)
            {
                var proxy = InfluenceMapProxies[i];
                var position = Positions[i];
                MapUtils.GetJobParamsFast(Width, Height, proxy.Width, proxy.Height, position, out var cellsCount,
                    out var smallerMapStartIndex, out var biggerMapStartIndex, out var workingWidth,
                    out var workingHeight);

                for (var h = 0; h < workingHeight && h < cellsCount; h++)
                {
                    var hOffsetA = smallerMapStartIndex + h * proxy.Width;
                    var hOffsetB = biggerMapStartIndex + h * Width;
                    for (var w = 0; w < workingWidth; w++)
                    {
                        var mapAIndex = hOffsetA + w;
                        var mapBIndex = hOffsetB + w;
                        Map[mapBIndex] = proxy[mapAIndex];
                    }
                }
            }
        }
    }

    [BurstCompile]
    public struct FitAddMapAToMapB:IJobParallelFor
    {
        [ReadOnly]
        public int MapATotalWidth;
        [ReadOnly]
        public int MapAStartIndex;
        [ReadOnly]
        public int WorkingWidth;
        [ReadOnly]
        public int MapBStartIndex;
        [ReadOnly]
        public int MapBTotalWidth;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] += MapA[mapAIndex];
        }
    }

    [BurstCompile]
    public struct FitAddWeightedMapAToMapB:IJobParallelFor
    {
        [ReadOnly]
        public int MapATotalWidth;
        [ReadOnly]
        public int MapAStartIndex;
        [ReadOnly]
        public int WorkingWidth;
        [ReadOnly]
        public int MapBStartIndex;
        [ReadOnly]
        public int MapBTotalWidth;
        [ReadOnly]
        public float Weight;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] += MapA[mapAIndex] * Weight;
        }
    }
    [BurstCompile]
    public struct FitAddMapAToMapBBatch:IJobParallelForBatch
    {
        [ReadOnly]
        public int MapATotalWidth;
        [ReadOnly]
        public int MapAStartIndex;
        [ReadOnly]
        public int WorkingWidth;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> MapB;

        public unsafe void Execute(int startIndex, int count)
        {
            var temp = stackalloc float[count];
            UnsafeUtility.MemCpy(temp, ((float*) MapB.GetUnsafeReadOnlyPtr())+startIndex, 4 * count);

            for (var i = 0; i < count; i++)
            {
                var index = startIndex + i;
                var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
                temp[i] = MapA[mapAIndex];
            }

            UnsafeUtility.MemCpy(((float*) MapB.GetUnsafeReadOnlyPtr())+startIndex,temp, 4 * count);
        }
    }

    [BurstCompile]
    public struct FitCopyMapAToMapB:IJobParallelFor
    {
        [ReadOnly]
        public int MapATotalWidth;
        [ReadOnly]
        public int MapAStartIndex;
        [ReadOnly]
        public int WorkingWidth;
        [ReadOnly]
        public int MapBStartIndex;
        [ReadOnly]
        public int MapBTotalWidth;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] = MapA[mapAIndex];
        }
    }

    [BurstCompile]
    public struct FitCopyWeightedMapAToMapB:IJobParallelFor
    {
        [ReadOnly]
        public int MapATotalWidth;
        [ReadOnly]
        public int MapAStartIndex;
        [ReadOnly]
        public int WorkingWidth;
        [ReadOnly]
        public int MapBStartIndex;
        [ReadOnly]
        public int MapBTotalWidth;
        [ReadOnly]
        public float Weight;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] = MapA[mapAIndex] * Weight;
        }
    }
}