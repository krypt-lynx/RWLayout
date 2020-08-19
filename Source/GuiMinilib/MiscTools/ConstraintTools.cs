// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace GuiMinilib
{
    struct AnchorMapper
    {
        public string debug;

        public Func<CElement, ClVariable> Leading;
        public Func<CElement, ClVariable> Trailing;
        public Func<CElement, ClVariable> SideA;
        public Func<CElement, ClVariable> SideB;
        public Func<CElement, ClVariable> Size;
        public double multipler;
    }

    public static class ConstraintTools
    {
        private static AnchorMapper toRight = new AnchorMapper
        {
            debug = "toRight",
            Leading = x => x.left,
            Trailing = x => x.right,
            SideA = x => x.top,
            SideB = x => x.bottom,
            Size = x => x.width,
            multipler = 1,
        };
        private static AnchorMapper toLeft = new AnchorMapper
        {
            debug = "toLeft",
            Leading = x => x.right,
            Trailing = x => x.left,
            SideA = x => x.bottom,
            SideB = x => x.top,
            Size = x => x.width,
            multipler = -1,
        };
        private static AnchorMapper toBotton = new AnchorMapper
        {
            debug = "toBotton",
            Leading = x => x.top,
            Trailing = x => x.bottom,
            SideA = x => x.left,
            SideB = x => x.right,
            Size = x => x.height,
            multipler = 1,
        };
        private static AnchorMapper toTop = new AnchorMapper
        {
            debug = "toTop",
            Leading = x => x.bottom,
            Trailing = x => x.top,
            SideA = x => x.right,
            SideB = x => x.left,
            Size = x => x.height,
            multipler = -1,
        };

        private static ClLinearExpression toLinearExpression(object obj)
        {
            if (obj is ClLinearExpression)
            {
                return (ClLinearExpression)obj;
            } 
            if (obj is ClAbstractVariable)
            {
                return new ClLinearExpression(obj as ClAbstractVariable);
            }
            try
            {
                return new ClLinearExpression(Convert.ToDouble(obj));
            }
            catch
            {
                throw new InvalidCastException($"{obj} cannot be converted to ClLinearExpression");
            }
        }

        public static void Embed(this CElement parent, CElement child, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Strong;
            }
            parent.solver.AddConstraint(new ClLinearEquation(parent.top, new ClLinearExpression(child.top), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.right, new ClLinearExpression(child.right), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.bottom, new ClLinearExpression(child.bottom), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.left, new ClLinearExpression(child.left), strength));
        }
        
        public static void Embed(this CElement parent, CElement child, EdgeInsets insets, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Strong;
            }
            parent.solver.AddConstraint(new ClLinearEquation(parent.top, Cl.Minus(child.top, insets.top), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.right, Cl.Plus(child.right, insets.right), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.bottom, Cl.Plus(child.bottom, insets.bottom), strength));
            parent.solver.AddConstraint(new ClLinearEquation(parent.left, Cl.Minus(child.left, insets.left), strength));
        }

        public static void ConstrainSize(this CElement element, double width, double height, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Strong;
            }

            element.solver.AddConstraint(new ClLinearEquation(element.width, toLinearExpression(width), strength));
            element.solver.AddConstraint(new ClLinearEquation(element.height, toLinearExpression(height), strength));
        }

        public static void StackLeft(this CElement parent, bool constrainSides, bool constrainEnd, ClStrength strength, params object[] items)
        {
            Stack(parent, toRight, items, constrainEnd, constrainSides, strength);
        }

        public static void StackTop(this CElement parent, bool constrainSides, bool constrainEnd, ClStrength strength, params object[] items)
        {
            Stack(parent, toBotton, items, constrainEnd, constrainSides, strength);
        }

        public static void StackRight(this CElement parent, bool constrainSides, bool constrainEnd, ClStrength strength, params object[] items)
        {
            Stack(parent, toLeft, items, constrainEnd, constrainSides, strength);
        }

        public static void StackBottom(this CElement parent, bool constrainSides, bool constrainEnd, ClStrength strength, params object[] items)
        {
            Stack(parent, toTop, items, constrainEnd, constrainSides, strength);
        }

        private static void Stack(CElement parent, AnchorMapper mapper, IEnumerable<object> items, bool constrainEnd, bool constrainSides, ClStrength strength)
        {
            Log.Message($"Stack direction {mapper.debug}");


            ClLinearExpression trailing = new ClLinearExpression(mapper.Leading(parent));
            foreach (var item in items)
            {
                Log.Message($"item: {item}");


                CElement element = null;
                ClLinearExpression size = null;
                
                if (item is CElement)
                {
                    element = item as CElement;
                    size = null;
                }
                else
                {
                    var type = item.GetType();
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTuple<,>))
                    {
                        var maybeElement = type.GetField("Item1").GetValue(item);
                        var maybeVar = type.GetField("Item2").GetValue(item);

                        element = maybeElement as CElement;
                        size = toLinearExpression(maybeVar);
                    }
                }

                if (element != null)
                {
                    var child = element;
                    parent.solver.AddConstraint(new ClLinearEquation(trailing, new ClLinearExpression(mapper.Leading(child)), strength));
                    trailing = new ClLinearExpression(mapper.Trailing(child));

                    if (size != null)
                    {
                        parent.solver.AddConstraint(new ClLinearEquation(mapper.Size(child), size, strength));
                    }
                    if (constrainSides)
                    {
                        parent.solver.AddConstraint(mapper.SideA(parent), mapper.SideB(parent), mapper.SideA(child), mapper.SideB(child),
                            (pa, pb, ca, cb) => pa == ca && pb == cb, strength);
                    }
                }
                else 
                {
                    var expression = toLinearExpression(item);
                    Log.Message($"expression: {expression}");
                    var multiplied = Cl.Times(expression, mapper.multipler);
                    Log.Message($"multiplied: {multiplied}");
                    trailing = Cl.Plus(trailing, multiplied);
                    Log.Message($"trailing: {trailing}");

                    
                }
            }

            if (constrainEnd)
            {
                var c = new ClLinearEquation(trailing, new ClLinearExpression(mapper.Trailing(parent)), strength);
                Log.Message($"finalizing stack: {c}");

                parent.solver.AddConstraint(c);
            }
        }
    }
}
