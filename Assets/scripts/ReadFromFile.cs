using System.Collections;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace SimulateSpace
{
    public class SimulateFile : MonoBehaviour{
    public InputField[] Fields;
    public string filePath;
    public Text rgo_value,hgo_value;
    public string rotationFile,deltaFile;
    private StreamReader reader,reader1,reader2;
    private Vector3 targetPosition,missilePosition,InitialMissilePosition,offset;
    public GameObject Missile, Target, fin1, fin2, fin3, fin4;
    public LineRenderer targetPath, missilePath;
    public ParticleSystem Explosion;
    Color Red = Color.red;
    Color Blue = Color.green;
    public AudioSource LiftOff,TxEn;
    private int[] columnNumbers=new int[9];
    public Slider playbackSlider;
    private int maxLines;
    private int currentLine; 
    public RectTransform attitudeImage; 
    public Toggle toggleButton;
    public GameObject attitudeMeter;
    void Start(){   
        string f1=!string.IsNullOrEmpty(Fields[0].text) ? Fields[0].text : "Pos9.txt";
        string f2=!string.IsNullOrEmpty(Fields[1].text) ? Fields[1].text : "Att9.txt";
        filePath = "Assets/scripts/"+f1;
        rotationFile = "Assets/scripts/"+f2;
        deltaFile = "Assets/scripts/Delta9.txt";
        reader = new StreamReader(filePath);
        reader1 =new StreamReader(rotationFile);
        reader2 =new StreamReader(deltaFile);
        IntializePositions();
        InitializeLineRenderer(targetPath,Red);
        InitializeLineRenderer(missilePath,Blue);
        columnNumbers[0]=!string.IsNullOrEmpty(Fields[2].text) ? int.Parse(Fields[2].text) : 2;
        columnNumbers[1]=!string.IsNullOrEmpty(Fields[3].text) ? int.Parse(Fields[3].text) : 3;
        columnNumbers[2]=!string.IsNullOrEmpty(Fields[4].text) ? int.Parse(Fields[4].text) : 4;
        columnNumbers[3]=!string.IsNullOrEmpty(Fields[5].text) ? int.Parse(Fields[5].text) : 8;
        columnNumbers[4]=!string.IsNullOrEmpty(Fields[6].text) ? int.Parse(Fields[6].text) : 9;
        columnNumbers[5]=!string.IsNullOrEmpty(Fields[7].text) ? int.Parse(Fields[7].text) : 10;
        columnNumbers[6]=!string.IsNullOrEmpty(Fields[8].text) ? int.Parse(Fields[8].text) : 7;
        columnNumbers[7]=!string.IsNullOrEmpty(Fields[9].text) ? int.Parse(Fields[9].text) : 8;
        columnNumbers[8]=!string.IsNullOrEmpty(Fields[10].text) ? int.Parse(Fields[10].text) : 9;
        RestartSimulation();
        GetFileLineCount();
        playbackSlider.onValueChanged.AddListener(OnSliderValueChanged);
        toggleButton.onValueChanged.AddListener(OnToggleValueChanged);
    }
     void IntializePositions(){
        ReadNextTransformData();
        ReadNextTransformData();
        Missile.transform.position = new Vector3(0f,3.5f,0f);
        Target.transform.position = targetPosition-missilePosition+ Missile.transform.position;
        InitialMissilePosition = missilePosition;
        offset = Missile.transform.position - InitialMissilePosition;
    }
    void OnSliderValueChanged(float value)
    {
        int lineNum=Mathf.RoundToInt(value * maxLines);
        reader = new StreamReader(filePath);
        reader1 =new StreamReader(rotationFile);
        SkipToLine(lineNum);
        IntializePositions();
        InitializeLineRenderer(targetPath,Red);
        InitializeLineRenderer(missilePath,Blue);
        targetPath.positionCount = 0;
        missilePath.positionCount = 0;
    }
    void ReadNextTransformData()
    {
        string line = reader.ReadLine();
        string line1 = reader1.ReadLine();
        line1 = reader1.ReadLine();
        string line2 = reader2.ReadLine();
        reader2.ReadLine();
        reader2.ReadLine();
        reader2.ReadLine();
        reader2.ReadLine();
        reader2.ReadLine();
        reader2.ReadLine();
        line2 = reader2.ReadLine();
        string[] coordinates = line.Split('\t',' ');
        string[] coordinates1 = line1.Split('\t',' ');
        string[] coordinates2 = line2.Split('\t',' ');
        float x1 = float.Parse(coordinates[columnNumbers[0]])/10;//2
        float y1 = Math.Abs(float.Parse(coordinates[columnNumbers[2]])/10);//4
        float z1 = float.Parse(coordinates[columnNumbers[1]])/10;//3
        targetPosition = new Vector3(x1, y1, z1);
        float x2 = float.Parse(coordinates[columnNumbers[3]])/10;//8
        float y2 = Math.Abs(float.Parse(coordinates[columnNumbers[5]])/10);//10
        float z2 = float.Parse(coordinates[columnNumbers[4]])/10;//9
        missilePosition = new Vector3(x2, y2, z2);
        float p = float.Parse(coordinates1[columnNumbers[6]]);//7
        float q = float.Parse(coordinates1[columnNumbers[7]]);//8
        float r = float.Parse(coordinates1[columnNumbers[8]]);//9
        Quaternion rotation = Quaternion.Euler(0,q, r-90);
        float D1 = float.Parse(coordinates2[18]);//7
        float D2 = float.Parse(coordinates2[19]);//8
        float D3 = float.Parse(coordinates2[20]);//9
        float D4 = float.Parse(coordinates2[21]);//9
        Quaternion d1 = rotation*Quaternion.Euler(D1-90,0, 0);
        Quaternion d2 = rotation*Quaternion.Euler(D2-90,0, 180);
        Quaternion d3 = rotation*Quaternion.Euler(D3-90,90, -180);
        Quaternion d4 = rotation*Quaternion.Euler(D4-90,90,0);
        fin1.transform.rotation = d1;
        fin2.transform.rotation = d2; 
        fin3.transform.rotation = d3;  
        fin4.transform.rotation = d4; 
        Missile.transform.rotation = rotation; 
        attitudeImage.anchoredPosition = new Vector2(0, -r*175/90); 
        attitudeImage.localRotation = Quaternion.Euler(0f, 0f, -q);
    }
    
    private float accumulatedTime = 0f;
    private float updateInterval = 0.025f;
    void Update()
    {
        accumulatedTime += Time.deltaTime;
        if (accumulatedTime >= updateInterval)
        {
            accumulatedTime -= updateInterval;
            if(reader.EndOfStream || reader1.EndOfStream || reader2.EndOfStream)
            {
                Target.transform.position = targetPosition+offset;
                Missile.transform.position = missilePosition+offset;
            }
            else
            {
                ReadNextTransformData();
                Target.transform.position = targetPosition+offset;
                targetPath.positionCount++;
                targetPath.SetPosition(targetPath.positionCount - 1, Target.transform.position);
                Missile.transform.position = missilePosition+offset;
                missilePath.positionCount++;
                missilePath.SetPosition(missilePath.positionCount - 1, Missile.transform.position);
                float rgo_Num=Vector3.Distance(targetPosition, missilePosition)*10;
                rgo_value.text="Rgo :  "+rgo_Num.ToString() + " meters";
                hgo_value.text="Height Difference :  "+(Math.Abs((targetPosition.y-missilePosition.y)*10)).ToString() + " meters";
            }
        }
    }
    void OnToggleValueChanged(bool isOn)
    {
        attitudeMeter.SetActive(isOn);
    }
    
    public void RestartSimulation(){
        reader.BaseStream.Position = 0;
        reader1.BaseStream.Position = 0;
        reader2.BaseStream.Position = 0;
        IntializePositions();
        InitializeLineRenderer(targetPath,Red);
        InitializeLineRenderer(missilePath,Blue);
        targetPath.positionCount = 0;
        missilePath.positionCount = 0;
    }
    void InitializeLineRenderer(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = color;
        lineRenderer.startWidth = 0.5f;
        lineRenderer.endWidth = 0.5f;
        lineRenderer.useWorldSpace = true;
    }
    void SkipToLine(int line){
        for(int i=0;i<line;i++){
        reader.ReadLine();
        reader1.ReadLine();
        reader1.ReadLine();
        }
    }
    void GetFileLineCount()
    {
        using (StreamReader file = new StreamReader(filePath))
        {
            while (!file.EndOfStream)
            {
                file.ReadLine();
                maxLines++;
            }
        }
    }

    public Vector3 GetTargetPosition()
    {
        return Target.transform.position;
    }
    public Vector3 GetMissilePosition()
    {
        return Missile.transform.position;
    }
    }
}