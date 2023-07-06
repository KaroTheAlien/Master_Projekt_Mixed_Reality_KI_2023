using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEngine.SceneManagement;

public class Index_Controller_Input : MonoBehaviour
{
    [Header("Steam VR Input")]
    public SteamVR_Action_Boolean A_Button;
    public SteamVR_Action_Boolean B_Button;
    public SteamVR_Action_Boolean Trigger_Button;

    public SteamVR_Input_Sources source_type;

    [Header("Refenced Scripts")]
    public Interaction interaction;
    public Grappling grapple;

    private bool lootable;
    private bool talkable;
 
    // Update is called once per frame

    void Start()
    {
        //interaction = gameObject.GetComponent<Interaction>();
    }
    void Update()
    {
        //Primary Button Pushed
        if (A_Button.GetStateDown(source_type))
        {
            if (interaction.Status_Menu_Open() != menuStatus.nomenu)
            {
                menuStatus status = interaction.Status_Menu_Open();
                switch (status)
                {
                    case menuStatus.dungeonmenu:
                        Time.timeScale = 1.0f;
                        interaction.CloseMenu();
                        interaction.UnpauseGame();
                        break;

                    case menuStatus.winscreen:
                        //SceneManager.LoadScene("Start");
                        break;
                    case menuStatus.deathscreen:
                        
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;

                    case menuStatus.startmenu:
                        //SceneManager.LoadScene("Dungeon");
                        break;
                }
            }
            else if (source_type == SteamVR_Input_Sources.RightHand)
            {
                
            }
                
            else
            {
                   grapple.StartGrapple();
            }
        }
        //Secondary Button Pushed

        if (B_Button.GetStateDown(source_type))
        {
            if(interaction.Status_Menu_Open() != menuStatus.nomenu)
            {
                menuStatus status = interaction.Status_Menu_Open();
                switch (status)
                {
                    case menuStatus.dungeonmenu:
                        //SceneManager.LoadScene("Start");
                        break;

                    case menuStatus.winscreen:
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                        break;
                    case menuStatus.deathscreen:
                        //Turtorialszene 
                        // SceneManager.LoadScene("Tutorial");
                        break;

                    case menuStatus.startmenu:
                        //SceneManager.LoadScene(Tutorial);
                        break;
                }
            }

            else if (source_type == SteamVR_Input_Sources.RightHand)
            {

            }

            else
            {
              if(grapple.GetGrabStatus())
                {
                    grapple.DropGrappleObject();
                }

            }


        }
        // Trigger pushed
        if (Trigger_Button.GetStateDown(source_type))
        {
            if (SceneManager.GetActiveScene().name == "DungeonScene" || SceneManager.GetActiveScene().name == "MergeScene")
            {

                //Open Dungeon Menus
                if(interaction.Status_Menu_Open() == menuStatus.nomenu)
                interaction.OpenDungeonMenu();
            }
            

            if (source_type == SteamVR_Input_Sources.RightHand)
            {

            }

            else
            {
                
                
                
            }

        }
       



    }

    
}
