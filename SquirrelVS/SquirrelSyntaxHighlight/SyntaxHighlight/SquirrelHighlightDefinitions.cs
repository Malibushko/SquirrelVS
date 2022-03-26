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
    [Name("Squirrel.keyword")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelKeywordDefinition = null;

    [Export]
    [Name("Squirrel.comment")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelCommentDefinition = null;

    [Export]
    [Name("Squirrel.string")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelStringDefinition = null;

    [Export]
    [Name("Squirrel.constant")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelConstantDefinition = null;

    [Export]
    [Name("Squirrel.constant.builtin")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelConstantBuiltinDefinition = null;

    [Export]
    [Name("Squirrel.number")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelNumberDefinition = null;

    [Export]
    [Name("Squirrel.punctuation.delimiter")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelPunctuationDelimeter = null;

    [Export]
    [Name("Squirrel.class")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelClassDefinition = null;

    [Export]
    [Name("Squirrel.function")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelFunctionDefinition = null;

    [Export]
    [Name("Squirrel.function.method")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelFunctionMethodDefinition = null;

    [Export]
    [Name("Squirrel.function.builtin")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelFunctionBuiltinDefinition = null;

    [Export]
    [Name("Squirrel.variable.builtin")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelVariableBuiltinDefinition = null;

    [Export]
    [Name("Squirrel.operator")]
    [BaseDefinition("Squirrel")]
    public static ClassificationTypeDefinition SquirrelFunctionVariableDefinition = null;

    #endregion

    #region Classification Format Definitions

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.keyword")]
    [Name("Squirrel.keyword")]
    [UserVisible(true)]
    public sealed class SquirrelKeywordFormat : ClassificationFormatDefinition
    {
      public SquirrelKeywordFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Keywords Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.comment")]
    [Name("Squirrel.comment")]
    [UserVisible(true)]
    public sealed class SquirrelCommentFormat : ClassificationFormatDefinition
    {
      public SquirrelCommentFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Comments Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.string")]
    [Name("Squirrel.string")]
    [UserVisible(true)]
    public sealed class SquirrelStringFormat : ClassificationFormatDefinition
    {
      public SquirrelStringFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel String Literals Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.constant")]
    [Name("Squirrel.constant")]
    [UserVisible(true)]
    public sealed class SquirrelConstantsFormat : ClassificationFormatDefinition
    {
      public SquirrelConstantsFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Constants Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.constant.builtin")]
    [Name("Squirrel.constant.builtin")]
    [UserVisible(true)]
    public sealed class SquirrelConstantsBuiltinFormat : ClassificationFormatDefinition
    {
      public SquirrelConstantsBuiltinFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Builtin Constants Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.number")]
    [Name("Squirrel.number")]
    [UserVisible(true)]
    public sealed class SquirrelNumbersFormat : ClassificationFormatDefinition
    {
      public SquirrelNumbersFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Number Literals Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.punctuation.delimiter")]
    [Name("Squirrel.punctuation.delimiter")]
    [UserVisible(true)]
    public sealed class SquirrelPunctuationDelimiterFormat : ClassificationFormatDefinition
    {
      public SquirrelPunctuationDelimiterFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Color For Punctuation Characters";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.function")]
    [Name("Squirrel.function")]
    [UserVisible(true)]
    public sealed class SquirrelFunctionsFormat : ClassificationFormatDefinition
    {
      public SquirrelFunctionsFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Function Names Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.function.method")]
    [Name("Squirrel.function.method")]
    [UserVisible(true)]
    public sealed class SquirrelFunctionMethodFormat : ClassificationFormatDefinition
    {
      public SquirrelFunctionMethodFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Class Methods Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.function.builtin")]
    [Name("Squirrel.function.builtin")]
    [UserVisible(true)]
    public sealed class SquirrelBuilinFunctionsFormat : ClassificationFormatDefinition
    {
      public SquirrelBuilinFunctionsFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Builin Functions Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.variable.builtin")]
    [Name("Squirrel.variable.builtin")]
    [UserVisible(true)]
    public sealed class SquirrelBuiltinVariablesFormat : ClassificationFormatDefinition
    {
      public SquirrelBuiltinVariablesFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Builtin Variables Color";
      }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.operator")]
    [Name("Squirrel.operator")]
    [UserVisible(true)]
    public sealed class SquirrelOperatorsFormat : ClassificationFormatDefinition
    {
      public SquirrelOperatorsFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Operators Color";
      }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "Squirrel.class")]
    [Name("Squirrel.class")]
    [UserVisible(true)]
    public sealed class SquirrelClassFormat : ClassificationFormatDefinition
    {
      public SquirrelClassFormat()
      {
        ForegroundColor = Color.FromRgb(0xFF, 0x22, 0x22);
        DisplayName = "Squirrel Class Color";
      }
    }

    #endregion

    #region Service

    static private Color? TryGetThemedColor(
          string _Name
        )
    {
      var Key = new ThemeResourceKey(Category, _Name, ThemeResourceKeyType.ForegroundColor);
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
