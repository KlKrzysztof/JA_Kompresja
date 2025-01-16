using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;




namespace JA_Kompresja
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TreeNode
    {
        public Int64 NodeByte { get; }
        public Int64 NodeValue {  get; }
        unsafe public TreeNode* LessNode {  get; }
        unsafe public TreeNode* GreaterNode { get; }

        unsafe public TreeNode()
        {
            NodeByte = 0;
            NodeValue = 0;
            LessNode = null;
            GreaterNode = null;
        }
    }

    internal struct TreeNodeIterator : IEnumerator<TreeNode>
    {
        private List< Tuple<TreeNode, char>> NestedNodes { get; set; }
        
        public TreeNode Current { get; set; }

        unsafe public bool MoveNext()
        {
            
            NestedNodes.RemoveAt(NestedNodes.Count - 1);
            while (NestedNodes.Last().Item2 != '0')
            {
                NestedNodes.RemoveAt(NestedNodes.Count - 1);
                if (NestedNodes.Count == 0) return false;
            }

            var currentNode = NestedNodes.Last().Item1;
            NestedNodes.RemoveAt(NestedNodes.Count - 1);
            NestedNodes.Add( new Tuple<TreeNode, char>(currentNode, '1'));

            var ptr = NestedNodes.Last().Item1.GreaterNode;
            NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));

            while (NestedNodes.Last().Item1.LessNode != null)
            {
                ptr = NestedNodes.Last().Item1.LessNode;
                NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));
            }

            Current = *ptr;
            return true;
               /* NestedNodes.Insert(NestedNodes.Count-1, new Tuple<TreeNode, char>(Current, '1'));
                Current = *NestedNodes.Last().Item1.GreaterNode;
                while (NestedNodes.Last().Item1.LessNode == null)
                {
                    var ptr = NestedNodes.Last().Item1.LessNode;
                    NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));
                    Current = *ptr;
                }
                return true;*/
            
            /*else 
            {
                while (NestedNodes.Last().Item2 != '0')
                {
                    NestedNodes.RemoveAt(NestedNodes.Count - 1);
                    if (NestedNodes.Count == 0) return false;
                }
                while (NestedNodes.Last().Item1.LessNode != null)
                {
                    var ptr = NestedNodes.Last().Item1.LessNode;
                    NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));
                    Current = *ptr;
                }
                return true;
            }*/
        }

        unsafe public TreeNodeIterator(TreeNode parent)
        {
            NestedNodes = new List<Tuple<TreeNode, char>>();
            NestedNodes.Add(new Tuple<TreeNode, char>(parent, '0'));
            while(NestedNodes.Last().Item1.LessNode != null)
            {
                var ptr = NestedNodes.Last().Item1.LessNode;
                NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));
                Current = *ptr;
            }
        }

        unsafe public void Reset()
        {
            var parent = NestedNodes.First().Item1;
            NestedNodes = new List<Tuple<TreeNode, char>>();
            NestedNodes.Add(new Tuple<TreeNode, char>(parent, '0'));
            while (NestedNodes.Last().Item1.LessNode != null)
            {
                var ptr = NestedNodes.Last().Item1.LessNode;
                NestedNodes.Add(new Tuple<TreeNode, char>(*ptr, '0'));
                Current = *ptr;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        void IDisposable.Dispose()
        {

        }

        public char[] getCode()
        {
            char[] code = new char[NestedNodes.Count-1];
            for (int i = 0; i < NestedNodes.Count-1; i++)
            {
                code[i] = NestedNodes[i].Item2;
            }

            return code;
        }
    }
}
