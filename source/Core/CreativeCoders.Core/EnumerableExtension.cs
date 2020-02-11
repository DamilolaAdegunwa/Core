﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CreativeCoders.Core.Comparing;
using JetBrains.Annotations;

namespace CreativeCoders.Core
{
    [PublicAPI]
    public static class EnumerableExtension
    {
        public static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            Ensure.IsNotNull(action, nameof(action));

            foreach (var item in self)
            {
                action(item);
            }
        }

        public static void ForEach<T>(this IEnumerable self, Action<T> action)
        {
            Ensure.IsNotNull(action, nameof(action));

            foreach (var obj in self)
            {
                if (obj is T variable)
                {
                    action(variable);
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
        {
            Ensure.IsNotNull(action, nameof(action));

            var index = 0;
            foreach (var element in source)
            {
                action(element, index++);
            }
        }

        public static void ForEach(this IEnumerable source, Action<object, int> action)
        {
            Ensure.IsNotNull(action, nameof(action));

            var index = 0;
            foreach (var element in source)
            {
                action(element, index++);
            }
        }

        public static void ForEach<T>(this Array self, Action<T> action)
        {
            Ensure.IsNotNull(action, nameof(action));

            (self as IEnumerable).ForEach(action);
        }

        public static IEnumerable<T> Pipe<T>(this IEnumerable<T> items, Action<T> pipeAction)
        {
            Ensure.IsNotNull(pipeAction, nameof(pipeAction));

            foreach (var item in items)
            {
                pipeAction(item);
                yield return item;
            }
        }

        public static IEnumerable<T> TakeUntil<T>(this IEnumerable<T> items, Func<T, bool> untilFunc)
        {
            Ensure.IsNotNull(untilFunc, nameof(untilFunc));

            foreach (var item in items)
            {
                yield return item;

                if (untilFunc(item))
                {
                    yield break;
                }
            }
        }

        public static IEnumerable<T> SkipUntil<T>(this IEnumerable<T> items, Func<T, bool> untilFunc)
        {
            Ensure.IsNotNull(untilFunc, nameof(untilFunc));

            var checkUntil = true;

            foreach (var item in items)
            {
                if (checkUntil)
                {
                    if (untilFunc(item))
                    {
                        checkUntil = false;
                    }

                    continue;
                }

                yield return item;
            }
        }

        public static IEnumerable<T> TakeEvery<T>(this IEnumerable<T> items, int step)
        {
            return items.Where((x, index) => index % step == 0);
        }

        public static void Remove<T>(this ICollection<T> self, IEnumerable<T> removeEntries)
        {
            Ensure.IsNotNull(removeEntries, nameof(removeEntries));

            foreach (var removeEntry in removeEntries)
            {
                self.Remove(removeEntry);
            }
        }

        public static void Remove<T>(this ICollection<T> self, Predicate<T> removeFunc)
        {
            Ensure.IsNotNull(removeFunc, nameof(removeFunc));

            var removeEntries = self.Where(item => removeFunc(item)).ToArray();
            self.Remove(removeEntries);
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> self)
            where T : class
        {
            return self.Where(item => item != null);
        }

        public static bool IsSingle<T>(this IEnumerable<T> items, Func<T, bool> predicateFunc)
        {
            Ensure.IsNotNull(predicateFunc, nameof(predicateFunc));

            var num = 0;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in items)
            {
                // ReSharper disable once InvertIf
                if (predicateFunc(item))
                {
                    if (++num > 1)
                    {
                        return false;
                    }
                }
            }

            return num == 1;
        }

        public static bool IsSingle<T>(this IEnumerable<T> items)
        {
            return items.IsSingle(item => true);
        }

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelectorFunc)
        {
            return items.Distinct(new FuncEqualityComparer<T, TKey>(keySelectorFunc));
        }

        public static IEnumerable<T> Distinct<T, TKey1, TKey2>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2)
        {
            return items.Distinct(new MultiFuncEqualityComparer<T, TKey1, TKey2>(keySelectorFunc1, keySelectorFunc2));
        }

