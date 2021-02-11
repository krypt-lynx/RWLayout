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
        /// Do not copy value
        /// </summary>
        None,
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
    /// <typeparam name="T"></typeparam>
    public class Bindable<T>
    {
        WeakReference binded;
        //MemberHandler bindedMember;

        bool bindedToStatic = false;
        Action<object, T> write;
        Func<object, T> read;
        /// <summary>
        /// Read mode
        /// </summary>
        public BindingMode ReadMode = BindingMode.Auto;

        /// <summary>
        /// Write mode
        /// </summary>
        public BindingMode WriteMode = BindingMode.Auto;

        private T value;

        /// <summary>
        /// This is the Value <i>property</i>. The write into it syncronized with binded property
        /// </summary>
        public T Value {
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

        public void Bind(object obj, string memberName, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
        {
            Bind(obj, obj.GetType().GetMember(memberName, flags).FirstOrDefault()); // todo: proper member search
        }

        public void Bind(object obj, MemberInfo prop)
        {
            if (prop.MemberType() != typeof(T))
            {
                throw new Exception($"type {obj.GetType()} of binded object {obj} does not match type {typeof(T)} of the property");
            }

            this.binded = obj != null ? new WeakReference(obj) : null;
            bindedToStatic = prop.IsStatic();

            if (ReadMode != BindingMode.None)
            {
                if (MemberHandler.CanRead(prop))
                {
                    read = prop.GetGetValueDelegate<T>();
                }
                else
                {
                    $"Member {prop} is binded to read, but it cannot be readen".Log(MessageType.Error);
                }
            }
            if (ReadMode != BindingMode.None)
            {
                if (MemberHandler.CanWrite(prop))
                {
                    write = prop.GetSetValueDelegate<T>();
                }
                else
                {
                    $"Member {prop} is binded to write, but it cannot be written".Log(MessageType.Error);
                }
            }

            SynchronizeFrom();
        }

        /// <summary>
        /// Copy value from binded property
        /// </summary>
        public void SynchronizeFrom()
        {
            if (read != null)
            {
                if (bindedToStatic)
                {
                    value = read(null);
                }
                else if (binded?.Target != null)
                {
                    value = read(binded.Target);
                }
            }
        }

        /// <summary>
        /// Copy value to binded property
        /// </summary>
        public void SynchronizeTo()
        {
            if (write != null)
            {
                if (bindedToStatic)
                {
                    write(null, value);
                }
                else if (binded?.Target != null)
                {
                    write(binded.Target, value);
                }
            }
        }
    }
}
