using UnityEngine;

public class Juggernog : Drink
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
        playerHealth.maxHealth *= 2;
        playerHealth.healthPoints = playerHealth.maxHealth;
    }
}