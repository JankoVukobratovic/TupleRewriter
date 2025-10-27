namespace TupleRewriterTesting;

public abstract class Tokenizer
{
    public abstract string? Peek();
    public abstract string? Consume();
    public abstract bool Check(string expected);
    public abstract void Expect(string expected);
}