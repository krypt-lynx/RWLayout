using Cassowary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RWLayout.alpha2
{
    public class ViewDef : Def
    {
        [Unsaved(false)]
        public ElementPrototype Prototype;
        public string test = "";
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            XmlElement node = xmlRoot as XmlElement;

            if (node != null)
            {
                defName = node.SelectSingleNode("defName/text()")?.Value;
                test = defName;
                var prototypeNode = node.SelectSingleNode("view/*") as XmlElement;
                if (prototypeNode != null)
                {
                    Prototype = new ElementPrototype(prototypeNode);
                }
            }
        }

        public CElement Instantiate()
        {
            Dictionary<string, CElement> viewMap = new Dictionary<string, CElement>();
            CElement root = InstantiateViewsTree(Prototype, viewMap);
            ApplyConstraintsRecursive(root, Prototype, viewMap);
            return root;
        }

        private void ApplyConstraintsRecursive(CElement owner, ElementPrototype root, Dictionary<string, CElement> viewMap)
        {
            ApplyConstraints(owner, root, viewMap);

            if (root.Subviews != null)
            {
                foreach (var view in root.Subviews)
                {
                    ApplyConstraints(owner, view, viewMap);
                }
            }
        }

        private void ApplyConstraints(CElement owner, ElementPrototype root, Dictionary<string, CElement> viewMap)
        {
            if (root.Constraints != null) {
                foreach (var constraintStr in root.Constraints)
                {
                    owner.AddConstraint(CreateConstraint(constraintStr, viewMap));
                }
            }
        }

        private CElement InstantiateViewsTree(ElementPrototype p, Dictionary<string, CElement> viewMap)
        {
            var ns = "RWLayout.alpha2";

            var type = GenTypes.GetTypeInAnyAssembly(p.Class, ns) ?? GenTypes.GetTypeInAnyAssembly($"{ns}.{p.Class}", ns);

            var view = (CElement)Activator.CreateInstance(type);
            if (p.Id != null)
            {
                viewMap[p.Id] = view;
            }

                if (p.Subviews != null)
            {
                view.AddElements(p.Subviews.Select(x => InstantiateViewsTree(x, viewMap)));
            }


            return view;
        }
    }

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

        static private void AddExpressionComponent(ClLinearExpression expression, char operation, ClLinearExpression value)
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
                    throw new ArgumentException(nameof(operation));
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

        static public ClConstraint CreateConstraint(string constraint, Dictionary<string, CElement> viewMap)
        {
            ClLinearExpression expression = new ClLinearExpression();


            ClVariable GetAnchor(string viewId, string v)
            {
                if (!viewMap.ContainsKey(viewId))
                {
                    throw new ArgumentOutOfRangeException(nameof(viewId));
                }

                var view = viewMap[viewId];

                var prop = view.GetType().GetProperty(v, typeof(ClVariable));
                if (prop == null)
                {
                    throw new Exception("prop");
                }

                return prop.GetValue(view) as ClVariable;
            }

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
                            else if (ch == '.')
                            {
                                viewId = word.ToString();
                                word.Clear();
                                state = ParseState.anchor;
                            }
                            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                            {
                                AddExpressionComponent(expression, op, Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture));
                                word.Clear();
                                op = ch;
                                state = ParseState.word;
                            }
                            else if (ch == '=' || ch == '<' || ch == '>')
                            {
                                AddExpressionComponent(expression, op, Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture));
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
                                }
                            }
                            else
                            {
                                state = ParseState.syntaxError;
                            }
                        }
                        break;
                    case ParseState.anchor:
                        {
                            if (char.IsLetterOrDigit(ch))
                            {
                                word.Append(ch);
                            }
                            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
                            {
                                var anchor = GetAnchor(viewId, word.ToString());
                                AddExpressionComponent(expression, op, anchor);
                                word.Clear();
                                op = ch;
                                state = ParseState.word;
                            }
                            else if (ch == '=' || ch == '<' || ch == '>')
                            {
                                var anchor = GetAnchor(viewId, word.ToString());
                                AddExpressionComponent(expression, op, anchor);
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
                        AddExpressionComponent(expression, op, Convert.ToDouble(word.ToString(), CultureInfo.InvariantCulture));
                        word.Clear();
                    }
                    break;
                case ParseState.anchor:
                    {
                        var anchor = GetAnchor(viewId, word.ToString());
                        AddExpressionComponent(expression, op, anchor);
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
                    throw new ArgumentException(nameof(strengthName));
            }
        }
    }

    public class ElementPrototype
    {
        public List<ElementPrototype> Subviews;
        public List<string> Constraints;
        public string Id;
        public string Class;

        public ElementPrototype(XmlElement node)
        {
            Class = node.Name;
            Id = node.GetAttribute("Id");

            var viewNodes = node.SelectNodes("subviews/*").AsEnumerable<XmlNode>().Where(x => x.NodeType == XmlNodeType.Element).Cast<XmlElement>();

            Subviews = viewNodes.Select(x => new ElementPrototype(x)).ToList();

            var constraintsStr = node.SelectSingleNode("constraints/text()")?.Value;
            Constraints = constraintsStr?.Split('\n').Select(x => x.Trim(" \t".ToCharArray())).Where(x => x.Length > 0).ToList();

           // ParseConstraints(constraintsStr);
        }

        internal void ParseConstraints(XmlElement prototypeNode)
        {
            throw new NotImplementedException();
        }

        /*
        private void ParseConstraints(string constraintsStr)
        {
            throw new NotImplementedException();
        }*/
    }
}
