using Cassowary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace RWLayout.alpha2
{
    public class LayoutDef : Def
    {
        [Unsaved(false)]
        public ElementPrototype[] Views;
        [Unsaved(false)]
        public BindingPrototype[] Assignments;


        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            XmlElement node = xmlRoot as XmlElement;

            if (node != null)
            {
                defName = node.SelectSingleNode("defName/text()")?.Value;
                var nodes = node.SelectNodes("views/*").WhereTypeIs<XmlElement>();

                Views = nodes.Select(n => new ElementPrototype(n)).ToArray();

                var bindingNodes = node.SelectNodes("bindings/binding");
                Assignments = bindingNodes?.Cast<XmlElement>()
                    .Select(x => new BindingPrototype(x))
                    .ToArray();
            }
        }

        public CElement[] Instantiate(Dictionary<string, object> externalObjects = null)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Dictionary<string, object> objectsMap = new Dictionary<string, object>(externalObjects);
            Dictionary<CElement, ElementPrototype> viewPrototypes = new Dictionary<CElement, ElementPrototype>();

            var views = Views.Select(x => InstantiateViewsTree(x, objectsMap, viewPrototypes)).ToArray();

            ApplyConstraints(objectsMap, viewPrototypes);
            ApplyProperties(objectsMap, viewPrototypes);
            ApplyAssignments(objectsMap, Assignments);

            timer.Stop();
            $"LayoutDef{{{this.defName}}}: objects constructed in: {((decimal)timer.Elapsed.Ticks) / 10000000:0.0000000}".Log(MessageType.Message);

            return views;
        }


        private void ApplyAssignments(Dictionary<string, object> objectsMap, BindingPrototype[] assignments)
        {
            foreach (var assignment in assignments)
            {
                try
                {
                    PropertyWriter.Assign(assignment, objectsMap);
                }
                catch (Exception e)
                {
                    LogHelper.LogException("exception during binding", e);
                }
            }
        }

        private void ApplyProperties(Dictionary<string, object> objectsMap, Dictionary<CElement, ElementPrototype> viewPrototypes)
        {
            foreach (var kvp in viewPrototypes)
            {
                try
                {
                    Binder.ApplyProperties(kvp.Key, kvp.Value.Properties, objectsMap);
                }
                catch (Exception e)
                {
                    LogHelper.LogException("exception during property deserialization", e);
                }
            }
        }


        private void ApplyConstraints(Dictionary<string, object> objectsMap, Dictionary<CElement, ElementPrototype> viewPrototypes)
        {
            foreach (var kvp in viewPrototypes)
            {
                if (kvp.Value.Constraints != null)
                {
                    foreach (var prototype in kvp.Value.Constraints)
                    {
                        try
                        {
                            var constraint = prototype.Materialize(objectsMap);
                            if (prototype.Id != null)
                            {
                                objectsMap[prototype.Id] = constraint;
                            }
                            kvp.Key.AddConstraints(constraint);
                        } 
                        catch (Exception e)
                        {
                            LogHelper.LogException("exception during constraint deserialization", e);
                        }
                    }
                }
            }
        }

        private CElement InstantiateViewsTree(ElementPrototype p, Dictionary<string, object> objectMap, Dictionary<CElement, ElementPrototype> viewPrototypes)
        {
            var ns = "RWLayout.alpha2";

            var type = GenTypes.GetTypeInAnyAssembly(p.Class, ns) ?? GenTypes.GetTypeInAnyAssembly($"{ns}.{p.Class}", ns);

            var view = (CElement)Activator.CreateInstance(type);
            if (p.Id != null)
            {
                objectMap[p.Id] = view;
            }
            viewPrototypes[view] = p;
            
            if (p.Subviews != null)
            {
                view.AddElements(p.Subviews.Select(x => InstantiateViewsTree(x, objectMap, viewPrototypes)));
            }

            return view;
        }
    }

    public struct BindingPath
    {
        public string Object;
        public string Member;

        public BindingPath(string path)
        {
            var pathParts = path.Split('.');
            if (pathParts.Length > 2)
            {
                throw new FormatException($"{path} is invalid source object path");
            }

            Object = pathParts[0].Trim();
            if (pathParts.Length == 2)
            {
                Member = pathParts[1].Trim();
            } 
            else
            {
                Member = null;
            }
        }

        public override string ToString()
        {
            return Member == null ? Object : $"{Object}.{Member}";
        }
    }

    public struct BindingPrototype
    {
        public BindingPath Source;
        public BindingPath Target;

        public BindingPrototype(XmlElement x)
        {
            var src = x.GetAttribute("object");
            Source = new BindingPath(src);

            var dst = x.GetAttribute("to");
            Target = new BindingPath(dst);

            if (Target.Member == null)
            {
                throw new FormatException($"{dst} is invalid target object path");
            }
        }
    }


    public class Prototype
    {
        public string Id;

        public Prototype(XmlElement node)
        {            
            Id = node.HasAttribute("Id") ? node.GetAttribute("Id") : null;
        }
    }

    public class PropertyPrototype : Prototype
    {
        public enum PropertyPrototypeKind
        {
            TextRepresentation,
            AssignFrom,
            BindValue,
        }

        public string Name;
        public PropertyPrototypeKind Kind;
        public string Value;
        public bool Translate;
        public BindingPath Path;
        public Type TypeHint;

        public PropertyPrototype(XmlElement node) : base(node)
        {
            Name = node.Name;
            if (node.HasAttribute("bind"))
            {
                Kind = PropertyPrototypeKind.BindValue;
                Path = new BindingPath(node.GetAttribute("bind"));
            } 
            else if (node.HasAttribute("object"))
            {
                Kind = PropertyPrototypeKind.AssignFrom;
                Path = new BindingPath(node.GetAttribute("object"));
            }
            else
            {
                Kind = PropertyPrototypeKind.TextRepresentation;
                var typeHintValue =
                    node.GetAttributeNode("type")?.Value ??
                    node.GetAttributeNode("class")?.Value;
                TypeHint = typeHintValue != null ? GenReflection.GetRWType(typeHintValue) : null;
                Value = node.InnerText;
                Translate = node.GetAttribute("translate").ToLowerInvariant() == "true";
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()} {{Name: {Name}; Kind: {Kind};}}";
        }
    }

    public class ConstraintPrototype : Prototype
    {
        public ConstraintDescription Constraint;

        public ConstraintPrototype(XmlElement node) : base(node) {
            Constraint = new ConstraintParser().Parse(node.InnerText.Trim(" \n\r\t".ToCharArray()));
        }

        internal ClLinearConstraint Materialize(Dictionary<string, object> objectsMap)
        {
            ClLinearExpression expression = new ClLinearExpression();
            foreach (var term in Constraint.Terms)
            {
                if (term.OwnerId != null)
                {
                    var anchor = GetAnchor(term.OwnerId, term.AnchorName, objectsMap);
                    expression.AddVariable(anchor, term.Multiplier);
                }
                else
                {
                    expression.Constant += term.Multiplier;
                }

            }

            return new ClLinearConstraint(expression, Constraint.Operator, Constraint.Strength);
        }

        static private ClVariable GetAnchor(string viewId, string anchorName, Dictionary<string, object> objects)
        {
            if (!objects.ContainsKey(viewId))
            {
                throw new ArgumentOutOfRangeException($"{viewId} is not a known view Id", nameof(viewId));
            }

            var view = objects[viewId];

            var prop = view.GetType().GetMemberHandler(anchorName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null || !typeof(ClAbstractVariable).IsAssignableFrom(prop.TargetType))
            {
                throw new Exception($"{anchorName} is not an anchor in object {view}");
            }

            return prop.GetValue(view) as ClVariable;
        }

    }

    public class ElementPrototype : Prototype
    {

        public ElementPrototype[] Subviews;
        public ConstraintPrototype[] Constraints;
        public PropertyPrototype[] Properties;

        public string Class;


        public ElementPrototype(XmlElement node) : base(node)
        {
            Class = node.Name;
            
            Subviews = node.SelectNodes("subviews/*").WhereTypeIs<XmlElement>()
                .Select(x => new ElementPrototype(x)).ToArray();
            Constraints = node.SelectNodes("constraints/constraint").WhereTypeIs<XmlElement>()
                .Select(x => new ConstraintPrototype(x)).ToArray();
            Properties = node.SelectNodes("properties/*").WhereTypeIs<XmlElement>()
                .Select(x => new PropertyPrototype(x)).ToArray();
        }
    }
}
