using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace SimulateSpace
{
public class TestHandler : MonoBehaviour
{
    public GameObject ManagerPanel, RadarPanel, InGamePanel;
    public Image MissileImage, TargetImage, SweepImage;
    public TestSimulate FileData;
    
    void Start()
    {
        Time.timeScale=0f; 
        ManagerPanel.SetActive(true);
        InGamePanel.SetActive(false);
        RadarPanel.SetActive(false);
    }
    
        void Update()
        {
            Vector3 missilePos = FileData.GetMissilePosition();
            Vector3 targetPos = FileData.GetTargetPosition();
            SetRadarImagePosition(MissileImage, missilePos);
            SetRadarImagePosition(TargetImage, targetPos);
            SweepImage.rectTransform.Rotate(Vector3.forward, Time.deltaTime * 100f);
        }
public void back(){
    SceneManager.LoadScene("Main");
}
        void SetRadarImagePosition(Image radarImage, Vector3 worldPosition)
        {
            float radarX = worldPosition.x * 240 / 500f;
            float radarY = worldPosition.z * 240 / 500f;
            radarImage.rectTransform.localPosition = new Vector3(radarX, radarY, 0);
            if (radarImage.rectTransform.localPosition.magnitude > 240)
            {
                radarImage.rectTransform.localPosition = radarImage.rectTransform.localPosition.normalized * 240;
            }
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
        Time.timeScale=0f;
    }
    public void openRadar(){
        RadarPanel.SetActive(true);
    }public void closeRadar(){
        RadarPanel.SetActive(false);
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