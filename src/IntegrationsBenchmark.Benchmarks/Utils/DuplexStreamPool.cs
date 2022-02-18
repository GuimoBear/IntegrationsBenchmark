using Grpc.Core;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IntegrationsBenchmark.Benchmarks.Utils
{
    internal sealed class DuplexStreamPool<TRequest, TResponse> : IAsyncDisposable
    {
        private struct Element
        {
            internal AsyncDuplexStreamingCall<TRequest, TResponse> Value;
        }

        internal delegate AsyncDuplexStreamingCall<TRequest, TResponse> Factory(CancellationToken cancellationToken);

        private AsyncDuplexStreamingCall<TRequest, TResponse> _firstItem;
        private readonly Element[] _items;

        private readonly Factory _factory;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        internal DuplexStreamPool(Factory factory, bool preInitialize = false)
            : this(factory, Environment.ProcessorCount * 2, preInitialize)
        { }

        internal DuplexStreamPool(Factory factory, int size, bool preInitialize = false)
        {
            Debug.Assert(size >= 1);
            _factory = factory;
            _items = new Element[size - 1];
            if (preInitialize)
                InitializeElements();
        }

        internal DuplexStreamPool(Func<DuplexStreamPool<TRequest, TResponse>, CancellationToken, AsyncDuplexStreamingCall<TRequest, TResponse>> factory, int size, bool preInitialize = false)
        {
            Debug.Assert(size >= 1);
            _factory = (cancellationToken) => factory(this, cancellationToken);
            _items = new Element[size - 1];
            if (preInitialize)
                InitializeElements();
        }

        private void InitializeElements()
        {
            _firstItem = CreateInstance();
            for(int i = 0; i < _items.Length; i++)
                _items[i].Value = CreateInstance();
        }

        private AsyncDuplexStreamingCall<TRequest, TResponse> CreateInstance()
        {
            var inst = _factory(_cancellationTokenSource.Token);
            return inst;
        }

        internal AsyncDuplexStreamingCall<TRequest, TResponse> Allocate()
        {
            var inst = _firstItem;
            if (inst == null || inst != Interlocked.CompareExchange(ref _firstItem, null, inst))
            {
                inst = AllocateSlow();
            }
            return inst;
        }

        private AsyncDuplexStreamingCall<TRequest, TResponse> AllocateSlow()
        {
            var items = _items;

            for (var i = 0; i < items.Length; i++)
            {
                var inst = items[i].Value;
                if (inst != null)
                {
                    if (inst == Interlocked.CompareExchange(ref items[i].Value, null, inst))
                    {
                        return inst;
                    }
                }
            }

            return CreateInstance();
        }

        internal void Free(AsyncDuplexStreamingCall<TRequest, TResponse> obj)
        {
            Validate(obj);

            if (_firstItem == null)
                _firstItem = obj;
            else
                FreeSlow(obj);
        }

        private void FreeSlow(AsyncDuplexStreamingCall<TRequest, TResponse> obj)
        {
            var items = _items;
            for (var i = 0; i < items.Length; i++)
            {
                if (items[i].Value == null)
                {
                    items[i].Value = obj;
                    return;
                }
            }
            DisposeItem(obj)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }

        private void Validate(object obj)
        {
            Debug.Assert(obj != null, "freeing null?");

            Debug.Assert(_firstItem != obj, "freeing twice?");

            var items = _items;
            for (var i = 0; i < items.Length; i++)
            {
                var value = items[i].Value;
                if (value == null)
                {
                    return;
                }

                Debug.Assert(value != obj, "freeing twice?");
            }
        }

        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            await DisposeItem(_firstItem);
            foreach(var item in _items)
                await DisposeItem(item.Value);
        }

        private async ValueTask DisposeItem(AsyncDuplexStreamingCall<TRequest, TResponse> item)
        {
            if (item is null)
                return;
            try { await item.RequestStream.CompleteAsync(); } catch { }
            try { item.Dispose(); } catch { }
        }
    }
}
