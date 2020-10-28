using UnityEngine;

public abstract class Drink : MonoBehaviour
{
    public string drinkName = "";
    public int cost = 10;
    public bool owned = false;

    public abstract void Buy(Player player);
    public abstract void GetEffect(Player player);
}