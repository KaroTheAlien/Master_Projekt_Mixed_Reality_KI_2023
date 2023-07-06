using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TMPro;


public enum menuStatus { 
    nomenu,
    dungeonmenu,
    hubmenu,
    winscreen,
    deathscreen,
    startmenu,
}

public class Interaction : MonoBehaviour
{

    [SerializeField] Player playerObj;
    //[SerializeField] Enemy enemy;
    [Header("Menu Prefabs")]
    public GameObject dungeonmenu_prefab;
    public GameObject startmenu_prefab;
    public GameObject pausemenu_prefab;
    public GameObject deathscreen_prefab;
    public GameObject winscreen_prefab;

    private int playerHealth;
    private int playerMaxHealth;
    private GameObject main_canvas;
    private Slider player_health;
    private Slider boss_health;
    private TMP_Text player_text;
    private TMP_Text boss_text;
    //private CustomActions customActions;

 
    private GameObject dungeonmenu;


    private GameObject winmenu;
    private GameObject deathmenu;

    private GameObject startmenu; 
    private GameObject pausemenu;
    private Enemy[] enemies;
    private int enemy_num;
    private int bossHealth;
    private menuStatus currmenuStatus;

    

    void Start() 
    {

        playerHealth = playerObj.GetHP();
        playerMaxHealth = playerObj.GetHP();
        bossHealth = 100;
        //Debug.Log(playerHealth);
        
        enemies = FindObjectsOfType<Enemy>();
        enemy_num = enemies.Length;
        
        //OpenStartMenu();
        
    }

    private void Update()
    {
        
        
    }

    

    private void ClearMainCanvas()
    {
        main_canvas = GameObject.Find("MainGameCanvas");

        foreach (Transform child in main_canvas.transform)
        {
            Destroy(child.gameObject);
        }
        currmenuStatus = menuStatus.nomenu;
    }

    public void OpenStartMenu()
    {
        if (currmenuStatus == menuStatus.nomenu)
        {
            currmenuStatus = menuStatus.startmenu;
        }
        PauseGame();
        startmenu = Instantiate(startmenu_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        main_canvas = GameObject.Find("MainGameCanvas");

        startmenu.transform.SetParent(main_canvas.transform);
        startmenu.SetActive(true);
        startmenu.transform.localPosition = Vector3.zero;
        startmenu.transform.localRotation = Quaternion.identity;
        startmenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }

    public void CloseMenu()
    {
        UnpauseGame();
        ClearMainCanvas();

    }

    public void OpenDungeonMenu()
    {
        if(currmenuStatus == menuStatus.nomenu)
        {
            currmenuStatus = menuStatus.dungeonmenu;
        }
        PauseGame();
        dungeonmenu = Instantiate(dungeonmenu_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        main_canvas = GameObject.Find("MainGameCanvas");

        dungeonmenu.transform.SetParent(main_canvas.transform);

        dungeonmenu.transform.localPosition = Vector3.zero;
        dungeonmenu.transform.localRotation = Quaternion.identity;
        dungeonmenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);


        if (dungeonmenu.transform.IsChildOf(main_canvas.transform))
        {
            GameObject[] texttagged = GameObject.FindGameObjectsWithTag("Text_UI");
            player_health = GameObject.FindGameObjectWithTag("Spieler_Healthbar").GetComponent<Slider>();
            //player_health.value = player.calculate_health_perc();
            player_health.value = (float)playerHealth / playerMaxHealth;
            
            for (int i = 0; i < enemy_num; i++)
            {
                if (enemies[i].name.Contains("boss"))
                {
                    bossHealth = enemies[i].getPosture();
                    boss_health.value = (float)bossHealth/100;
                    break;
                }
                // boss = enemy [i]
                
                //}

            }

            if (texttagged[0].GetComponent<TMP_Text>().name == "BossHP")
            {
                boss_text = texttagged[0].GetComponent<TMP_Text>();
                player_text = texttagged[1].GetComponent<TMP_Text>();
            }
            else
            {
                player_text = texttagged[0].GetComponent<TMP_Text>();
                boss_text = texttagged[1].GetComponent<TMP_Text>();
            }

            // player_text = gameobject.find("maingamecanvas/menucanvas/background/stats/spielerhp").getcomponent<textmeshpro>();
            //player_text.text = "spieler hp: " + player.gethealthstatus() + " / 100";
            player_text.SetText("Spieler HP: " + playerHealth + " / 100");

            boss_health = GameObject.FindGameObjectWithTag("BossHealthbar").GetComponent<Slider>();
           
            //boss_text = gameobject.find("maingamecanvas/menucanvas/background/stats/bosshp").getcomponent<textmeshpro>();
            boss_text.SetText("Boss Posture: " + bossHealth + " / 100");
        }



        //m_go.setactive(true);
    }

    public void OpenDeathMenu()
    {
        if (currmenuStatus == menuStatus.nomenu)
        {
            currmenuStatus = menuStatus.deathscreen;
        }
        PauseGame();
        deathmenu = Instantiate(deathscreen_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        main_canvas = GameObject.Find("MainGameCanvas");

        deathmenu.transform.SetParent(main_canvas.transform);
        deathmenu.SetActive(true);
        deathmenu.transform.localPosition = Vector3.zero;
        deathmenu.transform.localRotation = Quaternion.identity;
        deathmenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }
    public void OpenWinMenu()
    {
        if (currmenuStatus == menuStatus.nomenu)
        {
            currmenuStatus = menuStatus.winscreen;
        }
        PauseGame();
        winmenu = Instantiate(winscreen_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        main_canvas = GameObject.Find("MainGameCanvas");

        winmenu.transform.SetParent(main_canvas.transform);

        winmenu.SetActive(true);
        winmenu.transform.localPosition = Vector3.zero;
        winmenu.transform.localRotation = Quaternion.identity;
        winmenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    }
    
    public void OpenPause()
    {
        pausemenu = Instantiate(winscreen_prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        main_canvas = GameObject.Find("MainGameCanvas");

        pausemenu.transform.SetParent(main_canvas.transform);
        pausemenu.SetActive(true);
        pausemenu.transform.localPosition = Vector3.zero;
        pausemenu.transform.localRotation = Quaternion.identity;
        pausemenu.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public menuStatus Status_Menu_Open()
    {
        return currmenuStatus;
    }

    private void PauseGame()
    {
        if(currmenuStatus != menuStatus.nomenu)
        {
            Time.timeScale = 0;

        }
    }

    public void UnpauseGame()
    {
        if (currmenuStatus == menuStatus.nomenu)
        {
            Time.timeScale = 1;

        }
    }

    

}
