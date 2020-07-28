/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;
using OneScript.Language.LexicalAnalysis;

namespace OneScript.Language.SyntaxAnalysis.AstNodes
{
    public class LineMarkerNode : BslSyntaxNode
    {
        public LineMarkerNode(CodeRange location)
        {
            Location = location;
            Kind = NodeKind.BlockEnd;
        }
        
        public LineMarkerNode(CodeRange location, int kind)
        {
            Location = location;
            Kind = kind;
        }
        
        public override IReadOnlyList<BslSyntaxNode> Children => new BslSyntaxNode[0];
    }
}