using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayerMinions;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    //note to me : please make the new class or something for tidiness of code 

    #region Private_Variable
    
    //Numeric
    [SerializeField]
    private float timeLeft = 136f;
    //UI
    [SerializeField]
    private Image enemyBarImage;
    [SerializeField]
    private Image playerBarImage;
    //Class
    private Power enemyPower;
    private Power playerPower;
    //LayerMask
    [SerializeField]
    private LayerMask layerMask;

    #endregion

    #region Public_Variable

    [Header("UI Caller")]
    public TextMeshProUGUI timerTextSpace;

    [Header("Data and GameObject Caller")]
    public PlayerSpawnerData[] minionDatas;
    public GameObject parentWorld;
    public Camera mainCamera;
    
    #endregion


    private void Awake()
    {

        enemyPower = new Power();
        playerPower = new Power();

        minionDatas = Resources.LoadAll("Datas/Minions", typeof(PlayerSpawnerData)).Cast<PlayerSpawnerData>().ToArray();

    }

    private void Update()
    {

        EnergyBar();
        CountDownTimer();
        UseAmountOfEnergy();
        //if timer reach 0 battle ends

    }

    public void CountDownTimer()
    {
        timeLeft -= 1 * Time.deltaTime;
        timerTextSpace.text = timeLeft.ToString("0") + "s";
    }

    #region EnergyBar_Mechanic_and_Spawning
    
    void EnergyBar()//this function just for execute the class
    {

        enemyPower.RegenerationUpdate(); //Debug.Log("Enemy energy amount : " + enemyPower.eAmount);
        playerPower.RegenerationUpdate(); //Debug.Log("Player energy amount : " + playerPower.eAmount);
        enemyBarImage.fillAmount = enemyPower.GetEnergyNormalized();
        playerBarImage.fillAmount = playerPower.GetEnergyNormalized();

    }

    void UseAmountOfEnergy()
    {

        //need screen touch for execute the SpendEnergy(int amount) order (both player and enemy)
        for (int i = 0; i < minionDatas.Length; i++)
        {

            if (minionDatas[i].playerMinionName.Contains("Player") /**and player phase == true**/)
            {

                if (playerPower.eAmount >= minionDatas[i].energyCost && Mouse.current.leftButton.wasPressedThisFrame)
                {

                    PlayerActionTouchEvent(minionDatas[i].minionModel, parentWorld);
                    playerPower.SpendEnergy(minionDatas[i].energyCost);

                }

            }

        }
        

    }
    
    public void PlayerActionTouchEvent(GameObject minionPrefabs, GameObject parent)
    {

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit rayCastHit;

        if (Physics.Raycast(ray, out rayCastHit, float.MaxValue, layerMask))
        {
            //Debug.Log("Mouse Position on World = " + rayCastHit.point);
            SpawnAtLocation(rayCastHit.point, minionPrefabs, parent);
        }

    }

    public void SpawnAtLocation(Vector3 spawnPosition, GameObject minionModel, GameObject parent)
    {

        GameObject deployedMinion = Instantiate(minionModel, spawnPosition, Quaternion.identity);
        deployedMinion.transform.parent = parent.transform;

    }

    #endregion

}