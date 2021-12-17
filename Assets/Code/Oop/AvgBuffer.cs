using System.Collections.Generic;

namespace Code.Oop
{
    public class AvgBuffer
    {
        private readonly float _bufferSizeSeconds;
        private readonly float _cacheTtl;
        private Queue<Entry> _queue = new Queue<Entry>();
        private Entry _total;
        private float _cache;
        private float _cacheAge;

        private struct Entry
        {
            public float value;
            public float dt;
        }

        public AvgBuffer(float bufferSizeSeconds, float cacheTtl)
        {
            _bufferSizeSeconds = bufferSizeSeconds;
            _cacheTtl = cacheTtl;
        }

        public void Add(float value, float dt)
        {
            _queue.Enqueue(new Entry
            {
                value = value,
                dt = dt
            });
            _total.value += value;
            _total.dt += dt;

            while (_total.dt > _bufferSizeSeconds && _queue.Count > 0)
            {
                Entry entry = _queue.Dequeue();
                _total.value -= entry.value;
                _total.dt -= entry.dt;
            }

            _cacheAge += dt;
        }

        public float GetAvg()
        {
            if (_queue.Count <= 0)
            {
                return -1;
            }

            if (_cacheAge < _cacheTtl)
            {
                return _cache;
            }

            float avg = _total.value / _queue.Count;
            _cache = avg;
            _cacheAge = 0;
            return avg;
        }
    }
}