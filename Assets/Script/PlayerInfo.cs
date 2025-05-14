using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;
    public float speed = 5f;
    public int attackPower = 10;
    private PlayerUI playerUI;

    private void Awake()
    {
        currentHP = maxHP;
        playerUI = GetComponent<PlayerUI>();
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        if(currentHP <= 0)
        {
            Destroy(gameObject);
        }
        playerUI.UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHP += amount;
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        playerUI.UpdateUI();
    }

    public void IncreaseStats(int hp, float spd, int atk)
    {
        maxHP += hp;
        speed += spd;
        attackPower += atk;
        Heal(hp);
        playerUI.UpdateUI();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(10);
        }
    }
}
