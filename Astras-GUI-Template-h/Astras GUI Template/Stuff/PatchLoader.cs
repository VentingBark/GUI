using HarmonyLib;

namespace Astras_GUI_Template.Stuff;

public class PatchLoader
{
    public static void Apply()
    {
        Harmony VALLL = new Harmony(Constantss.GUID);
        VALLL.PatchAll();
    }
}