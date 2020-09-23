using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cassowary;
using Verse;

namespace RWLayout.alpha2
{
    /*
      stacking direction ---->
    +--------------------------------------- - - -
    |             SideA       
    |         +-----------+                   +-- - -
    |         |           |                   |         
    | Leading |<--Sizes-->| Traling   Leading |<- - -
    |         |           |                   |
    |         +-----------+                   +-- - -
    |             SideB       
    +--------------------------------------- - - -
     
    multipler: 
        1 if (leading -> traling) coordinates accending        
       -1 if (leading -> traling) coordinates deсending
     */

    struct AnchorMapper
    {
        public string debug;

        public Func<CElement, ClVariable> Leading;
        public Func<CElement, ClVariable> Trailing;
        public Func<CElement, ClVariable> SideA;
        public Func<CElement, ClVariable> SideB;
        public Func<CElement, ClVariable> Size;
        public Func<CElement, ClVariable> IntrinsicSize;
        public double multipler;

        public Func<EdgeInsets, double> LeadingInset;
        public Func<EdgeInsets, double> TrailingInset;
        public Func<EdgeInsets, double> SideAInset;
        public Func<EdgeInsets, double> SideBInset;
    }

    public struct StackOptions
    {
        public bool ConstrainStart;
        public bool ConstrainSides;
        public bool ConstrainEnd;
        public ClStrength Strength;
        public EdgeInsets Insets;
        public bool IntrinsicIfNotSet;

        public static StackOptions Default = new StackOptions
        {
            ConstrainStart = true,
            ConstrainSides = true,
            ConstrainEnd = true,
            Strength = ClStrength.Strong,
            Insets = EdgeInsets.Zero,
            IntrinsicIfNotSet = false,
        };

        public static StackOptions Create(
            bool constrainStart = true,
            bool constrainSides = true,
            bool constrainEnd = true,
            ClStrength strength = null,
            EdgeInsets? insets = null,
            bool intrinsicIfNotSet = false)
        {
            var options = new StackOptions();
            options.ConstrainStart = constrainStart;
            options.ConstrainSides = constrainSides;
            options.ConstrainEnd = constrainEnd;
            options.Strength = strength ?? ClStrength.Strong;
            options.Insets = insets.HasValue ? insets.Value : EdgeInsets.Zero; 
            options.IntrinsicIfNotSet = intrinsicIfNotSet;

            return options;
        }
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
            IntrinsicSize = x => x.intrinsicWidth,
            multipler = 1,
            LeadingInset = x => x.Left,
            TrailingInset = x => x.Right,
            SideAInset = x => x.Top,
            SideBInset = x => x.Bottom,
        };
        private static AnchorMapper toLeft = new AnchorMapper
        {
            debug = "toLeft",
            Leading = x => x.right,
            Trailing = x => x.left,
            SideA = x => x.bottom,
            SideB = x => x.top,
            Size = x => x.width,
            IntrinsicSize = x => x.intrinsicWidth,
            multipler = -1,
            LeadingInset = x => x.Right,
            TrailingInset = x => x.Left,
            SideAInset = x => x.Bottom,
            SideBInset = x => x.Top,
        };
        private static AnchorMapper toBotton = new AnchorMapper
        {
            debug = "toBottom",
            Leading = x => x.top,
            Trailing = x => x.bottom,
            SideA = x => x.left,
            SideB = x => x.right,
            Size = x => x.height,
            IntrinsicSize = x => x.intrinsicHeight,
            multipler = 1,
            LeadingInset = x => x.Top,
            TrailingInset = x => x.Bottom,
            SideAInset = x => x.Left,
            SideBInset = x => x.Right,
        };
        private static AnchorMapper toTop = new AnchorMapper
        {
            debug = "toTop",
            Leading = x => x.bottom,
            Trailing = x => x.top,
            SideA = x => x.right,
            SideB = x => x.left,
            Size = x => x.height,
            IntrinsicSize = x => x.intrinsicHeight,
            multipler = -1,
            LeadingInset = x => x.Bottom,
            TrailingInset = x => x.Top,
            SideAInset = x => x.Right,
            SideBInset = x => x.Left,
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
            parent.AddConstraint(new ClLinearConstraint(parent.top, child.top, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.right, child.right, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.bottom, child.bottom, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.left, child.left, strength));
        }
        
        public static void Embed(this CElement parent, CElement child, EdgeInsets insets, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Strong;
            }
            parent.AddConstraint(new ClLinearConstraint(parent.top, child.top - insets.Top, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.right, child.right + insets.Right, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.bottom, child.bottom + insets.Bottom, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.left, child.left - insets.Left, strength));
        }

        public static void ConstrainSize(this CElement element, double width, double height, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Strong;
            }

            element.AddConstraint(new ClLinearConstraint(element.width, toLinearExpression(width), strength));
            element.AddConstraint(new ClLinearConstraint(element.height, toLinearExpression(height), strength));
        }

        public static void StackLeft(this CElement parent, StackOptions options, params object[] items)
        {
            Stack(parent, toRight, options, items);
        }

        public static void StackTop(this CElement parent, StackOptions options, params object[] items)
        {
            Stack(parent, toBotton, options, items);
        }

        public static void StackRight(this CElement parent, StackOptions options, params object[] items)
        {
            Stack(parent, toLeft, options, items);
        }

        public static void StackBottom(this CElement parent, StackOptions options, params object[] items)
        {
            Stack(parent, toTop, options, items);
        }


        public static void StackLeft(this CElement parent, params object[] items)
        {
            Stack(parent, toRight, StackOptions.Default, items);
        }

        public static void StackTop(this CElement parent, params object[] items)
        {
            Stack(parent, toBotton, StackOptions.Default, items);
        }

        public static void StackRight(this CElement parent, params object[] items)
        {
            Stack(parent, toLeft, StackOptions.Default, items);
        }

        public static void StackBottom(this CElement parent, params object[] items)
        {
            Stack(parent, toTop, StackOptions.Default, items);
        }


        private static void Stack(CElement parent, AnchorMapper mapper, StackOptions options, IEnumerable<object> items)
        {
            ClLinearExpression trailing = mapper.Leading(parent) + mapper.LeadingInset(options.Insets) * mapper.multipler;
            foreach (var item in items)
            {
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
                    parent.AddConstraint(new ClLinearConstraint(trailing, new ClLinearExpression(mapper.Leading(child)), options.Strength));
                    trailing = new ClLinearExpression(mapper.Trailing(child));

                    if (size != null)
                    {
                        parent.AddConstraint(new ClLinearConstraint(mapper.Size(child), size, options.Strength));
                    } 
                    else if (options.IntrinsicIfNotSet)
                    {
                        parent.AddConstraint(mapper.Size(child) ^ mapper.IntrinsicSize(child));
                    }
                    if (options.ConstrainSides)
                    {
                        parent.AddConstraints(options.Strength,
                            mapper.SideA(child) ^ mapper.SideA(parent) + mapper.SideAInset(options.Insets) * mapper.multipler,
                            mapper.SideB(child) ^ mapper.SideB(parent) - mapper.SideBInset(options.Insets) * mapper.multipler
                        );
                    }
                }
                else 
                {
                    trailing = trailing + toLinearExpression(item) * mapper.multipler;
                }
            }

            if (options.ConstrainEnd)
            {
                parent.AddConstraint(trailing + mapper.TrailingInset(options.Insets) * mapper.multipler ^ mapper.Trailing(parent), options.Strength);
            }
        }
    }
}
