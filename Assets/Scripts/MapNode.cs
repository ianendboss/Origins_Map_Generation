using UnityEngine;
using System.Collections.Generic;

public enum EMapPointType
{
    NONE,
    BATTLE,
    REST,
    SHOP
}

public class MapNode
{
    public bool isSelected = false;
    public float radius;
    public string name;
    public Vector2 position;
    public EMapPointType nodeType = EMapPointType.NONE;
    public MapNode leftNode = null;
    public MapNode rightNode = null;
    public List<MapNode> neighbours;
}