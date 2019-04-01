using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Myth
{
    [ILAdapter]
    public class Adapt_GameEvent : CrossBindingAdaptor
    {
        public override Type BaseCLRType
        {
            get
            {
                return typeof(GameEventBase);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adaptor);
            }
        }

        public override object CreateCLRInstance(AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adaptor(appdomain, instance);
        }

        public class Adaptor : GameEventBase, CrossBindingAdaptorType
        {
            ILTypeInstance m_Instance;
            ILRuntime.Runtime.Enviorment.AppDomain m_AppDomain;

            private IMethod m_GetId;

            object[] param1 = new object[1];

            public Adaptor()
            {

            }

            public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.m_AppDomain = appdomain;
                this.m_Instance = instance;
            }

            public ILTypeInstance ILInstance
            {
                get
                {
                    return m_Instance;
                }
            }

            public ILRuntime.Runtime.Enviorment.AppDomain AppDomain
            {
                get
                {
                    return m_AppDomain;
                }
            }

            public override int Id
            {
                get
                {
                    if (m_GetId == null)
                    {
                        m_GetId = m_Instance.Type.GetMethod("get_Id", 0);
                    }

                    return (int)m_AppDomain.Invoke(m_GetId, m_Instance, null);
                }
            }
        }
    }
}