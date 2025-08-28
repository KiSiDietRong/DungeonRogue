using UnityEngine;
using UnityEngine.UI;

public class AvatarUIController : MonoBehaviour
{
    [SerializeField] private Image avatarImage;

    public void UpdateAvatar(Sprite newAvatar)
    {
        if (avatarImage != null && newAvatar != null)
        {
            avatarImage.sprite = newAvatar;
        }
    }
}
