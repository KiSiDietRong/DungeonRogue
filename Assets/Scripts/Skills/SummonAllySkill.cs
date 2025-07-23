using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Summon Ally Skill")]
public class SummonAllySkill : Skill
{
    public GameObject allyPrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        GameObject ally = Instantiate(allyPrefab, target, Quaternion.identity);
    }
}