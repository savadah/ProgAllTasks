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

        private bool IsRedNode(Node node)
        {
            if (node == null)
            {
                return false;
            }

            return node.IsRed;
        }

        private bool IsBlackNode(Node node)
        {
            return !IsRedNode(node);
        }

        private void SetRed(Node node)
        {
            if (node != null)
            {
                node.IsRed = true;
            }
        }

        private void SetBlack(Node node)
        {
            if (node != null)
            {
                node.IsRed = false;
            }
        }

        private Node ParentOf(Node node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Parent;
        }

        private Node LeftOf(Node node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Left;
        }

        private Node RightOf(Node node)
        {
            if (node == null)
            {
                return null;
            }

            return node.Right;
        }

        private void RotateLeft(Node node)
        {
            if (node == null)
            {
                return;
            }

            Node rightChild = node.Right;

            if (rightChild == null)
            {
                return;
            }

            node.Right = rightChild.Left;

            if (rightChild.Left != null)
            {
                rightChild.Left.Parent = node;
            }

            rightChild.Parent = node.Parent;

            if (node.Parent == null)
            {
                root = rightChild;
            }
            else if (node == node.Parent.Left)
            {
                node.Parent.Left = rightChild;
            }
            else
            {
                node.Parent.Right = rightChild;
            }

            rightChild.Left = node;
            node.Parent = rightChild;
        }

        private void RotateRight(Node node)
        {
            if (node == null)
            {
                return;
            }

            Node leftChild = node.Left;

            if (leftChild == null)
            {
                return;
            }

            node.Left = leftChild.Right;

            if (leftChild.Right != null)
            {
                leftChild.Right.Parent = node;
            }

            leftChild.Parent = node.Parent;

            if (node.Parent == null)
            {
                root = leftChild;
            }
            else if (node == node.Parent.Right)
            {
                node.Parent.Right = leftChild;
            }
            else
            {
                node.Parent.Left = leftChild;
            }

            leftChild.Right = node;
            node.Parent = leftChild;
        }

        //10
        public void Put(K key, V value)
        {
            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            if (root == null)
            {
                root = new Node(key, value, null);
                SetBlack(root);
                size = 1;
                return;
            }

            Node current = root;
            Node parent = null;
            int cmp = 0;

            while (current != null)
            {
                parent = current;
                cmp = CompareKeys(key, current.Key);

                if (cmp < 0)
                {
                    current = current.Left;
                }
                else if (cmp > 0)
                {
                    current = current.Right;
                }
                else
                {
                    current.Value = value;
                    return;
                }
            }

            Node newNode = new Node(key, value, parent);

            if (cmp < 0)
            {
                parent.Left = newNode;
            }
            else
            {
                parent.Right = newNode;
            }

            FixAfterInsertion(newNode);
            size++;
        }

        private void FixAfterInsertion(Node node)
        {
            while (node != null && node != root && IsRedNode(ParentOf(node)))
            {
                Node parent = ParentOf(node);
                Node grand = ParentOf(parent);

                if (parent == LeftOf(grand))
                {
                    Node uncle = RightOf(grand);

                    if (IsRedNode(uncle))
                    {
                        SetBlack(parent);
                        SetBlack(uncle);
                        SetRed(grand);
                        node = grand;
                    }
                    else
                    {
                        if (node == RightOf(parent))
                        {
                            node = parent;
                            RotateLeft(node);
                            parent = ParentOf(node);
                            grand = ParentOf(parent);
                        }

                        SetBlack(parent);
                        SetRed(grand);
                        RotateRight(grand);
                    }
                }
                else
                {
                    Node uncle = LeftOf(grand);

                    if (IsRedNode(uncle))
                    {
                        SetBlack(parent);
                        SetBlack(uncle);
                        SetRed(grand);
                        node = grand;
                    }
                    else
                    {
                        if (node == LeftOf(parent))
                        {
                            node = parent;
                            RotateRight(node);
                            parent = ParentOf(node);
                            grand = ParentOf(parent);
                        }

                        SetBlack(parent);
                        SetRed(grand);
                        RotateLeft(grand);
                    }
                }
            }

            SetBlack(root);
        }

        private Node FindNode(K key)
        {
            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            Node current = root;

            while (current != null)
            {
                int cmp = CompareKeys(key, current.Key);

                if (cmp == 0)
                {
                    return current;
                }

                if (cmp < 0)
                {
                    current = current.Left;
                }
                else
                {
                    current = current.Right;
                }
            }

            return null;
        }

        private Node MinNode(Node node)
        {
            if (node == null)
            {
                return null;
            }

            Node current = node;

            while (current.Left != null)
            {
                current = current.Left;
            }

            return current;
        }

        private Node MaxNode(Node node)
        {
            if (node == null)
            {
                return null;
            }

            Node current = node;

            while (current.Right != null)
            {
                current = current.Right;
            }

            return current;
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
            else
            {
                u.Parent.Right = v;
            }

            if (v != null)
            {
                v.Parent = u.Parent;
            }
        }

        private void FixAfterDeletion(Node node, Node parent)
        {
            while (node != root && IsBlackNode(node))
            {
                if (node == LeftOf(parent))
                {
                    Node brother = RightOf(parent);

                    if (IsRedNode(brother))
                    {
                        SetBlack(brother);
                        SetRed(parent);
                        RotateLeft(parent);
                        brother = RightOf(parent);
                    }

                    if (IsBlackNode(LeftOf(brother)) && IsBlackNode(RightOf(brother)))
                    {
                        SetRed(brother);
                        node = parent;
                        parent = ParentOf(node);
                    }
                    else
                    {
                        if (IsBlackNode(RightOf(brother)))
                        {
                            SetBlack(LeftOf(brother));
                            SetRed(brother);
                            RotateRight(brother);
                            brother = RightOf(parent);
                        }

                        if (IsRedNode(parent))
                        {
                            SetRed(brother);
                        }
                        else
                        {
                            SetBlack(brother);
                        }

                        SetBlack(parent);
                        SetBlack(RightOf(brother));
                        RotateLeft(parent);
                        node = root;
                    }
                }
                else
                {
                    Node brother = LeftOf(parent);

                    if (IsRedNode(brother))
                    {
                        SetBlack(brother);
                        SetRed(parent);
                        RotateRight(parent);
                        brother = LeftOf(parent);
                    }

                    if (IsBlackNode(LeftOf(brother)) && IsBlackNode(RightOf(brother)))
                    {
                        SetRed(brother);
                        node = parent;
                        parent = ParentOf(node);
                    }
                    else
                    {
                        if (IsBlackNode(LeftOf(brother)))
                        {
                            SetBlack(RightOf(brother));
                            SetRed(brother);
                            RotateLeft(brother);
                            brother = LeftOf(parent);
                        }

                        if (IsRedNode(parent))
                        {
                            SetRed(brother);
                        }
                        else
                        {
                            SetBlack(brother);
                        }

                        SetBlack(parent);
                        SetBlack(LeftOf(brother));
                        RotateRight(parent);
                        node = root;
                    }
                }
            }

            SetBlack(node);
        }

        private void TraverseInOrder(Node node, List<Node> list)
        {
            if (node == null)
            {
                return;
            }

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

        //11
        public V Remove(K key)
        {
            Node z = FindNode(key);

            if (z == null)
            {
                return default(V);
            }

            V removedValue = z.Value;
            Node y = z;
            bool yWasRed = y.IsRed;
            Node x;
            Node xParent;

            if (z.Left == null)
            {
                x = z.Right;
                xParent = z.Parent;
                Transplant(z, z.Right);
            }
            else if (z.Right == null)
            {
                x = z.Left;
                xParent = z.Parent;
                Transplant(z, z.Left);
            }
            else
            {
                y = MinNode(z.Right);
                yWasRed = y.IsRed;
                x = y.Right;

                if (y.Parent == z)
                {
                    xParent = y;
                }
                else
                {
                    xParent = y.Parent;
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    y.Right.Parent = y;
                }

                Transplant(z, y);
                y.Left = z.Left;
                y.Left.Parent = y;
                y.IsRed = z.IsRed;
            }

            size--;

            if (!yWasRed)
            {
                FixAfterDeletion(x, xParent);
            }

            return removedValue;
        }

        //12
        public int Size()
        {
            return size;
        }

        //8
        public bool IsEmpty()
        {
            return size == 0;
        }

        //3
        public void Clear()
        {
            root = null;
            size = 0;
        }

        //4
        public bool ContainsKey(K key)
        {
            return FindNode(key) != null;
        }

        //7
        public V Get(K key)
        {
            Node node = FindNode(key);

            if (node == null)
            {
                return default(V);
            }

            return node.Value;
        }

        //13
        public K FirstKey()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            return MinNode(root).Key;
        }

        //14
        public K LastKey()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            return MaxNode(root).Key;
        }

        //28
        public Entry FirstEntry()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            Node node = MinNode(root);
            return new Entry(node.Key, node.Value);
        }

        //29
        public Entry LastEntry()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            Node node = MaxNode(root);
            return new Entry(node.Key, node.Value);
        }

        //26
        public Entry PollFirstEntry()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            Node node = MinNode(root);
            Entry result = new Entry(node.Key, node.Value);
            Remove(node.Key);
            return result;
        }

        //27
        public Entry PollLastEntry()
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            Node node = MaxNode(root);
            Entry result = new Entry(node.Key, node.Value);
            Remove(node.Key);
            return result;
        }

        //18
        public Entry LowerEntry(K key)
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            List<Node> nodes = GetAllNodesInOrder();
            Node best = null;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) < 0)
                {
                    best = nodes[i];
                }
                else
                {
                    break;
                }
            }

            if (best == null)
            {
                throw new InvalidOperationException("Нет меньшего ключа");
            }

            return new Entry(best.Key, best.Value);
        }

        //22
        public K LowerKey(K key)
        {
            return LowerEntry(key).Key;
        }

        //19
        public Entry FloorEntry(K key)
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            List<Node> nodes = GetAllNodesInOrder();
            Node best = null;

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) <= 0)
                {
                    best = nodes[i];
                }
                else
                {
                    break;
                }
            }

            if (best == null)
            {
                throw new InvalidOperationException("Нет ключа меньше или равного");
            }

            return new Entry(best.Key, best.Value);
        }

        //23
        public K FloorKey(K key)
        {
            return FloorEntry(key).Key;
        }

        //20
        public Entry HigherEntry(K key)
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) > 0)
                {
                    return new Entry(nodes[i].Key, nodes[i].Value);
                }
            }

            throw new InvalidOperationException("Нет большего ключа");
        }

        //24
        public K HigherKey(K key)
        {
            return HigherEntry(key).Key;
        }

        //21
        public Entry CeilingEntry(K key)
        {
            if (root == null)
            {
                throw new InvalidOperationException("Пусто");
            }

            if (object.Equals(key, null))
            {
                throw new ArgumentNullException("key");
            }

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, key) >= 0)
                {
                    return new Entry(nodes[i].Key, nodes[i].Value);
                }
            }

            throw new InvalidOperationException("Нет ключа больше или равного");
        }

        //25
        public K CeilingKey(K key)
        {
            return CeilingEntry(key).Key;
        }

        //5
        public bool ContainsValue(V value)
        {
            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (object.Equals(nodes[i].Value, value))
                {
                    return true;
                }
            }

            return false;
        }

        //9
        public List<K> KeySet()
        {
            List<Node> nodes = GetAllNodesInOrder();
            List<K> result = new List<K>();

            for (int i = 0; i < nodes.Count; i++)
            {
                result.Add(nodes[i].Key);
            }

            return result;
        }

        //6
        public List<Entry> EntrySet()
        {
            List<Node> nodes = GetAllNodesInOrder();
            List<Entry> result = new List<Entry>();

            for (int i = 0; i < nodes.Count; i++)
            {
                result.Add(new Entry(nodes[i].Key, nodes[i].Value));
            }

            return result;
        }

        //15
        public MyTreeMap<K, V> HeadMap(K end)
        {
            if (object.Equals(end, null))
            {
                throw new ArgumentNullException("end");
            }

            MyTreeMap<K, V> result;

            if (comparator == null)
            {
                result = new MyTreeMap<K, V>();
            }
            else
            {
                result = new MyTreeMap<K, V>(comparator);
            }

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, end) < 0)
                {
                    result.Put(nodes[i].Key, nodes[i].Value);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        //17
        public MyTreeMap<K, V> TailMap(K start)
        {
            if (object.Equals(start, null))
            {
                throw new ArgumentNullException("start");
            }

            MyTreeMap<K, V> result;

            if (comparator == null)
            {
                result = new MyTreeMap<K, V>();
            }
            else
            {
                result = new MyTreeMap<K, V>(comparator);
            }

            List<Node> nodes = GetAllNodesInOrder();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (CompareKeys(nodes[i].Key, start) >= 0)
                {
                    result.Put(nodes[i].Key, nodes[i].Value);
                }
            }

            return result;
        }

        //16
        public MyTreeMap<K, V> SubMap(K start, K end)
        {
            if (object.Equals(start, null))
            {
                throw new ArgumentNullException("start");
            }

            if (object.Equals(end, null))
            {
                throw new ArgumentNullException("end");
            }

            if (CompareKeys(start, end) > 0)
            {
                throw new ArgumentException("start должен быть <= end");
            }

            MyTreeMap<K, V> result;

            if (comparator == null)
            {
                result = new MyTreeMap<K, V>();
            }
            else
            {
                result = new MyTreeMap<K, V>(comparator);
            }

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

    public class MyTreeSet<T>
    {
        private MyTreeMap<T, object> m;
        private static readonly object PRESENT = new object();

        //1
        public MyTreeSet()
        {
            m = new MyTreeMap<T, object>();
        }

        //2
        public MyTreeSet(MyTreeMap<T, object> map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            m = map;
        }

        //3
        public MyTreeSet(IComparer<T> comparator)
        {
            if (comparator == null)
            {
                throw new ArgumentNullException("comparator");
            }

            m = new MyTreeMap<T, object>(comparator);
        }

        //4
        public MyTreeSet(T[] a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            m = new MyTreeMap<T, object>();

            for (int i = 0; i < a.Length; i++)
            {
                Add(a[i]);
            }
        }

        //5
        public MyTreeSet(SortedSet<T> s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }

            m = new MyTreeMap<T, object>(s.Comparer);

            foreach (T item in s)
            {
                Add(item);
            }
        }

        //6
        public bool Add(T e)
        {
            bool had = m.ContainsKey(e);

            if (had)
            {
                return false;
            }

            m.Put(e, PRESENT);
            return true;
        }

        //7
        public bool AddAll(T[] a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            bool changed = false;

            for (int i = 0; i < a.Length; i++)
            {
                if (Add(a[i]))
                {
                    changed = true;
                }
            }

            return changed;
        }

        //8
        public void Clear()
        {
            m.Clear();
        }

        //9
        public bool Contains(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (!(o is T))
            {
                return false;
            }

            return m.ContainsKey((T)o);
        }

        //10
        public bool ContainsAll(T[] a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            for (int i = 0; i < a.Length; i++)
            {
                if (!Contains(a[i]))
                {
                    return false;
                }
            }

            return true;
        }

        //11
        public bool IsEmpty()
        {
            return m.IsEmpty();
        }

        //12
        public bool Remove(object o)
        {
            if (o == null)
            {
                return false;
            }

            if (!(o is T))
            {
                return false;
            }

            T value = (T)o;

            if (!m.ContainsKey(value))
            {
                return false;
            }

            m.Remove(value);
            return true;
        }

        //13
        public bool RemoveAll(T[] a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            bool changed = false;

            for (int i = 0; i < a.Length; i++)
            {
                if (Remove(a[i]))
                {
                    changed = true;
                }
            }

            return changed;
        }

        //14
        public bool RetainAll(T[] a)
        {
            if (a == null)
            {
                throw new ArgumentNullException("a");
            }

            List<T> current = m.KeySet();
            bool changed = false;

            for (int i = 0; i < current.Count; i++)
            {
                bool found = false;

                for (int j = 0; j < a.Length; j++)
                {
                    if (object.Equals(current[i], a[j]))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Remove(current[i]);
                    changed = true;
                }
            }

            return changed;
        }

        //15
        public int Size()
        {
            return m.Size();
        }

        //16
        public object[] ToArray()
        {
            List<T> keys = m.KeySet();
            object[] result = new object[keys.Count];

            for (int i = 0; i < keys.Count; i++)
            {
                result[i] = keys[i];
            }

            return result;
        }

        //17
        public T[] ToArray(T[] a)
        {
            List<T> keys = m.KeySet();
            T[] result;

            if (a == null || a.Length < keys.Count)
            {
                result = new T[keys.Count];
            }
            else
            {
                result = a;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                result[i] = keys[i];
            }

            return result;
        }

        //18
        public T First()
        {
            return m.FirstKey();
        }

        //19
        public T Last()
        {
            return m.LastKey();
        }

        //20
        public MyTreeSet<T> SubSet(T fromElement, T toElement)
        {
            MyTreeMap<T, object> subMap = m.SubMap(fromElement, toElement);
            return new MyTreeSet<T>(subMap);
        }

        //21
        public MyTreeSet<T> HeadSet(T toElement)
        {
            MyTreeMap<T, object> headMap = m.HeadMap(toElement);
            return new MyTreeSet<T>(headMap);
        }

        //22
        public MyTreeSet<T> TailSet(T fromElement)
        {
            MyTreeMap<T, object> tailMap = m.TailMap(fromElement);
            return new MyTreeSet<T>(tailMap);
        }
    }
}