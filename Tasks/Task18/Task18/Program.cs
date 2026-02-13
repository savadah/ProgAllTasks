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

        Console.WriteLine("ContainsValue(\"TEN\"): " + map.ContainsValue("TEN"));
        Console.WriteLine("ContainsValue(\"nope\"): " + map.ContainsValue("nope"));

        Console.WriteLine("KeySet: " + string.Join(", ", map.KeySet()));
        Console.WriteLine("EntrySet: " + string.Join(", ", map.EntrySet()));

        var head = map.HeadMap(12);
        Console.WriteLine("HeadMap(<12): " + string.Join(", ", head.EntrySet()));

        var tail = map.TailMap(12);
        Console.WriteLine("TailMap(>=12): " + string.Join(", ", tail.EntrySet()));

        var sub = map.SubMap(10, 15);
        Console.WriteLine("SubMap([10..15)): " + string.Join(", ", sub.EntrySet()));

        map.Clear();
        Console.WriteLine("После clear size: " + map.Size());
       }

    public class MyTreeMap<K, V>
    {
        private readonly IComparer<K> comparator;
        private Node root;
        private int size;

        // (Node) Внутренний узел дерева: хранит ключ/значение и ссылки на детей + родителя
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

        // (Entry) Пара "ключ-значение" для возврата 
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
        // 1
        public MyTreeMap()
        {
            comparator = null;
            root = null;
            size = 0;
        }
        // 2
        public MyTreeMap(IComparer<K> comp)
        {
            if (comp == null) throw new ArgumentNullException("comp");
            comparator = comp;
            root = null;
            size = 0;
        }

        // (CompareKeys) Универсальное сравнение ключей через comparator (если есть) или через IComparable<K>
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

        // (FindNode) Поиск узла по ключу в BST (идём влево/вправо по сравнению ключей)
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
        // 12
        public int Size()
        {
            return size;
        }
        // 8
        public bool IsEmpty()
        {
            return size == 0;
        }
        // 3
        public void Clear()
        {
            root = null;
            size = 0;
        }
        // 4
        public bool ContainsKey(K key)
        {
            return FindNode(key) != null;
        }
        // 7
        public V Get(K key)
        {
            Node node = FindNode(key);
            if (node == null) return default(V);
            return node.Value;
        }
        // 10
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

        // (Transplant) Замена одного поддерева другим: ставит v на место u (нужно для удаления)
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

        // (MinNode) Минимальный узел в поддереве (самый левый) — нужен для first/pollFirst и Remove
        private Node MinNode(Node node)
        {
            Node current = node;
            while(current.Left != null)
            {
                current = current.Left;
            }
            return current;
        }
        // 11
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
        // (MaxNode) Максимальный узел в поддереве (самый правый) — нужен для last/pollLast
        private Node MaxNode(Node node)
        {
            Node current = node;
            while(current.Right != null)
            {
                current = current.Right;
            }
            return current;
        }
        // 13
        public K FirstKey()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            return MinNode(root).Key;
        }
        // 14
        public K LastKey()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            return MaxNode(root).Key;
        }
        // 28
        public Entry FirstEntry()
        {
            if(root == null) throw new InvalidOperationException("Пусто");
            Node n = MinNode(root);
            return new Entry(n.Key, n.Value);
        }
        // 29
        public Entry LastEntry()
        {
            if (root == null) throw new InvalidOperationException("Пусто");
            Node n = MaxNode(root);
            return new Entry(n.Key, n.Value);
        }
        // 26
        public Entry PollFirstEntry()
        {
            if(root == null) throw new InvalidOperationException("Пусто");
            Node n = MinNode(root);
            Entry e = new Entry(n.Key, n.Value);
            Remove(n.Key);
            return e;
        }
        // 27
        public Entry PollLastEntry()
        {
            if (root == null) throw new InvalidOperationException("Пусто");

            Node n = MaxNode(root);
            Entry e = new Entry(n.Key, n.Value);
            Remove(n.Key);
            return e;
        }

        // (TraverseInOrder) Обход дерева (по возрастанию ключей) и сбор узлов в список
        private void TraverseInOrder(Node node, List<Node> list)
        {
            if (node == null) return;
            TraverseInOrder(node.Left, list);
            list.Add(node);
            TraverseInOrder(node.Right, list);
        }

        // (GetAllNodesInOrder) Возвращает все узлы дерева в отсортированном порядке (используется в set/range/near методах)
        private List<Node> GetAllNodesInOrder()
        {
            List<Node> list = new List<Node>();
            TraverseInOrder(root, list);
            return list;
        }
        // 18
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
        // 22
        public K LowerKey(K key)
        {
            return LowerEntry(key).Key;
        }
        // 19
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
        // 23
        public K FloorKey(K key)
        {
            return FloorEntry(key).Key;
        }
        // 29
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
        //24
        public K HigherKey(K key)
        {
            return HigherEntry(key).Key;
        }
        // 21
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
        // 25
        public K CeilingKey(K key)
        {
            return CeilingEntry(key).Key;
        }
        // 5
        public bool ContainsValue(V value)
        {
            if (root == null) return false;

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (object.Equals(nodes[i].Value, value))
                    return true;
            }

            return false;
        }
        // 9
        public List<K> KeySet()
        {
            List<Node> nodes = GetAllNodesInOrder();
            List<K> result = new List<K>();

            for (int i = 0; i < nodes.Count; i++)
                result.Add(nodes[i].Key);

            return result;
        }
        //6
        public List<Entry> EntrySet()
        {
            List<Node> nodes = GetAllNodesInOrder();
            List<Entry> result = new List<Entry>();

            for (int i = 0; i < nodes.Count; i++)
                result.Add(new Entry(nodes[i].Key, nodes[i].Value));

            return result;
        }
        // 15
        public MyTreeMap<K, V> HeadMap(K end)
        {
            if (object.Equals(end, null)) throw new ArgumentNullException("end");

            MyTreeMap<K, V> result = (comparator == null)
                ? new MyTreeMap<K, V>()
                : new MyTreeMap<K, V>(comparator);

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, end) < 0)
                    result.Put(nodes[i].Key, nodes[i].Value);
                else
                    break;
            }

            return result;
        }
        // 17
        public MyTreeMap<K, V> TailMap(K start)
        {
            if (object.Equals(start, null)) throw new ArgumentNullException("start");

            MyTreeMap<K, V> result = (comparator == null)
                ? new MyTreeMap<K, V>()
                : new MyTreeMap<K, V>(comparator);

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, start) >= 0)
                    result.Put(nodes[i].Key, nodes[i].Value);
            }

            return result;
        }
        // 16
        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            if (object.Equals(start, null)) throw new ArgumentNullException("start");
            if (object.Equals(end, null)) throw new ArgumentNullException("end");

            if (CompareKeys(start, end) > 0)
                throw new ArgumentException("start должен быть <= end");

            MyTreeMap<K, V> result = (comparator == null)
                ? new MyTreeMap<K, V>()
                : new MyTreeMap<K, V>(comparator);

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, start) >= 0 && CompareKeys(nodes[i].Key, end) < 0)
                {
                    result.Put(nodes[i].Key, nodes[i].Value);
                }
                else if (CompareKeys(nodes[i].Key, end) >= 0)
                {
                    break;
                }
            }

            return result;
        }

    }
}