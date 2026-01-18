using MapDistanceAPI.Models;

namespace MapDistanceAPI.Services
{
    public sealed class PathService
    {
        /// <summary>
        /// Returns the shortest path as a list of node IDs using Dijkstra.
        /// Treats edges as bi-directional as required by the brief.
        /// Dijekstra is a shortest path algorithm used to find the minimum 
        /// distance from a single source node to all other nodes in a 
        /// weighted graph with non-negative edge weights.
        /// </summary>
        public bool TryGetShortestPath(GraphDto graph, string from, string to, out List<string> path, out int distance, out string error)
        {
            path = new List<string>();
            distance = 0;
            error = string.Empty;

            if (graph is null)
            {
                error = "Map has not been set.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                error = "Missing parameters: from and/or to.";
                return false;
            }

            from = from.Trim();
            to = to.Trim();

            var nodeSet = new HashSet<string>(graph.Nodes.Select(n => n.Id.Trim()), StringComparer.OrdinalIgnoreCase);
            if (!nodeSet.Contains(from))
            {
                error = $"Unknown node name: '{from}'.";
                return false;
            }
            if (!nodeSet.Contains(to))
            {
                error = $"Unknown node name: '{to}'.";
                return false;
            }

            // Build adjacency list (bi-directional)
            var adj = new Dictionary<string, List<(string Neighbor, int Weight)>>(StringComparer.OrdinalIgnoreCase);
            foreach (var n in nodeSet)
            {
                adj[n] = new List<(string, int)>();
            }

            foreach (var e in graph.Edges)
            {
                var a = e.FromId.Trim();
                var b = e.ToId.Trim();
                var w = e.Weight;

                // bi-directional
                adj[a].Add((b, w));
                adj[b].Add((a, w));
            }

            // Dijkstra
            var dist = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var prev = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

            const int INF = int.MaxValue;

            foreach (var n in nodeSet)
            {
                dist[n] = INF;
                prev[n] = null;
            }

            dist[from] = 0;

            var pq = new PriorityQueue<string, int>();
            pq.Enqueue(from, 0);

            while (pq.Count > 0)
            {
                var u = pq.Dequeue();

                // Early exit if we reached destination
                if (u.Equals(to, StringComparison.OrdinalIgnoreCase))
                    break;

                var du = dist[u];
                if (du == INF) continue;

                foreach (var (v, w) in adj[u])
                {
                    // avoid overflow
                    if (du > INF - w) continue;

                    var alt = du + w;
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                        pq.Enqueue(v, alt);
                    }
                }
            }

            if (dist[to] == INF)
            {
                error = $"No route found from '{from}' to '{to}'.";
                return false;
            }

            // Reconstruct path
            var stack = new Stack<string>();
            string? cur = to;
            while (cur is not null)
            {
                stack.Push(cur);
                cur = prev[cur];
            }

            // If the stack doesn't start with 'from', no valid path
            if (stack.Count == 0 || !stack.Peek().Equals(from, StringComparison.OrdinalIgnoreCase))
            {
                error = $"No route found from '{from}' to '{to}'.";
                return false;
            }

            path = stack.ToList();
            distance = dist[to];
            return true;
        }
    }
    }
