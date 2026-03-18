using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public enum MatchShapeType
{
    HORIZONTAL_3,
    HORIZONTAL_4,
    HORIZONTAL_5,
    VERTICAL_3,
    VERTICAL_4,
    VERTICAL_5,
    SQUARE,
    L_BOTTOM_LEFT,
    L_BOTTOM_RIGHT,
    L_TOP_LEFT,
    L_TOP_RIGHT,

}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MatchShapeSO")]
public class MatchShape : ScriptableObject
{
    [SerializeField] public MatchShapeType matchShapeType;
    [SerializeField] public List<Vector2Int> matchPositions = new List<Vector2Int>();
    [SerializeField] public Vector2Int originPosition;
    [SerializeField] public Vector2 originPosOffset;
}
