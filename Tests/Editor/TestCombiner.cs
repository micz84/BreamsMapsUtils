using NUnit.Framework;
using pl.breams.dotsinfluancemaps.interfaces;
using Unity.Mathematics;
using UnityEngine;

namespace pl.breams.dotsinfluancemaps.tests
{
    public class TestCombiner
    {
        [Test]
        public void TestCopy()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.CopyMap(map);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(map.Data[i], mapCombiner.Data[i]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }
        [Test]
        public void TestAdd()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.AddMap(map);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(map.Data[i], mapCombiner.Data[i]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }

        [Test]
        public void TestAddWeighted()
        {
            var width = 16;
            var weight = 2;
            var map = new TestMap(width);
            var data = map.Data;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.AddWeightedMap(map, weight);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.IsTrue(Mathf.Approximately(weight*map.Data[i], mapCombiner.Data[i]));
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }


        [Test]
        public void TestSubtract()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.SubtractMap(map);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(-map.Data[i], mapCombiner.Data[i]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }

        [Test]
        public void TestSubtractWeighted()
        {
            var width = 16;
            var weight = 2;
            var map = new TestMap(width);
            var data = map.Data;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.SubtractWeightedMap(map, weight);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.IsTrue(Mathf.Approximately(-weight*map.Data[i], mapCombiner.Data[i]));
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }

        [Test]
        public void TestMultiplyByWeight()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            var weight = 2;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.CopyMap(map);
            mapCombiner.MultiplyByWeight(weight);

            mapCombiner.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(weight * map.Data[i], mapCombiner.Data[i]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }

        [Test]
        public void TestMultiplyByMap()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            var weight = 2;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.CopyMap(map);
            mapCombiner.MultiplyByWeight(weight);

            var mapCombiner2 = new WorkableInfluneceMap(width);
            mapCombiner2.CopyMap(map);
            mapCombiner2.MultiplyByMap(mapCombiner, mapCombiner.Handle);

            mapCombiner2.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(weight * map.Data[i], mapCombiner2.Data[i]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();

        }

        [Test]
        public void TestMultiplyByWeightedMap()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            var weight = 2;

            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width);
            mapCombiner.CopyMap(map);
            mapCombiner.MultiplyByWeight(weight);

            var mapCombiner2 = new WorkableInfluneceMap(width);
            mapCombiner2.CopyMap(map);
            mapCombiner2.MultiplyByWeightedMap(mapCombiner, weight, mapCombiner.Handle);

            mapCombiner2.Handle.Complete();
            for (int i = 0; i < map.Length; i++)
                Assert.AreEqual(weight * weight * map.Data[i], mapCombiner2.Data[i]);
            map.Dispose(mapCombiner.Handle);

            mapCombiner.Dispose(mapCombiner2.Handle);
            mapCombiner2.Dispose();

        }

        [Test]
        public void TestSplatMap()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;

            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width*2);
            mapCombiner.SplatMap(map, new int2(width/2,width/2));

            mapCombiner.Handle.Complete();
            Debug.Log(map.GetDataString());
            Debug.Log(mapCombiner.GetDataString());
            for (int i = 0; i < width*width; i++)
                Assert.AreEqual(map.Data[i], mapCombiner.Data[i%width+i/width*mapCombiner.Width]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();
        }

        [Test]
        public void TestSplatWeightedMap()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;
            var weight = 2;
            for (int i = 0; i < map.Length; i++)
                data[i] = 1;

            var mapCombiner = new WorkableInfluneceMap(width*2);
            mapCombiner.SplatWeightedMap(map, new int2(width/2,width/2), weight);

            mapCombiner.Handle.Complete();
            Debug.Log(map.GetDataString());
            Debug.Log(mapCombiner.GetDataString());
            for (int i = 0; i < width*width; i++)
                Assert.AreEqual(weight*map.Data[i], mapCombiner.Data[i%width+i/width*mapCombiner.Width]);
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();
        }

        [Test]
        public void TestLiftMap()
        {
            var width = 16;
            var map = new TestMap(width);
            var data = map.Data;

            for (int y = 0; y < map.Height/2; y++)
            {
                for (int x = 0; x < map.Width/2; x++)
                    data[x% map.Width + y * map.Width] = 1;
            }
            Debug.Log(map.GetDataString());

            var mapCombiner = new WorkableInfluneceMap(width/2);
            mapCombiner.LiftMap(map, new int2(width/2,width/2));

            mapCombiner.Handle.Complete();
            Debug.Log(mapCombiner.GetDataString());
            for (int y = 0; y < mapCombiner.Height/2; y++)
            {
                for (int x = 0; x < mapCombiner.Width/2; x++)
                    Assert.AreEqual( 1, mapCombiner.Data[x% mapCombiner.Width + y * mapCombiner.Width]);
            }
            map.Dispose(mapCombiner.Handle);
            mapCombiner.Dispose();
        }
    }

}