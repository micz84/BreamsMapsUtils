using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps
{

    [BurstCompile]
    public struct AddMapAToMapBJobGeneric<T> : IJobParallelForBatch where T:struct, IInfluenceMap
    {
        public T MapA;

        public T MapB;

        public void Execute(int startIndex, int count)
        {
            var mapB = MapB.Data;
            for (int j = startIndex, end = startIndex + count; j < end; j++)
            {
                var d = MapA.Data[j];
                mapB[j] += d;
            }
        }
    }

    [BurstCompile]
    public struct AddMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] += MapA[j];
        }
    }

    [BurstCompile]
    public struct AddWeightedMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] += MapA[j] * AWeight;
        }
    }

    [BurstCompile]
    public struct SubtractMapAFromMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] -= MapA[j];
        }
    }

    [BurstCompile]
    public struct SubtractWeightedMapAFromMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] -= MapA[j] * AWeight;
        }
    }
    [BurstCompile]
    public struct MultiplyMapAByMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] *= MapA[j];
        }
    }

    [BurstCompile]
    public struct MultiplyWeightedMapAByMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float> MapB;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapB[j] *= MapA[j] * AWeight;
        }
    }
    [BurstCompile]
    public struct CopyMapAToMapBJob : IJob
    {
        [ReadOnly] public NativeArray<float> MapA;
        [WriteOnly] public NativeArray<float> MapB;

        public unsafe void Execute()
        {
            UnsafeUtility.MemCpy(MapB.GetUnsafePtr(), MapA.GetUnsafeReadOnlyPtr(), 4 * MapB.Length);
        }
    }
    [BurstCompile]
    public struct CopyWeightedMapAToMapBJob : IJobParallelForBatch
    {
        [ReadOnly] public NativeArray<float> MapA;
        [ReadOnly] public float AWeight;
        public NativeArray<float> MapB;

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
        public NativeArray<float> MapA;

        public void Execute(int startIndex, int count)
        {
            for (int j = startIndex, end = startIndex + count; j < end; j++)
                MapA[j] *= AWeight;
        }
    }



    [BurstCompile]
    public struct MinBetweenTwoMapsNonDestructive:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly]
        public NativeArray<float> MapB;
        [WriteOnly]
        public NativeArray<float> MapC;
        public void Execute(int index)
        {
            MapC[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MinBetweenTwoMaps:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;

        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            MapB[index] = math.min(MapA[index], MapB[index]);
        }
    }

    [BurstCompile]
    public struct MaxBetweenTwoMapsNonDestructive:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly]
        public NativeArray<float> MapB;
        [WriteOnly]
        public NativeArray<float> MapC;
        public void Execute(int index)
        {
            MapC[index] = math.max(MapA[index], MapB[index]);
        }

    }

    [BurstCompile]
    public struct MaxBetweenTwoMaps:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;

        public NativeArray<float> MapB;

        public void Execute(int index)
        {
            MapB[index] = math.max(MapA[index], MapB[index]);
        }

    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveFactor:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly]
        public NativeArray<float> MapB;

        [ReadOnly] public float Factor;
        [WriteOnly]
        public NativeArray<float> MapC;


        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index],Factor);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsFactor:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly] public float Factor;

        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], Factor);
        }
    }
    [BurstCompile]
    public struct LerpBetweenTwoMapsNonDestructiveMapFactor:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly]
        public NativeArray<float> MapB;
        [ReadOnly] public NativeArray<float> MapF;
        [WriteOnly]
        public NativeArray<float> MapC;
        public void Execute(int index)
        {
            MapC[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }

    [BurstCompile]
    public struct LerpBetweenTwoMapsMapFactor:IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<float> MapA;
        [ReadOnly] public NativeArray<float> MapF;

        public NativeArray<float> MapB;
        public void Execute(int index)
        {
            MapB[index] = math.lerp(MapA[index], MapB[index], MapF[index]);
        }
    }
}