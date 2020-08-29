﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
            if (element.Parent != null)
            {
                throw new InvalidOperationException($"Element {0} already in has parent");
            }

            elements.Add(element);
            element.parent_ = new WeakReference(this, false);
            if (element.solver != null)
            {
                Solver.MergeWith(element.solver);
                element.solver = null;
            }

            element.PostAdd();

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
                throw new InvalidOperationException($"view {element} in not a subview of {this}");
            }

            List<ClConstraint> movedConstraints = ShearConstraints(element);

            elements.Remove(element);
            element.parent_ = null;

            Log.Message($"moved constraints:\n{string.Join("\n", movedConstraints)}");

            foreach (var cn in movedConstraints)
            {
                element.AddConstraint(cn);
            }
        }

        private List<ClConstraint> ShearConstraints(CElement element)
        {
            var cns = Solver.AllConstraints();
            var detachedAnchors = element.allAnchors().ToHashSet();
            var movedConstraints = new List<ClConstraint>();

            foreach (var cn in cns)
            {
                bool hasDetached = false;
                bool hasAttached = false;

                foreach (var var in cn.Expression.Terms.Keys)
                {
                    if (detachedAnchors.Contains(var))
                    {
                        hasDetached = true;
                    }
                    else
                    {
                        hasAttached = true;
                    }

                    if (hasAttached && hasDetached)
                    {
                        break;
                    }
                }

                if (hasDetached)
                {
                    Solver.RemoveConstraint(cn);
                    if (!hasAttached)
                    {
                        movedConstraints.Add(cn);
                    }
                }
            }

            foreach (var var in detachedAnchors)
            {
                Solver.RemoveVariable(var);
            }

            return movedConstraints;
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