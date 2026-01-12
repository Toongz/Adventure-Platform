using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int gridSizeX = 20;
    public int gridSizeY = 20;
    public float nodeRadius = 0.5f;
    public LayerMask obstacleMask;

    Node[,] grid;
    float nodeDiameter;
    public Vector2 worldBottomLeft => 
        (Vector2)transform.position - 
        Vector2.right * (gridSizeX * nodeDiameter / 2f) - 
        Vector2.up * (gridSizeY * nodeDiameter / 2f);

    void Awake() => CreateGrid();
    void OnValidate() => CreateGrid();

    public void CreateGrid()
    {
        nodeDiameter = nodeRadius * 2f;
        grid = new Node[gridSizeX, gridSizeY];

        Vector2 bottomLeft = worldBottomLeft;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector2 worldPoint = bottomLeft + 
                    Vector2.right * (x * nodeDiameter + nodeRadius) + Vector2.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics2D.OverlapCircle(worldPoint, nodeRadius * 0.9f, obstacleMask);
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector2 worldPosition)
    {
        float percentX = (worldPosition.x - worldBottomLeft.x) / (gridSizeX * nodeDiameter);
        float percentY = (worldPosition.y - worldBottomLeft.y) / (gridSizeY * nodeDiameter);
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
               
                int checkX = node.gridX + dx;
                int checkY = node.gridY + dy;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    neighbours.Add(grid[checkX, checkY]);
            }
        }

        return neighbours;
    }

    void OnDrawGizmos()
    {
        if (grid == null) return;
        for (int x = 0; x < gridSizeX; x++)
            for (int y = 0; y < gridSizeY; y++)
            {
                Node n = grid[x, y];
                Gizmos.color = n.walkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(n.worldPosition, Vector3.one * (nodeRadius * 2f * 0.9f));
            }
    }
}
