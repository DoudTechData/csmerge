﻿#region License

// SLNTools
// Copyright (c) 2009 
// by Christian Warren
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

#endregion

using System.Collections.Generic;
using System.Windows.Forms;

namespace CWDev.SLNTools.UIKit
{
    using Core.Merge;

    public partial class ConflictsControl : TreeView
    {
        public NodeConflict Data
        {
            set
            {
                this.Nodes.Clear();
                if (value != null)
                {
                    foreach (Conflict conflict in value.Subconflicts)
                    {
                        TreeNode node = this.Nodes.Add(conflict.ToString());
                        FillConflictNode(node, conflict);
                    }
                    this.Sort();
                }
             }
        }

        private void FillConflictNode(TreeNode node, Conflict conflict)
        {
            node.Tag = conflict;

            NodeConflict nodeConflict = conflict as NodeConflict;
            if (nodeConflict != null)
            {
                if (nodeConflict.AcceptedSubdifferences.Count > 0)
                {
                    TreeNode acceptedNode = node.Nodes.Add("Accepted Differences");
                    foreach (Difference subdifference in nodeConflict.AcceptedSubdifferences)
                    {
                        TreeNode subnode = acceptedNode.Nodes.Add(subdifference.ToString());
                        FillDifferenceNode(subnode, subdifference);
                    }
                }

                TreeNode conflictNode = node.Nodes.Add("Conflicts");
                foreach (Conflict subconflict in nodeConflict.Subconflicts)
                {
                    TreeNode subnode = conflictNode.Nodes.Add(subconflict.ToString());
                    FillConflictNode(subnode, subconflict);
                }
            }
        }

        private void FillDifferenceNode(TreeNode node, Difference difference)
        {
            node.Tag = difference;

            NodeDifference nodeDifference = difference as NodeDifference;
            if (nodeDifference != null)
            {
                foreach (Difference subdifference in nodeDifference.Subdifferences)
                {
                    TreeNode subnode = node.Nodes.Add(subdifference.ToString());
                    FillDifferenceNode(subnode, subdifference);
                }
            }
        }
    }
}
