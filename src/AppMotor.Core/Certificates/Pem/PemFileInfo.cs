﻿// SPDX-License-Identifier: MIT
// Copyright AppMotor Framework (https://github.com/skrysmanski/AppMotor)

using System.Collections.Immutable;
using System.Text.RegularExpressions;

using JetBrains.Annotations;

namespace AppMotor.Core.Certificates.Pem;

/// <summary>
/// Provides information about the blocks in a PEM file. Get the information
/// via <see cref="Blocks"/>.
/// </summary>
public sealed class PemFileInfo
{
    private static readonly Regex BLOCK_START_REGEX = new("^-----BEGIN (.+)-----$", RegexOptions.Compiled);

    private static readonly Regex BLOCK_END_REGEX = new("^-----END (.+)-----$", RegexOptions.Compiled);

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
                // NOTE: "-" is no valid character in Base64. Thus, a line starting
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
                        throw new PemFormatException("Malformed PEM file (block types don't match).");
                    }

                    if (curBlockStart is null || curBlockEnd is null)
                    {
                        throw new PemFormatException("Malformed PEM file (no block contents).");
                    }

                    var blockInfo = new PemBlockInfo(blockType, new Range(curBlockStart.Value, curBlockEnd.Value));
                    blocks.Add(blockInfo);
                    curBlockType = null;
                    curBlockStart = null;
                    curBlockEnd = null;
                }
            }
            else
            {
                // Non-empty content line

                if (curBlockType is null)
                {
                    throw new PemFormatException("Malformed PEM file (missing block begin).");
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
            throw new PemFormatException("Malformed PEM file (no content).");
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
