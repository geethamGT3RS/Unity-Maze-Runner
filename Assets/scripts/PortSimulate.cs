// using System;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// using UnityEngine.UI;
// using TMPro;

// [System.Serializable]
// public class PositionRotationData
// {
//     public float m_x;
//     public float m_y;
//     public float m_z;
//     public float phi;
//     public float psi;
//     public float theta;
//     public float t_x;
//     public float t_y;
//     public float t_z;
//     public float d1;
//     public float d2;
//     public float d3;
//     public float d4;
// }
// public class PortSimulate : MonoBehaviour
// {
//     public GameObject StartPanel, GamePanel;
//     public TextMeshPro D1, D2, D3, D4;
//     public Button StartButton;
//     public Button StopButton;
//     public InputField portInput;
//     public int port = 6000;
//     private UdpClient udpClient;
//     private IPEndPoint endPoint;
//     public GameObject target;
//     public GameObject missile;
//     public GameObject fin1, fin2, fin3, fin4;
//     public LineRenderer targetPath, missilePath;
//     public Color MissileLine, TargetLine;
//     private Queue<PositionRotationData> dataQueue = new Queue<PositionRotationData>();
//     private bool isProcessingData = false;
//     private Vector3 offset;
//     public Text RangeText, MissileVelText, TargetVelText, TargetHeightText, MissileHeightText;
//     public GameObject Thrust;
//     public Toggle ThrustToggle;
//     public Camera ThisCamera;
//     private void Start()
//     {
//         StartPanel.SetActive(true);
//         GamePanel.SetActive(false);
//         ThrustToggle.onValueChanged.AddListener(delegate {
//             Thrust.SetActive(ThrustToggle.isOn);
//         });
//         StartButton.onClick.AddListener(StartGame);
//         StopButton.onClick.AddListener(StopGame);
//     }

//     void StartGame()
//     {
//         StartPanel.SetActive(false);
//         GamePanel.SetActive(true);
//         IPAddress ip = IPAddress.Parse("0.0.0.0");
//         if (portInput.text != "")
//         {
//             port = int.Parse(portInput.text);
//         }else
//         {
//             port=6000;
//         }
//         udpClient = new UdpClient(port);
//         endPoint = new IPEndPoint(ip, port);
//         InitializeLineRenderer(targetPath, TargetLine);
//         InitializeLineRenderer(missilePath, MissileLine);
//         InitializePosition();
//         ReceiveData();
//     }
//     void InitializePosition()
//     {
//         missile.transform.position = new Vector3(0f, 6f, 0f);
//         target.transform.position = new Vector3(0f, 100f, 0f);
//         missile.transform.rotation = Quaternion.Euler(0, 0, 45);
//         D1.text = "D1: 0.00";
//         D2.text = "D2: 0.00";
//         D3.text = "D3: 0.00";
//         D4.text = "D4: 0.00";
//         RangeText.text = "Range to go:\n0.00 m";
//         TargetVelText.text = "Target Vel:\n0.00 m/s";
//         TargetHeightText.text = "Target Height:\n0.00 m";
//         MissileVelText.text = "Missile Vel:\n0.00 m/s";
//         MissileHeightText.text = "Missile Height:\n0.00 m";
//     }
//     public void StopGame()
//     {
//         StartPanel.SetActive(true);
//         GamePanel.SetActive(false);
//         udpClient.Close();
//     }

