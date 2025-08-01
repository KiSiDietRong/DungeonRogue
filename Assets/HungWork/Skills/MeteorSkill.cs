using UnityEngine;

[CreateAssetMenu(menuName = "Skills/Meteor Skill")]
public class MeteorSkill : Skill
{
    public GameObject meteorPrefab;
    public float spawnHeight = 10f;

    public override void Execute(GameObject user, Vector3 target)
    {
        Vector3 spawnPos = new Vector3(target.x, target.y + spawnHeight, 0f);
        GameObject meteor = Instantiate(meteorPrefab, spawnPos, Quaternion.identity);

        Meteor meteorScript = meteor.GetComponent<Meteor>();
        if (meteorScript != null)
        {
            meteorScript.SetTarget(target);
        }
    }
}