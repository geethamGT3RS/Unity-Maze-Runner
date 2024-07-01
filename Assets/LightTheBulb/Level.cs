using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Button button;
    public GameObject lockIcon;

    private int level;
    private bool unlocked;
    private bool current;

    public void Setup(int level, bool unlocked, bool current)
    {
        this.level = level;
        this.unlocked = unlocked;
        this.current = current;

        levelText.text = ""+level;
        lockIcon.SetActive(!unlocked);

        button.interactable = unlocked;
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        if (unlocked)
        {
            PlayLevel();
        }
    }

    private void PlayLevel()
    {
        Debug.Log("Playing level " + level);
    }
}
