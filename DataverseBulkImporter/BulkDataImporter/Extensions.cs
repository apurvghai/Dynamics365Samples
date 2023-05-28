using System.Collections.Generic;

namespace Microsoft.Support.Dataverse.Samples
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T>(this IEnumerable<T> items, int size)
        {
            List<List<T>> batches = new List<List<T>>();
            int count = 0;
            List<T> batch = new List<T>();

            foreach (T item in items)
            {
                if (count++ == size)
                {
                    batches.Add(batch);
                    batch = new List<T>();
                    count = 1;
                }

                batch.Add(item);
            }

            batches.Add(batch);

            return batches;
        }
    }
}
