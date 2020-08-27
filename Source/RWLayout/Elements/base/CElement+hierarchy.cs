// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RWLayout.Alpha1
{
    public partial class CElement
    {
        private List<CElement> elements = new List<CElement>(); // "readonly"
        public IReadOnlyList<CElement> Elements
        {
            get
            {
                return elements;
            }
        }
        public T AddElement<T>(T element) where T : CElement
        {
            elements.Add(element);
            element.parent_ = new WeakReference(this, false);

            element.PostAdd();

            return element;
        }
        public void RemoveElement(CElement element)
        {
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }

            element.RemoveImpliedConstraints(Solver);
            elements.Remove(element);
        }
        public void RemoveAllElements()
        {
            var views = new List<CElement>(Elements);
            foreach (var view in views)
            {
                this.RemoveElement(view);
            }
        }
        public virtual void PostAdd()
        {

        }

        public void BringToFront(CElement element)
        {
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }
            elements.Add(element);
        }
        public void SendToBack(CElement element)
        {
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }
            elements.Insert(0, element);
        }
        public void MoveToPosition(CElement element, int position)
        {
            if (position < 0 && position >= elements.Count())
            {
                throw new ArgumentException("position is out of bouds", "position");
            }
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }
            elements.Insert(position, element);
        }

        WeakReference parent_ = null;
        public CElement Parent
        {
            get { return parent_?.IsAlive ?? false ? parent_.Target as CElement : null; }
        }
        public CElement Root
        {
            get {
                var parent = this;
                while (parent.Parent != null)
                {
                    parent = parent.Parent;
                }
                return parent;
            }
        }
    }
}
