namespace TupleRewriterTesting;

public class SimpleTokenizer : Tokenizer
{
    private readonly List<string> _tokens;
    private int _position = 0;

    /// <summary>
    /// Assuming input string is tokenized by whitespaces, indents and newline characters
    /// </summary>
    /// <param name="input"></param>
    public SimpleTokenizer(string input)
    {
        _tokens = input.Split([' ', '\t', '\n'], StringSplitOptions.RemoveEmptyEntries).ToList();
    }

    public override string? Peek()
    {
        return _position < _tokens.Count ? _tokens[_position] : null;
    }

    public override string? Consume()
    {
        return _position < _tokens.Count ? _tokens[_position++] : null;
    }

    public override bool Check(string expected)
    {
        return Peek() == expected;
    }

    public override void Expect(string expected)
    {
        if (!Check(expected)) throw new Exception($"Expected '{expected}' but found '{Peek()}'");
        Consume();
    }
}