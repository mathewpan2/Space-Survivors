using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpOptionUI : MonoBehaviour
{
    public Image iconImage;
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button button;

    private PlayerUpgrade upgrade;
    private System.Action<PlayerUpgrade> onChosen;

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Setup(PlayerUpgrade upgrade, System.Action<PlayerUpgrade> onChosen)
    {
        this.upgrade = upgrade;
        this.onChosen = onChosen;

        if (iconImage) iconImage.sprite = upgrade.icon;
        if (titleText) titleText.text = upgrade.title;
        if (descriptionText) descriptionText.text = upgrade.description;
    }

    void OnClick()
    {
        onChosen?.Invoke(upgrade);
    }
}
