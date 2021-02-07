using Cassowary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.alpha2
{
    public class ConstraintSegment
    {
        public string OwnerId;
        public string AnchorName;
        public double Multiplier;

        public override string ToString()
        {
            return OwnerId != null ? $"{Multiplier}×{OwnerId}.{AnchorName}" : $"{Multiplier}";
        }
    }

    public class ConstraintDescription
    {
        public List<ConstraintSegment> Terms;
        public Cl.Operator Operator;
        public ClStrength Strength;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Strength.Name);
            sb.Append(":");

            bool isFirst = true;

            foreach (var term in Terms)
            {
                if (term.Multiplier >= 0)
                {
                    if (!isFirst)
                    {
                        sb.Append("+");
                    }
                }
                else
                {
                    sb.Append("-");
                }
                var abs = Math.Abs(term.Multiplier);
                if (abs != 1 || term.OwnerId == null)
                {
                    sb.Append(abs.ToString(CultureInfo.InvariantCulture));
                }
                if (term.OwnerId != null)
                {
                    if (abs != 1)
                    {
                        sb.Append($"×");
                    }
                    sb.Append($"{term.OwnerId}.{term.AnchorName}");
                }
                isFirst = false;
            }

            switch (Operator)
            {
                case Cl.Operator.EqualTo:
                    sb.Append("=");
                    break;
                case Cl.Operator.GreaterThanOrEqualTo:
                    sb.Append("≥");
                    break;
                case Cl.Operator.LessThanOrEqualTo:
                    sb.Append("≤");
                    break;
            }

            sb.Append("0");

            return sb.ToString();
        }
    }

    enum ConstraintParserState
    {
        firstChar,
        leadNumber,
        word,
        anchor,
        number,
        equality,
        done,
        syntaxError,
    }

    class SemanticErrorException : Exception
    {
        public SemanticErrorException() { }
        public SemanticErrorException(string message) : base(message) { }
        public SemanticErrorException(string message, Exception innerException) : base(message, innerException) { }
    }

    class SyntaxErrorException : Exception
    {
        public SyntaxErrorException() { }
        public SyntaxErrorException(string message) : base(message) { }
        public SyntaxErrorException(string message, Exception innerException) : base(message, innerException) { }
    }

    class InternalErrorException : Exception
    {
        public InternalErrorException() { }
        public InternalErrorException(string message) : base(message) { }
        public InternalErrorException(string message, Exception innerException) : base(message, innerException) { }
    }

    class ConstraintParser
    {
        /// <summary>
        /// table of state handlers
        /// </summary>
        Dictionary<ConstraintParserState, Func<ConstraintParserState, char, ConstraintParserState>> stateTable;

        /// <summary>
        /// current parser state
        /// </summary>
        ConstraintParserState state;

        /// <summary>
        /// object parser building
        /// </summary>
        public ConstraintDescription prototype;

        public ConstraintParser()
        {
            state = ConstraintParserState.word;
            stateTable = new Dictionary<ConstraintParserState, Func<ConstraintParserState, char, ConstraintParserState>>
            {
                { ConstraintParserState.firstChar, ParseFirstChar },
                { ConstraintParserState.leadNumber, ParseLeadNumber },
                { ConstraintParserState.word, ParseWord },
                { ConstraintParserState.anchor, ParseAnchor },
                { ConstraintParserState.number, ParseNumber },
                { ConstraintParserState.equality, ParseEquality },
                { ConstraintParserState.done, ParseDone },
            };
        }

        /// <summary>
        /// Syntax Error message
        /// </summary>
        string errorMessage = null;

        /// <summary>
        /// current word
        /// </summary>
        StringBuilder word = new StringBuilder();


        bool willAcceptStrength = true;

        bool eqRelationDefined = false;

        bool hasLeadNumber = false; // lead: +c*x -c*x  post: +x*c +x/c -x*c -x/c
        string segmentMultipler = null;
        string segmentTarget = null;
        string segmentAnchor = null;
        char segmentSign = '+';
        char segmentOp = '*';

        private ConstraintParserState ParseFirstChar(ConstraintParserState state, char ch)
        {
            if (ch == '\0')
            {
                if (eqRelationDefined)
                {
                    willAcceptStrength = false;
                    AppendCurrentTerm();
                    return ConstraintParserState.done;
                }
                else
                {
                    errorMessage = "End of expression is reached, but operator is not defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else if (char.IsDigit(ch))
            {
                willAcceptStrength = false;
                word.Append(ch);
                return ConstraintParserState.leadNumber;
            }
            else if (char.IsLetter(ch))
            {
                word.Append(ch);
                return ConstraintParserState.word;
            }
            else if (IsMultiplicationOperator(ch))
            {
                willAcceptStrength = false;
                SetMultipler();
                segmentOp = ch;
                hasLeadNumber = true;
                return ConstraintParserState.word;
            }
            else if (IsAditionOperator(ch))
            {
                willAcceptStrength = false;
                AppendCurrentTerm();
                segmentSign = ch;
                return ConstraintParserState.firstChar;
            }
            else if (IsEqualityOperator(ch))
            {
                if (!eqRelationDefined)
                {
                    willAcceptStrength = false;
                    AppendCurrentTerm();
                    SetEquationSign(ch);
                    return ConstraintParserState.equality;
                }
                else
                {
                    errorMessage = "Operator already defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else
            {
                errorMessage = "Unexpected symbol";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseLeadNumber(ConstraintParserState state, char ch)
        {
            if (ch == '\0')
            {
                if (eqRelationDefined)
                {
                    SetMultipler();
                    AppendCurrentTerm();
                    return ConstraintParserState.done;
                }
                else
                {
                    errorMessage = "End of expression is reached, but operator is not defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else if (char.IsDigit(ch) || ch == '.')
            {
                word.Append(ch);
                return ConstraintParserState.leadNumber;
            }
            else if (IsMultiplicationOperator(ch))
            {
                SetMultipler();
                segmentOp = ch;
                return ConstraintParserState.word;
            }
            else if (IsAditionOperator(ch))
            {
                SetMultipler();
                AppendCurrentTerm();
                segmentSign = ch;
                return ConstraintParserState.firstChar;
            }
            else if (IsEqualityOperator(ch))
            {
                if (!eqRelationDefined)
                {
                    willAcceptStrength = false;
                    SetMultipler();
                    AppendCurrentTerm();
                    SetEquationSign(ch);
                    return ConstraintParserState.equality;
                }
                else
                {
                    errorMessage = "Operator already defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else
            {
                errorMessage = "Unexpected symbol";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseWord(ConstraintParserState state, char ch)
        {
            if (ch == '\0')
            {
                errorMessage = "Unexpected end of expression";
                return ConstraintParserState.syntaxError;
            }
            else if (char.IsLetterOrDigit(ch))
            {
                word.Append(ch);
                return ConstraintParserState.word;
            }
            else if (ch == ':')
            {
                if (willAcceptStrength)
                {
                    SetStrength();
                    willAcceptStrength = false;
                    return ConstraintParserState.firstChar;
                }
                else
                {
                    errorMessage = "Unexpected symbol";
                    return ConstraintParserState.syntaxError;
                }
            }
            else if (ch == '.')
            {
                SetTarget();
                return ConstraintParserState.anchor;
            }
            else
            {
                errorMessage = "Anchor name expected";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseAnchor(ConstraintParserState state, char ch)
        {
            if (ch == '\0')
            {
                if (eqRelationDefined)
                {
                    SetAnchor();
                    AppendCurrentTerm();
                    return ConstraintParserState.done;
                }
                else
                {
                    errorMessage = "End of expression is reached, but operator is not defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else if (char.IsLetterOrDigit(ch))
            {
                word.Append(ch);
                return ConstraintParserState.anchor;
            }
            else if (IsMultiplicationOperator(ch))
            {
                if (hasLeadNumber)
                {
                    errorMessage = "term constant already defined";
                    return ConstraintParserState.syntaxError;
                }
                else
                {
                    SetAnchor();
                    segmentOp = ch;
                    return ConstraintParserState.number;
                }
            }
            else if (IsAditionOperator(ch))
            {
                SetAnchor();
                AppendCurrentTerm();
                segmentSign = ch;
                return ConstraintParserState.firstChar;
            }
            else if (IsEqualityOperator(ch))
            {
                if (!eqRelationDefined)
                {
                    willAcceptStrength = false;
                    SetAnchor();
                    AppendCurrentTerm();
                    SetEquationSign(ch);
                    return ConstraintParserState.equality;
                }
                else
                {
                    errorMessage = "Operator already defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else
            {
                errorMessage = "Unexpected symbol";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseNumber(ConstraintParserState state, char ch)
        {
            if (ch == '\0')
            {
                if (eqRelationDefined)
                {
                    SetMultipler();
                    AppendCurrentTerm();
                    return ConstraintParserState.done;
                }
                else
                {
                    errorMessage = "End of expression is reached, but operator is not defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else if (char.IsDigit(ch) || ch == '.')
            {
                word.Append(ch);
                return ConstraintParserState.number;
            }
            else if (IsAditionOperator(ch))
            {
                SetMultipler();
                AppendCurrentTerm();
                segmentSign = ch;
                return ConstraintParserState.firstChar;
            }
            else if (IsEqualityOperator(ch))
            {
                if (!eqRelationDefined)
                {
                    willAcceptStrength = false;
                    SetMultipler();
                    AppendCurrentTerm();
                    SetEquationSign(ch);
                    return ConstraintParserState.equality;
                }
                else
                {
                    errorMessage = "Operator already defined";
                    return ConstraintParserState.syntaxError;
                }
            }
            else
            {
                errorMessage = "Unexpected symbol";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseEquality(ConstraintParserState state, char ch)
        {
            if (ch == '=')
            {
                return ConstraintParserState.firstChar;
            }
            else
            {
                errorMessage = "Unexpected symbol";
                return ConstraintParserState.syntaxError;
            }
        }

        private ConstraintParserState ParseDone(ConstraintParserState state, char ch)
        {
            errorMessage = "symbol after expression end";
            return ConstraintParserState.syntaxError; // we are ended parsing, there shouldn't be any characters after that
        }

        /// <summary>
        /// sets anchor name and resets the word
        /// </summary>
        private void SetAnchor()
        {
            segmentAnchor = word.ToString();
            word.Clear();
        }

        /// <summary>
        /// sets target name and resets the word
        /// </summary>
        private void SetTarget()
        {
            segmentTarget = word.ToString();
            word.Clear();
        }

        /// <summary>
        /// sets strength and resets the word
        /// </summary>
        private void SetStrength()
        {
            prototype.Strength = GetStrength(word.ToString());
            word.Clear();
        }

        /// <summary>
        /// sets term multipler
        /// </summary>
        private void SetMultipler()
        {
            segmentMultipler = word.ToString();
            word.Clear();
        }

        /// <summary>
        /// creates term, adds it to expression, and resets variables (but not word)
        /// </summary>
        private void AppendCurrentTerm()
        {
            if (hasLeadNumber && segmentOp == '/')
            {
                throw new SemanticErrorException("division by variable is not allowed");
            }
            var Multiplier = segmentMultipler != null ? Convert.ToDouble(segmentMultipler, CultureInfo.InvariantCulture) : 1;
            if (segmentOp == '/')
            {
                if (Multiplier == 0)
                {
                    throw new SemanticErrorException("division by zero");
                }
                Multiplier = 1 / Multiplier;
            }

            if (segmentSign == '-')
            {
                Multiplier = -Multiplier;
            }

            if (eqRelationDefined)
            {
                Multiplier = -Multiplier;
            }

            prototype.Terms.Add(new ConstraintSegment
            {
                Multiplier = Multiplier,
                OwnerId = segmentTarget,
                AnchorName = segmentAnchor,
            });

            hasLeadNumber = false;
            segmentAnchor = null;
            segmentTarget = null;
            segmentMultipler = null;
            segmentSign = '*';
            segmentOp = '+';
        }

        /// <summary>
        /// assigns operator
        /// </summary>
        /// <param name="ch"></param>
        private void SetEquationSign(char ch)
        {
            switch (ch)
            {
                case '>':
                    prototype.Operator = Cl.Operator.GreaterThanOrEqualTo;
                    break;
                case '<':
                    prototype.Operator = Cl.Operator.LessThanOrEqualTo;
                    break;
                case '=':
                    prototype.Operator = Cl.Operator.EqualTo;
                    break;
                default:
                    throw new InvalidOperationException("invalid character passed as equality relation character");
            }
            eqRelationDefined = true;
        }

        static HashSet<char> additionCharSet = new HashSet<char>("+-");
        bool IsAditionOperator(char ch)
        {
            return additionCharSet.Contains(ch);
        }

        static HashSet<char> multipicationCharSet = new HashSet<char>("*/");
        bool IsMultiplicationOperator(char ch)
        {
            return multipicationCharSet.Contains(ch);
        }

        static HashSet<char> equalityCharSet = new HashSet<char>(">=<");
        bool IsEqualityOperator(char ch)
        {
            return equalityCharSet.Contains(ch);
        }

        static ClStrength GetStrength(string strengthName)
        {
            switch (strengthName.ToLowerInvariant())
            {
                case "r":
                case "required":
                    return ClStrength.Required;
                case "s":
                case "strong":
                    return ClStrength.Strong;
                case "m":
                case "medium":
                    return ClStrength.Medium;
                case "w":
                case "weak":
                    return ClStrength.Weak;
                default:
                    throw new ArgumentException($"{strengthName} is not a valid constraint strength mame", nameof(strengthName));
            }
        }

        public ConstraintDescription Parse(string constraint)
        {
            state = ConstraintParserState.firstChar;
            word.Clear();
            willAcceptStrength = true;
            prototype = new ConstraintDescription();
            prototype.Terms = new List<ConstraintSegment>();
            prototype.Strength = ClStrength.Default;
            errorMessage = null;

            int charNumber = 0;
            foreach (var ch in constraint.Append('\0'))
            {
                if (char.IsWhiteSpace(ch))
                {
                    charNumber++;
                    continue;
                }

                state = stateTable[state](state, ch);
                if (state == ConstraintParserState.syntaxError)
                {
                    throw new SyntaxErrorException($"{errorMessage} at char {charNumber}");
                }
                charNumber++;
            }

            if (state != ConstraintParserState.done)
            {
                throw new InternalErrorException($"parser enden work in invalid state: {state}");
            }

            return prototype;
        }

    }

}

