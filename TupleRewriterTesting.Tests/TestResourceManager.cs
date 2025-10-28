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
    
    public static string GetExpectedEquivalent(string name)
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
    
    private static Tokenizer GetEquivalentTokenizer(string name)
    {
        return new DelimitedTokenizer(GetExpectedEquivalent(name));
    }
    
    
    public static Block ParseOriginalBlock(string name)
    {
        var tokenizer = GetOriginalTokenizer(name);
        var parser = new SimpleAstParser(tokenizer);
        return parser.ParseBlock();
    }
    
    
    public static Block ParseEquivalentBlock(string name)
    {
        var tokenizer = GetEquivalentTokenizer(name);
        var parser = new SimpleAstParser(tokenizer);
        return parser.ParseBlock();
    }
    
    public static void AssertParsedBlockEqualsToEquivalent(Block sourceBlock, string expectedPath)
    {
        var expectedAst = ParseEquivalentBlock(expectedPath);
        
        Assert.Equal(sourceBlock, expectedAst);
    }
}
