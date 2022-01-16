using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Text;
using System;

namespace SquirrelSyntaxHighlight
{
  internal class SquirrelParserData
  {

  }

  internal class SquirrelParser
  {
    private IClassificationTypeRegistryService m_ClassificationRegistry;

    private SquirrelLexer                          m_Lexer;
    private string                                 m_SourceName;
    private string                                 m_ParseError;
    private Dictionary<string, SquirrelParserData> m_ParseResults;

    private EToken                                 m_Token;

    public AST                                     SyntaxTree;

    public SquirrelParser(
        SnapshotSpan                       _Snapshot, 
        IClassificationTypeRegistryService _Registry
      )
    {
      m_ClassificationRegistry = _Registry;
      
      m_Lexer = new SquirrelLexer(_Snapshot);

      SyntaxTree = new AST(0, _Snapshot.Snapshot.Length);

      Lex();
    }

    public void Parse()
    {
      while (m_Token > 0)
      {
        SyntaxTree.Feed(m_Lexer, m_Token);

        Lex();
      }
    }

    public string ParseError
    {
      get
      {
        return m_ParseError;
      }
    }

    private void Error(
        string _Message
      )
    {

    }
    private ClassificationSpan CreateSpan(int _Start, int _End, string Class)
    {
      return new ClassificationSpan(new SnapshotSpan(m_Lexer.Snapshot.Snapshot, _Start, _End - _Start),
                                    m_ClassificationRegistry.GetClassificationType(Class));
    }

    private void Statement()
    {
      switch (m_Token)
      {
        case (EToken)';':
          Lex();
          break;
        case EToken.If:
          break;
      }
    }

    private void OptionalSemicolon()
    {
      if ((int)m_Token == ';')
      {
        Lex();
        return;
      }
      if (IsEndOfStatement())
      {
        Error("end of statement expected (; or lf)"); 
      }
    }

    private bool IsEndOfStatement()
    {
      return (m_Lexer.PreviousToken == '\n')      || 
             (int)m_Token == ParsingConstants.EOB || 
             (int)m_Token == '}'                  ||
             (int)m_Token == ';';
    }
    private EToken Lex()
    {
      m_Token = m_Lexer.Lex();

      return m_Token;
    }
  }
}
