using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SquirrelSyntaxHighlight.Infrastructure;
using SquirrelSyntaxHighlight.Editor;
using Microsoft.VisualStudio.Text;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Parsing
{
  public sealed class Tokenizer
  {
    private static int MinimalValidColumnIndex = 1;
    private static int MinimalValidRowIndex    = 1;
    public Tokenizer(
        ErrorSink        _ErrorSink = null, 
        TokenizerOptions _Options   = TokenizerOptions.None
      )
    {
      Options   = _Options;
      ErrorSink = _ErrorSink;
    }

    private TextReader       Reader { get; set; }
    private ErrorSink        ErrorSink { get; set; }
    private TokenizerOptions Options { get; set; }
    private TSLanguage       Language { get; set; }

    public void Init(
        TextReader _SourceUnit
      )
    {
      Reader = _SourceUnit;
    }

    public void Shutdown()
    {
      Reader = null;
    }
    private TokenInfo NodeToTokenInfo(
        TSNode _Node
      )
    {
      TokenInfo Info = new TokenInfo();

      TSPoint Start = api.TsNodeStartPoint(_Node);
      TSPoint End   = api.TsNodeEndPoint(_Node);

      Info.SourceSpan = new SourceSpan(
          (int)Start.Row    + MinimalValidRowIndex, 
          (int)Start.Column + MinimalValidColumnIndex,
          (int)End.Row      + MinimalValidRowIndex,
          (int)End.Column   + MinimalValidColumnIndex
        );

      Info.Category   = TokenCategory.None;
      Info.Trigger    = TokenTriggers.None;

      ushort Symbol = api.TsNodeSymbol(_Node);

      string Name = api.TsLanguageSymbolName(Language, Symbol);

      switch (Name)
      {
        case "identifier":
          Info.Category = TokenCategory.Identifier;
          break;
        case "number":
          Info.Category = TokenCategory.NumericLiteral;
          break;
        case "comment":
          Info.Category = TokenCategory.Comment;
          break;
        case "unescaped_single_string_fragment":
          Info.Category = TokenCategory.CharacterLiteral;
          break;
        case "unescaped_double_string_fragment":
          Info.Category = TokenCategory.StringLiteral;
          break;
        case "if":
        case "switch":
        case "else":
        case "for":
        case "foreach":
        case "return":
        case "null":
        case "const":
        case "break":
        case "static":
        case "var":
        case "class":
        case "try":
        case "catch":
        case "case":
        case "local":
        case "function":
        case "delete":
        case "instanceof":
          Info.Category = TokenCategory.Keyword;
          break;
        case "operator":
          Info.Category = TokenCategory.Operator;
          break;
      }

      return Info;
    }

    public IEnumerable<TokenInfo> ReadTokens(
        int _CharacterCount
      )
    {
      if (Language == null)
        Language = squirrel.TreeSitterSquirrel();

      var Parser = api.TsParserNew();

      if (api.TsParserSetLanguage(Parser, Language))
      {
        var Text = "";
        
        for (int i = 0; i < _CharacterCount;i++)
          Text += (char)Reader.Read();

        var SyntaxTree = api.TsParserParseString(Parser, null, Text, (uint)Text.Length);
        var Root       = api.TsTreeRootNode(SyntaxTree);
        var Walker     = api.TsTreeCursorNew(Root);

        foreach (TSNode Node in SyntaxTreeWalker.Traverse(Walker))
        {
          TokenInfo Info = NodeToTokenInfo(Node);

          if (Info.Category == TokenCategory.None)
            continue;  

          yield return Info;
        }

        api.TsParserDelete(Parser);
        api.TsTreeCursorDelete(Walker);
      }
    }
  }
}
