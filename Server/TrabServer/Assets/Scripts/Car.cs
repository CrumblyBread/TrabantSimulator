using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Car")]
public class Car : ScriptableObject
{

    [Header("Variables")]
    public int type;
    public float maxHealth;
    public float currentHealth;
    public float maxFuel;
    public float currentFuel;
    public int passNum;

    public driveType drive = (driveType)0;
    public float maxRPM = 5000f, minRPM = 400f;
    public float[] gears;
    public AnimationCurve enginePower;

    public int wheelsNum = 4;

    public float smoothTime = 0.09f;

    [Header("Shop Details")]
    public int carPrice ;
    public string carName;
}
