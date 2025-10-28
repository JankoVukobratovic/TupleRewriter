namespace TupleRewriterTesting;

public class SimpleTokenizer : Tokenizer
{

    /// <summary>
    /// Assuming input string is tokenized by whitespaces, indents and newline characters
    /// Note: to future self this does not split commas or parentheses.
    /// (For example the segment "... new Complex(3.14, 2.12); ..." gets parsed into [..., "new", "Complex(3.14,", "2.12)", ...])
   
    /// </summary>
    /// <param name="input"></param>
    public SimpleTokenizer(string input)
    {
        tokens = input.Split([' ', '\t', '\n'], StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}