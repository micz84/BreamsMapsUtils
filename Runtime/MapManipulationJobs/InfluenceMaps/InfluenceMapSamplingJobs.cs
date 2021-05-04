using pl.breams.dotsinfluancemaps.implementations.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps
{
    [BurstCompile]
    public struct SampleDiscreet : IJobParallelFor
    {
        [ReadOnly] public int Width;
        [ReadOnly] public int ValuesStartIndex;
        [ReadOnly] public NativeArray<float> Map;
        [ReadOnly] public NativeArray<int2> Positions;
        [WriteOnly] public NativeArray<float> Values;
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
        [ReadOnly] public NativeArray<float> Map;
        [ReadOnly] public NativeArray<float2> Positions;
        [WriteOnly] public NativeArray<float> Values;
        public void Execute(int index)
        {
            var pos = Positions[index];
            var x1 = (int) math.floor(pos.x);
            var x2 = (int) math.ceil(pos.x);
            var xFrac = pos.x - x1;

            var y1 = (int) math.floor(pos.y);
            var y2 = (int)  math.ceil(pos.y);
            var yFrac = pos.y - y1;
            var val1 =  Map[x1 + y1 * Width];
            var val2 =  Map[x2 + y1 * Width];
            var val3 =  Map[x1 + y2 * Width];
            var val4 =  Map[x2 + y2 * Width];
            Values[index + ValuesStartIndex] = math.lerp(
                math.lerp(val1, val2, xFrac),
                math.lerp(val3, val4, xFrac),
                yFrac);
        }
    }

    [BurstCompile]
    public struct CopyDownRez:IJobParallelFor
    {
        [ReadOnly] public int MapAWidth;
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

        }
    }

    [BurstCompile]
    public struct CenterOfGravityJob:IJob
    {
        [ReadOnly] public int Length;
        [ReadOnly] public int MapAWidth;
        [ReadOnly]
        public int2 Center;
        [ReadOnly]
        public int ResultsIndex;

        [ReadOnly]
        public NativeArray<float> MapA;
        [NativeDisableParallelForRestriction]
        public NativeArray<float2> Results;
        public void Execute()
        {
            var totalMass = 0f;
            var position = int2.zero;
            var positionSum = float2.zero;
            var mass = 0f;
            for (var i = 0; i > Length; i++)
            {
                mass = MapA[i];
                totalMass += mass;
                position = new int2(i%MapAWidth, (int) math.floor(i/MapAWidth)) - Center;
                positionSum +=new float2(position.x * mass, position.y * mass);
            }

            Results[ResultsIndex] = positionSum / totalMass;
        }
    }


}