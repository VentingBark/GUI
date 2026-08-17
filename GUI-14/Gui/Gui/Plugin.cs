using UnityEngine;
using BepInEx;
using Gui.Core;
using Gui.Stuff;
using Gui.Libraries;

namespace Gui.Plugin;

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