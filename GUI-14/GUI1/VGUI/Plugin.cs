using UnityEngine;
using BepInEx;
using GUI1.core;
using GUI1.Stuff;
using GUI1.Libraries;

namespace GUI1.Plugin;

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