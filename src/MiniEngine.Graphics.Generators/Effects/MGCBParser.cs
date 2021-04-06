using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Text;

namespace MiniEngine.Graphics.Generators.Effects
{
    public delegate void ParseFunction(TextLine line, List<Token> tokens);

    public sealed class MGCBParser
    {
        private readonly List<ParseFunction> Functions;
        private readonly List<Token> TokenList;
        private readonly List<Block> BlockList;

        public MGCBParser()
        {
            this.Functions = new List<ParseFunction>
            {
                Create("/importer", ":", (l, s, a) => new Importer(l, s, a)),
                Create("/processor", ":", (l, s, a) => new Processor(l, s, a)),
                Create("/processorParam", ":", (l, s, a) => new ProcessorParam(l, s, a)),
                Create("/build", ":", (l, s, a) => new Build(l, s, a)),
                Create("#", " ", (l, s, a) => new Comment(l))
            };

            this.TokenList = new List<Token>();
            this.BlockList = new List<Block>();
        }

        public IReadOnlyList<Token> Tokens => this.TokenList;
        public IReadOnlyList<Block> Blocks => this.BlockList;

        public void Parse(TextLineCollection lines)
        {
            this.Tokenize(lines);
            this.Group();
        }

        private void Tokenize(TextLineCollection lines)
        {
            foreach (var line in lines)
            {
                var tokens = this.TokenList.Count;
                foreach (var function in this.Functions)
                {
                    function.Invoke(line, this.TokenList);
                }

                if (tokens == this.TokenList.Count && line.End > line.Start)
                {
                    this.TokenList.Add(new Unknown(line));
                }
            }
        }

        private void Group()
        {
            Comment comment = null;
            Importer importer = null;
            Processor processor = null;
            Build build = null;

            for (var i = 0; i < this.TokenList.Count; i++)
            {
                var token = this.TokenList[i];
                if (token is Comment co)
                {
                    comment = co;
                }
                else if (token is Importer im)
                {
                    importer = im;
                }
                else if (token is Processor pr)
                {
                    processor = pr;
                }
                else if (token is Build bu)
                {
                    build = bu;
                }

                if (comment != null && importer != null && processor != null && build != null)
                {
                    this.BlockList.Add(new Block(comment, importer, processor, build));
                    comment = null;
                    importer = null;
                    processor = null;
                    build = null;
                }
            }
        }

        public static ParseFunction Create(string prefix, string seperator, Func<TextLine, string, string, Token> generator)
        {
            return new ParseFunction((TextLine textLine, List<Token> tokens) =>
            {
                var line = textLine.ToString();
                if (line.StartsWith(prefix))
                {
                    var parts = line.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
                    var statement = parts[0];
                    var argument = parts[1];

                    tokens.Add(generator(textLine, statement, argument));
                }
            });
        }
    }

    public class Block
    {
        public Block(Comment comment, Importer importer, Processor processor, Build build)
        {
            this.Comment = comment;
            this.Importer = importer;
            this.Processor = processor;
            this.Build = build;
        }

        public Comment Comment { get; }
        public Importer Importer { get; }
        public Processor Processor { get; }
        public Build Build { get; }
    }

    public abstract class Token
    {
        public Token(TextLine line, string statement, string argument)
        {
            this.Line = line;
            this.Statement = statement;
            this.Argument = argument;
        }

        public TextLine Line { get; }
        public string Statement { get; }
        public string Argument { get; }

        public override string ToString() => this.Line.ToString();
    }

    public class Importer : Token
    {
        public Importer(TextLine line, string statement, string argument) : base(line, statement, argument)
        {
        }
    }

    public class Processor : Token
    {
        public Processor(TextLine line, string statement, string argument) : base(line, statement, argument)
        {
        }
    }

    public class ProcessorParam : Token
    {
        public ProcessorParam(TextLine line, string statement, string argument) : base(line, statement, argument)
        {
        }
    }

    public class Build : Token
    {
        public Build(TextLine line, string statement, string argument) : base(line, statement, argument)
        {
        }
    }

    public class Comment : Token
    {
        public Comment(TextLine line) : base(line, "", "")
        {
        }
    }

    public class Unknown : Token
    {
        public Unknown(TextLine line) : base(line, "", "")
        {
        }
    }
}
