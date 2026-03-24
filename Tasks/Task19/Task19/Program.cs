using System;
using System.Collections.Generic;

namespace Task19
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }

    public class MyTreeMap<K, V>
    {
        private Node root;
        private int size;
        private IComparer<K> comparator;

        private class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;
            public Node Parent;
            public bool IsRed;

            public Node(K key, V value, Node parent)
            {
                Key = key;
                Value = value;
                Parent = parent;
                Left = null;
                Right = null;
                IsRed = true;
            }
        }

        //1
        public MyTreeMap()
        {
            root = null;
            size = 0;
            comparator = null;
        }

        //2
        public MyTreeMap(IComparer<K> comp)
        {
            if (comp == null)
            {
                throw new ArgumentNullException("comp");
            }

            root = null;
            size = 0;
            comparator = comp;
        }

        public int CompareKeys(K a, K b)
        {
            if (object.Equals(a, null))
            {
                throw new ArgumentNullException("a");
            }

            if (object.Equals(b, null))
            {
                throw new ArgumentNullException("b");
            }

            if (comparator != null)
            {
                return comparator.Compare(a, b);
            }

            IComparable<K> cmp = a as IComparable<K>;

            if (cmp != null)
            {
                return cmp.CompareTo(b);
            }

            throw new InvalidOperationException("Тип ключа не поддерживает сравнение");
        }
    }
}