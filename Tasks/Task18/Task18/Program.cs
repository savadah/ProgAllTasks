using System;
using System.Collections.Generic;

class Program
{
       static void Main()
    {
        
    }

    public class MyTreeMap<K, V>
    {
        private readonly IComparer<K> comparator;
        private Node root;
        private int size;

        private class Node
        {
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;

            public Node(K key, V value)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
            }
        }

        public MyTreeMap()
        {
            comparator = null;
            root = null;
            size = 0;
        }

        public MyTreeMap(IComparer<K> comp)
        {
            if (comp == null) throw new ArgumentNullException("comp");
            comparator = comp;
            root = null;
            size = 0;
        }

        private int CompareKeys(K a, K b)
        {
            if (object.Equals(a, null)) throw new ArgumentNullException("a");
            if (object.Equals(b, null)) throw new ArgumentNullException("b");

            if(comparator != null)
            {
                return comparator.Compare(a, b);
            }

            IComparable<K> cmp = a as IComparable<K>;
            if (cmp != null)
            {
                return cmp.CompareTo(b);
            }
            else throw new InvalidOperationException("Тип ключа " + typeof(K).Name + " не поддерживает сравнение");
        }
    }
}