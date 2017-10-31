using System;
using ESRI.ArcGIS.Desktop.AddIns;
using System.Collections.Generic;

namespace StyleFileCreator.Internal
{
    [StartupObjectAttribute]//, DebuggerNonUserCode, CompilerGenerated]
    public sealed class AddInStartupObject  : AddInEntryPoint
    {
        #region fields

        private static AddInStartupObject _sAddInHostManager;
        private List<object> m_addinHooks = null;

        #endregion

        //[EditorBrowsable(EditorBrowsableState.Never)]

        #region constructor 
        public AddInStartupObject()
        {
        }
        #endregion

        //[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        protected override bool Initialize(object hook)
        {
            bool createSingleton = AddInStartupObject._sAddInHostManager == null;
            if (createSingleton)
            {
                AddInStartupObject._sAddInHostManager = this;
                this.m_addinHooks = new List<object>();
                this.m_addinHooks.Add(hook);
            }
            else if (!AddInStartupObject._sAddInHostManager.m_addinHooks.Contains(hook))
            {
                AddInStartupObject._sAddInHostManager.m_addinHooks.Add(hook);
            }
            return createSingleton;
        }

        ////Using the EditorBrowsable attribute like so will cause a method not to be shown in intellisense:
        //[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        protected override void Shutdown()
        {
            AddInStartupObject._sAddInHostManager = null;
            this.m_addinHooks = null;
        }

        //[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
        internal static T GetHook<T>() where T : class
        {
            T result;
            if (AddInStartupObject._sAddInHostManager != null)
            {
                foreach (object obj in AddInStartupObject._sAddInHostManager.m_addinHooks)
                {
                    if (obj is T)
                    {
                        result = (obj as T);
                        return result;
                    }
                }
            }
            result = default(T);
            return result;
        }

        public static AddInStartupObject GetThis()
        {
            return AddInStartupObject._sAddInHostManager;
        }
    }
}
