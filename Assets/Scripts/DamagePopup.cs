using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float moveUpSpeed = 1f;
    public float disappearTime = 1f;

    private void Start()
    {
        Destroy(gameObject, disappearTime);
    }

    private void Update()
    {
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
    }

    public void Setup(int damageAmount, bool isCritical)
    {
        damageText.text = damageAmount.ToString();
        damageText.color = isCritical ? Color.red : Color.white;

        if (isCritical)
        {
            damageText.fontSize += 5;
        }
    }
}
