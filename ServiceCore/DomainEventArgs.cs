using System;

namespace ServiceCore
{
    public class DomainEventArgs<T> : EventArgs
    {
        public T WrappedModule { get; set; }
    }
}
