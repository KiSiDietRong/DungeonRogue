using System.Collections;
using UnityEngine;

public class WeaponBuffController : MonoBehaviour
{
    public WeaponInfo weaponInfo;
    private int? originalDamage = null;
    private Coroutine currentBuff;

    public void ApplyDamageBuff(float multiplier, float duration)
    {
        if (weaponInfo == null) return;

        if (originalDamage == null || weaponInfo.weaponDamage <= 0)
            originalDamage = weaponInfo.weaponDamage;

        if (currentBuff != null)
            StopCoroutine(currentBuff);

        currentBuff = StartCoroutine(DamageBuffCoroutine(multiplier, duration));
    }

    private IEnumerator DamageBuffCoroutine(float multiplier, float duration)
    {
        weaponInfo.weaponDamage = Mathf.RoundToInt(originalDamage.Value * multiplier);

        yield return new WaitForSeconds(duration);

        weaponInfo.weaponDamage = originalDamage.Value;
        currentBuff = null;
    }
}
