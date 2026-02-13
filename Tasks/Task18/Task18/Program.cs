using System;
using System.Collections.Generic;

class Program
{
       static void Main()
       {
        var map = new MyTreeMap<int, string>();

        Console.WriteLine("Пусто? " + map.IsEmpty());
        map.Put(10, "ten");
        map.Put(5, "five");
        map.Put(15, "fifteen");

        Console.WriteLine("Размер: " + map.Size());
        Console.WriteLine("Get(10): " + (map.Get(10) ?? "null"));
        Console.WriteLine("Get(5): " + (map.Get(5) ?? "null"));
        Console.WriteLine("Есть ли ключ 15? " + map.ContainsKey(15));

        map.Put(10, "TEN");
        Console.WriteLine("Get(10) после замены: " + (map.Get(10) ?? "null"));
        Console.WriteLine("Размер после замены: " + map.Size());

        map.Clear();
        Console.WriteLine("После clear size: " + map.Size());
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

        private Node FindNode(K key)
        {
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            Node current = root;

            while(current != null)
            {
                int cmp = CompareKeys(key, current.Key);
                
                if(cmp == 0)
                {
                    return current;
                }
                if (cmp < 0)
                {
                    current = current.Left;
                }
                else current = current.Right;
            }
            return null;
        }

        public int Size()
        {
            return size;
        }

        public bool IsEmpty()
        {
            return size == 0;
        }

        public void Clear()
        {
            root = null;
            size = 0;
        }

        public bool ContainsKey(K key)
        {
            return FindNode(key) != null;
        }

        public V Get(K key)
        {
            Node node = FindNode(key);
            if (node == null) return default(V);
            return node.Value;
        }

        public void Put(K key, V value)
        {
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            if(root == null)
            {
                root = new Node(key, value);
                size = 1;
                return;
            }
            Node current = root;

            while(true)
            {
                int cmp = CompareKeys(key, current.Key);
                if(cmp == 0)
                {
                    current.Value = value;
                    return;
                }
                else if(cmp < 0)
                {
                    if(current.Left == null)
                    {
                        current.Left = new Node(key, value);
                        size++;
                        return;
                    }
                    current = current.Left;
                }
                else
                {
                    if(current.Right == null)
                    {
                        current.Right = new Node(key, value);
                        size++;
                        return;
                    }
                    current = current.Right;
                }
            }
        }
    }
}