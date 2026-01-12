using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    public GridManager grid;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        if (grid == null) grid = FindObjectOfType<GridManager>();
    }

    // Synchronous path find
    public List<Vector2> FindPath(Vector2 startWorld, Vector2 targetWorld)
    {
        Node startNode = grid.NodeFromWorldPoint(startWorld);
        Node targetNode = grid.NodeFromWorldPoint(targetWorld);

        if (!startNode.walkable || !targetNode.walkable)
            return new List<Vector2>(); 

        var openSet = new List<Node>();
        var closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        ResetGridCosts();

        startNode.gCost = 0;
        startNode.hCost = GetDistance(startNode, targetNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
                return RetracePath(startNode, targetNode);

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

        return new List<Vector2>(); 
    }

    void ResetGridCosts()
    {
        for (int x = 0; x < grid.gridSizeX; x++)
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                var node = typeof(GridManager).GetField("grid", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(grid) as Node[,];
              
            }

       
       
        for (int x = 0; x < grid.gridSizeX; x++)
            for (int y = 0; y < grid.gridSizeY; y++)
            {
                // compute world pos then get node (should be same node)
                Vector2 pos = grid.worldBottomLeft + Vector2.right * (x * grid.nodeRadius * 2f + grid.nodeRadius) + Vector2.up * (y * grid.nodeRadius * 2f + grid.nodeRadius);
                Node n = grid.NodeFromWorldPoint(pos);
                n.gCost = int.MaxValue / 4;
                n.hCost = 0;
                n.parent = null;
            }
    }

    List<Vector2> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        List<Vector2> waypoints = path.Select(n => n.worldPosition).ToList();
        return waypoints;
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        // diagonal cost 14, straight cost 10
        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
