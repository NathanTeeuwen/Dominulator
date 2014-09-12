using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CardTypes = Dominion.CardTypes;
using Dominion;

namespace Dominion.Data
{
    public class HistogramData
    {
        int totalCount = 0;
        Dictionary<int, int> mapBucketToCount = new Dictionary<int, int>();

        public void AddOneToBucket(int bucket)
        {            
            this.totalCount++;
            int value = 0;
            this.mapBucketToCount.TryGetValue(bucket, out value);
            value += 1;
            this.mapBucketToCount[bucket] = value;            
        }

        public void InitializeAllBucketsUpTo(int number)
        {
            for (int i = 0; i < number; ++i)
            {
                int value;
                if (!this.mapBucketToCount.TryGetValue(i, out value))
                {
                    this.mapBucketToCount[i] = 0;
                }
            }
        }

        public int GetXAxisValueCoveringUpTo(int threshhold)
        {                        
            float[] integratedData = GetYAxisIntegrated();
            int dataPoint = 0;
            for (; dataPoint < integratedData.Length-1; ++dataPoint)
            {
                if (integratedData[dataPoint] >= threshhold)
                {                    
                    break;
                }
            }

            return GetXAxis()[dataPoint];
        }

        public int[] GetXAxis(int threshHold = int.MaxValue)
        {
            return this.mapBucketToCount.Where(keyValuePair => keyValuePair.Key <= threshHold)
                                        .OrderBy(keyValuePair => keyValuePair.Key)
                                        .Select(keyValuePair => keyValuePair.Key)
                                        .ToArray();
        }

        public float[] GetYAxis(int threshHold = int.MaxValue)
        {
            return this.mapBucketToCount.Where(keyValuePair => keyValuePair.Key <= threshHold)
                                        .OrderBy(keyValuePair => keyValuePair.Key)
                                        .Select(keyValuePair => (float)keyValuePair.Value / this.totalCount * 100)
                                        .ToArray();
        }

        public float[] GetYAxisIntegrated(int threshHold = int.MaxValue)
        {
            float[] result = this.GetYAxis(threshHold);
            for (int i = 1; i < result.Length; ++i)
            {
                result[i] += result[i - 1];
            }

            return result;
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
