using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cribbage
{
    public class ConcurrentHashSet<T> : IEnumerable<T>
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly HashSet<T> _hashSet;

        public ConcurrentHashSet(HashSet<T> set)
        {
            _hashSet = set;
        }

        public ConcurrentHashSet()
        {
            _hashSet = new HashSet<T>();
        }

        public bool Add(T item)
        {
            try
            {
                _lock.EnterWriteLock();
                return _hashSet.Add(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void Clear()
        {
            try
            {
                _lock.EnterWriteLock();
                _hashSet.Clear();
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            try
            {
                _lock.EnterWriteLock();
                _hashSet.UnionWith(other);
            }
            catch (Exception ex)
            {
                Console.WriteLine("*****FAILURE: {0}******", ex.Message);
            }
            finally
            {
                if(_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }


        public bool Contains(T item)
        {
            try
            {
                _lock.EnterReadLock();
                return _hashSet.Contains(item);
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }

        public bool Remove(T item)
        {
            try
            {
                _lock.EnterWriteLock();
                return _hashSet.Remove(item);
            }
            finally
            {
                if (_lock.IsWriteLockHeld) _lock.ExitWriteLock();
            }
        }

        public int Count
        {
            get
            {
                try
                {
                    _lock.EnterReadLock();
                    return _hashSet.Count;
                }
                finally
                {
                    if (_lock.IsReadLockHeld) _lock.ExitReadLock();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            try
            {
                _lock.EnterReadLock();
                return _hashSet.GetEnumerator();
            }
            finally
            {
                if (_lock.IsReadLockHeld) _lock.ExitReadLock();
            }
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    
}
