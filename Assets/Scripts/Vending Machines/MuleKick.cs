using UnityEngine;

public class MuleKick : Drink
{
    public override void Buy(Player player)
    {
        player.ActivateDrink(this);
    }

    public override void GetEffect(Player player)
    {
        player.ChangeNumberOfSlots(3);
    }
}