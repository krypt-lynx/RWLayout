using Cassowary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
        [Unsaved(false)]
        public List<BindingPrototype> Bindings;

        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            XmlElement node = xmlRoot as XmlElement;

            if (node != null)
            {
                defName = node.SelectSingleNode("defName/text()")?.Value;
                var prototypeNode = node.SelectSingleNode("view/*") as XmlElement;
                if (prototypeNode != null)
                {
                    Prototype = new ElementPrototype(prototypeNode);
                }
                var bindingNodes = node.SelectNodes("bindings/binding");
                Bindings = bindingNodes?.AsEnumerable<XmlElement>()
                    .Select(x => new BindingPrototype(x))
                    .ToList();

            }
        }

        public CElement Instantiate(Dictionary<string, object> externalObjects = null)
        {
            Dictionary<string, CElement> viewMap = new Dictionary<string, CElement>();
            Dictionary<CElement, ElementPrototype> viewPrototypes = new Dictionary<CElement, ElementPrototype>();
            CElement root = InstantiateViewsTree(Prototype, viewMap, viewPrototypes);

            var objectsMap = viewMap.Select(kvp => (kvp.Key, (object)kvp.Value))
                .Concat(externalObjects?.Select(x => (x.Key, x.Value)) ?? Enumerable.Empty<(string, object)>())
                .ToDictionary(x => x.Item1, x => x.Item2);

            ApplyConstraints(objectsMap, viewPrototypes);
            ApplyProperties(objectsMap, viewPrototypes);
            ApplyBindings(objectsMap, Bindings);
            return root;
        }

        private void ApplyBindings(Dictionary<string, object> objectsMap, List<BindingPrototype> bindings)
        {
            foreach (var binding in bindings)
            {
                try
                {
                    Binder.Bind(binding, objectsMap);
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
                    foreach (var constraintStr in kvp.Value.Constraints)
                    {
                        try
                        {
                            kvp.Key.AddConstraints(ConstraintParser.CreateConstraint(constraintStr, objectsMap));
                        } 
                        catch (Exception e)
                        {
                            LogHelper.LogException("exception during constraint deserialization", e);
                        }
                    }
                }
            }
        }

        private CElement InstantiateViewsTree(ElementPrototype p, Dictionary<string, CElement> viewMap, Dictionary<CElement, ElementPrototype> viewPrototypes)
        {
            var ns = "RWLayout.alpha2";

            var type = GenTypes.GetTypeInAnyAssembly(p.Class, ns) ?? GenTypes.GetTypeInAnyAssembly($"{ns}.{p.Class}", ns);

            var view = (CElement)Activator.CreateInstance(type);
            if (p.Id != null)
            {
                viewMap[p.Id] = view;
            }
            viewPrototypes[view] = p;
            
            if (p.Subviews != null)
            {
                view.AddElements(p.Subviews.Select(x => InstantiateViewsTree(x, viewMap, viewPrototypes)));
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

    }

    public struct BindingPrototype
    {
        public BindingPath Source;
        public BindingPath Target;

        public BindingPrototype(XmlElement x)
        {
            var src = x.GetAttribute("object");
            Source = new BindingPath(src);

            var dst = x.GetAttribute("target");
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
        public List<string> Constraints;
        public string Id;
        public string Class;

        public XmlElement node;

        public ElementPrototype(XmlElement node)
        {
            Class = node.Name;
            Id = node.GetAttribute("Id");

            this.node = node;

            var viewNodes = node.SelectNodes("subviews/*").AsEnumerable<XmlNode>().Where(x => x.NodeType == XmlNodeType.Element).Cast<XmlElement>();

            Subviews = viewNodes.Select(x => new ElementPrototype(x)).ToList();

            var constraintNodes = node.SelectNodes("constraints/constraint/text()");
            Constraints = constraintNodes?.AsEnumerable<XmlNode>()
                .Select(x => x?.Value)
                .Select(x => x?.Trim(" \n\r\t".ToCharArray()))
                .Where(x => x?.Length > 0)
                .Where(x => x != null)
                .ToList();
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