        public static IEnumerable<T> Distinct<T, TKey1, TKey2, TKey3>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3)
        {
            return items.Distinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3));
        }

        public static IEnumerable<T> Distinct<T, TKey1, TKey2, TKey3, TKey4>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3,
            Func<T, TKey4> keySelectorFunc4)
        {
            return items.Distinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3, TKey4>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3, keySelectorFunc4));
        }

        public static IEnumerable<T> Distinct<T, TKey1, TKey2, TKey3, TKey4, TKey5>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3,
            Func<T, TKey4> keySelectorFunc4, Func<T, TKey5> keySelectorFunc5)
        {
            return items.Distinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3, TKey4, TKey5>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3, keySelectorFunc4, keySelectorFunc5));
        }

        public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> items,
            params Func<T, TKey>[] keySelectorFunctions)
        {
            return items.Distinct(new MultiFuncEqualityComparer<T, TKey>(keySelectorFunctions));
        }

        public static IEnumerable<T> NotDistinct<T>(this IEnumerable<T> items)
        {
            return items.NotDistinct(x => x);
        }

        public static IEnumerable<T> NotDistinct<T, TKey>(this IEnumerable<T> items, Func<T, TKey> keySelectorFunc)
        {
            return items.NotDistinct(new FuncEqualityComparer<T, TKey>(keySelectorFunc));
        }

        public static IEnumerable<T> NotDistinct<T, TKey1, TKey2>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2)
        {
            return items.NotDistinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2>(keySelectorFunc1, keySelectorFunc2));
        }

        public static IEnumerable<T> NotDistinct<T, TKey1, TKey2, TKey3>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3)
        {
            return items.NotDistinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3));
        }

        public static IEnumerable<T> NotDistinct<T, TKey1, TKey2, TKey3, TKey4>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3,
            Func<T, TKey4> keySelectorFunc4)
        {
            return items.NotDistinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3, TKey4>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3, keySelectorFunc4));
        }

        public static IEnumerable<T> NotDistinct<T, TKey1, TKey2, TKey3, TKey4, TKey5>(this IEnumerable<T> items,
            Func<T, TKey1> keySelectorFunc1, Func<T, TKey2> keySelectorFunc2, Func<T, TKey3> keySelectorFunc3,
            Func<T, TKey4> keySelectorFunc4, Func<T, TKey5> keySelectorFunc5)
        {
            return items.NotDistinct(
                new MultiFuncEqualityComparer<T, TKey1, TKey2, TKey3, TKey4, TKey5>(keySelectorFunc1, keySelectorFunc2,
                    keySelectorFunc3, keySelectorFunc4, keySelectorFunc5));
        }

        public static IEnumerable<T> NotDistinct<T, TKey>(this IEnumerable<T> items,
            params Func<T, TKey>[] keySelectorFunctions)
        {
            return items.NotDistinct(new MultiFuncEqualityComparer<T, TKey>(keySelectorFunctions));
        }

        public static IEnumerable<T> NotDistinct<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            var bufferedItems = items.SelectWithIndex().ToArray();
            var foundDuplicates = new List<int>();

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var item in bufferedItems)
            {
                var duplicates = bufferedItems
                    .Where(x =>
                        !foundDuplicates.Contains(x.Index) && comparer.Equals(item.Data, x.Data))
                    .ToArray();

                // ReSharper disable once InvertIf
                if (duplicates.Length > 1)
                {
                    foundDuplicates.AddRange(duplicates.Select(x => x.Index));

                    foreach (var duplicate in duplicates)
                    {
                        yield return duplicate.Data;
                    }
                }
            }
        }

        public static IEnumerable<ItemWithIndex<T>> SelectWithIndex<T>(this IEnumerable<T> items)
        {
            return items.Select((x, index) => new ItemWithIndex<T>(index, x));
        }

        public static IOrderedEnumerable<T> OrderBy<T, TKey1, TKey2>(this IEnumerable<T> items, Func<T, TKey1> keySelector1,
            Func<T, TKey2> keySelector2)
        {
            return items.Sort(new SortFieldInfo<T, TKey1>(keySelector1, SortOrder.Ascending),
                new SortFieldInfo<T, TKey2>(keySelector2, SortOrder.Ascending));
        }

        public static IOrderedEnumerable<T> OrderByDescending<T, TKey1, TKey2>(this IEnumerable<T> items,
            Func<T, TKey1> keySelector1, Func<T, TKey2> keySelector2)
        {
            return items.Sort(new SortFieldInfo<T, TKey1>(keySelector1, SortOrder.Descending),
                new SortFieldInfo<T, TKey2>(keySelector2, SortOrder.Descending));
        }

        public static IOrderedEnumerable<T> Sort<T, TKey1, TKey2>(this IEnumerable<T> items,
            SortFieldInfo<T, TKey1> sortFieldInfo1, SortFieldInfo<T, TKey2> sortFieldInfo2)
        {
            return items.OrderBy(x => x, new MultiFuncComparer<T, TKey1, TKey2>(sortFieldInfo1, sortFieldInfo2));
        }

        public static IOrderedEnumerable<T> Sort<T, TKey1, TKey2, TKey3>(this IEnumerable<T> items,
            SortFieldInfo<T, TKey1> sortFieldInfo1, SortFieldInfo<T, TKey2> sortFieldInfo2,
            SortFieldInfo<T, TKey3> sortFieldInfo3)
        {
            return items.OrderBy(x => x,
                new MultiFuncComparer<T, TKey1, TKey2, TKey3>(sortFieldInfo1, sortFieldInfo2, sortFieldInfo3));
        }

        public static IOrderedEnumerable<T> Sort<T, TKey1, TKey2, TKey3, TKey4>(this IEnumerable<T> items,
            SortFieldInfo<T, TKey1> sortFieldInfo1, SortFieldInfo<T, TKey2> sortFieldInfo2,
            SortFieldInfo<T, TKey3> sortFieldInfo3, SortFieldInfo<T, TKey4> sortFieldInfo4)
        {
            return items.OrderBy(x => x,
                new MultiFuncComparer<T, TKey1, TKey2, TKey3, TKey4>(sortFieldInfo1, sortFieldInfo2, sortFieldInfo3,
                    sortFieldInfo4));
        }

        public static IOrderedEnumerable<T> Sort<T, TKey1, TKey2, TKey3, TKey4, TKey5>(this IEnumerable<T> items,
            SortFieldInfo<T, TKey1> sortFieldInfo1, SortFieldInfo<T, TKey2> sortFieldInfo2,
            SortFieldInfo<T, TKey3> sortFieldInfo3, SortFieldInfo<T, TKey4> sortFieldInfo4,
            SortFieldInfo<T, TKey5> sortFieldInfo5)
        {
            return items.OrderBy(x => x,
                new MultiFuncComparer<T, TKey1, TKey2, TKey3, TKey4, TKey5>(sortFieldInfo1, sortFieldInfo2,
                    sortFieldInfo3, sortFieldInfo4, sortFieldInfo5));
        }

        public static IEnumerable<TResult> Choose<T, TResult>(this IEnumerable<T> items,
            Func<T, (bool IsChoosen, TResult Value)> chooseFunc)
        {
            Ensure.IsNotNull(chooseFunc, nameof(chooseFunc));

            return items
                .Select(chooseFunc)
                .Where(x => x.IsChoosen)
                .Select(x => x.Value);
        }
    }
}