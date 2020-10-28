using UnityEngine;

public class ElectricCherry : Drink
{
    public override void Buy(Player player)
    {
        player.ActivateDrink(this);
    }

    public override void GetEffect(Player player)
    {
    }
}