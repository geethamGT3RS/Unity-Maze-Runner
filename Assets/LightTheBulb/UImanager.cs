using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;
    public Button nextPageButton;
    public Button previousPageButton;
    public GameObject levelSelectionPanel;
    public GameObject inGamePanel;
    public GameObject pausePanel;
    public WireConnector wireConnector;

    private int currentPage = 0;
    private const int LevelsPerPage = 21;
    private List<LevelData> levelDataList;

    void Start()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        levelDataList = LoadLevelData();
        ShowPage(currentPage);
    }

    private List<LevelData> LoadLevelData()
    {
        List<LevelData> data = new List<LevelData>();
        int unlockedLevels = PlayerPrefs.GetInt("unlockedLevels", 1); // Default is 1
        int currentLevel = PlayerPrefs.GetInt("currentLevel", 1); // Default is 1

        for (int i = 0; i < 100; i++)
        {
            data.Add(new LevelData
            {
                level = i + 1,
                unlocked = i < unlockedLevels,
                current = i + 1 == currentLevel
            });
        }
        return data;
    }

    private void ShowPage(int page)
    {
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        int startIndex = page * LevelsPerPage;
        int endIndex = Mathf.Min(startIndex + LevelsPerPage, levelDataList.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            LevelData data = levelDataList[i];
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            LevelButton button = buttonObj.GetComponent<LevelButton>();
            button.Setup(data.level, data.unlocked, data.current);

            button.button.onClick.AddListener(() => OnLevelButtonClicked(data.level));
        }
    }

    private void OnLevelButtonClicked(int level)
    {
        if (levelDataList[level - 1].unlocked)
        {
            wireConnector.SetLevel(level - 1); 
            levelSelectionPanel.SetActive(false);
            inGamePanel.SetActive(true);
        }
    }

    private void NextPage()
    {
        int maxPage = Mathf.CeilToInt(levelDataList.Count / (float)LevelsPerPage) - 1;
        if (currentPage < maxPage)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    private void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }

    public void RetryLevel()
    {
        wireConnector.RestartLevel();
        ResumeGame();
    }

    public void CompleteLevel()
    {
        int nextLevel = PlayerPrefs.GetInt("currentLevel", 1) + 1;
        PlayerPrefs.SetInt("unlockedLevels", Mathf.Max(nextLevel, PlayerPrefs.GetInt("unlockedLevels")));
        PlayerPrefs.SetInt("currentLevel", nextLevel);
        PlayerPrefs.Save();

        levelSelectionPanel.SetActive(true);
        inGamePanel.SetActive(false);
        ShowPage(currentPage);
    }

    public void BackToLevelSelection()
    {
        ResumeGame();
        levelSelectionPanel.SetActive(true);
        inGamePanel.SetActive(false);
        ShowPage(currentPage);
    }
}


[System.Serializable]
public class LevelData
{
    public int level;
    public bool unlocked;
    public bool current;
}
