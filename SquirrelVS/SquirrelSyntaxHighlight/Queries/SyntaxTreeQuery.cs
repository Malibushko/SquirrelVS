using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualStudio.Text;
using tree_sitter;

namespace SquirrelSyntaxHighlight.Queries
{
  internal class SyntaxTreeQuery
  {
    public static SyntaxTreeQuery FromString(
        TSLanguage _Language,
        string     _Query
      )
    {
      uint         ErrorOffset = 0;
      TSQueryError Error       = TSQueryError.TSQueryErrorNone;

      TSQuery Query = api.TsQueryNew(_Language, _Query, (uint)_Query.Length, ref ErrorOffset, ref Error);

      if (Error != TSQueryError.TSQueryErrorNone)
        return null;

      return new SyntaxTreeQuery { Query = Query, Walker = api.TsQueryCursorNew() };
    }

    public void Reset()
    {
      api.TsQueryCursorDelete(Walker);

      Walker = api.TsQueryCursorNew();
    }

    public static SyntaxTreeQuery FromFile(
        TSLanguage _Language,
        string     _FilePath
      )
    {
      if (!File.Exists(_FilePath))
        return null;

      return FromString(_Language, File.ReadAllText(_FilePath));
    }
    
    public bool TryAddPredicateForPatternID(
        uint _PatternID
      )
    {
      if (Predicates.ContainsKey(_PatternID))
        return false;

      uint PredicateCount = 0;
      TSQueryPredicateStep[] Steps = api.TsQueryPredicatesForPattern(Query, _PatternID, ref PredicateCount);

      if (PredicateCount == 0)
        return false;

      if (Steps[0].Type != TSQueryPredicateStepType.TSQueryPredicateStepTypeString)
        return false;

      uint   Length        = 0;
      string PredicateName = api.TsQueryStringValueForId(Query, Steps[0].ValueId, ref Length);

      switch(PredicateName)
      {
        case "eq?":
        {
          Predicates[_PatternID] = new EqualQueryPredicate()
          {
            Capture = api.TsQueryStringValueForId(Query, Steps[1].ValueId, ref Length),
            Value   = api.TsQueryStringValueForId(Query, Steps[2].ValueId, ref Length)
          };

          return true;
        }
        case "match?":
        {
          Predicates[_PatternID] = new MatchQueryPredicate()
          {
            Capture = api.TsQueryStringValueForId(Query, Steps[1].ValueId, ref Length),
            Value   = api.TsQueryStringValueForId(Query, Steps[2].ValueId, ref Length)
          };
          return true;
        }
        default:
          return false;
      }
    }

    public IEnumerable<Tuple<string, Span>> Execute(
        ITextBuffer _Buffer,
        TSNode      _Root
      )
    {
      api.TsQueryCursorExec(Walker, Query, _Root);

      while (true)
      {
        TSQueryMatch Match = new TSQueryMatch();

        if (!api.TsQueryCursorNextMatch(Walker, Match))
          break;

        TryAddPredicateForPatternID(Match.PatternIndex);

        for (int i = 0; i < Match.CaptureCount; i++)
        {
          TSQueryCapture Capture = Match[i];

          uint   Length      = 0;
          string PatternName = api.TsQueryCaptureNameForId(Query, Capture.Index, ref Length);

          int Start = (int)api.TsNodeStartByte(Capture.Node);
          int End   = (int)api.TsNodeEndByte(Capture.Node);

          if (Predicates.TryGetValue(Match.PatternIndex, out QueryPredicate _Predicate))
          {
            string TextValue = _Buffer.CurrentSnapshot.GetText(Start, End - Start);

            if (!_Predicate.Match(TextValue))
              continue;
          }

          yield return new Tuple<string, Span>(PatternName, new Span(Start, End - Start));
        }
      }
    }
    private Dictionary<uint, QueryPredicate> Predicates = new Dictionary<uint, QueryPredicate>(); 
    private TSQuery                          Query;
    private TSQueryCursor                    Walker;
  }
}
