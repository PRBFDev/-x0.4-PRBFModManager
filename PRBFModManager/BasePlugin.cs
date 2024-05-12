using BepInEx;
using HarmonyLib;
using JetBrains.Annotations;
using MidiPlayerTK;
using Mono.Cecil.Cil;
using MTM101BaldAPI;
using MTM101BaldAPI.OptionsAPI;
using MTM101BaldAPI.Reflection;
using MTM101BaldAPI.SaveSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;
using static MonoMod.Cil.RuntimeILReferenceBag.FastDelegateInvokers;
using TMPro;
using UnityEngine.UI;
using MTM101BaldAPI.AssetTools;
using Steamworks;

namespace PRBFModManager
{
  
    [BepInPlugin("prbf.modmanager", "PRBF's Mod Manager", "0.1")]
    public class BasePlugin : BaseUnityPlugin
    {
        public static BasePlugin instance;

        public static OptionsMenu menu;

        public string[] toIgnore = new string[]
            {
                "MTM101BaldAPI",
                "Newtonsoft.Json",
                "PRBFModManager",
                "PixelInternalAPI",
               

            };



        public string[] TeacherAPImods = new string[]
        {
            "Addition",
            "TeacherExtension",
        };

        public string[] endlessMods = new string[]
        {
            "Endless"
        };

        public string[] unsupportedMods = new string[]
        {
         
            "ModManager",
        };

        GameObject ob;

        public List<Plugin> Plugins = new List<Plugin>();

        public List<ToggleObject> Toggles = new List<ToggleObject>();

        List<string> chachedData = new List<string>();

        public Sprite InfoSprite;

       






        public TextLocalizer author;

        public int page;

        public int maxPage = 1;

        public int maxModsOnPage = 5;

        public StandardMenuButton apply;

        public UnityAction applyFunction;

        public GameObject modsMain;

        int slideValue;

        public Image mask;

        public GameObject buttonMask;

        public bool crashGameOnSave = true;

        public bool ableToScroll = false;

        int scrollSpeed = 1;

        int maxScroll = 15;

        int scrollValue = 0;

        UnityAction infoEvent;

        public Sprite bgSprite;

        public GameObject bgParent;

        public Sprite prbfIco;

        public Sprite prbfIco_Discord;


        public StandardMenuButton prbfButton;

        public StandardMenuButton prbfButtonDiscord;

        public StandardMenuButton prbfDiscord;

        public UnityAction prbfAction;

        public UnityAction discordAction;

        public StandardMenuButton blitzoTG;

        public UnityAction blitzo;

        public Sprite BlitzoIcon;

        bool isTeacherAPIInstalled = false;

        bool isTeacherAPIDisabled = false;

        Color greenLight = Color.green;

       Color black = Color.black;

        public Color current = Color.black;

        public float animationSpeed = 8f;

        Color animatedColor = Color.black;

        GameObject addons;

        public MenuToggle inShop;

      


        void OnEnable()
        {
            if (ableToScroll == false )
            {
                ableToScroll = true;
            }
        }
        void OnDisable()
        {
            if (ableToScroll == false)
            {
                return;
            }

            ableToScroll = false;   
        }


        public void Blitzo_TG()
        {
            Application.OpenURL("https://t.me/BLITZO_HB");
        }




        public void ChangePluginState(Plugin p)
        {
            ChangePlugin(p.PluginName, p.toggleObject.toggleObject.Value);
      
        }

        public string GetExeFile()
        {
            try
            {
                return Process.GetCurrentProcess().MainModule.FileName;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("Error getting exe file: " + e.Message);
                return null;
            }
        }



        public void ApplyGame()
        {
            
            BepInEx.Bootstrap.Chainloader.Plugins.Clear();
            BepInEx.Bootstrap.Chainloader.Initialize(GetExeFile());
            BepInEx.Bootstrap.Chainloader.Start();

           
            // Вывод информации о загруженных плагинах
            foreach (var p in BepInEx.Bootstrap.Chainloader.Plugins)
            {
                UnityEngine.Debug.Log(p);
            }
      if (crashGameOnSave)
            {
                Application.Quit();
            }
           
        }


        public void ChangeToggle(string Name, bool val)
        {
            foreach (ToggleObject t in Toggles)
            {
                if (t.toggleDisplay == Name)
                {
                    t.toggleObject.Set(val);
                }
            }
        }


