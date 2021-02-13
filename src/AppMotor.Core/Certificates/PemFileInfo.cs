#region License
// Copyright 2021 AppMotor Framework (https://github.com/skrysmanski/AppMotor)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates
{
    /// <summary>
    /// Provides information about the blocks in a PEM file. Get the information
    /// via <see cref="Blocks"/>.
    /// </summary>
    public sealed class PemFileInfo
    {
        private static readonly Regex BLOCK_START_REGEX = new(@"^-----BEGIN (.+)-----$", RegexOptions.Compiled);

        private static readonly Regex BLOCK_END_REGEX = new(@"^-----END (.+)-----$", RegexOptions.Compiled);

        /// <summary>
        /// The blocks in the PEM file.
        /// </summary>
        [PublicAPI]
        public ImmutableArray<PemBlockInfo> Blocks { get; }

        /// <summary>
        /// Constructor.
        ///
        /// <para>Note: This constructor never puts any of the PEM file's block contents
        /// onto the heap (especially not as strings).</para>
        /// </summary>
        /// <param name="pemContents">The contents of the file.</param>
        public PemFileInfo(ReadOnlySpan<char> pemContents)
        {
            var pemLines = GetLines(pemContents);

            var pemBlocks = GetBlocks(pemContents, pemLines);

            this.Blocks = pemBlocks.ToImmutableArray();
        }

        private static List<Range> GetLines(ReadOnlySpan<char> data)
        {
            int curLineStart = 0;
            int curIndex = 0;
            bool lastCharWasCr = false;

            var lines = new List<Range>();

            foreach (var ch in data)
            {
                switch (ch)
                {
                    case '\r':
                        lastCharWasCr = true;
                        break;

                    case '\n':
                        if (lastCharWasCr)
                        {
                            lines.Add(new Range(curLineStart, curIndex - 1));
                        }
                        else
                        {
                            lines.Add(new Range(curLineStart, curIndex));
                        }

                        curLineStart = curIndex + 1;
                        break;
                }

                curIndex++;
            }

            return lines;
        }

        private static List<PemBlockInfo> GetBlocks(ReadOnlySpan<char> pemContents, IEnumerable<Range> pemLines)
        {
            var blocks = new List<PemBlockInfo>();

            string? curBlockType = null;
            Index? curBlockStart = null;
            Index? curBlockEnd = null;

            foreach (var pemLineRange in pemLines)
            {
                ReadOnlySpan<char> pemLine = pemContents[pemLineRange];
                if (pemLine.IsEmpty || pemLine.IsWhiteSpace())
                {
                    // Empty line. Ignore.
                    continue;
                }

                if (pemLine[0] == '-')
                {
                    // PEM block line.
                    // NOTE: "-" is no valid character in Base64. Thus a line starting
                    //   with "-" does not contain any sensitive information (at least
                    //   not in well-formed PEM files).
                    var lineAsString = new string(pemLine);

                    if (curBlockType is null)
                    {
                        curBlockType = DecodeBlockStart(lineAsString);
                    }
                    else
                    {
                        var blockType = DecodeBlockEnd(lineAsString);
                        if (blockType != curBlockType)
                        {
                            throw new PemFormatException("Malformed PEM file (block types don't match)");
                        }

                        if (curBlockStart is null || curBlockEnd is null)
                        {
                            throw new PemFormatException("Malformed PEM file (no block contents)");
                        }

                        var blockInfo = new PemBlockInfo(blockType, new Range(curBlockStart.Value, curBlockEnd.Value));
                        blocks.Add(blockInfo);
                        curBlockType = null;
                    }
                }
                else
                {
                    // Non-empty content line

                    if (curBlockType is null)
                    {
                        throw new PemFormatException("Malformed PEM file (missing block begin)");
                    }

                    // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
                    if (curBlockStart is null)
                    {
                        // First content line of the current block.
                        curBlockStart = pemLineRange.Start;
                    }

                    curBlockEnd = pemLineRange.End;
                }
            }

            if (blocks.Count == 0)
            {
                throw new PemFormatException("Malformed PEM file (no content)");
            }

            return blocks;
        }

        [MustUseReturnValue]
        private static string DecodeBlockStart(string line)
        {
            var beginMatch = BLOCK_START_REGEX.Match(line);
            if (!beginMatch.Success)
            {
                throw new PemFormatException("Malformed PEM file (expected block begin).");
            }

            return beginMatch.Groups[1].Value;
        }

        [MustUseReturnValue]
        private static string DecodeBlockEnd(string line)
        {
            var beginMatch = BLOCK_END_REGEX.Match(line);
            if (!beginMatch.Success)
            {
                throw new PemFormatException("Malformed PEM file (expected block end).");
            }

            return beginMatch.Groups[1].Value;
        }
    }

    /// <summary>
    /// Information about a block inside of a PEM file.
    /// </summary>
    public sealed class PemBlockInfo
    {
        /// <summary>
        /// The block type; i.e. the text after "----BEGIN " and "-----END ". For example,
        /// "CERTIFICATE", "PUBLIC KEY", "RSA PRIVATE KEY", ...
        /// </summary>
        /// <remarks>
        /// For this, see also: https://stackoverflow.com/a/5356351/614177
        /// </remarks>
        [PublicAPI]
        public string BlockType { get; }

        /// <summary>
        /// The range of the blocks contents within the PEM file. Excludes the "----BEGIN" and
        /// "-----END" lines. The index is byte-based (not line-based).
        /// </summary>
        /// <remarks>
        /// For security reasons, the actual contents of the block are not stored inside
        /// this instance - as they could be sensitive (i.e. a private key). This is why
        /// we only store the range.
        /// </remarks>
        [PublicAPI]
        public Range BlockContentRange { get; }

        public PemBlockInfo(string blockType, Range blockContentRange)
        {
            this.BlockType = blockType;
            this.BlockContentRange = blockContentRange;
        }
    }
}
