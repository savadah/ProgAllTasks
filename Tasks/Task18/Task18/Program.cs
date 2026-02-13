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

        map.Put(12, "twelve");
        map.Put(18, "eighteen");
        Console.WriteLine("FirstKey: " + map.FirstKey());
        Console.WriteLine("LastKey: " + map.LastKey());

        Console.WriteLine("FirstEntry: " + map.FirstEntry());
        Console.WriteLine("LastEntry: " + map.LastEntry());

        Console.WriteLine("PollFirstEntry: " + map.PollFirstEntry());
        Console.WriteLine("Размер после pollFirst: " + map.Size());

        Console.WriteLine("PollLastEntry: " + map.PollLastEntry());
        Console.WriteLine("Размер после pollLast: " + map.Size());

        Console.WriteLine("LowerKey(12): " + map.LowerKey(12));
        Console.WriteLine("FloorKey(12): " + map.FloorKey(12));
        Console.WriteLine("HigherKey(12): " + map.HigherKey(12));   
        Console.WriteLine("CeilingKey(11): " + map.CeilingKey(11)); 

        Console.WriteLine("LowerEntry(12): " + map.LowerEntry(12));
        Console.WriteLine("CeilingEntry(11): " + map.CeilingEntry(11));

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
            public Node Parent;
            public K Key;
            public V Value;
            public Node Left;
            public Node Right;

            public Node(K key, V value, Node parent)
            {
                Key = key;
                Value = value;
                Left = null;
                Right = null;
                Parent = parent;
            }
        }

        public class Entry
        {
            public K Key;
            public V Value;

            public Entry(K key, V value)
            {
                Key = key;
                Value = value;
            }

            public override string ToString()
            {
                return Key + "=" + Value;
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
                root = new Node(key, value, null);
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
                        current.Left = new Node(key, value, current);
                        size++;
                        return;
                    }
                    current = current.Left;
                }
                else
                {
                    if(current.Right == null)
                    {
                        current.Right = new Node(key, value, current);
                        size++;
                        return;
                    }
                    current = current.Right;
                }
            }
        }

        private void Transplant(Node u, Node v)
        {
            if (u.Parent == null)
            {
                root = v;
            }
            else if (u == u.Parent.Left)
            {
                u.Parent.Left = v;
            }
            else u.Parent.Right = v;
            if(v != null)
            {
                v.Parent = u.Parent;
            }
        }

        private Node MinNode(Node node)
        {
            Node current = node;
            while(current.Left != null)
            {
                current = current.Left;
            }
            return current;
        }

        public V Remove(K key)
        {
            Node z = FindNode(key);
            if(z == null)
            {
                return default(V);
            }

            V removedValue = z.Value;

            if(z.Left == null)
            {
                Transplant(z, z.Right);
            }
            else if (z.Right == null)
            {
                Transplant(z, z.Left);
            }
            else
            {
                Node y = MinNode(z.Right);

                if(y.Parent != z)
                {
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    if (y.Right != null) y.Right.Parent = y;
                }
                Transplant(z, y);
                y.Left = z.Left;
                if (y.Left != null) y.Left.Parent = y;
            }
            size--;
            return removedValue;
        }
        private Node MaxNode(Node node)
        {
            Node current = node;
            while(current.Right != null)
            {
                current = current.Right;
            }
            return current;
        }

        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            return MinNode(root).Key;
        }

        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            return MaxNode(root).Key;
        }

        public Entry FirstEntry()
        {
            if(root == null) throw new InvalidOperationException("Пусто");
            Node n = MinNode(root);
            return new Entry(n.Key, n.Value);
        }

        public Entry LastEntry()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            Node n = MaxNode(root);
            return new Entry(n.Key, n.Value);
        }

        public Entry PollFirstEntry()
        {
            if(root == null) throw new InvalidOperationException("Пусто");
            Node n = MinNode(root);
            Entry e = new Entry(n.Key, n.Value);
            Remove(n.Key);
            return e;
        }

        public Entry PollLastEntry()
        {
            if (root == null) throw new InvalidOperationException("Пусто");

            Node n = MaxNode(root);
            Entry e = new Entry(n.Key, n.Value);
            Remove(n.Key);
            return e;
        }

        private void TraverseInOrder(Node node, List<Node> list)
        {
            if (node == null) return;
            TraverseInOrder(node.Left, list);
            list.Add(node);
            TraverseInOrder(node.Right, list);
        }

        private List<Node> GetAllNodesInOrder()
        {
            List<Node> list = new List<Node>();
            TraverseInOrder(root, list);
            return list;
        }

        public Entry LowerEntry(K key)
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            List<Node> nodes = GetAllNodesInOrder();

            Node best = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) < 0)
                    best = nodes[i];
                else
                    break;
            }

            if (best == null) throw new InvalidOperationException("Нет меньшего ключа");
            return new Entry(best.Key, best.Value);
        }

        public K LowerKey(K key)
        {
            return LowerEntry(key).Key;
        }

        public Entry FloorEntry(K key)
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            List<Node> nodes = GetAllNodesInOrder();

            Node best = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) <= 0)
                    best = nodes[i];
                else
                    break;
            }

            if (best == null) throw new InvalidOperationException("Нет ключа меньше или равного");
            return new Entry(best.Key, best.Value);
        }

        public K FloorKey(K key)
        {
            return FloorEntry(key).Key;
        }

        public Entry HigherEntry(K key)
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) > 0)
                    return new Entry(nodes[i].Key, nodes[i].Value);
            }

            throw new InvalidOperationException("Нет большего ключа");
        }

        public K HigherKey(K key)
        {
            return HigherEntry(key).Key;
        }

        public Entry CeilingEntry(K key)
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            if (object.Equals(key, null)) throw new ArgumentNullException("key");

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) >= 0)
                    return new Entry(nodes[i].Key, nodes[i].Value);
            }

            throw new InvalidOperationException("Нет ключа больше или равного");
        }

        public K CeilingKey(K key)
        {
            return CeilingEntry(key).Key;
        }

    }
}