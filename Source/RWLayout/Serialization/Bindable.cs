using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    /// <summary>
    /// Binding mode
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Copy value on command
        /// </summary>
        Manual,
        /// <summary>
        /// Copy value on access
        /// </summary>
        Auto
    }

    /// <summary>
    /// Class binding values in runtime
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public class Bindable<TProperty>
    {
        WeakReference binded;
        MemberHandler bindedMember;

        /// <summary>
        /// Read mode
        /// </summary>
        public BindingMode ReadMode = BindingMode.Auto;

        /// <summary>
        /// Write mode
        /// </summary>
        public BindingMode WriteMode = BindingMode.Auto;

        private TProperty value;

        /// <summary>
        /// This is the Value <i>property</i>. The write into it syncronized with binded property
        /// </summary>
        public TProperty Value {
            get
            {
                if (ReadMode == BindingMode.Auto)
                {
                    SynchronizeFrom();
                }
                return value;
            }
            set
            {
                this.value = value;
                if (WriteMode == BindingMode.Auto)
                {
                    SynchronizeTo();
                }
            }
        }

        /// <summary>
        /// Creates Bindable
        /// </summary>
        public Bindable() { }

        /// <summary>
        /// Creates bindable
        /// </summary>
        /// <param name="readMode">Binding mode for reads</param>
        /// <param name="writeMode">Binding mode for writes</param>
        public Bindable(BindingMode readMode, BindingMode writeMode)
        {
            ReadMode = readMode;
            WriteMode = writeMode;
        }

        internal void Bind(object obj, MemberHandler prop)
        {
            this.binded = new WeakReference(obj);
            bindedMember = prop;
            SynchronizeFrom();
        }

        /// <summary>
        /// Copy value from binded property
        /// </summary>
        public void SynchronizeFrom()
        {
            if (binded?.Target != null)
            {
                value = (TProperty)bindedMember.GetValue(binded.Target);
            }
        }

        /// <summary>
        /// Copy value to binded property
        /// </summary>
        public void SynchronizeTo()
        {
            if (binded?.Target != null)
            {
                bindedMember.SetValue(binded.Target, value);
            }
        }
    }
}
