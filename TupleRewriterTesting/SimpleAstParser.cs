using JetBrains.Util;
using TupleRewriterTesting.records;

namespace TupleRewriterTesting;

/// <summary>
/// Effectively a stack-based parser that utilizes the actual program call stack
/// </summary>
public class SimpleAstParser(Tokenizer tokenizer)
{
    public Block ParseBlock()
    {
        bool braced = tokenizer.Check("{");
        if (braced)
            tokenizer.Consume(); // make it optional 


        var statements = new List<Stmt>();
        // Block ends when the token stream is exhausted or a } is hit (depending on whether it was started by a "{"
        while (tokenizer.Peek() != null && !tokenizer.Check("}"))
        {
            statements.Add(ParseStatement());
            tokenizer.Expect(";");
        }

        if (braced) // if it was opened by "{" it has to be closed by "}" 
        {
            tokenizer.Expect("}");
        }

        return new Block(statements);
    }

    private Stmt ParseStatement()
    {
        string token = tokenizer.Peek() ?? throw new Exception("Expected variable name after 'var'.");
        return token switch
        {
            "var" => ParseVarDecl(),
            "return" => ParseReturn(),
            "{" => ParseBlock(),
            _ => throw new Exception($"Unexpected token at start of statement: {token}")
        };
    }

    private Stmt ParseVarDecl()
    {
        tokenizer.Expect("var");
        string name = tokenizer.Consume() ?? throw new Exception("Expected variable name after 'var'.");

        tokenizer.Expect("=");
        Expr init = ParseExpression();

        return new VarDecl(name, init);
    }

    private Stmt ParseReturn()
    {
        tokenizer.Expect("return");

        var value = ParseExpression();

        return new Return(value);
    }

    public Expr ParseExpression()
    {
        if (tokenizer.Check("("))
        {
            return ParseTupleLiteral();
        }

        if (tokenizer.Check("new"))
        {
            return ParseNewExpr();
        }

        return ParseLeafExpression();
    }

    private Expr ParseNewExpr()
    {
        tokenizer.Expect("new");

        string typeName = tokenizer.Consume() ?? throw new Exception("Expected type name after 'new'.");

        tokenizer.Expect("(");

        var args = ParseExpressionList(")"); // Parse the arguments until ')'

        tokenizer.Expect(")");

        return new NewExpr(typeName, args);
    }

    private TupleLiteral ParseTupleLiteral()
    {
        //todo way more sense for ( and ) to be in the sub-method thb
        tokenizer.Expect("(");

        var elements = ParseExpressionList(")");

        tokenizer.Expect(")");
        return new TupleLiteral(elements);
    }

    /// <summary>
    /// Utility method to parse comma-separated expressions (used by TupleLiteral and NewExpr).
    /// </summary>
    private IReadOnlyList<Expr> ParseExpressionList(string closingDelimiter)
    {
        var elements = new List<Expr>();

        if (tokenizer.Check(closingDelimiter)) return elements;

        elements.Add(ParseExpression());

        while (tokenizer.Check(","))
        {
            tokenizer.Consume();
            elements.Add(ParseExpression());
        }

        return elements;
    }

    private Expr ParseLeafExpression()
    {
        string token = tokenizer.Consume() ?? throw new Exception("Expected Identifier or Number.");

        if (double.TryParse(token, out _))
        {
            return new Num(token);
        }

        return new Id(token);
    }
}