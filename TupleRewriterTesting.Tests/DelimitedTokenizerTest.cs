namespace TupleRewriterTesting.Tests;

using TupleRewriterTesting;


public class DelimitedTokenizerTest
{
    [Fact]
    public void Tokenizer_ShouldSplitTokensAndDelimiters()
    {
        var input = "var p = (10,b);";
        var tokenizer = new DelimitedTokenizer(input);

        // "var", "p", "=", "(", "10", ",", "b", ")", ";"

        Assert.Equal("var", tokenizer.Consume());
        Assert.Equal("p", tokenizer.Consume());
        Assert.Equal("=", tokenizer.Consume());
        Assert.Equal("(", tokenizer.Consume());
        Assert.Equal("10", tokenizer.Consume());
        Assert.Equal(",", tokenizer.Consume());
        Assert.Equal("b", tokenizer.Consume());
        Assert.Equal(")", tokenizer.Consume());
        Assert.Equal(";", tokenizer.Consume());
        Assert.Null(tokenizer.Peek());
    }

    [Fact]
    public void Tokenizer_WhitespaceAndNewlines()
    {
        var input = "return{id(1) \n + id(2)};";
        var tokenizer = new DelimitedTokenizer(input);

        //"return", "{", "id", "(", "1", ")", "+", "id", "(", "2", ")", "}", ";"

        Assert.Equal("return", tokenizer.Consume());
        Assert.Equal("{", tokenizer.Consume());
        Assert.Equal("id", tokenizer.Consume());
        Assert.Equal("(", tokenizer.Consume());
        Assert.Equal("1", tokenizer.Consume());
        Assert.Equal(")", tokenizer.Consume());
        Assert.Equal("+", tokenizer.Consume());
        Assert.Equal("id", tokenizer.Consume());
        Assert.Equal("(", tokenizer.Consume());
        Assert.Equal("2", tokenizer.Consume());
        Assert.Equal(")", tokenizer.Consume());
        Assert.Equal("}", tokenizer.Consume());
        Assert.Equal(";", tokenizer.Consume());
        Assert.Null(tokenizer.Peek());
    }

    [Fact]
    public void Tokenizer_Peek_ShouldNotMovePosition()
    {
        var input = "a b";
        var tokenizer = new DelimitedTokenizer(input);

        Assert.Equal("a", tokenizer.Peek());
        Assert.Equal("a", tokenizer.Peek());
        Assert.Equal("a", tokenizer.Peek());
        Assert.Equal("a", tokenizer.Consume());
        Assert.Equal("b", tokenizer.Peek());
    }

    [Fact]
    public void Tokenizer_Check_ShouldWorkCorrectly()
    {
        var input = "(10";
        var tokenizer = new DelimitedTokenizer(input);

        Assert.True(tokenizer.Check("("));
        Assert.False(tokenizer.Check("10"));

        tokenizer.Consume();

        Assert.True(tokenizer.Check("10"));
    }

    [Fact]
    public void Tokenizer_Expect_ShouldConsumeTokenOnSuccess()
    {
        var input = "var p";
        var tokenizer = new DelimitedTokenizer(input);

        tokenizer.Expect("var");

        Assert.Equal("p", tokenizer.Peek());
    }

    [Fact]
    public void Tokenizer_Expect_ShouldThrowExceptionOnFailure()
    {
        var input = "var p";
        var tokenizer = new DelimitedTokenizer(input);

        var exception = Record.Exception(() => tokenizer.Expect("const"));

        Assert.NotNull(exception);
        Assert.IsType<Exception>(exception);
        Assert.Contains("Expected 'const' but found 'var'", exception.Message);
        Assert.Equal("var", tokenizer.Peek());
    }
}