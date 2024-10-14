using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRN.Utility.Sample
{
    public enum eObserveState
    {
        Default,
        SomethingA,
        SomethingB, 
        SomethingC,
    }

    #region Without Built-In Code
    // 관찰자
    public interface IObserver
    {
        void Update(string inMessage);
    }

    // 관찰 주체
    public interface ISubject
    {
        public void Attach(IObserver inObserber);
        public void Detach(IObserver inObserber);
        public void Notify();
    }

    public class ExampleSubject : ISubject
    {
        private List<IObserver> m_Observers = new List<IObserver>();
        private eObserveState m_SubjectState;

        public eObserveState State
        {
            get { return m_SubjectState; }
            set
            {
                m_SubjectState = value;
                Notify();
            }
        }

        public void Attach(IObserver inObserver) {  m_Observers.Add(inObserver); }
        public void Detach(IObserver inObserver) {  m_Observers.Remove(inObserver); }
        public void Notify()
        {
            foreach (var observer in m_Observers)
                observer.Update(m_SubjectState.ToString());
        }
    }

    public class ExampleObserver : IObserver
    {
        private eObserveState m_State;

        public ExampleObserver(eObserveState inState)
        {
            m_State = inState;
        }

        public void Update(string inMessage)
        {
            //
        }
    }

    public class ObserverSample
    {
        public void Execute()
        {
            ExampleSubject subject = new ExampleSubject();

            ExampleObserver observer1 = new ExampleObserver(eObserveState.Default);
            ExampleObserver observer2 = new ExampleObserver(eObserveState.Default);

            subject.Attach(observer1);
            subject.Attach(observer2);

            subject.State = eObserveState.Default;
        }
    }
    #endregion

    #region With Built-In Code
    // IObservable 인터페이스는 Subscribe를 구현해야한다.
    public class Observerable : IObservable<eObserveState>
    {
        private List<IObserver<eObserveState>> m_Observers = new List<IObserver<eObserveState>>();

        public IDisposable Subscribe(IObserver<eObserveState> inObserver)
        {
            if(m_Observers.Contains(inObserver) == false)
                m_Observers.Add(inObserver);
            return new Unsubscribe(m_Observers, inObserver);
        }

        public void Notify(eObserveState inState)
        {
            foreach(var observer in m_Observers)
            {
                observer.OnNext(inState);
            }
        }

        public void Clear()
        {
            foreach (var observer in m_Observers)
                observer.OnCompleted();
            m_Observers.Clear();
        }

        private class Unsubscribe : IDisposable
        {
            private List<IObserver<eObserveState>> m_Observers;
            private IObserver<eObserveState> m_Observer;

            public Unsubscribe(List<IObserver<eObserveState>> inObservers, IObserver<eObserveState> inObserver)
            {
                this.m_Observers = inObservers;
                this.m_Observer = inObserver;
            }

            public void Dispose()
            {
                if (m_Observer != null && m_Observers.Contains(m_Observer))
                    m_Observers.Remove(m_Observer);
            }
        }

        public class Observer : IObserver<eObserveState>
        {
            private eObserveState m_State;

            public Observer(eObserveState inState) { m_State = inState; }
            public void OnCompleted() { Console.WriteLine("{_name} : Complete"); }
            public void OnError(Exception inError) { Console.WriteLine("{_name} : Error"); }
            public void OnNext(eObserveState inState) { Console.WriteLine("{_name} Next {inState}"); }
        }
    }
    #endregion
}
