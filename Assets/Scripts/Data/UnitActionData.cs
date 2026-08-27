using UnityEngine;

public class UnitActionData : ScriptableObject
{
    [Header("Informacion")]
    [SerializeField] private string actionName;
    [SerializeField] private Sprite actionIcon;

    [Header("Costes")]
    [SerializeField, Min(0)] private int moveCost;
    [SerializeField, MinAttribute(0)] private int actionPointCost;

    public string ActionName => actionName;
    public Sprite ActionIcon => actionIcon;
    public int MoveCost => moveCost;
    public int ActionPointCost => actionPointCost;
}
