using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace RWLayout.alpha2
{
    public class Bindable<TOwner, TProperty>
    {
        WeakReference owner;
        WeakReference binded;
        MemberHandler ownerMember;
        MemberHandler bindedMember;

        public Bindable(TOwner owner, FieldInfo field)
        {
            this.owner = new WeakReference(owner);
            ownerMember = new MemberHandler(field);
        }

        public Bindable(TOwner owner, PropertyInfo prop)
        {
            this.owner = new WeakReference(owner);
            ownerMember = new MemberHandler(prop);
        }

        internal void Bind(object obj, MemberHandler prop)
        {
            Log.Message("Bindable Bind called");
            this.binded = new WeakReference(obj);
            bindedMember = prop;
        }

        public void SynchronizeFrom()
        {
            if (owner?.Target != null && binded?.Target != null)
            {
                var value = bindedMember.GetValue(binded.Target);
                ownerMember.SetValue(owner.Target, value);
            }
        }

        public void SynchronizeTo()
        {
            if (owner?.Target != null && binded?.Target != null)
            {
                var value = ownerMember.GetValue(owner.Target);
                bindedMember.SetValue(binded.Target, value);
            }
        }
    }
}
