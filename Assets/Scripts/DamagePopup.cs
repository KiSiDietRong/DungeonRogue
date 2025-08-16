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
        damageText.color = isCritical ? Color.red : Color.red;

        if (isCritical)
        {
            damageText.fontSize += 15;
        }
    }

    public void Setup(string text, Color color)
    {
        damageText.text = text;
        damageText.color = color;
    }

    // Hàm tĩnh tạo DamagePopup
    public static DamagePopup Create(Vector3 position, string text, Color color)
    {
        // Tìm prefab DamagePopup trong Resources/DamagePopup.prefab
        GameObject prefab = Resources.Load<GameObject>("DamagePopup");
        if (prefab == null)
        {
            Debug.LogError("DamagePopup prefab not found in Resources folder!");
            return null;
        }

        GameObject popupObj = Instantiate(prefab, position, Quaternion.identity);
        DamagePopup popup = popupObj.GetComponent<DamagePopup>();
        popup.Setup(text, color);
        return popup;
    }
}
