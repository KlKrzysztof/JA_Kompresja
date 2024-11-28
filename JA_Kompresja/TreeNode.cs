using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JA_Kompresja
{
    struct TreeNode
    {
        private readonly Int64 nodeByte;
        private readonly Int64 nodeValue;
        unsafe private readonly TreeNode* lessNode;
        unsafe private readonly TreeNode* greaterNode;

        unsafe public TreeNode()
        {
            nodeByte = 0;
            nodeValue = 0;
            lessNode = null;
            greaterNode = null;
        }
    }
}
