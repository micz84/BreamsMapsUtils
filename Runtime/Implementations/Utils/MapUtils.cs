using Unity.Burst;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.implementations.Utils
{
    public static class MapUtils
    {
        public static bool GetJobParams(int biggerMapWidth, int biggerMapHeight, int smallerMapWidth, int smallerMapHeight, int2 center,
            out int cellsCount, out int smallerMapStartIndex,
            out int biggerMapStartIndex, out int workingWidth)
        {
            cellsCount = -1;
            smallerMapStartIndex = -1;
            biggerMapStartIndex = -1;
            biggerMapStartIndex = -1;
            workingWidth = -1;
            int workingHeight;
            var mapAHalfWidth = smallerMapWidth / 2;
            var mapAHalfHeight = smallerMapHeight / 2;
            var upperLeftX = center.x - mapAHalfWidth;
            var upperLeftY = center.y - mapAHalfHeight;
            if (upperLeftX < 0)
                workingWidth = smallerMapWidth + upperLeftX;
            else
                workingWidth = math.min(smallerMapWidth, biggerMapWidth - upperLeftX);
            if (workingWidth <= 0)
                return false;

            if (upperLeftY < 0)
                workingHeight = smallerMapHeight + upperLeftY;
            else
                workingHeight = math.min(smallerMapHeight, biggerMapHeight - upperLeftY);

            if (workingHeight <= 0)
                return false;

            cellsCount = workingWidth * workingHeight;

            smallerMapStartIndex = math.abs(math.min(upperLeftY, 0)) * smallerMapWidth + math.abs(math.min(upperLeftX, 0));
            biggerMapStartIndex = math.max(0, upperLeftY) * biggerMapWidth + math.max(0, upperLeftX);

            return true;
        }

        [BurstCompile]
        public static void GetJobParamsFast(int biggerMapWidth, int biggerMapHeight, int smallerMapWidth, int smallerMapHeight, int2 center,
            out int cellsCount, out int smallerMapStartIndex,
            out int biggerMapStartIndex, out int workingWidth, out int workingHeight)
        {
            var mapAHalfWidth = smallerMapWidth >> 1;
            var mapAHalfHeight = smallerMapHeight >> 1;
            var upperLeftX = center.x - mapAHalfWidth;
            var upperLeftY = center.y - mapAHalfHeight;

            workingWidth = math.select(math.min(smallerMapWidth, biggerMapWidth - upperLeftX),
                smallerMapWidth + upperLeftX, upperLeftX < 0);

            workingHeight = math.select(math.min(smallerMapHeight, biggerMapHeight - upperLeftY),
                smallerMapHeight + upperLeftY, upperLeftY < 0);

            cellsCount = math.select(workingWidth * workingHeight, 0 , workingWidth <=0 || workingHeight <=0);

            smallerMapStartIndex = math.abs(math.min(upperLeftY, 0)) * smallerMapWidth + math.abs(math.min(upperLeftX, 0));
            biggerMapStartIndex = math.max(0, upperLeftY) * biggerMapWidth + math.max(0, upperLeftX);
        }
    }
}