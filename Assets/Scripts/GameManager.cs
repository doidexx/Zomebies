using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int points = 50;
    public float doublePointsTime = 13f;
    public Weapon[] weapons = null;
    public Weapon[] packedWeapons = null;

    public static bool doubleTap = false;
    public static bool speedCola = false;
    public static bool juggernog = false;
    public static bool quickRevive = false;
    public static bool muleKick = false;
    public static bool electricCherry = false;

    public float doublePointsTimer = Mathf.Infinity;

    private void Update()
    {
        doublePointsTimer += Time.deltaTime;
    }

    public void AddPoints(int income)
    {
        if (doublePointsTimer > doublePointsTime)
            points += income;
        else
            points += income * 2;
    }

    public void ConsumePoints(int cost)
    {
        points = Mathf.Max(0, points - cost);
    }

    public Weapon GetNewWeapon(int ID)
    {
        foreach (Weapon w in weapons)
        {
            if (w.ID != ID)
                continue;
            return w;
        }
        return null;
    }

    public Weapon GetNewPackedWeapon(int ID)
    {
        foreach (Weapon w in packedWeapons)
        {
            if (w.ID != ID)
                continue;
            return w;
        }
        return null;
    }

    public static void ClearDrinks()
    {
        doubleTap = false;
        speedCola = false;
        juggernog = false;
        quickRevive = false;
        muleKick = false;
        electricCherry = false;
    }

    public Weapon GetPakckedWeapon(int ID)
    {
        foreach (Weapon w in packedWeapons)
        {
            if (w.ID != ID)
                continue;
            return w;
        }
        return null;
    }
}
