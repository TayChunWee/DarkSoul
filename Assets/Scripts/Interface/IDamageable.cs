using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void Die();
    public void TakeDamage(int dmg);
}
