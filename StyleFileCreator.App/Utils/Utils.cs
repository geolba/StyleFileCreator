using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ESRI.ArcGIS.Display;

namespace StyleFileCreator.App.Utils
{
    public static class Utils
    {
        public static void Write<T> (this IEnumerable<T> source)
        {
            foreach (var item in source)
            {
                Console.WriteLine(item);
            }
        }

        public static void AddItemList (this IStyleGallery source, List<IStyleGalleryItem> list)
        {
            int i = 0;
            list.ForEach(x =>
            {
                try
                {
                    source.AddItem(x);
                    i++;
                }
                catch (Exception ex)
                {
                    int test = i;
                    string name = x.Name;
                    Console.WriteLine(ex.Message);
                    return;
                }
            });
        }
           
        public static T FirstIfNotFound<T>(this IEnumerable<T> source, Func<T,bool> predicate)
        {
           if (source==null||predicate==null) throw new ArgumentNullException();
           return source.Any(predicate) ? source.FirstOrDefault(predicate) : source.FirstOrDefault();
        }
        
        /// <summary>
        ///     Start a function to run in a STA thread
        /// </summary>
        /// <typeparam name="T">Type of result returned by the function</typeparam>
        /// <param name="function">Delegate function to run in the STA thread</param>
        /// <returns>A task representing the function to be run in the STA thread</returns>
        public static Task<T> StartSTATask<T>(Func<T> function)
        {
            var completionSource = new TaskCompletionSource<T>();
            Thread backgroundThread = new Thread(() =>
            {
                try
                {
                    completionSource.SetResult(function());
                }
                catch (Exception e)
                {
                    completionSource.SetException(e);
                }
            });


            backgroundThread.SetApartmentState(ApartmentState.STA);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
            return completionSource.Task;
        }

        public static ListWithDuplicates<TKey, TElement> ToListWithDuplicates<TSource, TKey, TElement> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            return ToListWithDuplicates<TSource, TKey, TElement>(source, keySelector, elementSelector, null);
        }

        public static ListWithDuplicates<TKey, TElement> ToListWithDuplicates<TSource, TKey, TElement> (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            //if (source == null) throw Error.ArgumentNull("source");
            //if (keySelector == null) throw Error.ArgumentNull("keySelector");
            //if (elementSelector == null) throw Error.ArgumentNull("elementSelector");

            //Dictionary<TKey, TElement> d = new Dictionary<TKey, TElement>(comparer);
            //foreach (TSource element in source) d.Add(keySelector(element), elementSelector(element));
            //return d;

            var d = new ListWithDuplicates<TKey, TElement>();
            foreach (TSource element in source)
            {
                d.Add(keySelector(element), elementSelector(element));
            }
            return d;
        }
             
    }
}
