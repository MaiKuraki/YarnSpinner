// Copyright Yarn Spinner Pty Ltd Licensed under the MIT License. See LICENSE.md
// in project root for license information.

// Uncomment to ensure that all expressions have a known type at compile time
// #define VALIDATE_ALL_EXPRESSIONS

namespace Yarn.Compiler
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains debugging information for compiled Yarn Projects.
    /// </summary>
    public class ProjectDebugInfo
    {
        /// <summary>
        /// The debugging info for the nodes in the project.
        /// </summary>
        public List<NodeDebugInfo> Nodes { get; set; } = new List<NodeDebugInfo>();

        /// <summary>
        /// Gets the debugging info for a given node, if it exists.
        /// </summary>
        /// <param name="nodeName">The name of the node to get debugging info
        /// for.</param>
        /// <returns>The debugging info for the node, or <see langword="null"/>
        /// if none is present.</returns>
        public NodeDebugInfo? GetNodeDebugInfo(string nodeName)
        {
            foreach (var debugInfo in Nodes)
            {
                if (debugInfo.NodeName == nodeName)
                {
                    return debugInfo;
                }
            }
            return null;
        }

        internal static ProjectDebugInfo Combine(params ProjectDebugInfo[] debugInfos)
        {
            var newDebugInfo = new ProjectDebugInfo();
            foreach (var otherDebugInfo in debugInfos)
            {
                newDebugInfo.Nodes.AddRange(otherDebugInfo.Nodes);
            }

            return newDebugInfo;

        }
    }

    /// <summary>
    /// Contains debug information for a node in a Yarn file.
    /// </summary>
    public class NodeDebugInfo
    {
        /// <summary>
        /// Initialises a new instance of the NodeDebugInfo class.
        /// </summary>
        /// <param name="fileName">The file that the node was defined
        /// in.</param>
        /// <param name="nodeName">The name of the node.</param>
        public NodeDebugInfo(string? fileName, string nodeName)
        {
            this.FileName = fileName;
            this.NodeName = nodeName;
        }

        /// <summary>
        /// Gets or sets the file that this DebugInfo was produced from.
        /// </summary>
        public string? FileName { get; internal set; } = null;

        /// <summary>
        /// Gets or sets the node that this DebugInfo was produced from.
        /// </summary>
        public string NodeName { get; internal set; }

        /// <summary>
        /// Gets or sets the mapping of instruction numbers to <see
        /// cref="Range"/> information in the file indicated by <see
        /// cref="FileName"/>.
        /// </summary>
        internal Dictionary<int, Range> LineRanges { get; set; } = new Dictionary<int, Range>();

        /// <summary>
        /// The range in the file in which the node appears.
        /// </summary>
        public Range Range { get; internal set; } = new Range(Position.InvalidPosition, Position.InvalidPosition);

        internal IReadOnlyDictionary<int, string> Labels => this.instructionLabels;

        /// <summary>
        /// Gets or sets a value indicating whether this node was created by the
        /// compiler.
        /// </summary>
        /// <remarks>
        /// Nodes that exist in a .yarn file have an <see cref="IsImplicit"/>
        /// value of <see langword="false"/>. Nodes that are generated by the
        /// compiler include smart variable implementations, and node group
        /// 'hub' nodes.
        /// </remarks>
        public bool IsImplicit { get; internal set; } = false;

        private readonly Dictionary<int, string> instructionLabels = new Dictionary<int, string>();

        internal void AddLabel(string label, int instructionIndex)
        {
            // Ensure that this label is unique
            label = $"L{this.instructionLabels.Count}_" + label;

            this.instructionLabels[instructionIndex] = label;
        }

        internal string? GetLabel(int instructionIndex)
        {
            if (this.instructionLabels.TryGetValue(instructionIndex, out string label))
            {
                return label;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a <see cref="LineInfo"/> object that describes the specified
        /// instruction at the index <paramref name="instructionNumber"/>.
        /// </summary>
        /// <param name="instructionNumber">The index of the instruction to
        /// retrieve information for.</param>
        /// <returns>A <see cref="LineInfo"/> object that describes the position
        /// of the instruction.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref
        /// name="instructionNumber"/> is less than zero, or greater than the
        /// number of instructions present in the node.</exception>
        public LineInfo GetLineInfo(int instructionNumber)
        {
            if (this.LineRanges.TryGetValue(instructionNumber, out var range))
            {
                return new LineInfo
                {
                    FileName = this.FileName,
                    NodeName = this.NodeName,
                    Range = range,
                };
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(instructionNumber));
            }
        }

        /// <summary>
        /// Contains positional information about an instruction.
        /// </summary>
        public struct LineInfo
        {
            /// <summary>
            /// The file name of the source that this instruction was produced
            /// from.
            /// </summary>
            public string? FileName;

            /// <summary>
            /// The node name of the source that this instruction was produced
            /// from.
            /// </summary>
            public string NodeName;

            /// <summary>
            /// The range in <see cref="FileName"/> that contains the
            /// statement or expression that this line was produced from.
            /// </summary>
            public Range Range;
        }
    }
}
