using UnityEngine;

[CreateAssetMenu(menuName = "NewCharacterStat")]
public class CharacterStatSO : ScriptableObject
{
    public string className;
    public float maxHealth;
    public float skillDamage;
    public float moveSpeed;
}
