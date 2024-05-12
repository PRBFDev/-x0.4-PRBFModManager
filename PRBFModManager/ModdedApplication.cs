using BepInEx;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PRBFModManager
{
    public static class ModdedApplication
    {
        public static void RestartApplication()
        {
            Application.Quit(); // Завершаем текущее приложение
          

            System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", "") + ".exe"); // Запускаем приложение заново
            


        }
    }
}
