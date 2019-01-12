using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Core.Sorting
{
    public static class Sorter
    {
        public static void SpeciateSort<T>(this List<T> list) where T : IComparable<T>, ISortingLayer<T>
        {
            int listCount = list.Count, index, compareValue;
            T current, other;

            for (int i = 1; i < listCount; i++)
            {
                index = i;
                current = list[index];

                while(index > 0)
                {
                    other = list[index - 1];
                    compareValue = current.CompareToLayer(other);
                    if (compareValue > 0)
                        break;        
                    if (compareValue == 0)
                        if (current.CompareTo(other) > 0)
                            break;              
                    list[index] = other;
                    index--;
                    list[index] = current;
                }
            }
        }
    }

    public interface ISortingLayer<T>
    {
        int CompareToLayer(T other);
    }
}
