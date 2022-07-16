using System.Runtime.InteropServices;
using System.Text;

namespace CollectionJson;

public class CJsonReader
{
    private readonly string _buffer;
    private int _nextChar;

    public CJsonReader(string json)
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
                throw new CJsonLexerException($"Expected {s}, found {_buffer.Substring(startChar, bufferLength)}");
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

    public CJsonToken ReadFalse()
    {
        if (ConsumeIfMatch("false"))
        {
            return CreateToken("false", TokenType.False);
        }

        throw new CJsonLexerException("Invalid token starting at");
    }

    public CJsonToken ReadToken(string tokenString, TokenType tokenType)
    {
        if (ConsumeIfMatch(tokenString))
        {
            return CreateToken(tokenString, tokenType);
        }

        throw new CJsonLexerException("Invalid token starting at");
    }

    public CJsonToken ReadToken(char c, TokenType tokenType)
    {
        return CreateToken(Consume(c), tokenType);
    }

    public CJsonToken ReadTrue()
    {
        if (ConsumeIfMatch("true"))
        {
            return CreateToken("true", TokenType.True);
        }

        throw new CJsonLexerException("Invalid token starting at");
    }

    public CJsonToken ReadNull()
    {
        if (ConsumeIfMatch("null"))
        {
            return CreateToken("null", TokenType.Null);
        }

        throw new CJsonLexerException("Invalid token starting at");
    }


    public CJsonToken ReadNumber()
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

    public CJsonToken ReadWhiteSpace()
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

    public CJsonToken ReadString()
    {
        var buffer = new StringBuilder(Consume("\""));
        bool lookingForToken = true;

        while (lookingForToken && !Peek(out char c))
        {
            if (c != '"')
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

    private CJsonToken CreateToken(string s, TokenType tokenType)
    {
        return new CJsonToken(s, tokenType);
    }
}