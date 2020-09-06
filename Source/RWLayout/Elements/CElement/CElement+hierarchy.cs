// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace RWLayout.alpha2
{
    public partial class CElement
    {
        private List<CElement> elements = new List<CElement>();
        public IReadOnlyList<CElement> Elements
        {
            get
            {
                return elements;
            }
        }

        public T AddElement<T>(T element) where T : CElement
        {
            if (element.Parent != null)
            {
                throw new InvalidOperationException($"Element {0} already has a parent");
            }

            elements.Add(element);
            element.Parent = this;
            if (element.solver != null)
            {
                //Solver.MergeWith(element.solver); // todo: fix MergeWith
                Solver.AddConstraints(element.solver.AllConstraints()); // the same but slow
                element.solver = null;
            }

            element.PostAdd();
            SetNeedsUpdateLayout();

            return element;
        }

        public void AddElements(IEnumerable<CElement> elements)
        {
            foreach(var element in elements)
            {
                AddElement(element);
            }
        }

        public void AddElements(params CElement[] elements)
        {
            AddElements((IEnumerable<CElement>)elements);
        }

        public void RemoveElement(CElement element)
        {
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} is not a subview of {this}");
            }

            List<ClConstraint> movedConstraints = ShearConstraints(element);

            elements.Remove(element);
            element.parent_ = null;

            //Log.Message($"moved constraints:\n{string.Join("\n", movedConstraints)}");

            foreach (var cn in movedConstraints)
            {
                element.AddConstraint(cn);
            }
            SetNeedsUpdateLayout();
        }

        public void RemoveElements(IEnumerable<CElement> elements)
        {
            foreach (var element in elements) // todo: performance
            {
                this.RemoveElement(element);
            }
        }

        public void RemoveElements(params CElement[] elements)
        {
            RemoveElements((IEnumerable<CElement>)elements);
        }

        public void RemoveAllElements()
        {
            var views = new List<CElement>(Elements);
            foreach (var view in views) // todo: performance
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
            SetNeedsUpdateLayout();
        }
        public void SendToBack(CElement element)
        {
            if (!elements.Remove(element))
            {
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }
            elements.Insert(0, element);
            SetNeedsUpdateLayout();
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
            SetNeedsUpdateLayout();
        }

        WeakReference parent_ = null;
        public CElement Parent
        {
            get { return parent_?.IsAlive ?? false ? parent_.Target as CElement : null; }
            protected set { parent_ = new WeakReference(value, false); }
        }
        public CElement Root
        {
            get {
                var parent = Parent;
                if (parent == null)
                {
                    return this;
                }
                else
                {
                    return Parent.Root;
                }
            }
        }
    }
}
