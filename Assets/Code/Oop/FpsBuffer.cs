using System.Collections.Generic;

namespace Code.Oop
{
    public class FpsBuffer
    {
        private readonly float _bufferSizeSeconds;
        private readonly float _cacheTtl;
        private Queue<float> _queue = new Queue<float>();
        private float _total;
        private float _cache;
        private float _cacheAge;

        public FpsBuffer(float bufferSizeSeconds, float cacheTtl)
        {
            _bufferSizeSeconds = bufferSizeSeconds;
            _cacheTtl = cacheTtl;
        }

        public void AddDeltaTime(float deltaTime)
        {
            _queue.Enqueue(deltaTime);
            _total += deltaTime;

            while (_total > _bufferSizeSeconds && _queue.Count > 0)
            {
                _total -= _queue.Dequeue();
            }

            _cacheAge += deltaTime;
        }

        public float GetFps()
        {
            if (_queue.Count <= 0)
            {
                return -1;
            }

            if (_cacheAge < _cacheTtl)
            {
                return _cache;
            }

            float meanDeltaTime = _total / _queue.Count;
            float fps = 1f / meanDeltaTime;
            _cache = fps;
            _cacheAge = 0;
            return fps;
        }
    }
}