        public void ChangePlugin(string Name, bool val)
{
    string path = Application.dataPath + "/../";
    string pluginPath = Path.Combine(path, "BepInEx", "plugins");
    string[] items = Directory.GetFiles( pluginPath);

  

    bool fileFound = false;
    foreach (var item in items)
    {
        if (Path.GetFileName(item).Contains(Name))
        {
           FileTools.ChangeExtension(item, val? ".dll" : ".disabled");
            break; // Exit the loop once the file is found and moved
        }
    }

    if (!fileFound)
    {
        UnityEngine.Debug.LogError("Файл с именем '" + Name + "' не найден в папке " + (val ? "неактивных" : "плагинов") + ".");
    }



        }
        
       


        public int GetPluginsCount()
        {
            string dataPath = Application.dataPath + "/../";
            string pluginsFolder = Path.Combine(dataPath, "BepInEx", "plugins");
   

           
            var filesFromPlugins = Directory.GetFiles(pluginsFolder).ToList();
      

   

            for (int i = 0; i < filesFromPlugins.Count; i++)
            {
                if (toIgnore.Contains(Path.GetFileNameWithoutExtension(filesFromPlugins[i])))
                {
                   filesFromPlugins.RemoveAt(i);
                    i--;
                }
                 
                  


                
                
                
            }

            return filesFromPlugins.Count;
        }


        void TelegramLink()
        {
            Application.OpenURL("https://t.me/PrbfDev");
        }

        void DiscordLink()
        {
            Application.OpenURL("https://discord.gg/Va8mj65b");
        }

        void LoadPlugins()
        {
            Plugins.Clear(); // Clear existing plugins before loading new ones

            string dataPath = Application.dataPath + "/../";
            string pluginsFolder = Path.Combine(dataPath, "BepInEx", "plugins");
        

           

            var filesFromPlugins = Directory.GetFiles(pluginsFolder).ToList();
   

         

            foreach (string file in filesFromPlugins)
            {
               



                if (file.EndsWith(".dll") || file.EndsWith(".disabled"))
                {
                    if (file.Contains("TeacherAPI"))
                    {
                        isTeacherAPIInstalled = true;
                        if (file.Contains(".disabled"))
                        {
                            isTeacherAPIDisabled = true;
                        }
                    }

                    Plugin newPlugin = new Plugin();
                    newPlugin.PluginPath = file;
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    bool ignorePlugin = false;

                  
                    if (isTeacherAPIDisabled)
                    {
                        foreach (var api in TeacherAPImods)
                        {
                           if (fileName.Contains(api))
                            {
                                ChangeToggle(fileName,false);
                                ChangePlugin(fileName, false);
                            }
                        }
                    }
               
                    foreach (var ignoreTo in toIgnore)
                    {
                        if (fileName == ignoreTo)
                        {
                            ignorePlugin = true;
                            break;
                        }
                    }


                    if (ignorePlugin)
                    {
                        continue; // Skip this plugin
                    }

                    if (file.EndsWith(".dll"))
                    {
                        newPlugin.Active = true;
                    }
                    else
                    {
                        newPlugin.Active = false;
                    }
                    newPlugin.PluginName = fileName;

                    if (!Plugins.Any(p => p.PluginName == newPlugin.PluginName))
                    {
                        newPlugin.index = Plugins.Count;
                     
                        foreach (var plugin in BepInEx.Bootstrap.Chainloader.PluginInfos.Values)
                        {
                            if (plugin.Metadata.Name.Contains(newPlugin.PluginName))
                            {
                                newPlugin.PluginData = "No metadata found.";
                            }
                        }
                            
                        
                        Plugins.Add(newPlugin);
                    }
                }
                else
                {
                    MTM101BaldiDevAPI.CauseCrash(BasePlugin.instance.Info, new Exception("ERROR! Plugin with that format not supported by BepinEx! Contact the developer of the mod: PRBF#8804 - Discord"));
                }
            }
        }






