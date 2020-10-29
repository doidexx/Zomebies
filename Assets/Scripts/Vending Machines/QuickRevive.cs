using UnityEngine;

public class QuickRevive : Drink
{
    public override void Buy(Player player)
    {
        player.ActivateDrink(this);
    }

    public override void GetEffect(Player player)
    {
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth == null)
            return;
        playerHealth.DecreaseReviveTime();
    }
}