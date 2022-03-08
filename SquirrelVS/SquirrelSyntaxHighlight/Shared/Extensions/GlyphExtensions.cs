﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Windows.Media;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense;

namespace ShaderTools.CodeAnalysis.Editor.Shared.Extensions
{
    internal static class GlyphExtensions
    {
        public static StandardGlyphGroup GetStandardGlyphGroup(this Glyph glyph)
        {
            switch (glyph)
            {
                case Glyph.Class:
                    return StandardGlyphGroup.GlyphGroupClass;

                case Glyph.Constant:
                    return StandardGlyphGroup.GlyphGroupConstant;

                case Glyph.Field:
                    return StandardGlyphGroup.GlyphGroupField;

                case Glyph.Interface:
                    return StandardGlyphGroup.GlyphGroupInterface;

                case Glyph.IntrinsicClass:
                case Glyph.IntrinsicStruct:
                    return StandardGlyphGroup.GlyphGroupIntrinsic;

                case Glyph.Keyword:
                    return StandardGlyphGroup.GlyphKeyword;

                case Glyph.Label:
                    return StandardGlyphGroup.GlyphGroupIntrinsic;

                case Glyph.Local:
                    return StandardGlyphGroup.GlyphGroupVariable;

                case Glyph.Macro:
                    return StandardGlyphGroup.GlyphGroupMacro;

                case Glyph.Namespace:
                    return StandardGlyphGroup.GlyphGroupNamespace;

                case Glyph.Method:
                    return StandardGlyphGroup.GlyphGroupMethod;

                case Glyph.Module:
                    return StandardGlyphGroup.GlyphGroupModule;

                case Glyph.OpenFolder:
                    return StandardGlyphGroup.GlyphOpenFolder;

                case Glyph.Operator:
                    return StandardGlyphGroup.GlyphGroupOperator;

                case Glyph.Parameter:
                    return StandardGlyphGroup.GlyphGroupVariable;

                case Glyph.Structure:
                    return StandardGlyphGroup.GlyphGroupStruct;

                case Glyph.Typedef:
                    return StandardGlyphGroup.GlyphGroupTypedef;

                case Glyph.TypeParameter:
                    return StandardGlyphGroup.GlyphGroupType;

                case Glyph.CompletionWarning:
                    return StandardGlyphGroup.GlyphCompletionWarning;

                default:
                    throw new ArgumentException("glyph");
            }
        }

        public static StandardGlyphItem GetStandardGlyphItem(this Glyph icon)
        {
            return StandardGlyphItem.GlyphItemPublic;
        }

        public static ImageSource GetImageSource(this Glyph glyph, IGlyphService glyphService)
        {
            return glyphService.GetGlyph(glyph.GetStandardGlyphGroup(), glyph.GetStandardGlyphItem());
        }

        public static ImageId GetImageId(this Glyph glyph)
        {
            return new ImageId(KnownImageIds.ImageCatalogGuid, GetKnownImageId(glyph));
        }

        private static int GetKnownImageId(Glyph glyph)
        {
            switch (glyph)
            {
                case Glyph.None:
                    return KnownImageIds.None;

                case Glyph.Class:
                    return KnownImageIds.ClassPublic;

                case Glyph.Constant:
                    return KnownImageIds.ConstantPublic;

                case Glyph.Field:
                    return KnownImageIds.FieldPublic;

                case Glyph.Interface:
                    return KnownImageIds.InterfacePublic;

                // TODO: Figure out the right thing to return here.
                case Glyph.IntrinsicClass:
                case Glyph.IntrinsicStruct:
                    return KnownImageIds.Type;

                case Glyph.Keyword:
                    return KnownImageIds.IntellisenseKeyword;

                case Glyph.Label:
                    return KnownImageIds.Label;

                case Glyph.Macro:
                    return KnownImageIds.MacroPublic;

                case Glyph.Parameter:
                case Glyph.Local:
                    return KnownImageIds.LocalVariable;

                case Glyph.Namespace:
                    return KnownImageIds.Namespace;

                case Glyph.Method:
                    return KnownImageIds.MethodPublic;

                case Glyph.Module:
                    return KnownImageIds.ModulePublic;

                case Glyph.OpenFolder:
                    return KnownImageIds.OpenFolder;

                case Glyph.Operator:
                    return KnownImageIds.Operator;

                case Glyph.Structure:
                    return KnownImageIds.ValueTypePublic;

                case Glyph.Typedef:
                    return KnownImageIds.TypeDefinitionPublic;

                case Glyph.TypeParameter:
                    return KnownImageIds.Type;

                case Glyph.CompletionWarning:
                    return KnownImageIds.IntellisenseWarning;

                default:
                    throw new ArgumentException("glyph");
            }
        }
    }
}