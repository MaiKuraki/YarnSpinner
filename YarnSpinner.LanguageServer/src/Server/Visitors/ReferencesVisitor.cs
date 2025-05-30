﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using Yarn.Compiler;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace YarnLanguageServer
{
    internal class ReferencesVisitor : YarnSpinnerParserBaseVisitor<bool>
    {
        private readonly List<NodeInfo> nodeInfos = new();
        private readonly HashSet<string> nodeGroupNames = new();

        private NodeInfo? currentNodeInfo = null;

        private readonly YarnFileData yarnFileData;

        /// <summary>
        /// The CommonTokenStream derived from the file we're parsing. This
        /// is used to find documentation comments for declarations.
        /// </summary>
        private CommonTokenStream tokens;

        public ReferencesVisitor(YarnFileData yarnFileData, CommonTokenStream tokens)
        {
            this.yarnFileData = yarnFileData;
            this.tokens = tokens;
        }

        public static void
            Visit(YarnFileData yarnFileData, CommonTokenStream tokens, out IEnumerable<NodeInfo> nodeInfos, out IEnumerable<string> nodeGroupNames)
        {
            var visitor = new ReferencesVisitor(yarnFileData, tokens);

            if (yarnFileData.ParseTree != null)
            {
                try
                {
                    visitor.Visit(yarnFileData.ParseTree);
                    nodeInfos = visitor.nodeInfos;
                    nodeGroupNames = visitor.nodeGroupNames;
                    return;
                }
                catch (Exception)
                {
                    // Don't want an exception while parsing to take out the entire language server
                }
            }

            nodeInfos = Enumerable.Empty<NodeInfo>();
            nodeGroupNames = Enumerable.Empty<string>();
        }

        public override bool VisitNode([NotNull] YarnSpinnerParser.NodeContext context)
        {
            currentNodeInfo = new NodeInfo
            {
                File = yarnFileData,

                // antlr lines start at 1, but LSP lines start at 0
                HeaderStartLine = context.Start.Line - 1,

                NodeGroupComplexity = context.ComplexityScore,
            };

            Utility.TryGetNodeTitle(yarnFileData.Uri.ToString(), context, out string? sourceTitle, out string? uniqueTitle, out string? subtitle, out string? nodeGroupName);

            currentNodeInfo.SourceTitle = sourceTitle;
            currentNodeInfo.UniqueTitle = uniqueTitle;
            currentNodeInfo.Subtitle = subtitle;
            currentNodeInfo.NodeGroupName = nodeGroupName;

            if (nodeGroupName != null)
            {
                nodeGroupNames.Add(nodeGroupName);
            }

            // Get the first few lines of the node's body as a preview
            if (context.BODY_START != null)
            {
                var nodeStartLine = context.Start.Line;
                var bodyStartLine = context.BODY_START().Symbol.Line + 1;
                var bodyEndLine = context.BODY_END()?.Symbol.Line ?? bodyStartLine;

                int previewLineLength = 10;

                if (bodyStartLine + previewLineLength > bodyEndLine)
                {
                    previewLineLength = bodyEndLine - bodyStartLine;
                }

                var bodyLines = context.GetTextWithWhitespace()
                                   .Split('\n')
                                   .Skip(bodyStartLine - nodeStartLine)
                                   .Where(line => string.IsNullOrWhiteSpace(line) == false)
                                   .Take(previewLineLength)
                                   .Select(line => line.Trim());

                currentNodeInfo.PreviewText = string.Join(Environment.NewLine, bodyLines);
            }

            base.VisitNode(context);

            nodeInfos.Add(currentNodeInfo);

            // ANTLR lines are the line number (1-based), while LSP lines are
            // the line index (0-based).
            if (context.BODY_START() != null)
            {
                var bodyStartLineIndex = context.BODY_START().Symbol.Line - 1;

                // The first line after the BODY_START
                currentNodeInfo.BodyStartLine = bodyStartLineIndex + 1;
            }
            else
            {
                currentNodeInfo.BodyStartLine = currentNodeInfo.HeaderStartLine;
            }

            if (context.BODY_END() != null)
            {
                var bodyEndLineIndex = context.BODY_END().Symbol.Line - 1;

                // The line before the BODY_END
                currentNodeInfo.BodyEndLine = bodyEndLineIndex - 1;
            }
            else
            {
                currentNodeInfo.BodyEndLine = currentNodeInfo.BodyStartLine;
            }

            // Zero-length nodes will have "the line before BODY_END" be before
            // "the line after BODY_START", which is no good. In these cases,
            // ensure that the body starts and ends on the same line.
            if (currentNodeInfo.BodyEndLine < currentNodeInfo.BodyStartLine)
            {
                currentNodeInfo.BodyEndLine = currentNodeInfo.BodyStartLine;
            }

            return true;
        }

        public override bool VisitVariable([NotNull] YarnSpinnerParser.VariableContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitVariable(context);
            }

            currentNodeInfo.VariableReferences.Add(context.Stop);
            return base.VisitVariable(context);
        }

        public override bool VisitTitle_header([NotNull] YarnSpinnerParser.Title_headerContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitTitle_header(context);
            }

            if (context.title != null)
            {
                currentNodeInfo.TitleToken = context.title;

                if (context.HEADER_TITLE().Payload is not IToken headerTitle)
                {
                    // Parse error in this token
                    return base.VisitTitle_header(context);
                }

                currentNodeInfo.Headers.Add(new NodeHeader(
                    key: context.HEADER_TITLE().GetText(),
                    value: context.title.Text,
                    keyToken: headerTitle,
                    valueToken: context.title)
                );
            }

            return base.VisitTitle_header(context);
        }

        public override bool VisitHeader([NotNull] YarnSpinnerParser.HeaderContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitHeader(context);
            }

            if (context.header_key != null && context.header_value != null)
            {
                currentNodeInfo.Headers.Add(new NodeHeader(
                    key: context.header_key.Text,
                    value: context.header_value.Text,
                    keyToken: context.header_key,
                    valueToken: context.header_value
                ));
            }

            return base.VisitHeader(context);
        }

        public override bool VisitDetourToNodeName([NotNull] YarnSpinnerParser.DetourToNodeNameContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitDetourToNodeName(context);
            }

            if (context.destination == null)
            {
                // Missing destination; ignore
                return base.VisitDetourToNodeName(context);
            }

            var jump = new NodeJump(context.destination.Text, context.destination, NodeJump.JumpType.Detour);
            currentNodeInfo.Jumps.Add(jump);

            return base.VisitDetourToNodeName(context);
        }

        public override bool VisitJumpToNodeName([NotNull] YarnSpinnerParser.JumpToNodeNameContext context)
        {
            if (currentNodeInfo == null)
            {
                return VisitJumpToNodeName(context);
            }

            if (context.destination == null)
            {
                // Missing destination; ignore
                return base.VisitJumpToNodeName(context);
            }

            var jump = new NodeJump(context.destination.Text, context.destination, NodeJump.JumpType.Jump);
            currentNodeInfo.Jumps.Add(jump);

            return base.VisitJumpToNodeName(context);
        }

        public override bool VisitFunction_call([NotNull] YarnSpinnerParser.Function_callContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitFunction_call(context);
            }

            try
            {
                Range parametersRange;
                if (context.LPAREN() == null)
                {
                    parametersRange = PositionHelper.GetRange(yarnFileData.LineStarts, context.FUNC_ID().Symbol).CollapseToEnd();
                }
                else if (context.RPAREN() == null)
                {
                    parametersRange = PositionHelper.GetRange(yarnFileData.LineStarts, context.LPAREN().Symbol, context.Stop);
                    parametersRange = new Range(parametersRange.Start.Delta(0, 1), parametersRange.End.Delta(0, -1));
                }
                else
                {
                    parametersRange = PositionHelper.GetRange(yarnFileData.LineStarts, context.LPAREN().Symbol, context.RPAREN().Symbol);
                    parametersRange = new Range(parametersRange.Start.Delta(0, 1), parametersRange.End.Delta(0, -1));
                }

                var parameterCount = context.expression().Count();
                var commas = context.COMMA();
                var left = parametersRange.Start;
                var parameterRanges = new List<Range>(commas.Count() + 1);
                foreach (var right in commas.Select(c => PositionHelper.GetRange(yarnFileData.LineStarts, c.Symbol).End))
                {
                    parameterRanges.Add(new Range(left, right.Delta(0, -1)));
                    left = right;
                }

                parameterRanges.Add(new Range(left, parametersRange.End));

                currentNodeInfo.FunctionCalls.Add(new YarnActionReference
                {
                    NameToken = context.FUNC_ID().Symbol,
                    Name = context.FUNC_ID().Symbol.Text,
                    ExpressionRange = PositionHelper.GetRange(yarnFileData.LineStarts, context.Start, context.Stop),
                    ParameterRanges = parameterRanges,
                    ParameterCount = parameterCount,
                    IsCommand = false,
                    ParametersRange = parametersRange,
                });
            }
            catch (Exception)
            {
            }

            return base.VisitFunction_call(context);
        }

        public override bool VisitDeclare_statement([NotNull] YarnSpinnerParser.Declare_statementContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitDeclare_statement(context);
            }

            var token = context.variable().VAR_ID().Symbol;
            var documentation = GetDocumentComments(context, true)?.OrDefault($"(variable) {token.Text}");

            currentNodeInfo.VariableReferences.Add(token);

            return base.VisitDeclare_statement(context);
        }

        public override bool VisitCommand_statement([NotNull] YarnSpinnerParser.Command_statementContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitCommand_statement(context);
            }

            // TODO: figure out how command parameters should work when the
            // parser grammar is not separating parameters itself and
            // instead is effectly treating commands as "here is a run of
            // text"
            //
            // additional wrinkle: commands are permitted to start with an
            // expression (e.g. <<{0}>>), how should this be handled?

            // for now, register the first COMMAND_FORMATTED_TEXT as a
            // symbol and ignore the rest
            var text = context.command_formatted_text().GetText();
            var components = CommandTextSplitter.SplitCommandText(text);

            var firstTextToken = context.command_formatted_text().Start;

            var tokens = components.Select(c =>
            {
                var token = new CommonToken(YarnSpinnerLexer.COMMAND_TEXT, c.Text)
                {
                    Line = firstTextToken.Line,
                    Column = firstTextToken.Column + c.Offset,
                    StartIndex = firstTextToken.StartIndex + c.Offset,
                    StopIndex = firstTextToken.StartIndex + c.Offset + c.Text.Length - 1,
                };
                return token;
            });

            CommonToken commandName = tokens.First();

            var parameterRangeStart = PositionHelper.GetRange(yarnFileData.LineStarts, commandName).End
                .Delta(0, 1); // need at least one white space character after the command name before any parameters
            var parameterRangeEnd = PositionHelper.GetRange(yarnFileData.LineStarts, context.COMMAND_END().Symbol).Start;

            var parameters = tokens.Skip(1);
            var parameterCount = parameters.Count();
            var parameterRanges = new List<Range>(Math.Max(1, parameterCount));
            if (parameterCount == 0)
            {
                parameterRanges.Add(new Range(parameterRangeStart, parameterRangeEnd));
            }
            else
            {
                var left = parameterRangeStart;
                foreach (var right in parameters.Select(p => PositionHelper.GetRange(yarnFileData.LineStarts, p).End))
                {
                    parameterRanges.Add(new Range(left, right));
                    left = right;
                }

                parameterRanges[parameterCount - 1] = new Range(parameterRanges[parameterCount - 1].Start, parameterRangeEnd);
            }

            var result = new YarnActionReference
            {
                NameToken = commandName,
                Name = commandName.Text,
                ExpressionRange = PositionHelper.GetRange(yarnFileData.LineStarts, commandName, context.COMMAND_END().Symbol),
                ParametersRange = new Range(parameterRangeStart, parameterRangeEnd),
                ParameterRanges = parameterRanges,
                ParameterCount = parameterCount,
                IsCommand = true,
            };

            result.ExpressionRange = new Range(result.ExpressionRange.Start, result.ExpressionRange.End.Delta(0, -2)); // should get right up to the left of >>

            currentNodeInfo.CommandCalls.Add(result);

            return base.VisitCommand_statement(context);
        }

        public override bool VisitLine_statement([NotNull] YarnSpinnerParser.Line_statementContext context)
        {
            if (currentNodeInfo == null)
            {
                return base.VisitLine_statement(context);
            }

            var lineText = context.line_formatted_text().GetTextWithWhitespace();

            lineText = lineText.TrimStart();

            // TODO: this isn't great, since we're running the NameRegex over
            // lines twice (the semantic tokens visitor will return this, too.).
            // TODO: find a way to fetch info from semantic tokens, or for
            // semantic tokens to fetch info from this.
            var nameMatch = SemanticTokenVisitor.NameRegex.Match(lineText);

            if (nameMatch.Success)
            {
                var nameGroup = nameMatch.Groups[1];

                currentNodeInfo.CharacterNames.Add((nameGroup.ToString(), context.Start.Line - 1));
            }

            return base.VisitLine_statement(context);
        }

        public string? GetDocumentComments(ParserRuleContext context, bool allowCommentsAfter = true)
        {
            string? description = null;

            var precedingComments = tokens.GetHiddenTokensToLeft(context.Start.TokenIndex, YarnSpinnerLexer.COMMENTS);

            if (precedingComments != null)
            {
                var precedingDocComments = precedingComments

                    // There are no tokens on the main channel with this
                    // one on the same line
                    .Where(t => tokens.GetTokens()
                        .Where(ot => ot.Line == t.Line)
                        .Where(ot => ot.Type != YarnSpinnerLexer.INDENT && ot.Type != YarnSpinnerLexer.DEDENT)
                        .Where(ot => ot.Channel == YarnSpinnerLexer.DefaultTokenChannel)
                        .Count() == 0)
                    .Where(t => t.Text.StartsWith("///")) // The comment starts with a triple-slash
                    .Select(t => t.Text.Replace("///", string.Empty).Trim()); // Get its text

                if (precedingDocComments.Count() > 0)
                {
                    description = string.Join(" ", precedingDocComments);
                }
            }

            if (allowCommentsAfter)
            {
                var subsequentComments = tokens.GetHiddenTokensToRight(context.Stop.TokenIndex, YarnSpinnerLexer.COMMENTS);
                if (subsequentComments != null)
                {
                    var subsequentDocComment = subsequentComments
                        .Where(t => t.Line == context.Stop.Line) // This comment is on the same line as the end of the declaration
                        .Where(t => t.Text.StartsWith("///")) // The comment starts with a triple-slash
                        .Select(t => t.Text.Replace("///", string.Empty).Trim()) // Get its text
                        .FirstOrDefault(); // Get the first one, or null

                    if (subsequentDocComment != null)
                    {
                        description = subsequentDocComment;
                    }
                }
            }

            return description;
        }
    }
}
