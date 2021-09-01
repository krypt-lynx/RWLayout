using System;
using System.Collections;
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
        public Func<CElement, ClVariable> Center;
        public double multipler;

        public Func<EdgeInsets, double> LeadingInset;
        public Func<EdgeInsets, double> TrailingInset;
        public Func<EdgeInsets, double> SideAInset;
        public Func<EdgeInsets, double> SideBInset;

        public Func<EdgeConstraints, ClLinearExpression> LeadingExpression;
        public Func<EdgeConstraints, ClLinearExpression> TrailingExpression;
        public Func<EdgeConstraints, ClLinearExpression> SideAExpression;
        public Func<EdgeConstraints, ClLinearExpression> SideBExpression;

        public ClLinearExpression GetLeadingExpression(StackOptions options)
        {
            if (options.EdgeConstraints != null)
            {
                return LeadingExpression(options.EdgeConstraints) ?? LeadingInset(options.Insets);
            }
            else {
                return LeadingInset(options.Insets);
            }
        }
        public ClLinearExpression GetTrailingExpression(StackOptions options)
        {
            if (options.EdgeConstraints != null)
            {
                return TrailingExpression(options.EdgeConstraints) ?? TrailingInset(options.Insets);
            }
            else
            {
                return TrailingInset(options.Insets);
            }
        }
        public ClLinearExpression GetSideAExpression(StackOptions options)
        {
            if (options.EdgeConstraints != null)
            {
                return SideAExpression(options.EdgeConstraints) ?? SideAInset(options.Insets);
            }
            else
            {
                return SideAInset(options.Insets);
            }
        }
        public ClLinearExpression GetSideBExpression(StackOptions options)
        {
            if (options.EdgeConstraints != null)
            {
                return SideBExpression(options.EdgeConstraints) ?? SideBInset(options.Insets);
            }
            else
            {
                return SideBInset(options.Insets);
            }
        }
    }

    public class EdgeConstraints
    {

        /// <summary>
        /// top expression
        /// </summary>
        public ClLinearExpression Top;
        /// <summary>
        /// right expression
        /// </summary>
        public ClLinearExpression Right;
        /// <summary>
        /// bottom expression
        /// </summary>
        public ClLinearExpression Bottom;
        /// <summary>
        /// left expression
        /// </summary>
        public ClLinearExpression Left;

        public EdgeConstraints()
        {

        }

        public EdgeConstraints(ClLinearExpression top = null, ClLinearExpression right = null, ClLinearExpression bottom = null, ClLinearExpression left = null)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        public EdgeConstraints(double margin)
        {
            Top = margin;
            Right = margin;
            Bottom = margin;
            Left = margin;
        }

        public EdgeConstraints(EdgeInsets insets)
        {
            Top = insets.Top;
            Right = insets.Right;
            Bottom = insets.Bottom;
            Left = insets.Left;
        }
    }

    public struct StackOptions
    {
        public bool ConstrainStart;
        public bool ConstrainSides;
        public bool ConstrainEnd;
        public ClStrength Strength;
        public EdgeInsets Insets;
        public double Spacing;

        public bool IntrinsicIfNotSet;
        public bool AlignToCenter;
        public EdgeConstraints EdgeConstraints;

        public static StackOptions Default = new StackOptions
        {
            ConstrainStart = true,
            ConstrainSides = true,
            ConstrainEnd = true,
            Strength = ClStrength.Default,
            Insets = EdgeInsets.Zero,
            IntrinsicIfNotSet = false,
            AlignToCenter = false,
        };

        [Obsolete("This method is present for binary compatibility only")]
        public static StackOptions Create(
            bool constrainStart, bool constrainSides, bool constrainEnd,
            ClStrength strength, EdgeInsets? insets, bool intrinsicIfNotSet)
        {
            return Create(constrainStart, constrainSides, constrainEnd, strength, insets, intrinsicIfNotSet, false, 0, null);
        }

        [Obsolete("This method is present for binary compatibility only")]
        public static StackOptions Create(
            bool constrainStart, bool constrainSides, bool constrainEnd,
            ClStrength strength, EdgeInsets? insets, bool intrinsicIfNotSet,
            bool alignToCenter)
        {
            return Create(constrainStart, constrainSides, constrainEnd, strength, insets, intrinsicIfNotSet, alignToCenter, 0, null);
        }

        [Obsolete("This method is present for binary compatibility only")]
        public static StackOptions Create(
            bool constrainStart, bool constrainSides, bool constrainEnd,
            ClStrength strength, EdgeInsets? insets, bool intrinsicIfNotSet,
            bool alignToCenter, double spacing)
        {
            return Create(constrainStart, constrainSides, constrainEnd, strength, insets, intrinsicIfNotSet, alignToCenter, spacing, null);
        }

        public static StackOptions Create(
            bool constrainStart = true,
            bool constrainSides = true,
            bool constrainEnd = true,
            ClStrength strength = null,
            EdgeInsets? insets = null,
            bool intrinsicIfNotSet = false,
            bool alignToCenter = false,
            double spacing = 0, 
            EdgeConstraints edgeConstraints = null)
        {
            var options = new StackOptions();
            options.ConstrainStart = constrainStart;
            options.ConstrainSides = constrainSides;
            options.ConstrainEnd = constrainEnd;
            options.Strength = strength ?? ClStrength.Default;
            options.Insets = insets.HasValue ? insets.Value : EdgeInsets.Zero;
            options.IntrinsicIfNotSet = intrinsicIfNotSet;
            options.AlignToCenter = alignToCenter;
            options.Spacing = spacing;
            options.EdgeConstraints = edgeConstraints;

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
            Center = x => x.centerY,
            multipler = 1,
            LeadingInset = x => x.Left,
            TrailingInset = x => x.Right,
            SideAInset = x => x.Top,
            SideBInset = x => x.Bottom,
            LeadingExpression = x => x.Left,
            TrailingExpression = x => x.Right,
            SideAExpression = x => x.Top,
            SideBExpression = x => x.Bottom,
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
            Center = x => x.centerY,
            multipler = -1,
            LeadingInset = x => x.Right,
            TrailingInset = x => x.Left,
            SideAInset = x => x.Bottom,
            SideBInset = x => x.Top,
            LeadingExpression = x => x.Right,
            TrailingExpression = x => x.Left,
            SideAExpression = x => x.Bottom,
            SideBExpression = x => x.Top,
        };
        private static AnchorMapper toBottom = new AnchorMapper
        {
            debug = "toBottom",
            Leading = x => x.top,
            Trailing = x => x.bottom,
            SideA = x => x.left,
            SideB = x => x.right,
            Size = x => x.height,
            Center = x => x.centerX,
            IntrinsicSize = x => x.intrinsicHeight,
            multipler = 1,
            LeadingInset = x => x.Top,
            TrailingInset = x => x.Bottom,
            SideAInset = x => x.Left,
            SideBInset = x => x.Right,
            LeadingExpression = x => x.Top,
            TrailingExpression = x => x.Bottom,
            SideAExpression = x => x.Left,
            SideBExpression = x => x.Right,
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
            Center = x => x.centerX,
            multipler = -1,
            LeadingInset = x => x.Bottom,
            TrailingInset = x => x.Top,
            SideAInset = x => x.Right,
            SideBInset = x => x.Left,
            LeadingExpression = x => x.Bottom,
            TrailingExpression = x => x.Top,
            SideAExpression = x => x.Right,
            SideBExpression = x => x.Left,
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
                strength = ClStrength.Default;
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
                strength = ClStrength.Default;
            }
            parent.AddConstraint(new ClLinearConstraint(parent.top, child.top - insets.Top, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.right, child.right + insets.Right, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.bottom, child.bottom + insets.Bottom, strength));
            parent.AddConstraint(new ClLinearConstraint(parent.left, child.left - insets.Left, strength));
        }

        public static void MakeSizeIntristic(this CElement element, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Default;
            }

            element.AddConstraint(element.width ^ element.intrinsicWidth, strength);
            element.AddConstraint(element.height ^ element.intrinsicHeight, strength);
        }

        public static void MakeSizeIntristic(this CElement element, EdgeInsets insets, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Default;
            }

            element.AddConstraint(element.width ^ element.intrinsicWidth + insets.Width, strength);
            element.AddConstraint(element.height ^ element.intrinsicHeight + insets.Height, strength);
        }

        public static void Center(this CElement parent, CElement child, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Default;
            }

            parent.AddConstraint(parent.centerX ^ child.centerX, strength);
            parent.AddConstraint(parent.centerY ^ child.centerY, strength);
        }

        public static void Center(this CElement parent, CElement child, EdgeInsets insets, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Default;
            }

            parent.AddConstraint(parent.centerX ^ ((insets.Left - insets.Right) / 2) + child.centerX, strength);
            parent.AddConstraint(parent.centerY ^ ((insets.Top - insets.Bottom) / 2) + child.centerY, strength);
        }

        public static void ConstrainSize(this CElement element, double width, double height, ClStrength strength = null)
        {
            if (strength == null)
            {
                strength = ClStrength.Default;
            }

            element.AddConstraint(new ClLinearConstraint(element.width, toLinearExpression(width), strength));
            element.AddConstraint(new ClLinearConstraint(element.height, toLinearExpression(height), strength));
        }


        public static void StackLeft(this CElement parent, StackOptions options, params object[] items)
        {
            StackLeft(parent, options, (IEnumerable)items);
        }

        public static void StackTop(this CElement parent, StackOptions options, params object[] items)
        {
            StackTop(parent, options, (IEnumerable)items);
        }

        public static void StackRight(this CElement parent, StackOptions options, params object[] items)
        {
            StackRight(parent, options, (IEnumerable)items);
        }

        public static void StackBottom(this CElement parent, StackOptions options, params object[] items)
        {
            StackBottom(parent, options, (IEnumerable)items);
        }


        public static void StackLeft(this CElement parent, StackOptions options, IEnumerable items)
        {
            Stack(parent, toRight, options, items);
        }

        public static void StackTop(this CElement parent, StackOptions options, IEnumerable items)
        {
            Stack(parent, toBottom, options, items);
        }

        public static void StackRight(this CElement parent, StackOptions options, IEnumerable items)
        {
            Stack(parent, toLeft, options, items);
        }

        public static void StackBottom(this CElement parent, StackOptions options, IEnumerable items)
        {
            Stack(parent, toTop, options, items);
        }


        public static void StackLeft(this CElement parent, IEnumerable items)
        {
            Stack(parent, toRight, StackOptions.Default, items);
        }

        public static void StackTop(this CElement parent, IEnumerable items)
        {
            Stack(parent, toBottom, StackOptions.Default, items);
        }

        public static void StackRight(this CElement parent, IEnumerable items)
        {
            Stack(parent, toLeft, StackOptions.Default, items);
        }

        public static void StackBottom(this CElement parent, IEnumerable items)
        {
            Stack(parent, toTop, StackOptions.Default, items);
        }


        public static void StackLeft(this CElement parent, params object[] items)
        {
            StackLeft(parent, (IEnumerable)items);
        }

        public static void StackTop(this CElement parent, params object[] items)
        {
            StackTop(parent, (IEnumerable)items);
        }

        public static void StackRight(this CElement parent, params object[] items)
        {
            StackRight(parent, (IEnumerable)items);
        }

        public static void StackBottom(this CElement parent, params object[] items)
        {
            StackBottom(parent, (IEnumerable)items);
        }


        private static void Stack(CElement parent, AnchorMapper mapper, StackOptions options, IEnumerable items)
        {
            ClLinearExpression trailing = mapper.Leading(parent) + mapper.GetLeadingExpression(options) * mapper.multipler;
            bool isFirst = true;
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
                    if (!isFirst)
                    {
                        trailing = trailing + options.Spacing;
                    }
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
                            mapper.SideA(child) ^ mapper.SideA(parent) + mapper.GetSideAExpression(options) * mapper.multipler,
                            mapper.SideB(child) ^ mapper.SideB(parent) - mapper.GetSideBExpression(options) * mapper.multipler
                        );
                    }
                    if (options.AlignToCenter)
                    {
                        parent.AddConstraints(options.Strength,
                            mapper.Center(child) ^ mapper.Center(parent)
                            );
                    }
                    isFirst = false;
                }
                else 
                {
                    trailing = trailing + toLinearExpression(item) * mapper.multipler;
                }
            }

            if (options.ConstrainEnd)
            {
                parent.AddConstraint(trailing + mapper.GetTrailingExpression(options) * mapper.multipler ^ mapper.Trailing(parent), options.Strength);
            }
        }
    }
}
