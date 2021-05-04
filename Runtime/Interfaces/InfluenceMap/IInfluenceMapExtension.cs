using System;
using System.Text;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public static class IInfluenceMapExtension
    {
        public static string GetDataString(this IInfluenceMap self, string format = "F3", StringBuilder stringBuilder = null)
        {
            if(stringBuilder == null)
                stringBuilder = new StringBuilder();

            for (int y = 0; y < self.Height; y++)
            {
                for (int x = 0; x < self.Width; x++)
                    stringBuilder.Append($"[{self.Data[x + self.Width * y].ToString(format)}]");
                stringBuilder.Append($"\n");
            }
            return stringBuilder.ToString();
        }

        public static void AppendDataString(this IInfluenceMap self, StringBuilder stringBuilder, string format = "F3")
        {
            if(stringBuilder == null)
                throw new AggregateException($"{nameof(stringBuilder)} must not be null.");

            for (int y = 0; y < self.Height; y++)
            {
                for (int x = 0; x < self.Width; x++)
                    stringBuilder.Append($"[{self.Data[x + self.Width * y].ToString(format)}]");
                stringBuilder.Append($"\n");
            }
        }
    }
}