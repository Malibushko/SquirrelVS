using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using System.Text;

namespace SquirrelSyntaxHighlight
{
  public class SquirrelClassificationLexer
  {
    SnapshotSpan                       Snapshot;
    IClassificationTypeRegistryService ClassificationRegistry;

    int CurrentPosition;

    SortedSet<string> Keywords;

    const char SquirrelEOB = (char)0;

    public SquirrelClassificationLexer(SnapshotSpan _Snapshot, IClassificationTypeRegistryService _Registry)
    {
      Snapshot               = _Snapshot;
      ClassificationRegistry = _Registry;
      Keywords               = SquirrelKeywords();

      CurrentPosition = Snapshot.Start.Position;
    }
    
    public List<ClassificationSpan> Lex()
    {
      List<ClassificationSpan> OutputSpans = new List<ClassificationSpan>();

      while (CurrentChar() != SquirrelEOB)
      {
        switch (CurrentChar())
        {
          case '\t':
          case '\r':
          case ' ':
          {
              Next();
              continue;
          }
          case '\n':
          {
            CurrentPosition++;
            continue;
          }
          case '/':
            {
              Next();

              switch (CurrentChar())
              {
                case '*':
                {
                  int StartPosition = CurrentPosition - 1;
                  LexBlockComment();
                  int EndPosition = CurrentPosition;

                  OutputSpans.Add(CreateSpan(StartPosition, EndPosition, "Squirrel.Comment"));
                  continue;
                }
                case '/':
                {
                  int StartPosition = CurrentPosition - 1;

                  do { Next(); } while (CurrentChar() != '\n' && CurrentChar() != 0);

                  int EndPosition   = CurrentPosition;

                  OutputSpans.Add(CreateSpan(StartPosition, EndPosition, "Squirrel.Comment"));

                  continue;
                }
                case '=':
                  break; // Not implemeted
                case '>':
                  break; // Not implemented
              }

              continue;
            }

          case '\"':
          case '\'':
          {
            int StartPosition = CurrentPosition;
            char Separator    = CurrentChar();

            do { Next(); } while (CurrentChar() != Separator && CurrentChar() != '\n' && CurrentChar() != 0);

            int EndPosition   = CurrentPosition;

            OutputSpans.Add(CreateSpan(StartPosition, EndPosition, "Squirrel.String"));

            Next();

            continue;
          }

          case ':':
          {
              Next();

              if (CurrentChar() == ':')
              {
                int StartPosition = CurrentPosition - 1;

                do { Next(); } while (Char.IsLetterOrDigit(CurrentChar()) || CurrentChar() == '_');

                OutputSpans.Add(CreateSpan(StartPosition, CurrentPosition - 1, "Squirrel.Global"));

                Next();
              }
              
              continue;
          }

          default:
          {
            if (Char.IsDigit(CurrentChar()))
            {
                Next();
            }
            else if (Char.IsLetterOrDigit(CurrentChar()) || CurrentChar() == '_')
            {
                int StartPosition = CurrentPosition;

                var Identifier = new StringBuilder();
                while (Char.IsLetterOrDigit(CurrentChar()) || CurrentChar() == '_')
                {
                  Identifier.Append(CurrentChar());
                  Next();
                }

                string ID = Identifier.ToString();
                if (Keywords.Contains(ID))
                  OutputSpans.Add(CreateSpan(StartPosition, CurrentPosition - 1, "Squirrel.Keyword"));
                else
                if (ID.StartsWith("C") && Char.IsUpper(ID[1]) && ID.ToUpper() != ID) // Heuristic based on codestyle
                  OutputSpans.Add(CreateSpan(StartPosition, CurrentPosition - 1, "Squirrel.Class"));
              }
            else
            {
              if (Char.IsControl(CurrentChar()))
              {
                  // Error("unexpected character(control)") Log error

              }

                Next();
            }
            continue;
          }
        }
      }

      return OutputSpans;
    }

    private ClassificationSpan CreateSpan(int _Start, int _End, string Class)
    {
      return new ClassificationSpan(new SnapshotSpan(Snapshot.Snapshot, _Start, _End - _Start + 1),
                                    ClassificationRegistry.GetClassificationType(Class));
    }
    private char CurrentChar()
    {
      if (CurrentPosition >= Snapshot.End.Position)
        return SquirrelEOB;

      return Snapshot.Snapshot[CurrentPosition];
    }

    private char Next()
    {
      ++CurrentPosition;

      return CurrentChar();
    }

    private void LexBlockComment()
    {
      bool IsDone      = false;

      while (!IsDone)
      {
        switch (CurrentChar())
        {
          case '*':
          {
              Next();

              if (CurrentChar() == '/')
              {
                IsDone = true;

                Next();
              }

              continue;
          }

          case '\n':
            {
              ++CurrentPosition;
              continue;
          }

          case SquirrelEOB:
            IsDone = true;
            continue;

          default:
          {
            Next();
            continue;
          }
        }
      }
    }

    static private SortedSet<string> SquirrelKeywords()
    {
      var Keywords = new SortedSet<string>();

      Keywords.Add("while");
      Keywords.Add("do");
      Keywords.Add("if");
      Keywords.Add("else");
      Keywords.Add("break");
      Keywords.Add("continue");
      Keywords.Add("return");
      Keywords.Add("null");
      Keywords.Add("function");
      Keywords.Add("local");
      Keywords.Add("for");
      Keywords.Add("foreach");
      Keywords.Add("in");
      Keywords.Add("typeof");
      Keywords.Add("delegate");
      Keywords.Add("delete");
      Keywords.Add("try");
      Keywords.Add("catch");
      Keywords.Add("throw");
      Keywords.Add("clone");
      Keywords.Add("yield");
      Keywords.Add("resume");
      Keywords.Add("switch");
      Keywords.Add("case");
      Keywords.Add("default");
      Keywords.Add("this");
      Keywords.Add("parent");
      Keywords.Add("class");
      Keywords.Add("extends");
      Keywords.Add("constructor");
      Keywords.Add("instanceof");
      Keywords.Add("vargc");
      Keywords.Add("vargv");
      Keywords.Add("true");
      Keywords.Add("false");
      Keywords.Add("static");
      Keywords.Add("enum");
      Keywords.Add("const");

      return Keywords;
    }
  }
}
