using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Calmo.Web.JsonSerialization
{
    public sealed class JsonReader
    {
        internal static readonly long MinDateTimeTicks = (new DateTime(1970, 1, 1, 0, 0, 0)).Ticks;
        internal static readonly DateTime MinDate = new DateTime(100, 1, 1, 0, 0, 0);
        internal static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");

        private TextReader _reader;

        public JsonReader(string json)
            : this(new StringReader(json))
        {
        }

        public JsonReader(TextReader reader)
        {
            _reader = reader;
        }

        private char ReadNextCharacter()
        {
            return (char)_reader.Read();
        }

        private char PeekNextCharacter()
        {
            return (char)_reader.Peek();
        }

        private char ReadNextSignificantCharacter()
        {
            var ch = ReadNextCharacter();
            while ((ch != '\0') && Char.IsWhiteSpace(ch))
            {
                ch = ReadNextCharacter();
            }
            return ch;
        }

        private string ReadCharacters(int count)
        {
            var s = String.Empty;

            for (var i = 0; i < count; i++)
            {
                var ch = ReadNextCharacter();

                if (ch == '\0')
                    return null;

                s += ch;
            }

            return s;
        }

        private char PeekNextSignificantCharacter()
        {
            var ch = PeekNextCharacter();

            while ((ch != '\0') && Char.IsWhiteSpace(ch))
            {
                ReadNextCharacter();

                ch = PeekNextCharacter();
            }
            return ch;
        }

        private List<object> ReadArray()
        {
            var array = new List<object>();
            var arrayItems = (IList<object>)array;

            // Consume the '['
            ReadNextCharacter();

            while (true)
            {
                var ch = PeekNextSignificantCharacter();
                if (ch == '\0')
                    throw new FormatException("Unterminated array literal.");

                if (ch == ']')
                {
                    ReadNextCharacter();
                    return array;
                }

                if (arrayItems.Count != 0)
                {
                    if (ch != ',')
                        throw new FormatException("Invalid array literal.");

                    ReadNextCharacter();
                }

                var item = ReadValue();
                arrayItems.Add(item);
            }
        }

        private bool ReadBoolean()
        {
            var s = ReadName(false);

            if (s != null)
            {
                if (s.Equals("true", StringComparison.Ordinal))
                    return true;

                if (s.Equals("false", StringComparison.Ordinal))
                    return false;
            }

            throw new FormatException("Invalid boolean literal.");
        }

        private string ReadName(bool allowQuotes)
        {
            var ch = PeekNextSignificantCharacter();

            if ((ch == '"') || (ch == '\''))
            {
                if (allowQuotes)
                {
                    return ReadString();
                }
            }
            else
            {
                var sb = new StringBuilder();

                while (true)
                {
                    ch = PeekNextCharacter();

                    if ((ch != '_') && !Char.IsLetterOrDigit(ch))
                    {
                        return sb.ToString();
                    }

                    ReadNextCharacter();
                    sb.Append(ch);
                }
            }

            return null;
        }

        private void ReadNull()
        {
            var s = ReadName(false);

            if ((s == null) || !s.Equals("null", StringComparison.Ordinal))
            {
                throw new FormatException("Invalid null literal.");
            }
        }

        private object ReadNumber()
        {
            var ch = ReadNextCharacter();

            var sb = new StringBuilder();

            sb.Append(ch);
            while (true)
            {
                ch = PeekNextSignificantCharacter();

                if (!Char.IsDigit(ch) && (ch != '.'))
                    break;

                ReadNextCharacter();
                sb.Append(ch);
            }

            var s = sb.ToString();
            var hasDecimal = s.Contains(".");

            if (hasDecimal)
            {
                decimal value;
                if (Decimal.TryParse(s, NumberStyles.Any, DefaultCulture, out value))
                    return value;
            }
            else
            {
                int value;
                if (Int32.TryParse(s, out value))
                    return value;

                long lvalue;
                if (Int64.TryParse(s, out lvalue))
                    return lvalue;
            }

            throw new FormatException("Invalid numeric literal.");
        }

        private JsonObject ReadObject()
        {
            var recordItems = new Dictionary<string, object>();

            // Consume the '{'
            ReadNextCharacter();

            while (true)
            {
                var ch = PeekNextSignificantCharacter();

                if (ch == '\0')
                    throw new FormatException("Unterminated object literal.");

                if (ch == '}')
                {
                    ReadNextCharacter();
                    return new JsonObject(recordItems);
                }

                if (recordItems.Count != 0)
                {
                    if (ch != ',')
                        throw new FormatException("Invalid object literal.");

                    ReadNextCharacter();
                }

                var name = ReadName(true);
                ch = PeekNextSignificantCharacter();

                if (ch != ':')
                    throw new FormatException("Unexpected name/value pair syntax in object literal.");

                ReadNextCharacter();

                var item = ReadValue();
                recordItems[name] = item;
            }
        }

        private string ReadString()
        {
            bool dummy;
            return ReadString(out dummy);
        }

        private string ReadString(out bool hasLeadingSlash)
        {
            var sb = new StringBuilder();

            var endQuoteCharacter = ReadNextCharacter();
            var inEscape = false;
            var firstCharacter = true;

            hasLeadingSlash = false;

            while (true)
            {
                var ch = ReadNextCharacter();
                if (ch == '\0')
                    throw new FormatException("Unterminated string literal.");

                if (firstCharacter)
                {
                    if (ch == '\\')
                        hasLeadingSlash = true;

                    firstCharacter = false;
                }

                if (inEscape)
                {
                    if (ch == 'u')
                    {
                        var unicodeSequence = ReadCharacters(4);
                        if (unicodeSequence == null)
                            throw new FormatException("Unterminated string literal.");

                        ch = (char)Int32.Parse(unicodeSequence, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
                    }

                    sb.Append(ch);
                    inEscape = false;
                    continue;
                }

                if (ch == '\\')
                {
                    inEscape = true;
                    continue;
                }

                if (ch == endQuoteCharacter)
                {
                    return sb.ToString();
                }

                sb.Append(ch);
            }
        }

        public object ReadValue()
        {
            object value = null;
            var allowNull = false;

            var ch = PeekNextSignificantCharacter();
            switch (ch)
            {
                case '[':
                    value = ReadArray();
                    break;
                case '{':
                    value = ReadObject();
                    break;
                case '"':
                case '\'':
                    bool hasLeadingSlash;
                    var s = ReadString(out hasLeadingSlash);

                    if (hasLeadingSlash && s.StartsWith("@") && s.EndsWith("@"))
                    {
                        long ticks;

                        if (Int64.TryParse(s.Substring(1, s.Length - 2), out ticks))
                        {
                            value = new DateTime(ticks * 10000 + JsonReader.MinDateTimeTicks, DateTimeKind.Utc);
                        }
                    }

                    if (value == null)
                        value = s;

                    break;
                default:
                    if (Char.IsDigit(ch) || (ch == '-') || (ch == '.'))
                        value = ReadNumber();
                    else if ((ch == 't') || (ch == 'f'))
                        value = ReadBoolean();
                    else if (ch == 'n')
                    {
                        ReadNull();
                        allowNull = true;
                    }
                    break;
            }

            if ((value == null) && (allowNull == false))
            {
                throw new FormatException("Invalid JSON text.");
            }

            return value;
        }
    }
}