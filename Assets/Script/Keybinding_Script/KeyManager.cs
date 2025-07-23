using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance;

    private bool waitingForKey;
    private string currentKeyToRebind;

    [Header("Panel hiển thị khi rebind")]
    public GameObject pressKeyPanel;
    public Text pressKeyText;

    [Header("Button")]
    public Button btnUp, btnDown, btnLeft, btnRight;
    public Button btnSkill1, btnSkill2, btnAttack, btnDash, btnInteract, btnInventory;

    [Header("Image cho từng Button")]
    public Image imgUp, imgDown, imgLeft, imgRight;
    public Image imgSkill1, imgSkill2, imgAttack, imgDash, imgInteract, imgInventory;

    private Dictionary<string, KeyCode> keybinds = new Dictionary<string, KeyCode>();
    private Dictionary<string, Image> buttonImages = new Dictionary<string, Image>();

    // Cache tất cả sprite theo tên KeyCode
    private Dictionary<string, Sprite> keySprites = new Dictionary<string, Sprite>();

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        // Load toàn bộ sprite từ folder Resources/Sprites/Keys
        Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites/Keys");
        foreach (var sprite in sprites)
        {
            if (!keySprites.ContainsKey(sprite.name))
                keySprites.Add(sprite.name, sprite);
        }

        // Gắn Image UI tương ứng
        buttonImages["MoveUp"] = imgUp;
        buttonImages["MoveDown"] = imgDown;
        buttonImages["MoveLeft"] = imgLeft;
        buttonImages["MoveRight"] = imgRight;
        buttonImages["Skill1"] = imgSkill1;
        buttonImages["Skill2"] = imgSkill2;
        buttonImages["Attack"] = imgAttack;
        buttonImages["Dash"] = imgDash;
        buttonImages["Interact"] = imgInteract;
        buttonImages["Inventory"] = imgInventory;

        // Gắn Button tương ứng
        btnUp.onClick.AddListener(() => StartRebinding("MoveUp"));
        btnDown.onClick.AddListener(() => StartRebinding("MoveDown"));
        btnLeft.onClick.AddListener(() => StartRebinding("MoveLeft"));
        btnRight.onClick.AddListener(() => StartRebinding("MoveRight"));
        btnSkill1.onClick.AddListener(() => StartRebinding("Skill1"));
        btnSkill2.onClick.AddListener(() => StartRebinding("Skill2"));
        btnAttack.onClick.AddListener(() => StartRebinding("Attack"));
        btnDash.onClick.AddListener(() => StartRebinding("Dash"));
        btnInteract.onClick.AddListener(() => StartRebinding("Interact"));
        btnInventory.onClick.AddListener(() => StartRebinding("Inventory"));

        LoadDefaultKeys();
        UpdateAllButtonSprites();
        pressKeyPanel.SetActive(false);
    }

    void LoadDefaultKeys()
    {
        keybinds["MoveUp"] = GetSavedKey("MoveUp", KeyCode.W);
        keybinds["MoveDown"] = GetSavedKey("MoveDown", KeyCode.S);
        keybinds["MoveLeft"] = GetSavedKey("MoveLeft", KeyCode.A);
        keybinds["MoveRight"] = GetSavedKey("MoveRight", KeyCode.D);
        keybinds["Skill1"] = GetSavedKey("Skill1", KeyCode.Q);
        keybinds["Skill2"] = GetSavedKey("Skill2", KeyCode.E);
        keybinds["Attack"] = GetSavedKey("Attack", KeyCode.Mouse0);
        keybinds["Dash"] = GetSavedKey("Dash", KeyCode.Space);
        keybinds["Interact"] = GetSavedKey("Interact", KeyCode.F);
        keybinds["Inventory"] = GetSavedKey("Inventory", KeyCode.Tab);
    }

    KeyCode GetSavedKey(string action, KeyCode defaultKey)
    {
        string saved = PlayerPrefs.GetString(action, defaultKey.ToString());
        return (KeyCode)System.Enum.Parse(typeof(KeyCode), saved);
    }

    void Update()
    {
        if (waitingForKey)
        {
            foreach (KeyCode kc in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(kc))
                {
                    keybinds[currentKeyToRebind] = kc;
                    PlayerPrefs.SetString(currentKeyToRebind, kc.ToString());
                    UpdateSingleButtonSprite(currentKeyToRebind, kc);
                    waitingForKey = false;
                    pressKeyPanel.SetActive(false);
                    break;
                }
            }
        }
    }

    void StartRebinding(string keyName)
    {
        if (!waitingForKey)
        {
            currentKeyToRebind = keyName;
            waitingForKey = true;
            pressKeyPanel.SetActive(true);
            pressKeyText.text = "Press any key to rebind...";
        }
    }

    void UpdateSingleButtonSprite(string action, KeyCode key)
    {
        if (buttonImages.TryGetValue(action, out Image img))
        {
            string spriteName = GetSpriteNameFromKeyCode(key); 
            if (keySprites.TryGetValue(spriteName, out Sprite sprite))
            {
                img.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy sprite cho phím: {spriteName}");
            }
        }
    }


    void UpdateAllButtonSprites()
    {
        foreach (var kvp in keybinds)
        {
            string action = kvp.Key;
            KeyCode key = kvp.Value;

            if (buttonImages.TryGetValue(action, out Image img))
            {
                string spriteName = GetSpriteNameFromKeyCode(key);
                Sprite sprite = Resources.Load<Sprite>($"Sprites/Keys/{spriteName}");

                if (sprite != null)
                {
                    img.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy sprite: {spriteName}.png");
                }
            }
        }
    }


    string GetSpriteNameFromKeyCode(KeyCode key)
    {
        switch (key)
        {
            // Phím chữ
            case KeyCode.A: return "a";
            case KeyCode.B: return "b";
            case KeyCode.C: return "c";
            case KeyCode.D: return "d";
            case KeyCode.E: return "e";
            case KeyCode.F: return "f";
            case KeyCode.G: return "g";
            case KeyCode.H: return "h";
            case KeyCode.I: return "i";
            case KeyCode.J: return "j";
            case KeyCode.K: return "k";
            case KeyCode.L: return "l";
            case KeyCode.M: return "m";
            case KeyCode.N: return "n";
            case KeyCode.O: return "o";
            case KeyCode.P: return "p";
            case KeyCode.Q: return "q";
            case KeyCode.R: return "r";
            case KeyCode.S: return "s";
            case KeyCode.T: return "t";
            case KeyCode.U: return "u";
            case KeyCode.V: return "v";
            case KeyCode.W: return "w";
            case KeyCode.X: return "x";
            case KeyCode.Y: return "y";
            case KeyCode.Z: return "z";

            // Phím số hàng trên (Alpha)
            case KeyCode.Alpha0: return "0";
            case KeyCode.Alpha1: return "1";
            case KeyCode.Alpha2: return "2";
            case KeyCode.Alpha3: return "3";
            case KeyCode.Alpha4: return "4";
            case KeyCode.Alpha5: return "5";
            case KeyCode.Alpha6: return "6";
            case KeyCode.Alpha7: return "7";
            case KeyCode.Alpha8: return "8";
            case KeyCode.Alpha9: return "9";

            // Phím chức năng
            case KeyCode.Space: return "space";
            case KeyCode.Tab: return "tab";
            case KeyCode.Escape: return "escape";
            case KeyCode.Return: return "enter";
            case KeyCode.Backspace: return "backspace";
            case KeyCode.LeftShift: return "left-shift";
            case KeyCode.RightShift: return "right-shift";
            case KeyCode.LeftControl: return "left-ctrl";
            case KeyCode.RightControl: return "right-ctrl";
            case KeyCode.LeftAlt: return "left-alt";
            case KeyCode.RightAlt: return "right-alt";

            // Chuột
            case KeyCode.Mouse0: return "mouse-left";
            case KeyCode.Mouse1: return "mouse-right";
            case KeyCode.Mouse2: return "mouse-middle";

            // Di chuyển
            case KeyCode.UpArrow: return "arrow-up";
            case KeyCode.DownArrow: return "arrow-down";
            case KeyCode.LeftArrow: return "arrow-left";
            case KeyCode.RightArrow: return "arrow-right";

            // F1–F12
            case KeyCode.F1: return "f1";
            case KeyCode.F2: return "f2";
            case KeyCode.F3: return "f3";
            case KeyCode.F4: return "f4";
            case KeyCode.F5: return "f5";
            case KeyCode.F6: return "f6";
            case KeyCode.F7: return "f7";
            case KeyCode.F8: return "f8";
            case KeyCode.F9: return "f9";
            case KeyCode.F10: return "f10";
            case KeyCode.F11: return "f11";
            case KeyCode.F12: return "f12";

            default:
                return key.ToString().ToLower().Replace("_", "-"); // fallback
        }
    }



    public KeyCode GetKey(string action)
    {
        return keybinds[action];
    }
}
