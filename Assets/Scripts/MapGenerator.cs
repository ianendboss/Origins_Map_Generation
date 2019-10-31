using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("Map Properties")]
    [SerializeField]
    [Min(4)]
    private int totalNumberMapPoints = 10;

    [Header("Visual Properties")]
    [SerializeField]
    private GameObject nodeContainer;
    [SerializeField]
    private GameObject nodeShapePrefab;
    [SerializeField]
    private float[] radiusOptions;
    [SerializeField]
    private int nodeSpawnCount;

    private bool isPathDisplayed = false;
    private bool isTopNodesDisplayed = false;
    private List<MapNode> topNodesDraw;

    private List<Transform> spawnedNodesList;
    private List<MapNode> mapNodeList;
    private MapNode startNode;
    

    private void Start()
    {
        mapNodeList = new List<MapNode>();
        spawnedNodesList = new List<Transform>();
        GenerateMapLayout();
    }

    public virtual void GenerateMapLayout()
    {
        StartCoroutine(SpawnNodeShapes(totalNumberMapPoints));

    }
    
    private IEnumerator SpawnNodeShapes(int _count)
    {
        for (int i = 0; i < nodeSpawnCount; i++)
        {
            GameObject spawnNode = GameObject.Instantiate(nodeShapePrefab, Vector2.one * Random.insideUnitCircle + (Vector2) transform.position, Quaternion.identity, nodeContainer.transform);
            spawnNode.transform.localScale = Vector2.one * radiusOptions[Random.Range(0, radiusOptions.Length)];

            spawnedNodesList.Add(spawnNode.transform);

            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(4.0f);

        foreach(var spawnNode in spawnedNodesList)
        {
            MapNode newNode = new MapNode();
            newNode.position = spawnNode.transform.position;
            newNode.radius = spawnNode.transform.localScale.x;
            newNode.name = spawnNode.name;

            mapNodeList.Add(newNode);
            Debug.Log(newNode.position.y);
        }

        nodeContainer.SetActive(false);
        PlotNodePoints();
        yield break;
    }

    private void PlotNodePoints()
    {
        foreach (var node1 in mapNodeList)
        {
            foreach(var node2 in mapNodeList)
            {
                if (node1 != node2 && Vector2.Distance(node1.position, node2.position) < 1.1 * (node1.radius + node2.radius))
                {
                    if (node1.neighbours == null)
                        node1.neighbours = new List<MapNode>();

                    node1.neighbours.Add(node2);
                }
            }
            Debug.Log(node1.neighbours.Count);
        }

        isPathDisplayed = true;

        PlotPath();
    }

    public void PlotPath()
    {
        var randomTopNode = GetRandomTopNode(mapNodeList);
        

    }

    private void OnDrawGizmos()
    {
        if (isPathDisplayed)
        {
            Gizmos.color = Color.white;
            foreach (var node in mapNodeList)
            {
                foreach(var neighbour in node.neighbours)
                {
                    Gizmos.DrawLine(node.position, neighbour.position);
                }
            }
        }

        if (isTopNodesDisplayed)
        {
            foreach(var nodes in topNodesDraw)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(nodes.position, nodes.radius);
            }
        }

    }

    private MapNode GetRandomTopNode(List<MapNode> _nodeList)
    {
        float minY = 0;
        float maxY = 0;
        foreach (var node in _nodeList)
        {
            if (node.position.y < minY)
                minY = node.position.y;
            if (node.position.y > maxY)
                maxY = node.position.y;
        }

        float cutoff = Mathf.Abs(maxY - minY) / 25;

        List<MapNode> topNodes = new List<MapNode>();
        foreach(var node in _nodeList)
        {
            if (node.position.y >= maxY - cutoff)
            {
                topNodes.Add(node);
            }
        }

        topNodesDraw = topNodes;
        isTopNodesDisplayed = true;

        throw new System.Exception("No top level nodes available");
    }

    private MapNode GetRandomNeighbourDown(MapNode _node)
    {
        List<MapNode> neighboursDown = new List<MapNode>();

        foreach(var neighbour in _node.neighbours)
        {
            if (neighbour.position.y > _node.position.y)
            {
                neighboursDown.Add(neighbour);
            }
        }

        if (neighboursDown.Count != 0)
            return neighboursDown[Random.Range(0, neighboursDown.Count)];

        return null;
    }
}