//     private async void ReceiveData()
//     {
//         UdpReceiveResult result = await udpClient.ReceiveAsync();
//         string jsonString = Encoding.ASCII.GetString(result.Buffer);
//         missile.transform.position = new Vector3(0f, 6f, 0f);
//         target.transform.position = new Vector3(0f, 100f, 0f);
//         PositionRotationData data = JsonUtility.FromJson<PositionRotationData>(jsonString);
//         offset = missile.transform.position - new Vector3(data.m_x/10, -data.m_z/10, data.m_y/10);
//         target.transform.position = new Vector3(data.t_x/10, -data.t_z/10, data.t_y/10)+offset;
//         UpdateCameraPosition(missile.transform, target.transform);
//         while (true)
//         {
//             result = await udpClient.ReceiveAsync();
//             jsonString = Encoding.ASCII.GetString(result.Buffer);
//             PositionRotationData newData = JsonUtility.FromJson<PositionRotationData>(jsonString);
//             lock (dataQueue)
//             {
//                 dataQueue.Enqueue(newData);
//                 if (!isProcessingData)
//                 {
//                     StartCoroutine(ProcessDataQueue());
//                 }
//             }
//         }
//     }

//     private IEnumerator ProcessDataQueue()
//     {
//         isProcessingData = true;
//         while (dataQueue.Count > 0)
//         {
//             PositionRotationData data;
//             lock (dataQueue)
//             {
//                 data = dataQueue.Dequeue();
//             }
//             ParseAndMove(data);
//             yield return null; 
//         }
//         isProcessingData = false;
//     }
//     int functionCounter = 0;
//     private void ParseAndMove(PositionRotationData data)
//     {
//         missile.transform.position = new Vector3(data.m_x/10, -data.m_z/10, data.m_y/10)+offset;
//         Quaternion rotation=Quaternion.Euler(0, -data.phi, data.theta);
//         missile.transform.rotation = rotation;
//         target.transform.position = new Vector3(data.t_x/10, -data.t_z/10, data.t_y/10)+offset;
//         Quaternion container = Quaternion.Euler(0, 90, 45);
//         fin1.transform.rotation = container*Quaternion.Euler(data.d1, 0, 0);
//         fin2.transform.rotation = container*Quaternion.Euler(data.d2, 0, 180);
//         fin3.transform.rotation = container*Quaternion.Euler(0, data.d3, -90);
//         fin4.transform.rotation = container*Quaternion.Euler(0, data.d4, 90);
//         if (functionCounter % 10 == 0||(functionCounter>1500&&functionCounter%3==0))
//         {
//             D1.text = "D1: " + data.d1.ToString("F2");
//             D2.text = "D2: " + data.d2.ToString("F2");
//             D3.text = "D3: " + data.d3.ToString("F2");
//             D4.text = "D4: " + data.d4.ToString("F2");
//             RangeText.text = "Rgo: \n" + (Vector3.Distance(missile.transform.position, target.transform.position)*10).ToString("F2") + "m";
//             TargetVelText.text = "Target Vel:\n" + "-" + " m/s";
//             TargetHeightText.text = "Target Height:\n" + (-data.t_z).ToString("F2") + " m";
//             MissileVelText.text = "Missile Vel:\n" + "-" + " m/s";
//             MissileHeightText.text = "Missile Height:\n" + (-data.m_z).ToString("F2") + " m";
//         }
//         AddPositionToPath(targetPath, target.transform.position);
//         AddPositionToPath(missilePath, missile.transform.position);
//         functionCounter++;
//     }
//     private void AddPositionToPath(LineRenderer lineRenderer, Vector3 position)
//     {
//         int positionCount = lineRenderer.positionCount;
//         lineRenderer.positionCount++;
//         lineRenderer.SetPosition(positionCount, position);
//     }
//     void InitializeLineRenderer(LineRenderer lineRenderer, Color color)
//     {
//         lineRenderer.material = new Material(Shader.Find("Standard"));
//         lineRenderer.material.color = color;
//         lineRenderer.startWidth = 1f;
//         lineRenderer.endWidth = 1f;
//         lineRenderer.useWorldSpace = true;
//         lineRenderer.positionCount = 0;
//     }
//     void OnDisable()
//     {
//         udpClient.Close();
//     }
//      void UpdateCameraPosition(Transform missile, Transform target)
//     {
//         Vector3 midpoint = (missile.position + target.position) / 2.0f;
//         Vector3 direction = target.position - missile.position;
//         float distanceBetweenObjects = direction.magnitude;
//         float verticalFOV = ThisCamera.fieldOfView * Mathf.Deg2Rad;
//         float distance = distanceBetweenObjects / (2 * Mathf.Tan(verticalFOV / 2));
//         Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, direction).normalized;
//         Vector3 cameraPosition = midpoint + perpendicularDirection * distance;
//         ThisCamera.transform.position = cameraPosition;
//         ThisCamera.transform.LookAt(midpoint);
//     }
// }
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class PositionRotationData
{
    public float m_x, m_y, m_z, phi, psi, theta, t_x, t_y, t_z, d1, d2, d3, d4;
}

