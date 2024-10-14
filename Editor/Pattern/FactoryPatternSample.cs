using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRN.Utility.Sample
{
    public interface IFactoryBasic
    {
        void Work();
    }

    public class BasicA : IFactoryBasic { public void Work() { } }
    public class BasicB : IFactoryBasic { public void Work() { } }

    public abstract class FactoryMethod
    {
        public abstract IFactoryBasic Method();

        public void Operation()
        {
            var method = Method();
            method.Work();
        }
    }

    public class MethodA : FactoryMethod { public override IFactoryBasic Method() { return new BasicA(); } }
    public class MethodB : FactoryMethod {  public override IFactoryBasic Method() { return new BasicB(); } }


    public interface IAbstractFactory
    {
        IFactoryBasic Basic();
        FactoryMethod Method();
    }

    public class AbstractA : IAbstractFactory
    {
        public IFactoryBasic Basic() { return new BasicA(); }
        public FactoryMethod Method() { return new MethodA(); }
    }
}
