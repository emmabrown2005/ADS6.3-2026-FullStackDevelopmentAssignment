using MapDistanceAPI.Models;
using MapDistanceAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace MapDistanceAPI.Controllers;

[ApiController]
[Route("api/map")]
public sealed class MapController : ControllerBase
{
    private readonly MapStore _store;
    private readonly PathService _pathService;

    public MapController(MapStore store, PathService pathService)
    {
        _store = store;
        _pathService = pathService;
    }

    // POST /api/map/SetMap
    [HttpPost("SetMap")]
    public IActionResult SetMap([FromBody] GraphDto graph)
    {
        if (!GraphValidator.TryValidate(graph, out var error))
            return BadRequest(error);

        _store.Set(graph);
        return Ok();
    }

    // GET /api/map/GetMap
    [HttpGet("GetMap")]
    public IActionResult GetMap()
    {
        var map = _store.Get();
        if (map is null)
            return BadRequest("Map has not been set.");

        return Ok(map);
    }

    // GET /api/map/ShortestRoute?from=G&to=E
    [HttpGet("ShortestRoute")]
    public IActionResult ShortestRoute([FromQuery] string from, [FromQuery] string to)
    {
        var map = _store.Get();
        if (map is null)
            return BadRequest("Map has not been set.");

        if (!_pathService.TryGetShortestPath(map, from, to, out var path, out _, out var error))
            return BadRequest(error);

        // Output: Path string like "GACE" :contentReference[oaicite:3]{index=3}
        var pathString = string.Concat(path);
        return Ok(pathString);
    }

    // GET /api/map/ShortestDistance?from=G&to=E
    [HttpGet("ShortestDistance")]
    public IActionResult ShortestDistance([FromQuery] string from, [FromQuery] string to)
    {
        var map = _store.Get();
        if (map is null)
            return BadRequest("Map has not been set.");

        if (!_pathService.TryGetShortestPath(map, from, to, out _, out var distance, out var error))
            return BadRequest(error);

        // Output: int like 933 :contentReference[oaicite:4]{index=4}
        return Ok(distance);
    }
}
