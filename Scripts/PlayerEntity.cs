using UnityEngine;

public class PlayerEntity : BaseEntity
{
    public override void rf_OnDeath() => rf_OnPlayerDeath();

    private void rf_OnPlayerDeath()
    {

    }
}