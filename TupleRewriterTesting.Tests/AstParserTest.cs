using JetBrains.Util;
using Shouldly;
using TupleRewriterTesting.records;

namespace TupleRewriterTesting.Tests;

public class AstParserTest
{
    IReadOnlyList<Expr> ReadOnlyList(params Expr[] elements)
    {
        return elements.ToList().ToIReadOnlyList();
        // had to do it this way because the object types have to be the same. This was the only way. :/
    }

    IReadOnlyList<Stmt> ReadOnlyList(params Stmt[] elements)
    {
        return elements.ToList().ToIReadOnlyList();
    }

    private Block ParseInput(string input)
    {
        var tokenizer = new DelimitedTokenizer(input);
        var parser = new SimpleAstParser(tokenizer);
        return parser.ParseBlock();
    }

    [Fact]
    public void Parser_ShouldHandleEmptyBlock()
    {
        var block = ParseInput("{}");
        Assert.Empty(block.Statements);
    }

    [Fact]
    public void Parser_ShouldParse_SingleVarDecl_WithLeafExpression()
    {
        // Source: "var count = 42;"
        var input = "var count = 42;";

        var expectedAst = new Block(ReadOnlyList(
            new VarDecl("count", new Num("42"))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldParse_SingleReturn_WithLeafExpression()
    {
        var input = "return my_id;";

        var expectedAst = new Block(ReadOnlyList(
            new Return(new Id("my_id"))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldParse_NewExpr_WithArguments()
    {
        var input = "var v = new Vector(x, 10);";

        var expectedAst = new Block(ReadOnlyList(
            new VarDecl("v", new NewExpr("Vector", ReadOnlyList(
                new Id("x"),
                new Num("10")
            )))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Praser_ShouldParse_NewExpr_NoArguments()
    {
        var input = "var v = new Vector();";

        var expectedAst = new Block(ReadOnlyList(
            new VarDecl("v", new NewExpr("Vector", new List<Expr>()))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldParse_TupleLiteral_Simple()
    {
        var input = "var t = (a, 10, b);";

        var expectedAst = new Block(ReadOnlyList(
            new VarDecl("t", new TupleLiteral(ReadOnlyList(
                new Id("a"),
                new Num("10"),
                new Id("b")
            )))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldParse_NestedBlock()
    {
        var input = "{ var x = 1; { return 2; } }";

        var expectedAst = new Block(ReadOnlyList(
            new VarDecl("x", new Num("1")),
            new Block(ReadOnlyList(
                new Return(new Num("2"))
            ))
        ));

        var actualAst = ParseInput(input);
        actualAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldThrowException_WhenSemicolonIsMissing()
    {
        var input = "var x = 1 return 2;";

        var exception = Record.Exception(() => ParseInput(input));

        Assert.NotNull(exception);
        Assert.IsType<Exception>(exception);
        Assert.Contains("Expected ';'", exception.Message);
    }

    [Fact]
    public void Parser_ShouldCorrectlyParse_test1_Resource()
    {
        // Source (test1/original.txt): "{ var p = (10, 20); return new Vec3(p, 0); }"

        var originalAst = TestResourceManager.ParseOriginalBlock("test1");

        var expectedAst = new Block(ReadOnlyList(
            // var p = (10, 20);
            new VarDecl("p", new TupleLiteral(ReadOnlyList(
                new Num("10"),
                new Num("20")
            ))),

            // return new Vec3(p, 0);
            new Return(new NewExpr("Vec3", ReadOnlyList(
                new Id("p"),
                new Num("0")
            )))
        ));
        originalAst.ShouldBeEquivalentTo(expectedAst);
    }

    [Fact]
    public void Parser_ShouldCorrectlyParse_test1_Resource_original()
    {
        var originalAst = TestResourceManager.ParseOriginalBlock("test2");
        /*
        var vec = new Vector((p, 5), 10);
        {
            var r = (p, vec);
            return new Model(r, (30, 40));
        }

         */

        var expected = new Block(ReadOnlyList(
            new VarDecl("vec", new NewExpr("Vector",
                ReadOnlyList(
                    new TupleLiteral(ReadOnlyList(new Id("p"),
                        new Num("5"))
                    ), new Num("10")))),
            new Block(ReadOnlyList(
                new VarDecl("r", new TupleLiteral(ReadOnlyList(new Id("p"), new Id("vec")))),
                new Return(new NewExpr(
                    "Model",
                    ReadOnlyList(
                        new Id("r"),
                        new TupleLiteral(ReadOnlyList(
                                new Num("30"),
                                new Num("40")
                            )
                        )
                    )
                ))
            ))
        )); 
        
        originalAst.ShouldBeEquivalentTo(expected);
    }
}