#if FW2
using System;
using System.Collections;
using System.Collections.Generic;

namespace CLAP
{
    #region Delegates

    public delegate void Action();
    public delegate void Action<T>(T arg);
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    public delegate TResult Func<TResult>();
    public delegate TResult Func<T, TResult>(T arg);
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);

    #endregion Delegates

    #region Enumerable

    internal static class Enumerable
    {
        public static IEnumerable<T> Cast<T>(this IEnumerable collection)
        {
            foreach (var item in collection)
            {
                yield return (T)item;
            }
        }

        public static IEnumerable<T> Where<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }

        public static T First<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                return item;
            }

            throw new InvalidOperationException("No elements in the collection that matches the predicate");
        }

        public static T First<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            throw new InvalidOperationException("No elements in the collection that matches the predicate");
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                return item;
            }

            return default(T);
        }

        public static T FirstOrDefault<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    return item;
                }
            }

            return default(T);
        }

        public static bool Any<T>(this IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                return true;
            }

            return false;
        }

        public static bool Any<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool All<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (!predicate(item))
                {
                    return false;
                }
            }

            return true;
        }

        public static int Count<T>(this IEnumerable<T> collection)
        {
            var c = 0;

            foreach (var item in collection)
            {
                c++;
            }

            return c;
        }

        public static int Count<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            var c = 0;

            foreach (var item in collection)
            {
                if (predicate(item))
                {
                    c++;
                }
            }

            return c;
        }

        public static T[] ToArray<T>(this IEnumerable<T> collection)
        {
            var arr = new T[collection.Count()];

            var i = 0;

            foreach (var item in collection)
            {
                arr[i] = item;

                i++;
            }

            return arr;
        }

        public static IEnumerable<TItem> OrderBy<TItem, TResult>(
            this IEnumerable<TItem> collection,
            Func<TItem, TResult> func)
        {
            var items = collection.ToArray();
            var keysList = new List<TResult>();

            foreach (var item in collection)
            {
                keysList.Add(func(item));
            }

            var keys = keysList.ToArray();

            Array.Sort(keys, items);

            return items;
        }

        public static IEnumerable<T> Skip<T>(this IEnumerable<T> collection, int count)
        {
            foreach (var item in collection)
            {
                count--;

                if (count >= 0)
                {
                    continue;
                }

                yield return item;
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(
            this IEnumerable<TSource> collection,
            Func<TSource, TResult> selector)
        {
            foreach (var item in collection)
            {
                yield return selector(item);
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(
            this IEnumerable<TSource> collection,
            Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var item in collection)
            {
                var innerItems = selector(item);

                foreach (var innerItem in innerItems)
                {
                    yield return innerItem;
                }
            }
        }

        public static IEnumerable<T> Distinct<T>(this IEnumerable<T> collection)
        {
            var list = new List<T>();

            foreach (var item in collection)
            {
                if (!list.Contains(item))
                {
                    list.Add(item);

                    yield return item;
                }
            }
        }

        public static IEnumerable<TSource> Union<TSource>(
            this IEnumerable<TSource> first,
            IEnumerable<TSource> second)
        {
            foreach (var item in first)
            {
                yield return item;
            }

            foreach (var item in second)
            {
                yield return item;
            }
        }

        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value)
        {
            foreach (var item in source)
            {
                if (object.Equals(item, value))
                {
                    return true;
                }
            }

            return false;
        }

        public static List<T> ToList<T>(this IEnumerable<T> collection)
        {
            var list = new List<T>();

            foreach (var item in collection)
            {
                list.Add(item);
            }

            return list;
        }
    }

    #endregion Enumerable
}

namespace System.Runtime.CompilerServices
{
    internal class ExtensionAttribute : Attribute
    {
    }
}
#endif