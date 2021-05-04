using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps
{
    [BurstCompile]
    public struct CopyMapAToMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] = MapA[j];
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct AddMapAToMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] += MapA[j];
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct AddWeightedMapAToMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] += MapA[j] * AWeight;
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct SubtractMapAFromMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] -= MapA[j];
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct SubtractWeightedMapAFromMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] -= MapA[j] * AWeight;
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct MultiplyMapAByMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] *= MapA[j];
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct MultiplyWeightedMapAByMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] *= MapA[j] * AWeight;
                MapB[j] = element;
            }
        }
    }
    [BurstCompile]
    public struct CopyWeightedMapAToMapBSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public float AWeight;
        public NativeArray<float4> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapB[j];
                element[ComponentIndex] = MapA[j] * AWeight;
                MapB[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct MultiplyMapByWeightSingleComponentJob : IJobParallelForBatch
    {
        [ReadOnly] public float AWeight;
        [ReadOnly] public int ComponentIndex;
        public NativeArray<float4> MapA;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var element = MapA[j];
                element[ComponentIndex] *= AWeight;
                MapA[j] = element;
            }
        }
    }

    [BurstCompile]
    public struct FitAddMapAToMapSingleComponentB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            var element = MapB[mapBIndex];
            element[ComponentIndex] += MapA[mapAIndex];
            MapB[mapBIndex] = element;
        }
    }

    [BurstCompile]
    public struct FitAddWeightedMapAToMapSingleComponentB : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public float Weight;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            var element = MapB[mapBIndex];
            element[ComponentIndex] += MapA[mapAIndex] * Weight;
            MapB[mapBIndex] = element;

        }
    }

    [BurstCompile]
    public struct FitAddMapAToMapBBatchSingleComponent : IJobParallelForBatch
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public unsafe void Execute(int startIndex, int count)
        {
            var temp = stackalloc float4[count];
            UnsafeUtility.MemCpy(temp, (float4*) MapB.GetUnsafeReadOnlyPtr() + startIndex, sizeof(float4) * count);

            for (var i = 0; i < count; i++)
            {
                var index = startIndex + i;
                var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
                var element = temp[i];
                element[ComponentIndex] = MapA[mapAIndex];
                temp[i] = element;
            }

            UnsafeUtility.MemCpy((float4*) MapB.GetUnsafeReadOnlyPtr() + startIndex, temp, sizeof(float4) * count);
        }
    }

    [BurstCompile]
    public struct FitCopyMapAToMapBSingleComponent : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            var element = MapB[mapBIndex];
            element[ComponentIndex] = MapA[mapAIndex];
            MapB[mapBIndex] = element;
        }
    }

    [BurstCompile]
    public struct FitCopyWeightedMapAToMapBSingleComponent : IJobParallelFor
    {
        [ReadOnly] public int MapATotalWidth;
        [ReadOnly] public int MapAStartIndex;
        [ReadOnly] public int WorkingWidth;
        [ReadOnly] public int MapBStartIndex;
        [ReadOnly] public int MapBTotalWidth;
        [ReadOnly] public float Weight;
        [ReadOnly] public int ComponentIndex;
        [ReadOnly] public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction] public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            var mapAIndex = MapAStartIndex + index / WorkingWidth * MapATotalWidth + index % WorkingWidth;
            var mapBIndex = MapBStartIndex + index / WorkingWidth * MapBTotalWidth + index % WorkingWidth;
            var element = MapB[mapBIndex];
            element[ComponentIndex] = MapA[mapAIndex] * Weight;
            MapB[mapBIndex] = element;
        }
    }
    /*
    [BurstCompile]
    public struct SampleDiscreetSingleComponent : IJobParallelFor
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
    public struct SampleLerpSingleComponent : IJobParallelFor
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
    public struct MinBetweenTwoMapsNonDestructiveSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MinBetweenTwoMapsSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MaxBetweenTwoMapsNonDestructiveSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.max(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MaxBetweenTwoMapsSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.max(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveFactorSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float4> MapB;

        [ReadOnly] public float Factor;
        [WriteOnly] public NativeArray<float4> MapC;


        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index], Factor);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsFactorSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public float Factor;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], Factor);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveMapFactorSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float4> MapB;
        [ReadOnly] public NativeArray<float> MapF;
        [WriteOnly] public NativeArray<float4> MapC;

        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsMapFactorSingleComponent : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float> MapF;

        public NativeArray<float4> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }
*/
}