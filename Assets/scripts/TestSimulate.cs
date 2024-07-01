using System.Collections;
using System.IO;
using UnityEngine;
using System;
using UnityEngine.UI;   
namespace SimulateSpace
{
    public class TestSimulate : MonoBehaviour
    {       
    public float TargetSpeed=5f;
    public float TargetRotationSpeed = 5f;
    public float MissileSpeed = 5f;
    public float MissileRotationSpeed = 5f;
    public float arrivalDistance = 10f;
    public Text rgo_value, hgo_value ;
    
    public LineRenderer missilePathRenderer;
    public LineRenderer targetPathRenderer;
    public ParticleSystem explosion;
    public GameObject Target, Missile;
    Color redColor = Color.red;
    Color blueColor = Color.blue;
    void Start()
    {
        InitializeLineRenderer(missilePathRenderer,redColor );
        InitializeLineRenderer(targetPathRenderer, blueColor);
        missilePathRenderer.positionCount = 1;
        targetPathRenderer.positionCount = 1;
        missilePathRenderer.SetPosition(0, Missile.transform.position);
        targetPathRenderer.SetPosition(0, Target.transform.position);
    }
    void Update()
    {
        float rudderControl = Input.GetKey(KeyCode.A) ? -TargetRotationSpeed : Input.GetKey(KeyCode.D) ? TargetRotationSpeed : 0f;
        Target.transform.Rotate(0f, rudderControl * Time.deltaTime, 0f);
        float roll = Input.GetAxis("Horizontal") * TargetRotationSpeed * Time.deltaTime;
        float pitch = Input.GetAxis("Vertical") * TargetRotationSpeed * Time.deltaTime;
        float yaw = Input.GetKey(KeyCode.LeftShift) ? -Input.GetAxis("Horizontal") * TargetRotationSpeed * Time.deltaTime : 0f;
        Target.transform.Rotate(pitch, yaw, -roll, Space.Self);
        Target.transform.Translate(Vector3.forward * TargetSpeed * Time.deltaTime);
        Vector3 directionToTarget = Target.transform.position - Missile.transform.position;
        Missile.transform.LookAt(Target.transform.position);
        Missile.transform.Translate(Missile.transform.forward * MissileSpeed * Time.deltaTime, Space.World);
        if (directionToTarget.magnitude > arrivalDistance){
            missilePathRenderer.positionCount++;
            missilePathRenderer.SetPosition(missilePathRenderer.positionCount - 1, Missile.transform.position);
            targetPathRenderer.positionCount++;
            targetPathRenderer.SetPosition(targetPathRenderer.positionCount - 1, Target.transform.position);
        }else {
            EndSimulation();
        }
        rgo_value.text="Rgo :  "+(Vector3.Distance(Target.transform.position, Missile.transform.position)*50).ToString() + " meters";
        hgo_value.text="Height Difference :  "+(Math.Abs((Target.transform.position.y-Missile.transform.position.y)*50)).ToString() + " meters";
    }
    public Vector3 GetTargetPosition()
    {
        return Target.transform.position;
    }
    public Vector3 GetMissilePosition()
    {
        return Missile.transform.position;
    }
    void InitializeLineRenderer(LineRenderer lineRenderer, Color color)
    {
        lineRenderer.material = new Material(Shader.Find("Standard"));
        lineRenderer.material.color = color;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.useWorldSpace = true;
    }
    private void EndSimulation()
    {
        Debug.Log("Simulation ended. Missile reached the target!");
        explosion.gameObject.SetActive(true);
        explosion.Play();
    }
}
}