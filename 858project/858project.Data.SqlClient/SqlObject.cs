using System;
using System.Collections.Generic;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Project858.Data.SqlClient
{
    /// <summary>
    /// Sql objekt na nacitanie dat
    /// </summary>
    public sealed class SqlObject : DynamicObject
    {
        #region - Constructor -
        /// <summary>
        /// Initialize this class
        /// </summary>
        public SqlObject()
        {
            this.m_dictionary = new Dictionary<String, Object>();
        }
        #endregion

        #region - Properties -
        /// <summary>
        /// Dictionary na ukladanie dat
        /// </summary>
        private Dictionary<String, Object> m_dictionary = null;
        #endregion

        #region - Public Methods -
        /// <summary>
        /// Vrati mena vsetkych property ktore boli pridane
        /// </summary>
        /// <returns>Kolekcia property names</returns>
        public List<String> GetAllPropertyName()
        {
            return this.m_dictionary.Keys.ToList();
        }
        /// <summary>
        /// Nastavi objektu property
        /// </summary>
        /// <param name="name">Meno property ktory chceme nastavit</param>
        /// <param name="value">Hodnota ktoru chceme nastavit</param>
        public void SetMember(String name, Object value)
        {
            var binder = Binder.SetMember(CSharpBinderFlags.None, name, typeof(Object), new List<CSharpArgumentInfo> { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            var callsite = CallSite<Func<CallSite, Object, Object, Object>>.Create(binder);
            callsite.Target(callsite, this, value);
        }
        /// <summary>
        /// Vrati hodnotu z property
        /// </summary>
        /// <param name="name">Meno property ktorej hodnotu chceme nacitat</param>
        /// <returns>Objekt ziskany z property</returns>
        public Object GetMember(String name)
        {
            var callsite = CallSite<Func<CallSite, Object, Object>>.Create(Binder.GetMember(CSharpBinderFlags.None, name, typeof(Object), new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) }));
            return callsite.Target(callsite, this);
        }
        /// <summary>
        /// If you try to get a value of a property not defined in the class, this method is called. 
        /// </summary>
        /// <param name="binder">SetMemberBinder</param>
        /// <param name="result">Value</param>
        /// <returns>True = operacia bola uspesna, inak false</returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return this.m_dictionary.TryGetValue(name, out result);
        }
        /// <summary>
        /// If you try to set a value of a property that is not defined in the class, this method is called. 
        /// </summary>
        /// <param name="binder">SetMemberBinder</param>
        /// <param name="value">Value</param>
        /// <returns>True = operacia bola uspesna, inak false</returns>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            string name = binder.Name.ToLower();
            this.m_dictionary[name] = value;
            return true;
        }
        #endregion
    }
}
