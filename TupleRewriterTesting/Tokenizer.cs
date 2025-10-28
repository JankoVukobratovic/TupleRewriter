namespace TupleRewriterTesting;

public abstract class Tokenizer()
{
    protected List<string> tokens;
    protected int position = 0;
    
    

    public virtual string? Peek()
    {
        return position < tokens.Count ? tokens[position] : null;
    }

    public virtual string? Consume()
    {
        return position < tokens.Count ? tokens[position++] : null;
    }

    public virtual bool Check(string expected)
    {
        return Peek() == expected;
    }

    public virtual void Expect(string expected)
    {
        if (!Check(expected)) throw new Exception($"Expected '{expected}' but found '{Peek()}' at  position {position}.");
        Consume();
    }
}