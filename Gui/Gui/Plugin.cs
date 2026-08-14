using UnityEngine;
using BepInEx;
using Astras_GUI_Template.Core;
using Astras_GUI_Template.Stuff;
using Astras_GUI_Template.Libraries;

namespace Astras_GUI_Template.Plugin;

[BepInPlugin(Constantss.GUID, Constantss.Name, Constantss.Version)]
public class Plugin : BaseUnityPlugin
{
    void Start()
    {
        PatchLoader.Apply();
    }

    void Awake()
    {
        GameObject Plugin = new GameObject(Constantss.ObjectName);
        Plugin.AddComponent<Main>();
        Plugin.AddComponent<OnScreenNotify>();
        DontDestroyOnLoad(Plugin);
    }
}