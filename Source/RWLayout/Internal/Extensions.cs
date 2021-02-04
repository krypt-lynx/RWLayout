using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RWLayout.alpha2
{
    public static class XmlNodeListExtentions
    {
        /// <summary>
        /// Converts IEnumerable to IEnumerable&lt;T&gt;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        [Obsolete("User .Cast<T>() instead")]
        public static IEnumerable<T> AsEnumerable<T>(this IEnumerable list)
        {
            foreach (var node in list)
            {
                yield return (T)node;
            }
        }


        static Dictionary<Type, XmlNodeType> nodeTypesMap = new Dictionary<Type, XmlNodeType>
        {
            //{ typeof(?), XmlNodeType.None},
            { typeof(XmlElement), XmlNodeType.Element },
            { typeof(XmlAttribute), XmlNodeType.Attribute },
            { typeof(XmlText), XmlNodeType.Text },
            { typeof(XmlCDataSection), XmlNodeType.CDATA },
            { typeof(XmlEntityReference), XmlNodeType.EntityReference },
            { typeof(XmlEntity), XmlNodeType.Entity },
            { typeof(XmlProcessingInstruction), XmlNodeType.ProcessingInstruction },
            { typeof(XmlComment), XmlNodeType.Comment },
            { typeof(XmlDocument), XmlNodeType.Document },
            { typeof(XmlDocumentType), XmlNodeType.DocumentType },
            { typeof(XmlDocumentFragment), XmlNodeType.DocumentFragment },
            { typeof(XmlNotation), XmlNodeType.Notation },
            { typeof(XmlWhitespace), XmlNodeType.Whitespace },
            { typeof(XmlSignificantWhitespace), XmlNodeType.SignificantWhitespace },
            //{ typeof(?), XmlNodeType.EndElement },
            //{ typeof(?), XmlNodeType.EndEntity },
            { typeof(XmlDeclaration), XmlNodeType.XmlDeclaration },
        };

        public static IEnumerable<T> WhereNodeTypeIs<T>(this XmlNodeList list) where T : XmlNode
        {
            Type type = typeof(T);
            XmlNodeType nodeType;
            if (!nodeTypesMap.TryGetValue(type, out nodeType))
            {
                yield break;
            } 
            else
            {
                foreach (XmlNode element in list)
                {
                    if (element.NodeType == nodeType)
                    {
                        yield return (T)element;
                    }
                }
            }
        } 

        public static IEnumerable<T> WhereTypeIs<T>(this IEnumerable list)
        {
            
            Type type = typeof(T);
            foreach (var element in list)
            {
                if (type.IsAssignableFrom(element.GetType()))
                {
                    yield return (T)element;
                }
            }
        }
    }
}