        void OnMen(OptionsMenu __instance)
        {
            if (Singleton<CoreGameManager>.Instance != null)
            {
                return;
            }


            if (menu != __instance)
            {
                menu = __instance;
            }



    


            prbfAction = TelegramLink;
            discordAction = DiscordLink;    
            prbfButton = CustomOptionsCore.CreateTextButton(__instance, new Vector2(-140f, -120f), "<size=12>", "PRBF Games \n Developer \n Telegram", prbfAction);
            CreateIconObject();
            prbfButtonDiscord = CustomOptionsCore.CreateTextButton(__instance, new Vector2(-60f, -120f), "<size=12>", "PRBF's Games Server \n Discord", discordAction);
            CreateDiscordIconObject();



            mask = GameObject.Instantiate(new GameObject()).AddComponent<Image>();

           mask.gameObject.AddComponent<RectTransform>();
            mask.GetComponent<RectTransform>().sizeDelta = new Vector2(320,170);
            mask.gameObject.AddComponent<Mask>();
            mask.name = "ModsMask";
 buttonMask = GameObject.Instantiate(new GameObject());
            
            buttonMask.gameObject.AddComponent<RectTransform>();
            buttonMask.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 50);
            buttonMask.name = "Buttons";


            modsMain = GameObject.Instantiate(new GameObject());
            modsMain.gameObject.AddComponent<RectTransform>();
            modsMain.name = "MainSlider";

            blitzo = Blitzo_TG;
            blitzoTG = CustomOptionsCore.CreateTextButton(__instance, new Vector2(30f, -120f), "<size=16>", "Blitzo \n Mod Tester \n <size=12>Telegram", blitzo);
            CreateBlitzoIconObject();

            author = CustomOptionsCore.CreateText(__instance, new Vector2(0f, -180f), "<size=12>©2024 PRBF Games");

            applyFunction = ApplyGame; 
            apply = CustomOptionsCore.CreateApplyButton(__instance,"<size=14>Saves Mods! \n <size=12> CAUTION! \n  (Closes  Current Application Session!)",applyFunction);


           
            


            ob = CustomOptionsCore.CreateNewCategory(menu, "Opt_ModManager");

            addons = CustomOptionsCore.CreateNewCategory(menu, "Game Settings");

            


            blitzoTG.transform.SetParent(ob.transform,false);


     

            modsMain.transform.SetParent(ob.transform,false);

            mask.transform.SetParent(modsMain.transform,false);

            buttonMask.transform.SetParent(mask.transform,false);

            prbfButtonDiscord.transform.SetParent(ob.transform, false);

            prbfButton.transform.SetParent(ob.transform, false);

            author.transform.SetParent(ob.transform, false);
            apply.transform.SetParent(ob.transform, false);

            // Load plugins before creating buttons
            LoadPlugins();
            foreach (var plug in Plugins)
            {
                bool pluginFound = false;

            






                if (!toIgnore.Contains(plug.PluginName)) // Check if the plugin should not be ignored
                {
                    if (plug.toggleObject == null)
                    {
                        plug.toggleObject = CreatePluginButton(__instance,plug.PluginName, plug.index, buttonMask.transform, plug.Active,plug.PluginData);
                    }


                }
            }

        }

        void InfoCalled()
        {

        }

        public void Clear()
        {
            chachedData.Clear();
            Plugins.Clear();
            foreach (var toggle in Toggles)
            {
                GameObject.Destroy(toggle.toggleObject.gameObject);
                Toggles.Remove(toggle);
            }
         
            
        }


