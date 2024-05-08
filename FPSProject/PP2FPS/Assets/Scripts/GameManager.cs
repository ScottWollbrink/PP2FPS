using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    GameObject activeMenu;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject levelSelectMenu;
    [SerializeField] GameObject winMenu;
    [SerializeField] GameObject loseMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject reticle;
    public GameObject checkpointMenu;

    public GameObject playerDamageScreen;
    public Image playerHPBar;
    public TMP_Text enemyCounter;

    public TMP_Text usedTime;
    public TMP_Text levelTime;

    public TMP_Text ammoCurr;
    public TMP_Text ammoMax;
    private int enemyCount;
    public GameObject goalLabel;
    public float goalMsgDisplayTime;

    public GameObject player;
    public playerController playerCntrl;
    public HoldController holdController;
    public GameObject playerSpawnPos;
    public GameObject cubeSpawnPos;

    public bool isPaused;
    public bool reticleIsShowing;


    void Awake()
    {
        Instance = this;
        player = GameObject.FindWithTag("Player");
        playerCntrl = player.GetComponent<playerController>();
        holdController = Camera.main.GetComponent<HoldController>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Position");
        cubeSpawnPos = GameObject.FindWithTag("Cube Spwan Position");
    }

    private void Start()
    {
        // main menu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            ShowHUDInMenu(false);
        }
        else
        {
            CheckReticle();
            StartCoroutine(ToggleGoalLabel(goalMsgDisplayTime));
            SetLevelTimer();
        }
    }

    private void SetLevelTimer()
    {
        // Assign the time stamp string to TMP_Text field
        levelTime.text = TimeManager.Instance.GetLevelTime(SceneManager.GetActiveScene().buildIndex);
        TimeManager.Instance.ToggleTimer();
    }

    private void ShowHUDInMenu(bool val)
    {
        activeMenu = mainMenu;
        activeMenu.SetActive(true);

        playerCntrl.enabled = val;
        player.GetComponentInChildren<cameraController>().enabled = val;
        playerHPBar.transform.parent.gameObject.SetActive(val);

        levelTime.gameObject.SetActive(val);
        ammoMax.gameObject.SetActive(val);

        Cursor.visible = !val;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(val);

    }

    void Update()
    {        
        HandlePause(); // escape = pause
        if (TimeManager.Instance.isCounting)
        {
            usedTime.text = TimeManager.Instance.GetCurrentTime();
        }
    }

    private void HandlePause()
    {
        if (Input.GetButtonDown("Cancel") && activeMenu != mainMenu) // button input
        {
            if (!isPaused) // Not paused
            {
                if (activeMenu != null) // if active menu close menu
                {
                    activeMenu.SetActive(false);
                    activeMenu = null;
                }

                statePaused();
                activeMenu = pauseMenu;
                activeMenu.SetActive(isPaused);
            }
            else // paused
            {
                if (activeMenu == levelSelectMenu || activeMenu == settingsMenu)
                {
                    activeMenu.SetActive(!isPaused);
                    activeMenu = pauseMenu;
                    activeMenu.SetActive(isPaused);

                }
                else if (activeMenu == loseMenu)
                {
                    // dont allow to unpause
                }
                else
                {
                    stateUnpaused();
                }
            }
        }
    }

    public void statePaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        reticle.SetActive(false);
        TimeManager.Instance.ToggleTimer();
    }

    public void stateUnpaused()
    {
        isPaused = !isPaused;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(isPaused);
        activeMenu = null;
        if (reticleIsShowing)
        {
            reticle.SetActive(true);
        }
        TimeManager.Instance.ToggleTimer();
    }

    IEnumerator ToggleGoalLabel(float time)
    {
        goalLabel.SetActive(true);
        yield return new WaitForSeconds(time);
        goalLabel.SetActive(false);
    }

    public void UpdateEnemyCounter(int amount)
    {
        enemyCount += amount;
        enemyCounter.text = enemyCount.ToString("F0");

    }

    public void UpdateLevelTimer(float time)
    {       
        levelTime.text = time.ToString("F0");
    }

    public void UpdateUsedTime(float time)
    {
        usedTime.text = time.ToString("F0");
    }

    public void ReturnToPause()
    {
        SwitchScene(pauseMenu);
    }

    public void DisplayLevelSelect()
    {
        SwitchScene(levelSelectMenu);
    }

    public void SelectLevel(int level)
    {
        if (activeMenu == mainMenu)
        {
            mainMenu.SetActive(false);
            ShowHUDInMenu(true);
        }

        GetComponent<ButtonFunctions>().SelectLevel(level);
    }

    public void DisplaySettings()
    {
        SwitchScene(settingsMenu);
    }

    private void SwitchScene(GameObject scene)
    {     
        activeMenu.SetActive(false);
        activeMenu = scene;
        activeMenu.SetActive(true);
    }

    private void CheckReticle()
    {
        reticleIsShowing = reticle.activeSelf;
    }

    public void SetReticle()
    {
        reticleIsShowing = !reticleIsShowing;
    }

    private void ToggleReticle()
    {
        SetReticle();
        reticle.SetActive(reticleIsShowing);
    }

    public void WinGame()
    {
        statePaused();

        activeMenu = winMenu;
        activeMenu.SetActive(isPaused);
    }

    public void LoseGame()
    {
        statePaused();
        activeMenu = loseMenu;
        activeMenu.SetActive(isPaused);

    }
}
