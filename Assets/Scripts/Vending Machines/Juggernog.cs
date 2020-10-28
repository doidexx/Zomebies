using UnityEngine;

public class Juggernog : Drink
{
    public override void Buy(Player player)
    {
        player.ActivateDrink(this);
    }

    public override void GetEffect(Player player)
    {
        player.GetComponent<Health>().maxHealth *= 2;
    }
}