        void Awake()
        {




            Harmony harmony = new Harmony("prbf.modmanager");
            CustomOptionsCore.OnMenuInitialize += OnMen;
          
            
            instance = this;

            bgSprite = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(Application.streamingAssetsPath,"Modded", "prbf.modmanager", "Textures", "bg.png")),100f);
            prbfIco = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(Application.streamingAssetsPath, "Modded", "prbf.modmanager", "Textures", "PRBF.png")), 100f);
            prbfIco_Discord = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(Application.streamingAssetsPath, "Modded", "prbf.modmanager", "Textures", "PRBF_discord.png")), 100f);
            BlitzoIcon = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(Application.streamingAssetsPath, "Modded", "prbf.modmanager", "Textures", "blitzo.png")), 100f);

            harmony.PatchAll();

          
        }

        public void CreateBGObject()
        {
            if (bgSprite != null)
            {
                Image img = GameObject.Instantiate(new GameObject()).AddComponent<Image>();
                img.sprite = bgSprite;  
                RectTransform rc = img.gameObject.AddComponent<RectTransform>();
                bgSprite.name = "BGObject";
                img.gameObject.transform.SetParent(bgParent.transform, false);

            }
            else
            {
                UnityEngine.Debug.LogError("BGSprite not found!");
            }
        }

        public void CreateIconObject()
        {
            if (prbfIco != null)
            {
                Image img = GameObject.Instantiate(new GameObject()).AddComponent<Image>();
                img.sprite = prbfIco;
              img.gameObject.AddComponent<RectTransform>();
                RectTransform rc = img.gameObject.GetComponent<RectTransform>();
                if (rc != null)
                {
                    rc.sizeDelta = new Vector2(64f, 64f);
                }
              
                bgSprite.name = "PRBFIcon";
                
                img.gameObject.transform.SetParent(prbfButton.transform, false);

                rc.sizeDelta = new Vector2(64f, 64f);

                img.raycastTarget = false;

            }
            else
            {
                UnityEngine.Debug.LogError("BGSprite not found!");
            }
        }

        public void CreateDiscordIconObject()
        {
            if (prbfIco_Discord != null)
            {
                Image img = GameObject.Instantiate(new GameObject()).AddComponent<Image>();
                img.sprite = prbfIco_Discord;
                img.gameObject.AddComponent<RectTransform>();
                RectTransform rc = img.gameObject.GetComponent<RectTransform>();
                if (rc != null)
                {
                    rc.sizeDelta = new Vector2(64f, 64f);
                }

                bgSprite.name = "PRBFIcon";

                img.gameObject.transform.SetParent(prbfButtonDiscord.transform, false);

                rc.sizeDelta = new Vector2(64f, 64f);

                img.raycastTarget = false;

            }
            else
            {
                UnityEngine.Debug.LogError("BGSprite not found!");
            }
        }


        public void CreateBlitzoIconObject()
        {
            if (BlitzoIcon != null)
            {
                Image img = GameObject.Instantiate(new GameObject()).AddComponent<Image>();
                img.sprite = BlitzoIcon;
                img.gameObject.AddComponent<RectTransform>();
                RectTransform rc = img.gameObject.GetComponent<RectTransform>();
                if (rc != null)
                {
                    rc.sizeDelta = new Vector2(64f, 64f);
                }

                bgSprite.name = "BlitzoIcon";

                img.gameObject.transform.SetParent(blitzoTG.transform, false);

                rc.sizeDelta = new Vector2(64f, 64f);

                img.raycastTarget = false;

            }
            else
            {
                UnityEngine.Debug.LogError("BGSprite not found!");
            }
        }
         

        public ToggleObject CreatePluginButton(OptionsMenu __instance, string ModName, int val, Transform parent, bool enabled, string metData = "")
        {
            string modState = string.Empty;
            Color baseColor = Color.black;
            Color disabled = Color.red;
            bool modRequiresTeacherAPI = false;
            bool TeacherAPIItself = false;  

            if (ModName == "TeacherAPI")
            {
                TeacherAPIItself = true;
            }

            foreach (var t in TeacherAPImods)
            {
                if (ModName.Contains(t))
                {
                    if (isTeacherAPIDisabled)
                    {
                        modRequiresTeacherAPI = true;
                    }
                   
                }
            }


            modState = "Enable/Disable Mod";
            if (modRequiresTeacherAPI)
            {
               modState = "<color=red> Required Teacher API to be ON!";
            }
            if (TeacherAPIItself)
            {
             if (isTeacherAPIDisabled)
                {
                    modState = "Enable/Disable Mod \n (Highly Recommened to Enable this mod if you want to play with mods \n that glowing with red light!)";
                    baseColor = animatedColor;
                }
            }
            if (!TeacherAPIItself)
            {
                baseColor = modRequiresTeacherAPI ? disabled : Color.black;
            }
          
            MenuToggle ModToggle = CustomOptionsCore.CreateToggleButton(__instance, new Vector2(30f, 30f - 100f * val), string.Format("{0} \n  {1}",ModName,metData)  , enabled, modState);

           
            ModToggle.transform.SetParent(parent, false);
            ToggleObject newToggler = new ToggleObject();
            newToggler.text = ModToggle.gameObject.transform.Find("ToggleText").GetComponent<TMP_Text>();
            newToggler.text.color = baseColor;
            newToggler.toggleDisplay = ModName;
            newToggler.toggleObject = ModToggle;
            newToggler.IsTeacherAPI = TeacherAPIItself;

            if (!Toggles.Any(t => t.toggleDisplay == newToggler.toggleDisplay))
            {
                Toggles.Add(newToggler);
            }

            return newToggler;
        }


        


        void CreateModManager()
        {
            ModManager modManager = GameObject.Instantiate(new GameObject().AddComponent<ModManager>());
            modManager.gameObject.name = "ModManager";
        }

        void Update()
        {



            if (isTeacherAPIDisabled)
            {
                animatedColor = Color.Lerp(animatedColor, current, animationSpeed * Time.deltaTime);
                if (animatedColor == current)
                {
                    if (current == black)
                    {
                        current = greenLight;
                    }
                    else if (current == greenLight)
                    {
                        current = black;
                    }
                }

                foreach (var tog in Toggles)
                { 
                    if (tog.IsTeacherAPI)
                    {
                       tog.text.color = animatedColor;
                    }
                }

                foreach (var api in TeacherAPImods)
                {
                 
                    foreach (var plugin in Plugins)
                    {
                       if (plugin.PluginName.Contains(api))
                        {
                            if (plugin.toggleObject.toggleObject.Value == true)
                            {
                                plugin.toggleObject.toggleObject.Set(false);
                              
                            }
                        }
                    }
                }
            }



            bool mouseMode = Input.mouseScrollDelta.y != 0? true : false;
            if (ableToScroll)
            {
             

                if (mouseMode)
                {
                    scrollValue = maxScroll;
                }
                else
                {
                    scrollValue = scrollSpeed;
                }

                if (Input.GetKey(KeyCode.UpArrow) || (Input.mouseScrollDelta.y > 0))
                {
                    slideValue = -1 * scrollValue;
                }
                else if (Input.GetKey(KeyCode.DownArrow) || (Input.mouseScrollDelta.y < 0))
                {
                    slideValue = 1 * scrollValue;
                }
                else
                {
                    slideValue = 0;
                }

                Vector3 position = buttonMask.GetComponent<RectTransform>().anchoredPosition3D;
                position.x = 15f;
                position.y += slideValue;
                buttonMask.GetComponent<RectTransform>().anchoredPosition3D = position;


            }



            foreach (var t in Plugins)
            {
              foreach (var s in unsupportedMods)
                {
                    if (t.PluginName == s)
                    {
                        MTM101BaldiDevAPI.CauseCrash(BasePlugin.instance.Info, new Exception(string.Format("ERROR! Plugin {0} is not supported by PRBF's Mod Manager! Please Contact the developer of the mod: PRBF#8804 - Discord",t.PluginName)));
                    }
                }
            }


            if (Plugins.Count != GetPluginsCount())
            {
                Clear();
                // Load plugins
                LoadPlugins();

                foreach (var plug in Plugins)
                {
                    if (!toIgnore.Contains(plug.PluginName)) // Check if the plugin should not be ignored
                    {
                        if (plug.toggleObject == null)
                        {
                            plug.toggleObject = CreatePluginButton(menu, plug.PluginName, plug.index, ob.transform, plug.Active);
                        }
                    }
                }
            }

                foreach (Plugin p in Plugins)
            {
                ToggleObject correspondingToggle = Toggles.Find(t => t.toggleDisplay == p.PluginName);
                if (correspondingToggle != null)
                {
                    p.toggleObject = correspondingToggle;
                    ChangePluginState(p);
                }
            }


            if (ModManager.instance == null)
            {
                CreateModManager();
            }

         //   if (DefinePlugins().Length != plugins.Count)
          //  {
           //     Do();
          //  }

          //  if (DefineDisabled().Length != disabled.Count)
           // {
           //     Do();
            //}

         
        }


    }

    [System.Serializable]
    public class ToggleObject
    {
        public MenuToggle toggleObject;
        public string toggleDisplay;
        public TMP_Text text;
        public bool IsTeacherAPI;
    }



    [System.Serializable]
    public class Plugin
    {
       
        public string PluginName;
        public string PluginData;
        public string PluginPath;
        public int index;
        public ToggleObject toggleObject;
        public bool Active;
    }
}
