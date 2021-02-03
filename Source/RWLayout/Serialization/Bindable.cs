using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    public enum BindingMode
    {
        Never,
        Manual,
        Auto
    }

    public class Bindable<TProperty>
    {
        WeakReference binded;
        MemberHandler bindedMember;

        public BindingMode ReadMode = BindingMode.Auto;
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

        public Bindable() { }

        public Bindable(BindingMode readMode, BindingMode writeMode)
        {
            ReadMode = readMode;
            WriteMode = writeMode;
        }

        internal void Bind(object obj, MemberHandler prop)
        {
            Log.Message("Bindable Bind called");
            this.binded = new WeakReference(obj);
            bindedMember = prop;
            SynchronizeFrom();
        }

        public void SynchronizeFrom()
        {
            if (binded?.Target != null)
            {
                Value = (TProperty)bindedMember.GetValue(binded.Target);
            }
        }

        public void SynchronizeTo()
        {
            if (binded?.Target != null)
            {
                bindedMember.SetValue(binded.Target, Value);
            }
        }
    }
}
