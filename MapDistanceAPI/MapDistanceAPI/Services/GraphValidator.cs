using MapDistanceAPI.Models;

namespace MapDistanceAPI.Services
{
    public static class GraphValidator
    {
        /// <summary>
        /// Validates the graph against assignment rules:
        /// - Nodes & edges must exist
        /// - Node IDs must be unique, non-empty
        /// - Edge endpoints must exist
        /// - Weights must be > 0
        /// </summary>
        public static bool TryValidate(GraphDto graph, out string error)
        {
            error = string.Empty;

            if (graph is null)
            {
                error = "Map data is missing.";
                return false;
            }

            if (graph.Nodes is null || graph.Nodes.Count == 0)
            {
                error = "Map must contain at least one node.";
                return false;
            }

            if (graph.Edges is null || graph.Edges.Count == 0)
            {
                error = "Map must contain at least one edge.";
                return false;
            }

            // Validate node IDs
            var nodeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var n in graph.Nodes)
            {
                if (n is null || string.IsNullOrWhiteSpace(n.Id))
                {
                    error = "All nodes must have a non-empty Id.";
                    return false;
                }

                if (!nodeIds.Add(n.Id.Trim()))
                {
                    error = $"Duplicate node Id detected: '{n.Id}'.";
                    return false;
                }
            }

            // Validate edges
            foreach (var e in graph.Edges)
            {
                if (e is null)
                {
                    error = "Edge entry is null.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(e.FromId) || string.IsNullOrWhiteSpace(e.ToId))
                {
                    error = "All edges must have FromId and ToId.";
                    return false;
                }

                if (e.Weight <= 0)
                {
                    error = "Edge Weight must be greater than 0.";
                    return false;
                }

                if (!nodeIds.Contains(e.FromId.Trim()) || !nodeIds.Contains(e.ToId.Trim()))
                {
                    error = $"Edge references unknown node(s): '{e.FromId}' -> '{e.ToId}'.";
                    return false;
                }
            }

            return true;
        }
    }
    }
