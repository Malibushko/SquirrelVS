using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace SquirrelSyntaxHighlight
{
    internal class SquirrelClassificationDefinitions
    {
        private static Guid Category = new Guid("75A05685-00A8-4DED-BAE5-E7A50BFA929A");
        
        #region Content Type and File Extension Definition

        [Export]
        [Name("Squirrel")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition SquirrelContentTypeDefinition = null;

        [Export]
        [FileExtension(".nut")]
        [ContentType("Squirrel")]
        internal static FileExtensionToContentTypeDefinition SquirrelFileTypeDefinition = null;

        #endregion

        #region Classification Type Definitions

        [Export]
        [Name("Squirrel")]
        public static ClassificationTypeDefinition SquirrelTypeDefinition = null;

        [Export]
        [Name("Squirrel.Keyword")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelKeywordDefinition = null;

        [Export]
        [Name("Squirrel.Comment")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelCommentDefinition = null;
        
        [Export]
        [Name("Squirrel.String")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelStringDefinition  = null;
        
        [Export]
        [Name("Squirrel.Global")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelGlobalVariablesDefinition = null;

        [Export]
        [Name("Squirrel.Class")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelClassDefinition = null;

        [Export]
        [Name("Squirrel.LoopVariable")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelLoopVariableDefinition = null;

        [Export]
        [Name("Squirrel.FunctionVariable")]
        [BaseDefinition("Squirrel")]
        public static ClassificationTypeDefinition SquirrelFunctionVariableDefinition = null;

        #endregion

        #region Classification Format Definitions

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Keyword")]
        [Name("Squirrel.Keyword")]
        [UserVisible(true)]
        public sealed class SquirrelKeywordFormat : ClassificationFormatDefinition
        {
          public SquirrelKeywordFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Keywords Color";
          }
        }
        
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Comment")]
        [Name("Squirrel.Comment")]
        [UserVisible(true)]
        public sealed class SquirrelCommentFormat : ClassificationFormatDefinition
        {
          public SquirrelCommentFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Comments Color";
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.String")]
        [Name("Squirrel.String")]
        [UserVisible(true)]
        public sealed class SquirrelStringFormat : ClassificationFormatDefinition
        {
          public SquirrelStringFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel String Literals Color";
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Global")]
        [Name("Squirrel.Global")]
        [UserVisible(true)]
        public sealed class SquirrelGlobalVariablesFormat : ClassificationFormatDefinition
        {
          public SquirrelGlobalVariablesFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Globals Color";
          }
        }
        
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.Class")]
        [Name("Squirrel.Class")]
        [UserVisible(true)]
        public sealed class SquirrelClassFormat : ClassificationFormatDefinition
        {
          public SquirrelClassFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Class Color";
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.LoopVariable")]
        [Name("Squirrel.LoopVariable")]
        [UserVisible(true)]
        public sealed class SquirrelLoopVariableFormat : ClassificationFormatDefinition
        {
          public SquirrelLoopVariableFormat()
          { 
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Loop Variable Color";
          }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = "Squirrel.FunctionVariable")]
        [Name("Squirrel.FunctionVariable")]
        [UserVisible(true)]
        public sealed class SquirrelFunctionVariableFormat : ClassificationFormatDefinition
        {
          public SquirrelFunctionVariableFormat()
          {
            ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
            DisplayName     = "Squirrel Function Parameter Color";
          }
        }
    #endregion

        #region Service

        static private Color? TryGetThemedColor(
              string _Name
            )
        {
          var  Key  = new ThemeResourceKey(Category, _Name, ThemeResourceKeyType.ForegroundColor);
          uint ARGB = 0;
          
          if (VsColors.GetCurrentThemedColorValues().TryGetValue(Key, out ARGB))
          { 
            byte[] Bytes = BitConverter.GetBytes(ARGB);
            return Color.FromArgb(Bytes[3], Bytes[2], Bytes[1], Bytes[0]);
          }

          return null;
        }

    #endregion
  }
}
