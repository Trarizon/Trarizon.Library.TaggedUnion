using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;

namespace Trarizon.Library.TaggedUnion.SourceGenerator.Utilities;
internal static class IndentedTextWriterExtensions
{
    public static AutoIndentBackMultiple DeferIndentBack(this IndentedTextWriter writer)
    {
        return new AutoIndentBackMultiple(writer);
    }

    public static AutoIndentBack Indent(this IndentedTextWriter writer, string? deferText = null)
    {
        writer.Indent++;
        return new AutoIndentBack(writer,deferText);
    }

    public ref struct AutoIndentBack(IndentedTextWriter writer, string? deferText)
    {
        public readonly void Dispose()
        {
            writer.Indent--;
            if (deferText is not null)
                writer.WriteLine(deferText);
        }
    }

    public ref struct AutoIndentBackMultiple(IndentedTextWriter writer)
    {
        private Stack<string?>? _suffixes;

        public void Indent(string? deferText)
        {
            writer.Indent++;
            (_suffixes ??= []).Push(deferText);
        }

        public readonly void Dispose()
        {
            if (_suffixes is null)
                return;

            foreach (var after in _suffixes) {
                writer.Indent--;
                if (after is not null)
                    writer.WriteLine(after);
            }
            _suffixes.Clear();
        }
    }
}
