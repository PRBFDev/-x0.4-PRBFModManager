using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using System.IO;
using System.Linq;
using UnityEngine.SceneManagement;
using MTM101BaldAPI.OptionsAPI;
using UnityEngine.Events;

namespace PRBFModManager
{
    public  class ModManager : MonoBehaviour
    {
        public static ModManager instance;

        public StandardMenuButton TelegramButton;

        public UnityAction Url;

        void OpenURL()
        {
            Application.OpenURL("https://t.me/PrbfDev");
        }
    


        void Awake()
        {
            instance = this;
        }
        void CreateTelegram()
        {
            if (TelegramButton == null)
            {
                foreach (var parent in GameObject.FindObjectsOfType<Transform>(true))
                {
                    if (parent.name == "About")
                    {
                        if (parent.GetComponent<StandardMenuButton>() != null)
                        {
                            return;
                        }
                        else
                        {
                            Transform ob = parent;
                            Url = OpenURL;
                            TelegramButton = MenuClassHandler.CreateTextButton(ob,new Vector2(0,-75),"Telegram",Url);
                        }
                    }
                }
                
            }
        }

        void Update()
        {
          if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                CreateTelegram();   
            }
        }



    }
}