public class PortSimulate : MonoBehaviour
{
    public GameObject StartPanel, GamePanel, target, missile, fin1, fin2, fin3, fin4, Thrust;
    public TextMeshPro D1, D2, D3, D4;
    public Button StartButton, StopButton;
    public InputField portInput;
    public LineRenderer targetPath, missilePath;
    public Color MissileLine, TargetLine;
    public Text RangeText, MissileVelText, TargetVelText, TargetHeightText, MissileHeightText;
    public Toggle ThrustToggle;
    public Camera ThisCamera;

    private UdpClient udpClient;
    private IPEndPoint endPoint;
    private int port = 6000;
    private ConcurrentQueue<PositionRotationData> dataQueue = new ConcurrentQueue<PositionRotationData>();
    private Vector3 offset;
    private Thread receiveThread;
    private bool isRunning = false;
    private int functionCounter = 0;
    private bool isFirstPacket = true;

    private void Start()
    {
        StartPanel.SetActive(true);
        GamePanel.SetActive(false);
        ThrustToggle.onValueChanged.AddListener((isOn) => Thrust.SetActive(isOn));
        StartButton.onClick.AddListener(StartGame);
        StopButton.onClick.AddListener(StopGame);
    }

    private void Update()
    {
        while (dataQueue.TryDequeue(out PositionRotationData data))
        {
            ParseAndMove(data);
        }
    }

    void StartGame()
    {
        StartPanel.SetActive(false);
        GamePanel.SetActive(true);
        port = string.IsNullOrEmpty(portInput.text) ? 6000 : int.Parse(portInput.text);
        udpClient = new UdpClient(port);
        InitializeLineRenderer(targetPath, TargetLine);
        InitializeLineRenderer(missilePath, MissileLine);
        InitializePosition();
        isRunning = true;
        CaluclateOffsetAndStart();
    }
    async void CaluclateOffsetAndStart()
    {
        UdpReceiveResult result = await udpClient.ReceiveAsync();
        PositionRotationData data = JsonUtility.FromJson<PositionRotationData>(Encoding.ASCII.GetString(result.Buffer));
        offset = missile.transform.position - new Vector3(data.m_x / 10, -data.m_z / 10, data.m_y / 10);
        target.transform.position = new Vector3(data.t_x / 10, -data.t_z / 10, data.t_y / 10) + offset;
        UpdateCameraPosition(missile.transform, target.transform);
        receiveThread = new Thread(ReceiveData);
        receiveThread.Start();
    }
    void InitializePosition()
    {
        missile.transform.position = new Vector3(0f, 4f, 0f);
        target.transform.position = new Vector3(0f, 100f, 0f);
        missile.transform.rotation = Quaternion.Euler(0, 0, 45);
        ResetUIText();
    }

    void ResetUIText()
    {
        D1.text = D2.text = D3.text = D4.text = "D: 0.00";
        RangeText.text = "Range to go:\n0.00 m";
        TargetVelText.text = "Target Vel:\n0.00 m/s";
        TargetHeightText.text = "Target Height:\n0.00 m";
        MissileVelText.text = "Missile Vel:\n0.00 m/s";
        MissileHeightText.text = "Missile Height:\n0.00 m";
    }

