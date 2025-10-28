using System.Text;

namespace TupleRewriterTesting;

/// <summary>
/// Tokenizes the string in a way that whitespace characters are removed yet delimiters remain as tokens.
/// </summary>
public class DelimitedTokenizer : Tokenizer
{
    /// <summary>
    /// Tokenizes the string with the default delimiter list which is ['(', ')', '{', '}', ',', '=', ';']
    /// </summary>
    /// <param name="input"></param>
    public DelimitedTokenizer(string input) : this(input, ['(', ')', '{', '}', ',', '=', ';'])
    {
    }


    /// <summary>
    /// Tokenizes the string forgetting whitespaces but keeping delimiters as tokens.
    /// </summary>
    /// <param name="input"></param>
    /// <param name="delimiters"></param>
    public DelimitedTokenizer(string input, char[] delimiters)
    {
        var tokens = new List<string>();

        var buffer = new StringBuilder();
        
        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (buffer.Length > 0)
                {
                    tokens.Add(buffer.ToString());
                    buffer.Clear();
                }
            }

            else if (delimiters.Contains(c))
            {
                if (buffer.Length > 0)
                {
                    tokens.Add(buffer.ToString());
                    buffer.Clear();
                }

                tokens.Add(c.ToString());
            }
            else
            {
                buffer.Append(c);
            }
        }

        if (buffer.Length > 0)
        {
            tokens.Add(buffer.ToString());
        }

        this.tokens = tokens.Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
    }
}