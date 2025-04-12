using UnityEngine;

public class DropItem : MonoBehaviour
{
    public enum DropType { Common, Rare }
    public DropType dropType;

    public int commonScore = 10;
    public int rareScore = 50;
}