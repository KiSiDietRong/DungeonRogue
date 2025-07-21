using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Black Hole Skill")]
public class BlackHoleSkill : Skill
{
    public GameObject blackHolePrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        GameObject area = Instantiate(blackHolePrefab, target, Quaternion.identity);
    }
}