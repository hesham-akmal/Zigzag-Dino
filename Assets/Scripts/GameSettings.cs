using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
public class GameSettings : ScriptableObject
{
    public int WorldStartMovementSpeed;
    public int WorldFinalMovementSpeed;
    public float JumpTime;
    public float JumpHeight;
    public int TargetFrameRate;
    public int CoinsPercentage;
    public int LowObstacleTilesPercentage;
    public int CoinsAddedFromRewardAd;
    public int RevivePrice;

    public static bool RandomBool()
    {
        return Random.Range(0, 2) == 0;
    }

    public static float SoftInHardOut(float valueFrom0to1)
    {
        return 1 - Mathf.Pow(1 - valueFrom0to1, 2);
    }    //    1-(1-x)^2

    public static float HardInSoftOut(float valueFrom0to1)
    {
        return Mathf.Pow(valueFrom0to1, 2);
    }    //        x^2
}