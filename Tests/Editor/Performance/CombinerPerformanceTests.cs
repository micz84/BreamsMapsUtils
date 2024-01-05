using NUnit.Framework;
using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.PerformanceTesting;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace pl.breams.dotsinfluancemaps.tests.performance
{
    public class CombinerPerformanceTests
    {
        private readonly int _Iterations = 1;
        private readonly int _MeasureCount = 1;
        private readonly int _WarmUpCount = 1;

        [Test]
        [Performance]
        public void SimpleCopySplatPlusLift([Values(64,128,512, 1024)] int size)
        {
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(size/2);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            Measure.Method(() =>
                {
                    var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
                    var combiner2 = new WorkableInfluneceMap(size/2, Allocator.Persistent);
                    combiner.CopyMap(mapA);
                    combiner.SplatMap(mapB, new int2(combiner.Width/2, combiner.Height/2));
                    combiner2.LiftMap(combiner, new int2(combiner.Width/4, combiner.Height/4), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner.Dispose(combiner2.Handle);
                    combiner2.Dispose();
                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();
            mapA.Dispose();
            mapB.Dispose();
        }

        [Test]
        [Performance]
        public void SimpleCopy5XSplatPlusLift([Values(64,128,512, 1024)] int size)
        {
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(32);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            Measure.Method(() =>
                {
                    var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
                    var combiner2 = new WorkableInfluneceMap(size/2, Allocator.Persistent);
                    combiner.CopyMap(mapA);
                    combiner.SplatMap(mapB, new int2(combiner.Width/2, combiner.Height/2));
                    combiner.SplatMap(mapB, new int2(combiner.Width/4, combiner.Height/4));
                    combiner.SplatMap(mapB, new int2(combiner.Width/4*3, combiner.Height/4*3));
                    combiner.SplatMap(mapB, new int2(combiner.Width/4, combiner.Height/4*3));
                    combiner.SplatMap(mapB, new int2(combiner.Width/4*3, combiner.Height/4));
                    combiner2.LiftMap(combiner, new int2(combiner.Width/4, combiner.Height/4), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner.Dispose(combiner2.Handle);
                    combiner2.Dispose();
                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();
            mapA.Dispose();
            mapB.Dispose();
        }

        [Test]
        [Performance]
        public void SimpleCopyXxMultiSplatPlusLift([Values(1,2,3,4,5)] int splatCountMultiply)
        {
            var size = 128;
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(32);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            Measure.Method(() =>
                {
                    var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
                    var combiner2 = new WorkableInfluneceMap(size/2, Allocator.Persistent);
                    combiner.CopyMap(mapA);
                    for (int i = 0; i < splatCountMultiply; i++)
                    {
                        combiner.SplatMap(mapB, new int2(combiner.Width / 2, combiner.Height / 2));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4, combiner.Height / 4));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4 * 3, combiner.Height / 4 * 3));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4, combiner.Height / 4 * 3));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4 * 3, combiner.Height / 4));
                    }

                    combiner2.LiftMap(combiner, new int2(combiner.Width/4, combiner.Height/4), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner.Dispose(combiner2.Handle);
                    combiner2.Dispose();
                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();
            mapA.Dispose();
            mapB.Dispose();
        }

        [Test]
        [Performance]
        public void XxMultiSplatPlusLift([Values(1,2,3,4,5)] int splatCountMultiply)
        {
            var size = 128;
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(32);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            Measure.Method(() =>
                {
                    var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
                    var combiner2 = new WorkableInfluneceMap(size/2, Allocator.Persistent);
                    for (int i = 0; i < splatCountMultiply; i++)
                    {
                        combiner.SplatMap(mapB, new int2(combiner.Width / 2, combiner.Height / 2));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4, combiner.Height / 4));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4 * 3, combiner.Height / 4 * 3));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4, combiner.Height / 4 * 3));
                        combiner.SplatMap(mapB, new int2(combiner.Width / 4 * 3, combiner.Height / 4));
                    }

                    combiner2.LiftMap(combiner, new int2(combiner.Width/4, combiner.Height/4), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner.Dispose(combiner2.Handle);
                    combiner2.Dispose();
                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();
            mapA.Dispose();
            mapB.Dispose();
        }

        [Test]
        [Performance]
        public void Map1024XxMultiSplatPlusLift([Values(1,2,3,4,5,10,20,50)] int splatCountMultiply)
        {
            var size = 1024;
            var splatSize = 32;
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(splatSize);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
            Measure.Method(() =>
                {
                    var combiner2 = new WorkableInfluneceMap(splatSize, Allocator.Persistent);
                    for (int i = 0; i < splatCountMultiply; i++)
                    {
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    }

                    combiner2.LiftMap(combiner, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner2.Dispose();

                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();

            combiner.Dispose();
            mapA.Dispose(combiner.Handle);
            mapB.Dispose(combiner.Handle);
        }

        [Test]
        [Performance]
        public void Map128XxMultiSplatPlusLift([Values(1,2,3,4,5,10,20,50)] int splatCountMultiply)
        {
            var size = 128;
            var splatSize = 32;
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(splatSize);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
            Measure.Method(() =>
                {
                    var combiner2 = new WorkableInfluneceMap(splatSize, Allocator.Persistent);
                    for (int i = 0; i < splatCountMultiply; i++)
                    {
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    }

                    combiner2.LiftMap(combiner, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner2.Dispose();

                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();

            combiner.Dispose();
            mapA.Dispose(combiner.Handle);
            mapB.Dispose(combiner.Handle);
        }

        [Test]
        [Performance]
        public void Map32XxMultiSplatPlusLift([Values(1,10,15,20,25,50,100,250)] int splatCountMultiply)
        {
            var size = 32;
            var splatSize = 8;
            var mapA = new TestMap(size);
            var mapAData = mapA.Data;
            var rand = new Random(42);
            for (int i = 0; i < mapA.Length; i++)
                mapAData[i] = rand.NextFloat();
            var mapB = new TestMap(splatSize);
            var mapBData = mapB.Data;
            for (int i = 0; i < mapB.Length; i++)
                mapBData[i] = rand.NextFloat();
            var combiner = new WorkableInfluneceMap(size, Allocator.Persistent);
            Measure.Method(() =>
                {
                    var combiner2 = new WorkableInfluneceMap(splatSize, Allocator.Persistent);
                    for (int i = 0; i < splatCountMultiply; i++)
                        combiner.SplatMap(mapB, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    combiner2.LiftMap(combiner, new int2(rand.NextInt(0,combiner.Width), rand.NextInt(0,combiner.Height)), combiner.Handle);
                    combiner2.Handle.Complete();
                    combiner2.Dispose();

                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();

            Debug.Log(combiner.GetDataString());
            combiner.Dispose();
            mapA.Dispose(combiner.Handle);
            mapB.Dispose(combiner.Handle);
        }
    }
}