﻿// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Runtime.CompilerServices;

namespace Markdig.Helpers
{
    /// <summary>
    /// A lightweight struct that represents a slice of a string.
    /// </summary>
    /// <seealso cref="ICharIterator" />
    public struct StringSlice : ICharIterator
    {
        /// <summary>
        /// An empty string slice.
        /// </summary>
        public static readonly StringSlice Empty = new StringSlice(string.Empty);

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSlice"/> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        public StringSlice(string text)
        {
            Text = text;
            Start = 0;
            End = (Text?.Length ?? 0) - 1;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringSlice"/> struct.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public StringSlice(string text, int start, int end)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            Text = text;
            Start = start;
            End = end;
        }

        /// <summary>
        /// The text of this slice.
        /// </summary>
        public readonly string Text;

        /// <summary>
        /// Gets or sets the start position within <see cref="Text"/>.
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// Gets or sets the end position (inclusive) within <see cref="Text"/>.
        /// </summary>
        public int End { get; set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public int Length => End - Start + 1;

        /// <summary>
        /// Gets the current character.
        /// </summary>
        public char CurrentChar => Start <= End ? this[Start] : '\0';

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        public bool IsEmpty => Start > End;

        /// <summary>
        /// Gets the <see cref="System.Char"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>A character in the slice at the specified index (not from <see cref="Start"/> but from the begining of the slice)</returns>
        public char this[int index] => Text[index];

        /// <summary>
        /// Goes to the next character, incrementing the <see cref="Start" /> position.
        /// </summary>
        /// <returns>
        /// The next character. `\0` is end of the iteration.
        /// </returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char NextChar()
        {
            Start++;
            if (Start > End)
            {
                Start = End + 1;
                return '\0';
            }
            return Text[Start];
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current <see cref="Start"/> position
        /// inside the range <see cref="Start"/> and <see cref="End"/>, returns `\0` if outside this range.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekChar(int offset)
        {
            var index = Start + offset;
            return index >= Start && index <= End ? Text[index] : (char) 0;
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current beginning of the string, without taking into account <see cref="Start"/> and <see cref="End"/>
        /// </summary>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekCharAbsolute(int index)
        {
            return index >= 0 && index < Text.Length ? Text[index] : (char)0;
        }

        /// <summary>
        /// Peeks a character at the specified offset from the current begining of the slice
        /// without using the range <see cref="Start"/> or <see cref="End"/>, returns `\0` if outside the <see cref="Text"/>.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns>The character at offset, returns `\0` if none.</returns>
        [MethodImpl(MethodImplOptionPortable.AggressiveInlining)]
        public char PeekCharExtra(int offset)
        {
            var index = Start + offset;
            return index >= 0 && index < Text.Length ? Text[index] : (char)0;
        }

        /// <summary>
        /// Matches the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public bool Match(string text, int offset = 0)
        {
            return Match(text, End, offset);
        }

        /// <summary>
        /// Matches the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="end">The end.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public bool Match(string text, int end, int offset)
        {
            var index = Start + offset;
            int i = 0;
            for (; index <= end && i < text.Length; i++, index++)
            {
                if (text[i] != Text[index])
                {
                    return false;
                }
            }

            return i == text.Length;
        }

        /// <summary>
        /// Matches the specified text using lowercase comparison.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="end">The end.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text matches; <c>false</c> otherwise</returns>
        public bool MatchLowercase(string text, int end, int offset)
        {
            var index = Start + offset;
            int i = 0;
            for (; index <= end && i < text.Length; i++, index++)
            {
                if (text[i] != char.ToLowerInvariant(Text[index]))
                {
                    return false;
                }
            }

            return i == text.Length;
        }

        /// <summary>
        /// Searches the specified text within this slice.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns><c>true</c> if the text was found; <c>false</c> otherwise</returns>
        public bool Search(string text, out int index)
        {
            return Search(text, 0, out index);
        }

        /// <summary>
        /// Searches the specified text within this slice.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text was found; <c>false</c> otherwise</returns>
        public bool Search(string text, int offset, out int index)
        {
            var end = End - text.Length + 1;
            index = Start + offset;
            for (int i = index; i <= end; i ++)
            {
                if (Match(text, End, i - Start))
                {
                    index = i + text.Length;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Searches for the specified character within this slice.
        /// </summary>
        /// <returns>A value >= 0 if the character was found, otherwise &lt; 0</returns>
        public int IndexOf(char c)
        {
            for (int i = Start; i <= End; i++)
            {
                if (Text[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Searches the specified text within this slice (matching lowercase).
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="offset">The offset.</param>
        /// <returns><c>true</c> if the text was found; <c>false</c> otherwise</returns>
        public bool SearchLowercase(string text, out int endOfIndex)
        {
            var end = End - text.Length + 1;
            endOfIndex = 0;
            for (int i = Start; i <= end; i++)
            {
                if (MatchLowercase(text, End, i - Start))
                {
                    endOfIndex = i + text.Length;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Trims whitespaces at the beginning of this slice starting from <see cref="Start"/> position.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if it has reaches the end of the iterator
        /// </returns>
        public bool TrimStart()
        {
            // Strip leading spaces
            var start = Start;
            for (; Start <= End; Start++)
            {
                if (!Text[Start].IsWhitespace())
                {
                    break;
                }
            }
            return start != Start;
        }

        /// <summary>
        /// Trims whitespaces at the beginning of this slice starting from <see cref="Start"/> position.
        /// </summary>
        /// <param name="spaceCount">The number of spaces trimmed.</param>
        public void TrimStart(out int spaceCount)
        {
            spaceCount = 0;
            // Strip leading spaces
            for (; Start <= End; Start++)
            {
                if (!Text[Start].IsWhitespace())
                {
                    break;
                }
                spaceCount++;
            }
        }

        /// <summary>
        /// Trims whitespaces at the end of this slice, starting from <see cref="End"/> position.
        /// </summary>
        /// <returns></returns>
        public bool TrimEnd()
        {
            for (; Start <= End; End--)
            {
                if (!Text[End].IsWhitespace())
                {
                    break;
                }
            }
            return IsEmpty;
        }

        /// <summary>
        /// Trims whitespaces from both the start and end of this slice.
        /// </summary>
        public void Trim()
        {
            TrimStart();
            TrimEnd();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Text != null && Start <= End)
            {
                var length = Length;
                if (Start == 0 && Text.Length == length)
                {
                    return Text;
                }

                return Text.Substring(Start, length);
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines whether this slice is empty or made only of whitespaces.
        /// </summary>
        /// <returns><c>true</c> if this slice is empty or made only of whitespaces; <c>false</c> otherwise</returns>
        public bool IsEmptyOrWhitespace()
        {
            for (int i = Start; i <= End; i++)
            {
                if (!Text[i].IsWhitespace())
                {
                    return false;
                }
            }
            return true;
        }
    }
}