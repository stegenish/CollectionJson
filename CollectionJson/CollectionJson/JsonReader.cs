using System.Runtime.InteropServices;
using System.Text;

namespace CollectionJson;

public class JsonReader
{
    private readonly string _buffer;
    private int _nextChar;

    public JsonReader(string json)
    {
        _buffer = json;
        _nextChar = 0;
    }

    public (char c, bool eof) Peek()
    {
        return ReadChar(_nextChar);
    }

    public bool Peek(out char c)
    {
        bool eof;
        (c, eof) = Peek();
        return eof;
    }

    public (char c, bool eof) Read()
    {
        return ReadChar( _nextChar++);
    }

    private (char c, bool eof) ReadChar(int i)
    {
        if (_buffer.Length > i)
        {
            return (_buffer[i], false);
        }

        return ('.', true);
    }

    public string Consume(string s)
    {
        int startChar = _nextChar;
        foreach (var c in s)
        {
            char b;
            bool eof;
            (b, eof) = Read();
            if (b != c || eof)
            {
                var bufferLength = _buffer.Length >= _nextChar ? _buffer.Length - 1 : _nextChar;
                throw new LexerException($"Expected {s}, found {_buffer.Substring(startChar, bufferLength)}");
            }
        }

        return s;
    }

    public bool ConsumeIfMatch(string s)
    {
        int startChar = _nextChar;
        foreach (var c in s)
        {
            char b;
            bool eof;
            (b, eof) = Read();
            if (b != c || eof)
            {
                _nextChar = startChar;
                return false;
            }
        }

        return true;
    }

    public string Consume(char c)
    {
        return Consume(c.ToString());
    }

    public JsonToken ReadToken(string tokenString, TokenType tokenType)
    {
        if (ConsumeIfMatch(tokenString))
        {
            return CreateToken(tokenString, tokenType);
        }

        throw new LexerException("Invalid token starting at");
    }

    public JsonToken ReadToken(char c, TokenType tokenType)
    {
        return CreateToken(Consume(c), tokenType);
    }

    public JsonToken ReadNumber()
    {
        var buffer = new StringBuilder();
        bool lookingForToken = true;

        var tokenType = TokenType.Integer;

        while (lookingForToken && !Peek(out char c))
        {
            if (Char.IsDigit(c))
            {
                buffer.Append(Read().c);
            }
            else if (c == '.')
            {
                buffer.Append(Read().c);
                tokenType = TokenType.Decimal;
            }
            else
            {
                lookingForToken = false;
            }
        }

        return CreateToken(buffer.ToString(), tokenType);
    }

    public JsonToken ReadWhiteSpace()
    {
        var buffer = new StringBuilder();
        bool lookingForToken = true;

        while (lookingForToken && !Peek(out char c))
        {
            if (Char.IsWhiteSpace(c))
            {
                buffer.Append(Read().c);
            }
            else
            {
                lookingForToken = false;
            }
        }

        return CreateToken(buffer.ToString(), TokenType.WhiteSpace);
    }

    public JsonToken ReadString()
    {
        var buffer = new StringBuilder(Consume("\""));
        bool lookingForToken = true;

        while (lookingForToken && !Peek(out char c))
        {
            if (c == '\\')
            {
                buffer.Append(ReadEscapeSequence());
            }
            else if (c != '"')
            {
                
                buffer.Append(Read().c);
            }
            else
            {
                buffer.Append(Consume("\""));

                lookingForToken = false;
            }
        }

        return CreateToken(buffer.ToString(), TokenType.String);
    }

    private char ReadEscapeSequence()
    {
        Consume("\\");
        var value = Read().c switch
        {
            '"' => '"',
            '\\' => '\\',
            '/' => '/',
            'b' => '\b',
            'f' => '\f',
            'r' => '\r',
            'n' => '\n',
            't' => '\t',
            var c => throw new LexerException($"Invalid escape sequence: {c}")
        };
        return value;
    }

    private JsonToken CreateToken(string s, TokenType tokenType)
    {
        return new JsonToken(s, tokenType);
    }
}