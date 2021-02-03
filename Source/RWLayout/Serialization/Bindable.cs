using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    public class Bindable<TProperty>
    {
        WeakReference binded;
        MemberHandler bindedMember;

        public TProperty Value;

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
