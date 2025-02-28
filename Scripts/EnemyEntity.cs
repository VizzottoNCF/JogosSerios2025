using System.Globalization;
using UnityEngine;

public class EnemyEntity : BaseEntity
{
    public override void rf_OnDeath() => rf_OnEnemyDeath();

    private void rf_OnEnemyDeath()
    {

    }
}
