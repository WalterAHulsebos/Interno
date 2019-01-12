using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Heap
{
    public class Heap<T> where T : class, IHeapable<T>
    {
        private T[] items;
        private int currentItemCount = 0;

        public Heap(int size)
        {
            items = new T[size];
        }

        // Add an item and sort it based on it's CompareTo function
        public void Add(T item)
        {
            item.HeapIndex = currentItemCount;
            items[currentItemCount] = item;
            SortUp(item);
            currentItemCount++;
        }

        // Get the top from the heap
        public T Get()
        {
            T item = items[0];
            currentItemCount--;
            items[0] = items[currentItemCount];
            items[0].HeapIndex = 0;
            SortDown(items[0]);
            return item;
        }

        public int Count
        {
            get
            {
                return currentItemCount;
            }
        }

        public bool Contains(T item)
        {
            return Equals(items[item.HeapIndex], item);
        }

        public void Clear()
        {
            for (int i = 0; i < currentItemCount; i++)
                items[i] = null;
            currentItemCount = 0;
        }

        // Sort an item up based on it's value, where the best will be on top
        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;
            T parentItem;

            while (true)
            {
                parentItem = items[parentIndex];

                if (item.CompareTo(parentItem) > 0)
                    Swap(item, parentItem);
                else
                    break;

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        // Sort an item down based on it's value, where the best will be on top
        private void SortDown(T item)
        {
            int left, right, swapIndex;

            while (true)
            {
                left = item.HeapIndex * 2 + 1;
                right = left + 1;
                swapIndex = 0;

                if (left < currentItemCount)
                {
                    swapIndex = left;

                    if (right < currentItemCount)
                        if (items[left].CompareTo(items[right]) < 0)
                            swapIndex = right;

                    if (item.CompareTo(items[swapIndex]) < 0)
                        Swap(item, items[swapIndex]);
                    else
                        return;
                }
                else
                    return;
            }
        }

        // Swap two items based on their indexes
        private void Swap(T a, T b)
        {
            items[a.HeapIndex] = b;
            items[b.HeapIndex] = a;

            int index = a.HeapIndex;
            a.HeapIndex = b.HeapIndex;
            b.HeapIndex = index;
        }
    }

    public interface IHeapable<T>
    {
        int HeapIndex { get; set; }
        int CompareTo(T other);
    }
}
