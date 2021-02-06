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
                    Binder.Assign(assignment, objectsMap);
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
                    PropertyWriter.ApplyProperty(kvp.Key, kvp.Value.node, objectsMap);
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
                    foreach (var constraintData in kvp.Value.Constraints)
                    {
                        try
                        {
                            var constraint = ConstraintParser.CreateConstraint(constraintData.Item2, objectsMap);
                            if (constraintData.Item1 != null)
                            {
                                objectsMap[constraintData.Item1] = constraint;
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

    public class ElementPrototype
    {

        public List<ElementPrototype> Subviews;
        public List<(string, string)> Constraints;
        public string Id;
        public string Class;

        public XmlElement node;

        public ElementPrototype(XmlElement node)
        {
            Class = node.Name;
            Id = node.GetAttribute("Id");

            this.node = node;

            var viewNodes = node.SelectNodes("subviews/*").WhereTypeIs<XmlElement>();

            Subviews = viewNodes.Select(x => new ElementPrototype(x)).ToList();

            var constraintNodes = node.SelectNodes("constraints/constraint").Cast<XmlElement>();
            Constraints = constraintNodes.Select(x => (x.HasAttribute("Id") ? x.GetAttribute("Id") : null, x.InnerText.Trim(" \n\r\t".ToCharArray()))).ToList();
            /*
            var constraintNodes = node.SelectNodes("constraints/constraint/text()");
            Constraints = constraintNodes?.AsEnumerable<XmlNode>()
                .Select(x => x?.Value)
                .Select(x => x?.Trim(" \n\r\t".ToCharArray()))
                .Where(x => x?.Length > 0)
                .Where(x => x != null)
                .ToList();*/
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
