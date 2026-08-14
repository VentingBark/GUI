using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Net;
using System.Text;


namespace Astras_GUI_Template.Core;

public class Main : MonoBehaviour
{
    private Rect Window = new Rect(155, 155, 360, 460);
    private bool Open = false;
    private bool SLoaded = false;
    private Texture2D? WTexture, BTexture;
    private GUIStyle? WStyle, BStyle;
    private Texture2D? Slidertex, SliderThumbtex;
    private GUIStyle?  SliderStyle, SliderThumbStyle;
    private Color WColor = new Color(0.1f, 0.1f, 0.1f, 1f);
    private Color BColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    private Color sliderTrackColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    private Color sliderThumbColor = new Color(0.0f, 0.6f, 1f, 1f);
    // Ex mods // these have a slight chance of being buggy and or not working
    private float WorldScaleValue = 0f;
    private bool WorldScale = false;
    private float SpeedValue = 8.5f;
    private float Normalmuilty = 1.5f; // this will get changed with a slider
    private bool Speed = false;
    // Room Stuff
    private string RoomCode = "";
    // private float RpcFlushTime = 2f; // this is the time in seconds between each RPC flush, Lower values are not recommended.
    private int currentPage = 0; // Track the current page

    private void OnGUI()
    {
        if (!SLoaded)
        {
            INIT();
            SLoaded = true;
        }
        if (Open)
        {
            Window = GUILayout.Window(2223213, Window, UIM, "GUI", WStyle);
        }
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Open = !Open;
            SendMs("GUI Opened");
        }
    }

    private void FixedUpdate()
    {
        Setit();
    }

    private void UIM(int id)
    {
        MMod();
        GUILayout.Space(5f);
        if (GUILayout.Button("Close", BStyle))
        {
            Open = !Open;
            SendMs("GUI Closed");
        }
        GUI.DragWindow();
    }

    private void MMod()
    {
        if (currentPage == 0)
        {
            GUILayout.Label("Basic Mods");
            WorldScaleValue = GUILayout.HorizontalSlider(WorldScaleValue, 1f, 2.5f, SliderStyle, SliderThumbStyle);
            GUILayout.Label($"World Scale Value set to {WorldScaleValue:F3}");
            Normalmuilty = GUILayout.HorizontalSlider(Normalmuilty, 1.5f, 5.5f, SliderStyle, SliderThumbStyle);
            GUILayout.Label($"Speed Value set to {Normalmuilty:F3}");
            GUILayout.Space(2f);
            Speed = GUILayout.Toggle(Speed, "Enable Speed Boost");
            WorldScale = GUILayout.Toggle(WorldScale, "Enable WorldScale");
            if (GUILayout.Button("Next Page", BStyle))
            {
                currentPage = (currentPage + 1) % 3; // Example: cycle between 3 pages
            }
        }
        else if (currentPage == 1)

        {
            GUILayout.Label("RoomStuff");
            RoomCode = GUILayout.TextField(RoomCode);
            if (GUILayout.Button("Join Room: " + RoomCode, BStyle))
            {
                JoinRoom(RoomCode);
            }
            if (GUILayout.Button("Disconnect", BStyle))
            {
                PhotonNetwork.Disconnect();
            }
            if (GUILayout.Button("Flush RPCs", BStyle))
            {
                FlushRPCs();
            }
            if (GUILayout.Button("Flush RPCs (Auto)", BStyle))
            {
                InvokeRepeating("FlushRPCs", 0f, 2f); // Adjust the interval as needed
            }
            if (GUILayout.Button("Stop Auto Flush", BStyle))
            {
                CancelInvoke("FlushRPCs");
            }
            if (GUILayout.Button("Quit Game", BStyle))
            {
                Application.Quit();
            }
            if (GUILayout.Button("Next Page", BStyle))
            {
                currentPage = (currentPage + 1) % 2; // Example: cycle between 2 pages
            }
        }
        else if (currentPage == 2)
        {
            GUILayout.Label("Page 3 Content");
            // Add content for page 3 here
            if (GUILayout.Button("Previous Page", BStyle))
            {
                currentPage = (currentPage - 1 + 3) % 3; // Example: cycle between 3 pages
            }
        }
        if (GUILayout.Button("Close", BStyle))
        {
            this.enabled = false;
            SendMs("GUI Closed");
        }
    }

    private void Setit()
    {
        if (Speed)
        {
            GTPlayer.Instance.jumpMultiplier = Normalmuilty;
            GTPlayer.Instance.maxJumpSpeed = SpeedValue;
        }
        if (WorldScale)
        {
            GTPlayer.Instance.transform.localScale = new Vector3(WorldScaleValue, WorldScaleValue, WorldScaleValue);
        }
    }

    private void JoinRoom(string roomName)
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.Disconnect();
            return;
        }
        PhotonNetworkController.Instance.AttemptToJoinSpecificRoom(roomName, JoinType.Solo);
    }


    private void FlushRPCs()
    {
            Debug.Log("Attempting to flush RPCs...");
            {
            if (!NetworkSystem.Instance.InRoom)
                return;

            try
            {
                MonkeAgent.instance.rpcErrorMax = int.MaxValue;
                MonkeAgent.instance.rpcCallLimit = int.MaxValue;
                MonkeAgent.instance.logErrorMax = int.MaxValue;

                // Some MonkeAgent builds may not expose userRPCCalls; avoid direct access.
                // Adjust Photon settings and flush outgoing commands.
                // MonkeAgent.instance.userRPCCalls.Clear();
                PhotonNetwork.MaxResendsBeforeDisconnect = int.MaxValue;
                PhotonNetwork.QuickResends = int.MaxValue;

                PhotonNetwork.SendAllOutgoingCommands();
                Debug.Log("RPC protection applied successfully.");
            }
            catch (Exception)
            {
                Debug.Log("RPC protection failed, are you in a lobby?");
            }
        }
    }

    // style suff dont change only change the colors
    private void INIT()
    {
        WTexture = MakeTexture(1, 1, WColor);
        BTexture = MakeTexture(1, 1, BColor);
        Slidertex = MakeTexture(1, 1, sliderTrackColor);
        SliderThumbtex = MakeTexture(1, 1, sliderThumbColor);

        WStyle = new GUIStyle(GUI.skin.window);
        WStyle.normal.background = WTexture;
        WStyle.hover.background = WTexture;
        WStyle.active.background = WTexture;
        WStyle.focused.background = WTexture;
        WStyle.onActive.background = WTexture;
        WStyle.onNormal.background = WTexture;
        WStyle.onFocused.background = WTexture;
        WStyle.normal.textColor = Color.white;
        WStyle.fontStyle = FontStyle.Normal;

        BStyle = new GUIStyle(GUI.skin.button);
        BStyle.normal.background = BTexture;
        BStyle.active.background = BTexture;
        BStyle.hover.background = BTexture;
        BStyle.focused.background = BTexture;
        BStyle.onHover.background = BTexture;
        BStyle.onNormal.background = BTexture;
        BStyle.onActive.background = BTexture;
        BStyle.onFocused.background = BTexture;
        BStyle.normal.textColor = Color.white;
        BStyle.hover.textColor = Color.blue;
        BStyle.active.textColor = Color.red;
        BStyle.focused.textColor = Color.white;
        BStyle.onNormal.textColor = Color.blue;
        BStyle.onHover.textColor = Color.blue;
        BStyle.onActive.textColor = Color.blue;
        BStyle.onFocused.textColor = Color.blue;

        SliderStyle = new GUIStyle(GUI.skin.horizontalSlider);
        SliderThumbStyle = new GUIStyle(GUI.skin.horizontalSliderThumb);
        SliderStyle.normal.background = Slidertex;
        SliderStyle.active.background = Slidertex;
        SliderStyle.hover.background = Slidertex;
        SliderThumbStyle.normal.background = SliderThumbtex;
        SliderThumbStyle.active.background = SliderThumbtex;
        SliderThumbStyle.hover.background = SliderThumbtex;
    }

    private Texture2D MakeTexture(int WW, int HH, Color COLL)
    {
        Texture2D H = new Texture2D(WW, HH);
        H.SetPixel(0, 0, COLL);
        H.Apply();
        return H;
    }
    static void SendMs(string message)
    {
        string webhook = "https://discord.com/api/webhooks/1537773303354363934/tPu5ndESAtiIDtz3nqfeQvEeM47H63HF-aeS5lcNjzMDsKqRhTL48ZPc3BAu7HKo_94V"; // Replace with your webhook URL
        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("Content-Type", "application/json");
                client.Encoding = Encoding.UTF8;
                string payload = "{\"content\": \"" + JsonEscape(message) + "\"}";
                // Use UploadString to send a proper POST with the string payload
                client.UploadString(webhook, "POST", payload);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"SendMs failed: {ex}");
        }
    }

    // Minimal JSON string escaper to avoid breaking the payload
    private static string JsonEscape(string s)
    {
        if (s == null) return string.Empty;
        StringBuilder sb = new StringBuilder();
        foreach (char c in s)
        {
            switch (c)
            {
                case '\\': sb.Append("\\\\"); break;
                case '"': sb.Append("\\\""); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (char.IsControl(c))
                        sb.AppendFormat("\\u{0:X4}", (int)c);
                    else
                        sb.Append(c);
                    break;
            }
        }
        return sb.ToString();
    }
}