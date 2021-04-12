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
    //gameobject
    [SerializeField]
    private GameObject ballPrefabs;
    //bool
    private bool enemyWin = false;
    private bool playerWin = false;

    #endregion

    #region Public_Variable

    [Header("UI Caller")]
    public TextMeshProUGUI timerTextSpace;

    [Header("Data and GameObject Caller")]
    public PlayerSpawnerData[] minionDatas;
    public GameObject parentAttacker;
    public GameObject parentDefender;
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
        PlayerActionTouchEvent();
        //if timer reach 0 battle ends

    }

    public void CountDownTimer()
    {
        timeLeft -= 1 * Time.deltaTime;
        timerTextSpace.text = timeLeft.ToString("0") + "s";
    }

    #region EnergyBarMechanic_and_Spawning
    
    void EnergyBar()//this function just for execute the class
    {

        enemyPower.RegenerationUpdate(); //Debug.Log("Enemy energy amount : " + enemyPower.eAmount);
        playerPower.RegenerationUpdate(); //Debug.Log("Player energy amount : " + playerPower.eAmount);
        enemyBarImage.fillAmount = enemyPower.GetEnergyNormalized();
        playerBarImage.fillAmount = playerPower.GetEnergyNormalized();

    }

    
    public void PlayerActionTouchEvent()
    {

        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit rayCastHit;

        if (Physics.Raycast(ray, out rayCastHit/**, float.MaxValue, layerMask**/))
        {

            string colliderName = rayCastHit.collider.name;
            //Debug.Log("Mouse On the" + rayCastHit.collider.name);

            //need screen touch for execute the SpendEnergy(int amount) order (both player and enemy)
            for (int i = 0; i < minionDatas.Length; i++)
            {

                if (minionDatas[i].playerMinionName.Contains("Attacker") && colliderName.Contains("Attacker") /**&& player phase**/)
                {

                    if (playerPower.eAmount >= minionDatas[i].energyCost && Mouse.current.leftButton.wasPressedThisFrame)
                    {

                        SpawnAtLocation(
                            rayCastHit.point, 
                            minionDatas[i].minionModel, 
                            parentAttacker, 
                            minionDatas[i].playerMinionName, 
                            minionDatas[i].spawnTime, 
                            minionDatas[i].normalSpeed,
                            minionDatas[i].carryingSpeed,
                            minionDatas[i].returnSpeed,
                            minionDatas[i].reactiveTime,
                            minionDatas[i].ballSpeed,
                            minionDatas[i].detectionRange);
                        playerPower.SpendEnergy(minionDatas[i].energyCost);

                    }

                }

                if (minionDatas[i].playerMinionName.Contains("Defender") && colliderName.Contains("Defender"))
                {

                    if (enemyPower.eAmount >= minionDatas[i].energyCost && Mouse.current.leftButton.wasPressedThisFrame)
                    {
                        
                        SpawnAtLocation(
                            rayCastHit.point, 
                            minionDatas[i].minionModel, 
                            parentDefender, 
                            minionDatas[i].playerMinionName, 
                            minionDatas[i].spawnTime,
                            minionDatas[i].normalSpeed,
                            minionDatas[i].carryingSpeed,
                            minionDatas[i].returnSpeed,
                            minionDatas[i].reactiveTime,
                            minionDatas[i].ballSpeed,
                            minionDatas[i].detectionRange);
                        enemyPower.SpendEnergy(minionDatas[i].energyCost);

                    }

                }

            }

        }

    }

    public void SpawnAtLocation(Vector3 spawnPosition, GameObject minionModel, GameObject parent, string name, float spTime, float normalSpeed, float carryingSpeed, float returnSpeed, float reactiveTime, float ballSpeed, float detectionRange)
    {

        GameObject deployedMinion = Instantiate(minionModel, spawnPosition, Quaternion.identity);
        deployedMinion.transform.parent = parent.transform;
        deployedMinion.transform.rotation = parent.transform.rotation;
        deployedMinion.gameObject.GetComponent<MinionBehaviour>().setDefault(name, spTime, normalSpeed, returnSpeed, carryingSpeed, reactiveTime, ballSpeed, detectionRange);

    }

    #endregion


    #region Clearing_Area_Every_Phase_Begin

    void ClearAll()
    {

        foreach (Transform child in parentAttacker.transform)
        {
            Destroy(child);
        }
        foreach (Transform child in parentDefender.transform)
        {
            Destroy(child);
        }

    }

    #endregion
}