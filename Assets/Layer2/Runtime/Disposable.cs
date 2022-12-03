using System;

namespace Jagapippi.Layer2
{
    internal sealed class Disposable : IDisposable
    {
        public static readonly Disposable Empty = new(null);
        public static IDisposable Create(Action action) => new Disposable(action);

        private readonly Action _action;

        private Disposable(Action action)
        {
            _action = action;
        }

        void IDisposable.Dispose() => _action?.Invoke();
    }
}
