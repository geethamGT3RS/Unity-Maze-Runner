using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class WireConnector : MonoBehaviour
{
    public string levelFileName = "levels.json";
    public GameObject plugPrefab;

    private GameObject selectedCable;
    private GameObject selectedPlug;
    private List<GameObject> plugs = new List<GameObject>();
    private Dictionary<GameObject, GameObject> connections = new Dictionary<GameObject, GameObject>();
    private Dictionary<GameObject, float> wireLengths = new Dictionary<GameObject, float>();
    public Camera mainCamera;

    public GameObject livePlug;
    public GameObject bulb; 
    public GameObject Light;
    public UIManager uiManager;

    private PlayerData levelData;

    void Start()
    {
        Light.SetActive(false);
        LoadLevelData(levelFileName);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject.CompareTag("Cable"))
                {
                    OnCableSelected(hitObject);
                }
                else if (hitObject.CompareTag("Plug"))
                {
                    OnPlugSelected(hitObject);
                }
            }
        }
    }

    void LoadLevelData(string fileName)
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            levelData = JsonUtility.FromJson<PlayerData>(json);
        }
        else
        {
            Debug.LogError("Level file not found: " + filePath);
        }
    }

    public void SetLevel(int levelIndex)
    {
        if (levelData != null && levelData.levels != null && levelIndex < levelData.levels.Length)
        {
            InitializeLevel(levelData.levels[levelIndex]);
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }

    void InitializeLevel(Level level)
    {
        foreach (var plug in plugs)
        {
            Destroy(plug);
        }
        plugs.Clear();
        connections.Clear();
        wireLengths.Clear();

        foreach (var plugData in level.plugs)
        {
            Vector3 position = new Vector3(plugData.position[0], plugData.position[1], 0);
            GameObject plug = Instantiate(plugPrefab, position, Quaternion.identity);
            plugs.Add(plug);
            wireLengths[plug] = plugData.wireLength; // Initialize wire length for each plug
        }
        plugs.Add(livePlug);
        plugs.Add(bulb);
        wireLengths[livePlug] = 10;
        wireLengths[bulb] = 10; 
    }

    void OnCableSelected(GameObject cable)
    {
        if (selectedCable == cable)
        {
            ResetScene();
            return;
        }

        selectedCable = cable;
        selectedPlug = cable.transform.parent.gameObject;

        foreach (var plug in plugs)
        {
            if (CanConnect(selectedPlug, plug))
            {
                plug.SetActive(true);
            }
            else
            {
                plug.SetActive(false);
            }
        }
    }

    void OnPlugSelected(GameObject plug)
    {
        if (selectedPlug != null && selectedCable != null)
        {
            if (selectedPlug == plug)
            {
                ResetScene();
                return;
            }

            if (CanConnect(selectedPlug, plug))
            {
                ConnectCableToPlug(selectedCable, plug);
                connections[selectedPlug] = plug;
                ResetScene();
                CheckForCompletion();
            }
        }
    }

    void ConnectCableToPlug(GameObject cable, GameObject plug)
    {
        cable.transform.position = plug.transform.position;
        Vector3 newRotation = cable.transform.eulerAngles;
        newRotation.z += 180f;
        cable.transform.eulerAngles = newRotation;
        Vector3 newPosition = cable.transform.position;
        newPosition.z -= 0.5f;
        cable.transform.position = newPosition;
        PlugCableConnector cableConnector = cable.GetComponent<PlugCableConnector>();
        if (cableConnector != null)
        {
            cableConnector.SetWireLength(wireLengths[plug]);
        }
    }

    void ResetScene()
    {
        foreach (var p in plugs)
        {
            p.SetActive(true);
        }
        selectedCable = null;
        selectedPlug = null;
    }

    public void RestartLevel()
    {
        foreach (var p in plugs)
        {
            p.SetActive(true);
        }
        connections.Clear();
    }

    bool CanConnect(GameObject from, GameObject to)
    {
        float distance = Vector3.Distance(from.transform.position, to.transform.position);
        return distance <= wireLengths[from]; // Use variable wire length
    }

    public void CheckForCompletion()
    {
        HashSet<GameObject> visited = new HashSet<GameObject>();
        bool isComplete = IsConnectedToLivePlug(bulb, visited);

        if (isComplete)
        {
            Debug.Log("Level Complete! Bulb is connected to live plug.");
            Light.SetActive(true);
            uiManager.CompleteLevel();
        }
    }

    bool IsConnectedToLivePlug(GameObject plug, HashSet<GameObject> visited)
    {
        if (plug == livePlug)
        {
            return true;
        }

        if (visited.Contains(plug))
        {
            return false;
        }

        visited.Add(plug);

        if (connections.ContainsKey(plug))
        {
            return IsConnectedToLivePlug(connections[plug], visited);
        }

        return false;
    }
}

[System.Serializable]
public class PlayerData
{
    public Level[] levels;
}

[System.Serializable]
public class Level
{
    public PlugData[] plugs;
}

[System.Serializable]
public class PlugData
{
    public float[] position;
    public float wireLength;
}
