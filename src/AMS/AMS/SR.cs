using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace AMS
{
    class SR : ISyntaxReceiver
    {
        public List<AttributeSyntax> versions= new List<AttributeSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if(syntaxNode is AttributeSyntax asn)
            {
                if (asn.Name.ToFullString().Contains("VersionReleased"))
                {
                    versions.Add(asn);
                }

            }
        }
    }
}
