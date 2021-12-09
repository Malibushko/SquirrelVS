using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace SquirrelSyntaxHighlight
{
    internal static class SquirrelClassificationDefinitions
    {
        #region Content Type and File Extension Definition

        [Export]
        [Name("Squirrel")]
        [BaseDefinition("text")]
        internal static ContentTypeDefinition SquirrelContentTypeDefinition = null;

        [Export]
        [FileExtension(".nut")]
        [ContentType("Squirrel")]
        internal static FileExtensionToContentTypeDefinition SquirrelFileTypeDefinition = null;

        #endregion

        #region Classification Type Definitions

        [Export]
        [Name("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelTypeDefinition = null;

        [Export]
        [Name("Squirrel.Keyword")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelKeywordDefinition = null;

        [Export]
        [Name("Squirrel.Comment")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelCommentDefinition = null;
        
        [Export]
        [Name("Squirrel.String")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelStringDefinition  = null;
        
        [Export]
        [Name("Squirrel.Global")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelGlobalVariablesDefinition = null;

        [Export]
        [Name("Squirrel.Class")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelClassDefinition = null;

        [Export]
        [Name("Squirrel.LoopVariable")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelLoopVariableDefinition = null;

        [Export]
        [Name("Squirrel.FunctionVariable")]
        [BaseDefinition("Squirrel")]
        internal static ClassificationTypeDefinition SquirrelFunctionVariableDefinition = null;

        #endregion

        #region Classification Format Definitions

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Keyword")]
        [Name("Squirrel.Keyword")]
        internal sealed class SquirrelKeywordFormat : ClassificationFormatDefinition
        {
            public SquirrelKeywordFormat()
            {
              ForegroundColor = Colors.Blue;
            }
        }
        
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Comment")]
        [Name("Squirrel.Comment")]
        internal sealed class SquirrelCommentFormat : ClassificationFormatDefinition
        {
          public SquirrelCommentFormat()
          {
            ForegroundColor = Colors.Green;
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.String")]
        [Name("Squirrel.String")]
        internal sealed class SquirrelStringFormat : ClassificationFormatDefinition
        {
          public SquirrelStringFormat()
          {
            ForegroundColor = Colors.DarkRed;
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Global")]
        [Name("Squirrel.Global")]
        internal sealed class SquirrelGlobalVariablesFormat : ClassificationFormatDefinition
        {
          public SquirrelGlobalVariablesFormat()
          {
            ForegroundColor = Colors.DarkGoldenrod;
          }
        }
        
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Class")]
        [Name("Squirrel.Class")]
        internal sealed class SquirrelClassFormat : ClassificationFormatDefinition
        {
          public SquirrelClassFormat()
          {
            ForegroundColor = Colors.DarkCyan;
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.LoopVariable")]
        [Name("Squirrel.LoopVariable")]
        internal sealed class SquirrelLoopVariableFormat : ClassificationFormatDefinition
        {
          public SquirrelLoopVariableFormat()
          {
            ForegroundColor = Colors.DarkGray;
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.FunctionVariable")]
        [Name("Squirrel.FunctionVariable")]
        internal sealed class SquirrelFunctionVariableFormat : ClassificationFormatDefinition
        {
          public SquirrelFunctionVariableFormat()
          {
            ForegroundColor = Colors.DarkGray;
          }
        }
    #endregion
  }
}
