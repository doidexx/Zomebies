using UnityEngine;

public abstract class Drink : MonoBehaviour
{
    public DrinkNames drinkName;
    public int cost = 10;
    public bool owned = false;

    public abstract void Buy(Player player);
    public abstract void GetEffect(Player player);
}

public enum DrinkNames
{
    DoubleTap,
    Juggernog,
    Mulekick,
    SpeedCola,
    QuickRevive,
    ElectricCherry
}