    public void StopGame()
    {
        StartPanel.SetActive(true);
        GamePanel.SetActive(false);
        isRunning = false;
        udpClient.Close();
        receiveThread.Join();
    }

    private void ReceiveData()
    {
        while (isRunning)
        {
            try
            {
                UdpReceiveResult result = udpClient.ReceiveAsync().Result;
                PositionRotationData data = JsonUtility.FromJson<PositionRotationData>(Encoding.ASCII.GetString(result.Buffer));
                dataQueue.Enqueue(data);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error receiving data: {ex.Message}");
            }
        }
    }

    private void ParseAndMove(PositionRotationData data)
    {
        missile.transform.position = new Vector3(data.m_x / 10, -data.m_z / 10, data.m_y / 10) + offset;
        missile.transform.rotation =  Quaternion.Euler(0, -data.phi, data.theta);
        fin1.transform.localRotation = Quaternion.Euler(data.d1, 0, 0);
        fin2.transform.localRotation = Quaternion.Euler(data.d2, 0, 180);
        fin3.transform.localRotation = Quaternion.Euler(0, data.d3, -90);
        fin4.transform.localRotation =  Quaternion.Euler(0, data.d4, 90);
        target.transform.position = new Vector3(data.t_x / 10, -data.t_z / 10, data.t_y / 10) + offset;


        if (functionCounter % 10 == 0)
        {
            UpdateUIText(data);
        }

        AddPositionToPath(targetPath, target.transform.position);
        AddPositionToPath(missilePath, missile.transform.position);
        functionCounter++;
    }

    private void UpdateUIText(PositionRotationData data)
    {
        D1.text = $"D1: {data.d1:F2}";
        D2.text = $"D2: {data.d2:F2}";
        D3.text = $"D3: {data.d3:F2}";
        D4.text = $"D4: {data.d4:F2}";
        RangeText.text = $"Rgo: \n{Vector3.Distance(missile.transform.position, target.transform.position) * 10:F2} m";
        TargetVelText.text = "Target Vel:\n- m/s";
        TargetHeightText.text = $"Target Height:\n{-data.t_z:F2} m";
        MissileVelText.text = "Missile Vel:\n- m/s";
        MissileHeightText.text = $"Missile Height:\n{-data.m_z:F2} m";
    }

    private void AddPositionToPath(LineRenderer lineRenderer, Vector3 position)
    {
        int positionCount = lineRenderer.positionCount;
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(positionCount, position);
    }

    void InitializeLineRenderer(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = color;
        lineRenderer.startWidth = 1f;
        lineRenderer.endWidth = 1f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 0;
    }

    void OnDisable()
    {
        isRunning = false;
        udpClient?.Close();
        receiveThread?.Join();
    }

    void UpdateCameraPosition(Transform missile, Transform target)
    {
        Vector3 midpoint = (missile.position + target.position) / 2.0f;
        Vector3 direction = target.position - missile.position;
        float distanceBetweenObjects = direction.magnitude;
        float verticalFOV = ThisCamera.fieldOfView * Mathf.Deg2Rad;
        float distance = distanceBetweenObjects / (2 * Mathf.Tan(verticalFOV / 2));
        Vector3 perpendicularDirection = Vector3.Cross(Vector3.up, direction).normalized;
        Vector3 cameraPosition = midpoint + perpendicularDirection * distance;
        ThisCamera.transform.position = cameraPosition;
        if(ThisCamera.transform.position.y-30f<0)
        {
            ThisCamera.transform.position = new Vector3(ThisCamera.transform.position.x, 30f, ThisCamera.transform.position.z);
        }
        CameraHandler.instance.SavedPosition = ThisCamera.transform.position;
        CameraHandler.instance.SavedLookAt = midpoint;
        ThisCamera.transform.LookAt(midpoint);
    }
}
