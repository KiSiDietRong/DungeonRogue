using UnityEngine;

[CreateAssetMenu(menuName = "Skills/FlamethrowerSkill")]
public class FlameBreath : Skill
{
    public GameObject flamePrefab;

    public override void Execute(GameObject user, Vector3 target)
    {
        GameObject flame = Instantiate(flamePrefab, user.transform.position, Quaternion.identity);
        flame.GetComponent<FlameController>().Initialize(user.transform, target);
    }
}