using System;
using System.Text;
using System.Collections.Generic;

namespace Grokkingly.ScratchPad.Algo
{
    class HeapBase
    {
        protected int[] A;
        protected int size;

        public HeapBase(int maxSize)
        {
            A = new int[maxSize];
            size = 0;
        }

        public HeapBase(int[] a)
        {
            A = a;
            size = a.Length;
        }

        protected int Parent(int index)
        {
            return Math.Abs((index - 1) / 2);
        }

        protected int Left(int index)
        {
            return index * 2 + 1;
        }

        protected int Right(int index)
        {
            return index * 2 + 2;
        }

        protected void Exchange(int[] a, int i, int j)
        {
            int temp = a[i];
            a[i] = a[j];
            a[j] = temp;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.GetType().ToString() + "{ Elements:" + size.ToString() + ", Array:{");

            for (int i = 0; i < size; i++)
                sb.Append(A[i].ToString() + ", ");

            sb.Append("}}");
            return sb.ToString();
        }
    }


    class MinHeap : HeapBase
    {
        public MinHeap(int maxSize)
            : base(maxSize)
        { }

        public MinHeap(int[] a)
            : base(a)
        { }

        protected void MinHeapify(int[] a, int i)
        {
            int smallest;
            int left = Left(i);
            int right = Right(i);

            if ((left < size) && (a[left] < a[i]))
                smallest = left;
            else
                smallest = i;

            if ((right < size) && (a[right] < a[smallest]))
                smallest = right;

            if (smallest != i)
            {
                Exchange(a, i, smallest);
                MinHeapify(a, smallest);
            }
        }

        private void BuildMinHeap(int[] a)
        {
            for (int i = (int)((a.Length / 2) - 1); i >= 0; i--)
                MinHeapify(a, i);
        }
    }


    class MaxHeap : HeapBase
    {
        public MaxHeap(int maxSize)
            : base(maxSize)
        { }

        public MaxHeap(int[] a)
            : base(a)
        { }

        protected void MaxHeapify(int[] a, int i)
        {
            int largest;
            int left = Left(i);
            int right = Right(i);

            if ((left < size) && (a[left] > a[i]))
                largest = left;
            else
                largest = i;

            if ((right < size) && (a[right] > a[largest]))
                largest = right;

            if (largest != i)
            {
                Exchange(a, i, largest);
                MaxHeapify(a, largest);
            }
        }

        private void BuildMaxHeap(int[] a)
        {
            for (int i = (int)((a.Length / 2) - 1); i >= 0; i--)
                MaxHeapify(a, i);
        }

        public void HeapSort()
        {
            BuildMaxHeap(A);
            for (int i = A.Length - 1; i >= 1; i--)
            {
                Exchange(A, 0, i);
                size -= 1;
                MaxHeapify(A, 0);
            }
        }
    }

    //PRIORITY QUEUES
    class MaxPriorityQueue : MaxHeap
    {
        public MaxPriorityQueue(int maxSize)
            : base(maxSize)
        { }

        public MaxPriorityQueue(int[] a)
            : base(a)
        { }

        public int Maximum
        {
            get { return A[0]; }
        }

        public int ExtractMax()
        {
            int max;

            if (size < 1)
                throw new Exception("Heap underflow");

            max = A[0];
            A[0] = A[size - 1];
            size -= 1;
            MaxHeapify(A, 0);

            return max;
        }

        public void IncreaseKey(int position, int key) //position is 1-based
        {
            int i = position - 1; //translate 1-based position to 0-based index

            if (key < A[i]) 
                throw new Exception("New key is smaller than current key!");

            A[i] = key;

            while ((i > 0) && (A[Parent(i)] < A[i]))
            {
                Exchange(A, i, Parent(i));
                i = Parent(i);
            }
        }

        public void Insert(int key)
        {
            size += 1;
            A[size] = int.MinValue;
            IncreaseKey(size, key);
        }
    }

    class MinPriorityQueue : MinHeap
    {
        public MinPriorityQueue(int maxSize)
            : base(maxSize)
        { }

        public MinPriorityQueue(int[] a)
            : base(a)
        { }

        public int Minimum
        {
            get { return A[0]; }
        }

        public int ExtractMin()
        {
            int min;

            if (size < 1)
                throw new Exception("Heap underflow");

            min = A[0];
            A[0] = A[size - 1];
            size -= 1;
            MinHeapify(A, 0);

            return min;
        }

        public void DecreaseKey(int position, int key) //position is 1-based
        {
            int i = position - 1; //translate 1-based position to 0-based index

            if (key > A[i])
                throw new Exception("New key is greater than current key!");

            A[i] = key;

            while ((i > 0) && (A[Parent(i)] > A[i]))
            {
                Exchange(A, i, Parent(i));
                i = Parent(i);
            }
        }

        public void Insert(int key)
        {
            size += 1;
            A[size] = int.MinValue;
            DecreaseKey(size, key);
        }
    }


    /// <summary>
    /// A Generic implementation of a heap using internal array.
    /// An IComparer object determines the heap type.
    /// </summary>
    class Heap<T>
    {
        const int MinLength = 16;
        // When do we resize the array, heap size as % of array length
        const int MinUtilization = 25;
        const int MaxUtilization = 75;
        // Internal array
        protected T[] data;
        // Number of elements in the heap structure
        protected int heapSize;
        // The IComparer in use makes it Min heap or Max heap
        protected IComparer<T> comparer;

        /// <summary>
        /// Creates empty heap with size 0 and default internal array length using the provided IComparer
        /// </summary>
        /// <param name="comparer"></param>
        public Heap(IComparer<T> comparer)
        {
            this.comparer = comparer;
            heapSize = 0;
            data = new T[MinLength];
        }

        /// <summary>
        /// creates heap from a source array using the provided IComparer
        /// </summary>
        /// <param name="source"></param>
        /// <param name="comparer"></param>
        public Heap(T[] source, IComparer<T> comparer)
        {
            this.comparer = comparer;
            heapSize = source.Length;

            int arrayLength = MinLength;
            while (source.Length * 100 / arrayLength > MaxUtilization)
                arrayLength *= 2;

            data = new T[arrayLength];
            Array.Copy(source, data, source.Length);
        }

        public bool IsEmpty
        {
            get { return heapSize == 0; }
        }

        private int Utilization
        {
            get { return (heapSize * 100 / data.Length); }
        }

        // Lengthens the array if MaxUtilization is reached
        protected void LengthenArray()
        {
            if (this.Utilization < MaxUtilization)
                return;

            ResizeArray(data.Length * 2);
        }

        // Shortens the array if MinUtilization is reached, while not going below MinLength
        protected void ShortenArray()
        {
            if (this.Utilization > MinUtilization)
                return;

            int newLength = data.Length / 2;
            newLength = Math.Max(newLength, MinLength);
            
            if (newLength != data.Length)
                ResizeArray(newLength);
        }

        private void ResizeArray(int newLength)
        {
            T[] newData = new T[newLength];
            Array.Copy(data, newData, data.Length);
            data = newData;
        }

        protected int Parent(int index)
        {
            return Math.Abs((index - 1) / 2);
        }

        protected void Exchange(int i, int j)
        {
            var temp = data[i];
            data[i] = data[j];
            data[j] = temp;
        }

        // Non-recursive implementation of heapify, using IComparer object
        protected void Heapify(int index)
        {
            int leftChild;
            int rightChild;
            int largerChild;
            
            var top = data[index];

            while (index < heapSize / 2)
            {
                leftChild = 2 * index + 1;
                rightChild = leftChild + 1;

                if ((rightChild < heapSize) && (comparer.Compare(data[rightChild], data[leftChild]) > 0))
                    largerChild = rightChild;
                else
                    largerChild = leftChild;

                if (comparer.Compare(top, data[largerChild]) >= 0)
                    break;

                data[index] = data[largerChild];
                index = largerChild;
            }

            data[index] = top;
        }

        private void BuildHeap()
        {
            for (int i = (heapSize / 2 - 1); i >= 0; i--)
                Heapify(i);
        }

        /// <summary>
        /// Sorts the heap in order determined by the IComparer object
        /// </summary>
        public void Sort()
        {
            int preserveSize = heapSize;

            BuildHeap();

            for (int i = heapSize - 1; i >= 1; i--)
            {
                Exchange(0, i);
                heapSize -= 1;
                Heapify(0);
            }

            heapSize = preserveSize;
        }

        /// <summary>
        /// Returns a copy of the internal array, trimmed to the length of the heap
        /// </summary>
        public T[] Data
        {
            get
            {
                var trimmedData = new T[heapSize];
                Array.Copy(data, trimmedData, trimmedData.Length);
                return trimmedData;
            }
        }

        public T Root
        {
            get { return data[0]; }
        }

        public T ExtractRoot()
        {
            T root;

            if (heapSize < 1)
                throw new Exception("Heap underflow");

            root = data[0];
            data[0] = data[heapSize - 1];
            heapSize -= 1;
            Heapify(0);

            ShortenArray();

            return root;
        }

        private void BubbleUp(int i) //the index i is 0-based
        {
            while ((i > 0) && (comparer.Compare(data[Parent(i)], data[i]) < 0))
            {
                Exchange(i, Parent(i));
                i = Parent(i);
            }
        }

        public void ChangeKey(int position, T key) //position is 1-based
        {
            int i = position - 1; //translate 1-based position to 0-based index

            if (comparer.Compare(key, data[i]) < 0)
                throw new Exception("Keys can only be increased in Max Priority Queue and decreased in Min Priority Queue!");

            data[i] = key;

            BubbleUp(i);
        }

        public void Insert(T key)
        {
            heapSize += 1;
            data[heapSize] = key;
            BubbleUp(heapSize);
            LengthenArray();
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(this.GetType().Name + "{ Length:" + heapSize.ToString() + ", Array:{");

            for (int i = 0; i < heapSize; i++)
                sb.Append(data[i].ToString() + ",");
            sb.Remove(sb.Length - 1, 1);
            
            sb.Append("}}");
            return sb.ToString();
        }
    }

    /// <summary>
    /// Given IComparable object, implements regular IComparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class RegularComparer<T> : IComparer<T>
        where T : IComparable<T>
    {
        public RegularComparer() { }

        int IComparer<T>.Compare(T one, T two)
        {
            return one.CompareTo(two); //regular comparison
        }
    }

    /// <summary>
    /// Given IComparable object, implements inverse IComparer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class InverseComparer<T> : IComparer<T>
    where T : IComparable<T>
    {
        public InverseComparer() { }

        int IComparer<T>.Compare(T one, T two)
        {
            return two.CompareTo(one); //inverse comparison
        }
    }

    /// <summary>
    /// Provides static methods for creation of Maximum and Minimum heaps of types that implement IComparable
    /// </summary>
    class HeapBuilder        
    {
        /// <summary>
        /// Creates an empty heap
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Heap<T> MaxHeap<T>() where T : IComparable<T>
        {
            var comparer = new RegularComparer<T>();
            return new Heap<T>(comparer);
        }

        /// <summary>
        /// Creates an empty heap using reversed comparison
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Heap<T> MinHeap<T>() where T : IComparable<T>
        {
            var comparer = new InverseComparer<T>();
            return new Heap<T>(comparer);
        }

        /// <summary>
        /// Creates heap from the provided array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Heap<T> MaxHeap<T>(T[] data) where T : IComparable<T>
        {
            var comparer = new RegularComparer<T>();
            return new Heap<T>(data, comparer);
        }

        /// <summary>
        /// Creates heap from the provided array, using reversed comparison
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Heap<T> MinHeap<T>(T[] data) where T : IComparable<T>
        {
            var comparer = new InverseComparer<T>();
            return new Heap<T>(data, comparer);
        }
    }
}
