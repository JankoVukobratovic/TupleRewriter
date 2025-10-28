using Shouldly;
using TupleRewriterTesting.records;

namespace TupleRewriterTesting.Tests;

public class TupleLiteralRewriterTest
{
    private void AssertAstEquivalence(Block ast, string expected)
    {
        var expectedAst = SimpleAstParser.ParseWithDefaults(expected);
        ast.ShouldBeEquivalentTo(expectedAst);
    }

    private void AssertRewrittenAstEquivalence(string input, string typename, string expected)
    {
        var inputAst = SimpleAstParser.ParseWithDefaults(input);
        var generatedAst = TupleLiteralRewriter.Rewrite(inputAst, typename);
        AssertAstEquivalence(generatedAst, expected);
    }
    
    [Fact]
    public void Rewriter_ShouldRewriteSimpleDeclarationStatement()
    {
        const string input = "var a = (b, 15);";
        const string expected = "var a = new Foo(b, 15);";
        AssertRewrittenAstEquivalence(input, typename: "Foo" ,expected);
    }
    
    [Fact]
    public void Rewriter_ShouldRewriteTupleInReturnStatement()
    {
        const string input = "return (x, y, z);";
        const string expected = "return new Bar(x, y, z);";

        AssertRewrittenAstEquivalence(input, "Bar", expected);
    }
    
    [Fact]
    public void Rewriter_ShouldRewriteTupleNestedInNewExpression()
    {
        const string input = "var v = new Vector(p, (x, 1));";
        const string expected = "var v = new Vector(p, new Point(x, 1));";
        
        AssertRewrittenAstEquivalence(input, "Point", expected);
    }
    
    [Fact]
    public void Rewriter_ShouldRewriteTupleNestedInTuple()
    {
        const string input = "var t = (a, (1, 2));";
        
        const string expected = "var t = new Container(a, new Container(1, 2));";
        
        AssertRewrittenAstEquivalence(input, "Container", expected);
    }
    
    [Fact]
    public void Rewriter_ShouldHandleNoTupleExpression()
    {
        const string input = "var x = new Data(42);";
        
        AssertRewrittenAstEquivalence(input, "Slay", input);
    }

    [Fact]
    public void Rewriter_ShouldHandleBigASTTree() // I really dont know how to name this better
    {
        var sourceAst = TestResourceManager.ParseOriginalBlock("test2");
        var generatedAst = TupleLiteralRewriter.Rewrite(sourceAst,"Foo");
        TestResourceManager.AssertAstEqualsToExpectedEquivalent(generatedAst, "test2");
    }
}