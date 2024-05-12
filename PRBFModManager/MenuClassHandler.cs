using MTM101BaldAPI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Events;
using UnityEngine;

namespace PRBFModManager
{
   public static class MenuClassHandler
    {

        public static void SetTooltip(StandardMenuButton smb, string tooltipText)
        {
            TooltipController t = (TooltipController)smb.OnHighlight.GetPersistentTarget(0);
            smb.OnHighlight = new UnityEvent();
            if (!(tooltipText == ""))
            {
                smb.OnHighlight.AddListener(delegate
                {
                    t.UpdateTooltip(tooltipText);
                });
            }
        }


        public static StandardMenuButton CreateApplyButton( UnityAction actOnPress)
        {
            StandardMenuButton component = UnityEngine.Object.Instantiate(GameObject.FindObjectOfType<OptionsMenu>(true).transform.Find("Graphics").transform.Find("ApplyButton").gameObject).GetComponent<StandardMenuButton>();
            component.OnPress = new UnityEvent();
            component.OnPress.AddListener(actOnPress);
         
            return component;
        }

        public static StandardMenuButton CreateTextButton(Transform p ,Vector2 pos, string text, UnityAction actOnPress)
        {
            StandardMenuButton standardMenuButton = CreateApplyButton( actOnPress);
            standardMenuButton.transform.position = new Vector3(pos.x, pos.y, standardMenuButton.transform.position.z);
            standardMenuButton.text.GetLocalizer().key = text;
            standardMenuButton.transform.SetParent(p, false);
            return standardMenuButton;
        }
    }
}
