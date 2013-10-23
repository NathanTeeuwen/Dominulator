using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Program
{
    class HistogramData
    {
        int totalCount = 0;
        Dictionary<int, int> mapBucketToCount = new Dictionary<int, int>();

        public void AddOneToBucket(int bucket)
        {
            lock (this)
            {
                this.totalCount++;

                int value = 0;
                this.mapBucketToCount.TryGetValue(bucket, out value);
                value += 1;
                this.mapBucketToCount[bucket] = value;
            }
        }

        public int[] GetXAxis()
        {
            return this.mapBucketToCount.OrderBy(keyValuePair => keyValuePair.Key).Select(keyValuePair => keyValuePair.Key).ToArray();
        }

        public float[] GetYAxis()
        {
            return this.mapBucketToCount.OrderBy(keyValuePair => keyValuePair.Key).Select(keyValuePair => (float)keyValuePair.Value / this.totalCount * 100).ToArray();
        }

        public void WriteBuckets(System.IO.TextWriter writer)
        {
            foreach (var pair in this.mapBucketToCount.OrderByDescending(keyValuePair => keyValuePair.Key))
            {
                writer.WriteLine("{0} points:   {2}% = {1}", pair.Key, pair.Value, (double)pair.Value / this.totalCount * 100);
            }
        }
    }
}
