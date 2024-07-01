using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace SimulateSpace
{
public class SimulationHandler : MonoBehaviour
{
    public GameObject FileMode,PortMode;
    public GameObject FilePanel, PortPanel;
    public GameObject StartPanel, ManagerPanel, RadarPanel, InGamePanel;
    public Image MissileImage, TargetImage, SweepImage;
    public SimulateFile FileData;
    private bool Paused=false;
    public Text ButtonText;
    public GameObject PauseImage,ResumeImage;
    public Slider rotationSlider; // Assign the Slider component in the inspector
    public Light directionalLight; // Assign the Directional Light in the inspector

    private void Start()
    {
        Time.timeScale=0f; 
        StartPanel.SetActive(true);
        InGamePanel.SetActive(false);
        FilePanel.SetActive(false);
        PortPanel.SetActive(false);
        ManagerPanel.SetActive(false);
        RadarPanel.SetActive(false);
        ButtonText.text="Pause";
        PauseImage.SetActive(true);
        ResumeImage.SetActive(false);
        rotationSlider.onValueChanged.AddListener(OnSliderValueChanged);
    }
    void OnSliderValueChanged(float value)
    {
        directionalLight.transform.rotation = Quaternion.Euler(value*180, 0f, 0f);
    }
        void Update()
        {
            Vector3 missilePos = FileData.GetMissilePosition();
            Vector3 targetPos = FileData.GetTargetPosition();
            SetRadarImagePosition(MissileImage, missilePos);
            SetRadarImagePosition(TargetImage, targetPos);
            SweepImage.rectTransform.Rotate(Vector3.forward, Time.deltaTime * 100f);
        }

        void SetRadarImagePosition(Image radarImage, Vector3 worldPosition)
        {
            float radarX = worldPosition.x * 240 / 5000f;
            float radarY = worldPosition.z * 240 / 5000f;
            radarImage.rectTransform.localPosition = new Vector3(radarX, radarY, 0);
            if (radarImage.rectTransform.localPosition.magnitude > 240)
            {
                radarImage.rectTransform.localPosition = radarImage.rectTransform.localPosition.normalized * 240;
            }
        }
    public void StartFileSimulation(){
        FilePanel.SetActive(true);
    }
    public void CloseFileSimulation(){
        FilePanel.SetActive(false);
        FileMode.SetActive(false);
    }
    public void confirmFileSimulation(){
        FileMode.SetActive(true);
        PortMode.SetActive(false);
        StartPanel.SetActive(false);
        ManagerPanel.SetActive(true);
    }
    public void confirmTestSimulation(){
        FileMode.SetActive(false);
        PortMode.SetActive(false);
        StartPanel.SetActive(false);
        SceneManager.LoadScene("TestSimulate");
    }
    public void RestartSimulation()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void startSimulation(){
        ManagerPanel.SetActive(false);    
        Time.timeScale=1f;
    }
    public void pause(){
        Paused=!Paused;
        if(Paused){
        Time.timeScale=0f;
        PauseImage.SetActive(false);
        ResumeImage.SetActive(true);
        ButtonText.text="Resume";
        }else{
        Time.timeScale=1f;
        PauseImage.SetActive(true);
        ResumeImage.SetActive(false);
        ButtonText.text="Pause";   
        }
    }
    public void openRadar(){
        RadarPanel.SetActive(true);
    }public void closeRadar(){
        RadarPanel.SetActive(false);
    }
    public void back(){
        ManagerPanel.SetActive(false);
        StartPanel.SetActive(true);
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
    public void pauseSimulation(){
        ManagerPanel.SetActive(true);
        Time.timeScale=0f;
    }
    public void continueToSimulation(){
        ManagerPanel.SetActive(false);
        InGamePanel.SetActive(true);
        Time.timeScale=1f; 
    }
}
}