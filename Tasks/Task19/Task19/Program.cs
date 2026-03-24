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
    }
}