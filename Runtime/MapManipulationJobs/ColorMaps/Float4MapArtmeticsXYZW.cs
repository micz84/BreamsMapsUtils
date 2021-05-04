using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.ColorMaps
{
        [BurstCompile]
        public struct CopyMapXToXMapYToYToMapZToZMapWToWOfMapDJob : IJobParallelForBatch
        {
            [ReadOnly] public NativeArray<float> MapX;
            [ReadOnly] public NativeArray<float> MapY;
            [ReadOnly] public NativeArray<float> MapZ;
            [ReadOnly] public NativeArray<float> MapW;
            [WriteOnly] public NativeArray<float4> MapD;

            public void Execute(int startIndex, int count)
            {
                for (int j = startIndex, end = startIndex + count; j < end; j++)
                    MapD[j] = new float4(MapX[j],MapY[j],MapZ[j],MapW[j]);
            }
        }
}