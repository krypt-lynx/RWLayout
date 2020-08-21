// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuiMinilib
{
    public partial class CElement
    {
        public List<CElement> elements = new List<CElement>();
        public T AddElement<T>(T element) where T : CElement
        {
            elements.Add(element);
            element.parent_ = new WeakReference(this, false);

            element.PostAdd();
            return element;
        }

        WeakReference parent_ = null;
        public CElement parent
        {
            get { return parent_?.IsAlive ?? false ? parent_.Target as CElement : null; }
        }

        // todo: RemoveElement

        public virtual void PostAdd()
        {

        }
    }
}
