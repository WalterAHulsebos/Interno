using System;
using System.Collections.Generic;
using Random = System.Random;

namespace Core.Utilities.DataManagement
{
    public class List<T> : System.Collections.Generic.List<T>
    {
        public List() : base() { Initialize(); }
        public List(int size) : base(size) { Initialize(); }

        protected Random random;

        protected virtual void Initialize()
        {
            random = new Random(DateTime.Now.GetHashCode());
        }

        public T First
        {
            get
            {
                return this[0];
            }
            set
            {
                this[0] = value;
            }
        }

        public T Last
        {
            get
            {
                return this[Count - 1];
            }
            set
            {
                this[Count - 1] = value;
            }
        }

        public T Random
        {
            get
            {
                return this[random.Next(0, Count - 1)];
            }
        }

        #region Conversion
        public static implicit operator List<T>(T[] array)
        {
            int count = array.Length;
            List<T> ret = new List<T>(count);
            foreach (T t in array)
                ret.Add(t);
            return ret;
        }

        public static implicit operator List<T>(T[,] array)
        {
            int count = array.Length;
            List<T> ret = new List<T>(count);
            foreach (T t in array)
                ret.Add(t);
            return ret;
        }
        #endregion

        #region Operators
        public static List<T> operator +(List<T> a, System.Collections.Generic.List<T> b)
        {
            foreach (T t in b)
                a.Add(t);
            return a;
        }

        public static List<T> operator -(List<T> a, System.Collections.Generic.List<T> b)
        {
            foreach (T t in b)
                a.Remove(t);
            return a;
        }

        public static List<T> operator +(List<T> a, T[] b)
        {
            foreach (T t in b)
                a.Add(t);
            return a;
        }

        public static List<T> operator -(List<T> a, T[] b)
        {
            foreach (T t in b)
                if (a.Contains(t))
                    a.Remove(t);
            return a;
        }
        #endregion

        #region Sorting
        public void OrderBy(Func<T, int> func)
        {
            int listCount = Count, index;
            T current, other;
            int currentValue;

            for (int i = 1; i < listCount; i++)
            {
                index = i;
                current = this[index];
                currentValue = func(current);

                while (index > 0)
                {
                    other = this[index - 1];
                    if (func(other) > currentValue)
                        break;
                    this[index] = other;
                    index--;
                    this[index] = current;
                }
            }
        }

        public void OrderBySpeciated(Func<T, int> func, Func<T, int> speciate)
        {
            int listCount = Count, index;
            T current, other;
            int currentValue, currentSpeciateValue;

            for (int i = 1; i < listCount; i++)
            {
                index = i;
                current = this[index];
                currentValue = func(current);
                currentSpeciateValue = speciate(current);

                while (index > 0)
                {
                    other = this[index - 1];
                    if (currentSpeciateValue > speciate(other))
                        break;
                    if (currentSpeciateValue == speciate(other))
                        if (currentValue > func(other))
                            break;
                    this[index] = other;
                    index--;
                    this[index] = current;
                }
            }
        }
        #endregion

        #region Get
        public T Get(Func<T, bool> func)
        {
            foreach (T t in this)
                if (func(t))
                    return t;
            return default;
        }

        public void GetAll(Func<T, bool> func, System.Collections.Generic.List<T> list)
        {
            foreach (T t in this)
                if (func(t))
                    list.Add(t);
        }

        public T Extract(Func<T, bool> func)
        {
            foreach (T t in this)
                if (func(t))
                {
                    T ret = t;
                    Remove(t);
                    return ret;
                }
            return default;
        }

        public void ExtractAll(Func<T, bool> func, System.Collections.Generic.List<T> list)
        {
            int count = Count;
            for (int i = count - 1; i >= 0; i--)
            {
                if (!func(this[i]))
                    continue;
                list.Add(this[i]);
                RemoveAt(i);
            }
        }
        #endregion

        #region Utility
        public void Do(Func<T, T> func)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
                this[i] = func(this[i]);
        }

        public void Do(Action<T> action)
        {
            int count = Count;
            for (int i = 0; i < count; i++)
                action(this[i]);
        }

        public void RemoveDoubles(Func<T, T, bool> compare)
        {
            int count = Count;
            for (int i = count - 1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (!compare(this[i], this[j]))
                        continue;
                    RemoveAt(i);
                    break;
                }
            }
        }

        public float GetValue(Func<T, float> func)
        {
            float ret = 0;
            foreach (T t in this)
                ret += func(t);
            return ret;
        }
        #endregion
    }

    public class SimpleArray<T>
    {
        private T[] content;

        public SimpleArray(int size)
        {
            content = new T[size];
        }

        public T this[int index]
        {
            get
            {
                return content[index];
            }
            set
            {
                content[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return content.Length;
            }
        }

        public int Count { get; private set; } = 0;

        public void Add(T t)
        {
            content[Count] = t;
            Count++;
        }

        public void Remove(T t)
        {
            content[Count] = default;
            Count--;
        }
    }
}