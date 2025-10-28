using TupleRewriterTesting.records;

namespace TupleRewriterTesting;

using System.Linq;
using JetBrains.Util;

public static class TupleLiteralRewriter
{
    /// <summary>
    /// Replace tuple literals with 'new [typeName](...)'.
    /// </summary>
    public static Block Rewrite(Block root, string typeName)
    {
        return (Block)RewriteStatement(root, typeName);
    }

    #region Statement Rewriting

    private static Stmt RewriteStatement(Stmt original, string typeName)
    {
        return original switch
        {
            VarDecl decl => new VarDecl(decl.Name, RewriteExpression(decl.Init, typeName)),
            Return ret => new Return(RewriteExpression(ret.Value, typeName)),
            Block block => RewriteBlockStatement(block, typeName),
            _ => original 
        };
    }

    private static Stmt RewriteBlockStatement(Block original, string typeName)
    {
        return new Block(
            Statements: original.Statements.Select(s => RewriteStatement(s, typeName))
                .ToIReadOnlyList()
        );
    }

    #endregion

    #region Expression Rewriting

    private static Expr RewriteExpression(Expr originalExpr, string typeName)
    {
        return originalExpr switch
        {
            NewExpr newExpr => RewriteNewExpression(newExpr, typeName),
            TupleLiteral tupleLiteral => RewriteTupleLiteral(tupleLiteral, typeName),
            _ => originalExpr // includes Id and Num
        };
    }

    private static Expr RewriteTupleLiteral(TupleLiteral tupleLiteral, string typeName)
    {
        return new NewExpr(
            TypeName: typeName,
            Args: tupleLiteral.Elements
                .Select(expr => RewriteExpression(expr, typeName))
                .ToIReadOnlyList()
        );
    }

    private static Expr RewriteNewExpression(NewExpr newExpr, string typeName)
    {
        return newExpr with
        {
            Args = newExpr.Args
                .Select(expr => RewriteExpression(expr, typeName))
                .ToIReadOnlyList()
        };
    }

    #endregion
}