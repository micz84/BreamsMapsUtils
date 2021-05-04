using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps
{
    [BurstCompile]
    public struct AddMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] += MapA[j];
        }
    }

    [BurstCompile]
    public struct AddWeightedMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] += MapA[j] * AWeight;
        }
    }

    [BurstCompile]
    public struct SubtractMapAFromMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] -= MapA[j];
        }
    }

    [BurstCompile]
    public struct SubtractWeightedMapAFromMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] -= MapA[j] * AWeight;
        }
    }

    [BurstCompile]
    public struct MultiplyMapAByMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] *= MapA[j];
        }
    }

    [BurstCompile]
    public struct MultiplyWeightedMapAByMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] *= MapA[j] * AWeight;
        }
    }

    [BurstCompile]
    public struct CopyMapAToMapBJob : IJob
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [WriteOnly] public NativeArray<float4> MapB;

        public unsafe void Execute()
        {
            UnsafeUtility.MemCpy(MapB.GetUnsafePtr(), MapA.GetUnsafeReadOnlyPtr(), sizeof(float4) * MapB.Length);
        }
    }

    [BurstCompile]
    public struct CopyWeightedMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] = MapA[j] * AWeight;
        }
    }

    [BurstCompile]
    public struct MultiplyMapByWeightJob : IJobParallelForBatch
    {
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapA;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapA[j] *= AWeight;
        }
    }

    [BurstCompile]
    public struct FitAddMapAToMapB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;

        [ReadOnly] public NativeArray<float4> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] += MapA[mapAIndex];
        }
    }

    [BurstCompile]
    public struct FitAddWeightedMapAToMapB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public float Weight;

        [ReadOnly] public NativeArray<float4> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] += MapA[mapAIndex] * Weight;
        }
    }

    [BurstCompile]
    public struct FitAddMapAToMapBBatch : IJobParallelForBatch
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;

        [ReadOnly] public NativeArray<float4> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public unsafe void Execute(int startIndex, int count)
        {
            var temp = stackalloc float4[count];
            UnsafeUtility.MemCpy(temp, (float4*) MapB.GetUnsafeReadOnlyPtr() + startIndex, sizeof(float4) * count);

            for (var i = 0; i < count; i++)
            {
                var index = startIndex + i;
                var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
                temp[i] = MapA[mapAIndex];
            }

            UnsafeUtility.MemCpy((float4*) MapB.GetUnsafeReadOnlyPtr() + startIndex, temp, sizeof(float4) * count);
        }
    }

    [BurstCompile]
    public struct FitCopyMapAToMapB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;

        [ReadOnly] public NativeArray<float4> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] = MapA[mapAIndex];
        }
    }

    [BurstCompile]
    public struct FitCopyWeightedMapAToMapB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public float Weight;

        [ReadOnly] public NativeArray<float4> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            MapB[mapBIndex] = MapA[mapAIndex] * Weight;
        }
    }
    [BurstCompile]
    public struct SampleDiscreet : IJobParallelFor
    {
        [ReadOnly] public int Width;
        [ReadOnly] public int ValuesStartIndex;
        [ReadOnly] public NativeArray<float4> Map;
        [ReadOnly] public NativeArray<int2> Positions;
        [WriteOnly] public NativeArray<float4> Values;
        public void Execute(int index)
        {
            var pos = Positions[index];
            Values[index + ValuesStartIndex] = Map[pos.x + pos.y * Width];
        }
    }
    [BurstCompile]
    public struct SampleLerp : IJobParallelFor
    {
        [ReadOnly] public int Width;
        [ReadOnly] public int ValuesStartIndex;
        [ReadOnly] public NativeArray<float4> Map;
        [ReadOnly] public NativeArray<float2> Positions;
        [WriteOnly] public NativeArray<float4> Values;
        public void Execute(int index)
        {
            var pos = Positions[index];
            var x1 = (int) math.floor(pos.x);
            var x2 = (int) math.ceil(pos.x);
            var xFrac = pos.x - x1;

            var y1 = (int) math.floor(pos.y);
            var y2 = (int)  math.ceil(pos.y);
            var yFrac = pos.y - y1;
            var val1 = Map[x1 + y1 * Width];
            var val2 = Map[x2 + y1 * Width];
            var val3 = Map[x1 + y2 * Width];
            var val4 = Map[x2 + y2 * Width];
            Values[index + ValuesStartIndex] = math.lerp(
                math.lerp(val1, val2, xFrac),
                math.lerp(val3, val4, xFrac),
                yFrac);
        }
    }

    [BurstCompile]
    public struct MinBetweenTwoMapsNonDestructive : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MinBetweenTwoMaps : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MaxBetweenTwoMapsNonDestructive : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.max(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MaxBetweenTwoMaps : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.max(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveFactor : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public NativeArray<float4> MapB;

        [ReadOnly] public float Factor;
        [WriteOnly] public NativeArray<float4> MapC;


        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index], Factor);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsFactor : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public float Factor;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], Factor);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveMapFactor : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [ReadOnly] public NativeArray<float> MapF;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsMapFactor : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float4> MapA;
        [ReadOnly] public NativeArray<float> MapF;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }
}