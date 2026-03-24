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
    }
}