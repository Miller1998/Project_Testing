using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    #region Private_Variable
    //numeric
    private const int maxEnergyPoint = 6;
    private float energyAmount;
    private readonly float regenAmount;
    #endregion

    #region Public_Variable

    public float eAmount;

    #endregion

    public Power()
    {
        energyAmount = 0f;
        regenAmount = 0.5f;
    }

    //get energy every 0.5 sec
    public void RegenerationUpdate()
    {
        energyAmount += regenAmount * Time.deltaTime;
        energyAmount = Mathf.Clamp(energyAmount, 0f, maxEnergyPoint);//clamp energy amount between min and max
        eAmount = energyAmount;//just for checking
    }
    //normalization the energy amount by max stack of energy
    public float GetEnergyNormalized()
    {
        return energyAmount / maxEnergyPoint;
    }

    //spend energy point when deploying soldier(s)
    public void SpendEnergy(float amount)
    {

        if (energyAmount >= amount)
        {
            energyAmount -= amount;
        }

    }

}
