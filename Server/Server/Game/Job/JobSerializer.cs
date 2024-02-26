using System;
using System.Collections.Generic;

namespace Server.Game
{
    public class JobSerializer
    {
        private Queue<IJob> _jobQueue = new Queue<IJob>();
        private object _lock = new object();
        private bool _isFlush = false;

        public void Push(Action action)
        {
            Push(new Job(action));
        } 
        
        public void Push<T1>(Action<T1> action, T1 t1)
        {
            Push(new Job<T1>(action, t1));
        }
        
        public void Push<T1, T2>(Action<T1, T2> action, T1 t1, T2 t2)
        {
            Push(new Job<T1, T2>(action, t1, t2));
        }
        
        public void Push<T1, T2, T3>(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            Push(new Job<T1, T2, T3>(action, t1, t2, t3));
        }

        public void Push(IJob job)
        {
            var isFlush = false;

            lock (_lock)
            {
                _jobQueue.Enqueue(job);
                if (_isFlush == false)
                {
                    isFlush = _isFlush = true;
                }
            }

            if (isFlush)
            {
                Flush();
            }
        }

        private void Flush()
        {
            while (true)
            {
                var action = Pop();
                if (action == null)
                {
                    return;
                }

                action.Execute();
            }
        }

        private IJob Pop()
        {
            lock (_lock)
            {
                if (_jobQueue.Count == 0)
                {
                    _isFlush = false;
                    return null;
                }

                return _jobQueue.Dequeue();
            }
        }
    }
}