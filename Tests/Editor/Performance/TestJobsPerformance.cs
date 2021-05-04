using NUnit.Framework;
using pl.breams.dotsinfluancemaps.interfaces;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs;
using pl.breams.dotsinfluancemaps.mapmanipulationjobs.influencemaps;
using Unity.Jobs;
using Unity.PerformanceTesting;
using UnityEngine;

namespace pl.breams.dotsinfluancemaps.tests.performance
{
    public class TestJobsPerformance
    {
        private readonly int _Iterations = 10;
        private readonly int _MeasureCount = 20;
        private readonly int _WarmUpCount = 2;

        [Test]
        [Performance]
        public void CopyMapAToMapB([Values(32, 64, 128, 256, 512, 1024)] int size)
        {
            var mapA = new TestMap(size);
            var mapB = new TestMap(size);
            Measure.Method(() =>
                {
                    var job = new CopyMapAToMapBJob
                    {
                        MapA = mapA.Data, MapB = mapB.Data
                    };
                    var handle = job.Schedule();
                    handle.Complete();
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
        public void AddMapAToMapB([Values(32, 64, 128, 256, 512, 1024)] int size, [Values(2048)] int batchPart)
        {
            var mapA = new TestMap(size);
            var mapB = new TestMap(size);
            Measure.Method(() =>
                {
                    var job = new AddMapAToMapBJob
                    {
                        MapA = mapA.Data, MapB = mapB.Data
                    };
                    var handle = job.ScheduleBatch(mapA.Width * mapA.Width, Mathf.Min(mapA.Width, batchPart));
                    handle.Complete();
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
        public void AddMapAToMapBGeneric([Values(32, 64, 128, 256, 512, 1024)] int size, [Values(2048)] int batchPart)
        {
            var mapA = new TestMap(size);
            var mapB = new TestMap(size);
            Measure.Method(() =>
                {
                    var job = new AddMapAToMapBJobGeneric<TestMap>
                    {
                        MapA = mapA, MapB = mapB
                    };
                    var handle = job.ScheduleBatch(mapA.Width * mapA.Width, Mathf.Min(mapA.Width, batchPart));
                    handle.Complete();
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
        public void AddWeightedMapAToMapB([Values(32, 64, 128, 256, 512, 1024)] int size,
            [Values(1, 2, 4, 8)] int batchPart)
        {
            var mapA = new TestMap(size);
            var mapB = new TestMap(size);
            Measure.Method(() =>
                {
                    var job = new AddWeightedMapAToMapBJob
                    {
                        MapA = mapA.Data, MapB = mapB.Data, AWeight = 2
                    };
                    var handle = job.ScheduleBatch(mapA.Width * mapA.Width, mapA.Width * mapA.Width / batchPart);
                    handle.Complete();
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
        public void AddMapAToMapBAndMultiplyBByWeight([Values(32, 64, 128, 256, 512, 1024)] int size,
            [Values(1, 2, 4, 8)] int batchPart)
        {
            var mapA = new TestMap(size);
            var mapB = new TestMap(size);
            Measure.Method(() =>
                {
                    var job = new AddMapAToMapBJob
                    {
                        MapA = mapA.Data, MapB = mapB.Data
                    };
                    var handle = job.ScheduleBatch(mapA.Width * mapA.Width, mapA.Width * mapA.Width / batchPart);
                    var job2 = new MultiplyMapByWeightJob
                    {
                        MapA = mapB.Data, AWeight = 2
                    };
                    handle = job2.ScheduleBatch(mapA.Width * mapA.Width, mapA.Width * mapA.Width / batchPart, handle);
                    handle.Complete();
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
        public void MultiplyMapByWeight([Values(32, 64, 128, 256, 512, 1024)] int size,
            [Values(1, 2, 4, 8)] int batchPart)
        {
            var mapA = new TestMap(size);
            Measure.Method(() =>
                {
                    var job2 = new MultiplyMapByWeightJob
                    {
                        MapA = mapA.Data, AWeight = 2
                    };
                    var handle = job2.ScheduleBatch(mapA.Width * mapA.Width, mapA.Width * mapA.Width / batchPart);
                    handle.Complete();
                })
                .WarmupCount(_WarmUpCount)
                .MeasurementCount(_MeasureCount)
                .IterationsPerMeasurement(_Iterations)
                .GC()
                .Run();
            mapA.Dispose();
        }
    }
}