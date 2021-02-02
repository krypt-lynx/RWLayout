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
    static class ConstraintParser
    {
        private enum ParseState
        {
            firstWord,
            word,
            anchor,
            equality,
            syntaxError,
        };

        static private void AddExpressionComponent(ref ClLinearExpression expression, char operation, ClLinearExpression value)
        {
            switch (operation)
            {
                case '+':
                    expression += value;
                    break;
                case '-':
                    expression -= value;
                    break;
                case '*':
                    expression *= value;
                    break;
                case '/':
                    expression /= value;
                    break;
                default:
                    throw new ArgumentException($"{operation} in not a valid linear operation", nameof(operation));
            }
        }

        static private Cl.Operator codeToOperator(char code)
        {
            switch (code)
            {
                case '>':
                    return Cl.Operator.GreaterThanOrEqualTo;
                case '<':
                    return Cl.Operator.LessThanOrEqualTo;
                case '=':
                    return Cl.Operator.EqualTo;
                default:
                    throw new ArgumentException(nameof(code));
            }
        }

        static private ClVariable GetAnchor(string viewId, string anchorName, Dictionary<string, object> objects)
        {
            if (!objects.ContainsKey(viewId))
            {
                throw new ArgumentOutOfRangeException($"{viewId} is not a known view Id", nameof(viewId));
            }

            var view = objects[viewId];

            var prop = view.GetType().GetMemberHandler(anchorName, BindingFlags.Public | BindingFlags.Instance  );
            if (prop == null || !typeof(ClAbstractVariable).IsAssignableFrom(prop.MemberType))
            {
                throw new Exception($"{anchorName} is not an anchor in object {view}");
            }

            return prop.GetValue(view) as ClVariable;
        }

        static public ClConstraint CreateConstraint(string constraint, Dictionary<string, object> objects)
        {
            ClLinearExpression expression = new ClLinearExpression();


            ClStrength strength = ClStrength.Default;
            ParseState state = ParseState.firstWord;
            StringBuilder word = new StringBuilder();
            string viewId = "";
            char op = '+';
            char eq = '\0';

            foreach (var ch in constraint)
            {
                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }

                switch (state)
                {
                    case ParseState.firstWord:
                    case ParseState.word:
                    case ParseState.anchor:
                        {
                            if (char.IsLetterOrDigit(ch))
                            {
                                word.Append(ch);
                            }
                            else if (ch == ':' && state == ParseState.firstWord)
                            {
                                strength = GetStrength(word.ToString());
                                word.Clear();
                                state = ParseState.word;
                            }
                            else if (ch == '.' && (state == ParseState.firstWord || state == ParseState.word))
                            {
                                viewId = word.ToString();
                                word.Clear();
                                state = ParseState.anchor;
                            }
                            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                            {
                                ClLinearExpression component;
                                if (state == ParseState.firstWord || state == ParseState.word)
                                {
                                    component = Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    component = GetAnchor(viewId, word.ToString(), objects);
                                }

                                AddExpressionComponent(ref expression, op, component);

                                word.Clear();
                                op = ch;
                                state = ParseState.word;
                            }
                            else if (ch == '=' || ch == '<' || ch == '>')
                            {
                                ClLinearExpression component;
                                if (state == ParseState.firstWord || state == ParseState.word)
                                {
                                    component = Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    component = GetAnchor(viewId, word.ToString(), objects);
                                }
                                
                                AddExpressionComponent(ref expression, op, component);

                                word.Clear();
                                expression.MultiplyMe(-1);
                                op = '+';

                                if (eq == 0)
                                {
                                    eq = ch;
                                    state = ParseState.equality;
                                }
                                else
                                {
                                    state = ParseState.syntaxError;
                                    throw new FormatException("equality symbol already used");
                                }
                            }
                            else
                            {
                                state = ParseState.syntaxError;
                            }
                        }
                        break;
                    case ParseState.equality:
                        {
                            if (ch == '=')
                            {
                                state = ParseState.word;
                            }
                            else
                            {
                                state = ParseState.syntaxError;
                            }
                        }
                        break;
                    case ParseState.syntaxError:
                        throw new FormatException($"syntax error: {constraint}");
                    default:
                        throw new InvalidOperationException();
                }
            }

            switch (state)
            {
                case ParseState.firstWord:
                case ParseState.word:
                    {
                        AddExpressionComponent(ref expression, op, Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture));
                        word.Clear();
                    }
                    break;
                case ParseState.anchor:
                    {
                        var anchor = GetAnchor(viewId, word.ToString(), objects);
                        AddExpressionComponent(ref expression, op, anchor);
                        word.Clear();
                    }
                    break;
                case ParseState.equality:
                case ParseState.syntaxError:
                    throw new FormatException($"syntax error: {constraint}");
                default:
                    throw new InvalidOperationException();
            }

            return new ClLinearConstraint(0, codeToOperator(eq), expression, strength);
        }

        private static ClStrength GetStrength(string strengthName)
        {
            switch (strengthName.ToLowerInvariant())
            {
                case "required":
                    return ClStrength.Required;
                case "strong":
                    return ClStrength.Strong;
                case "medium":
                    return ClStrength.Medium;
                case "weak":
                    return ClStrength.Weak;
                default:
                    throw new ArgumentException($"{strengthName} is not a valid strengthName", nameof(strengthName));
            }
        }
    }

}
