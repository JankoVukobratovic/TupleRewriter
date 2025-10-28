using Shouldly;
using TupleRewriterTesting.records;

namespace TupleRewriterTesting.Tests;

public static class TestResourceManager
{
    private const string ResourceBase = "Resources";
    
    private static string GetResourcePath(string name, string file)
    {
        return Path.Combine(AppContext.BaseDirectory, ResourceBase, name, file);
    }
    
    public static string GetOriginalSource(string name)
    {
        var path = GetResourcePath(name, "original.txt");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Original source file not found for test '{name}'. Expected path: {path}");
        }
        return File.ReadAllText(path);
    }
    
    public static string GetExpectedSource(string name)
    {
        var path = GetResourcePath(name, "expected.txt");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Expected equivalent file not found for test '{name}'. Expected path: {path}");
        }
        return File.ReadAllText(path);
    }
    
    public static DelimitedTokenizer GetOriginalTokenizer(string name)
    {
        return new DelimitedTokenizer(GetOriginalSource(name));
    }
    
    public static Tokenizer GetExpectedTokenizer(string name)
    {
        return new DelimitedTokenizer(GetExpectedSource(name));
    }
    
    
    public static Block ParseOriginalBlock(string name)
    {
        var tokenizer = GetOriginalTokenizer(name);
        var parser = new SimpleAstParser(tokenizer);
        return parser.ParseBlock();
    }
    
    
    public static Block ParseExpectedBlock(string name)
    {
        var tokenizer = GetExpectedTokenizer(name);
        var parser = new SimpleAstParser(tokenizer);
        return parser.ParseBlock();
    }
    
    public static void AssertAstEqualsToExpectedEquivalent(Block sourceBlock, string expectedPath)
    {
        var expectedAst = ParseExpectedBlock(expectedPath);
        
        sourceBlock.ShouldBeEquivalentTo(expectedAst);
    }
}
