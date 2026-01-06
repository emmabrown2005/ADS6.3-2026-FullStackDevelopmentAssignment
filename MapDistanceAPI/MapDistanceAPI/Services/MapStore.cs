using MapDistanceAPI.Models;

namespace MapDistanceAPI.Services
{
    public sealed class MapStore
    {
        private readonly object _lock = new();
        private GraphDto? _current;

        public void Set(GraphDto graph)
        {
            lock (_lock)
            {
                _current = graph;
            }
        }

        public GraphDto? Get()
        {
            lock (_lock)
            {
                return _current;
            }
        }

        public bool HasMap()
        {
            lock (_lock)
            {
                return _current is not null;
            }

        }
    }
}

