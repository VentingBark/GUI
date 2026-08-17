using GorillaLocomotion;
using GorillaNetworking;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;


namespace GUI1.core
{
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
        private float delaybetweenscore = 0f; // Delay between score setting actions
        private string floatString = "0";
        private bool isEditing = false;
        private float myFloat = 0f;
        private string NameTosetTo = "0"; // Variable to hold the name to set

        private void OnGUI()
        {
            if (!Open)
            {
                return;
            }

            if (!SLoaded)
            {
                INIT();
                SLoaded = true;
            }

            Window = GUILayout.Window(2223213, Window, UIM, "GUI", WStyle);
        }

        private void Update()
        {
            if (Keyboard.current.numpad0Key.wasPressedThisFrame)
            {
                ToggleMenu();
            }
        }

        private void FixedUpdate()
    
        {
            Setit();
        }

        private void ToggleMenu()
        {
            Open = !Open;

            // Keep the menu toggle lightweight. The old Discord webhook call could freeze the main thread.
            if (Open)
            {
                Debug.Log("GUI Opened");
            }
            else
            {
                Debug.Log("GUI Closed");
            }
        }

        private void UIM(int id)
        {
            MMod();
            GUILayout.Space(5f);
            if (GUILayout.Button("Close", BStyle))
            {
                ToggleMenu();
            }
            if (GUILayout.Button("Next Page", BStyle))
            {
                currentPage = (currentPage + 1) % 5;
            }
            if (GUILayout.Button("Previous Page", BStyle))
            {
                currentPage = (currentPage - 1 + 5) % 5;
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
                Normalmuilty = GUILayout.HorizontalSlider(Normalmuilty, 1.5f, 99.5f, SliderStyle, SliderThumbStyle);
                GUILayout.Label($"Speed Value set to {Normalmuilty:F3}");
                GUILayout.Space(2f);
                Speed = GUILayout.Toggle(Speed, "Enable Speed Boost");
                WorldScale = GUILayout.Toggle(WorldScale, "Enable WorldScale");
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
            }
            else if (currentPage == 2)
            {
                GUILayout.Label("Page 3 Content");
                // Add content for page 3 here
                if (GUILayout.Button("SetScore", BStyle))
                {
                    if (Time.time > delaybetweenscore)
                    {
                        delaybetweenscore = Time.time + 1f;
                        VRRig.LocalRig.SetQuestScore(int.Parse(floatString));

                    }
                    
                GUI.SetNextControlName("FloatField");
                string newText = GUILayout.TextField(floatString);
                if (newText != floatString)
                    {
                        floatString = newText;
                        isEditing = true;
                    }
                }
                if (GUILayout.Button("Set Name to: " + NameTosetTo, BStyle))
                {
                    ChangeName(NameTosetTo);
                }
                string Nametext = GUILayout.TextField(NameTosetTo);
                if (Nametext != NameTosetTo)
                {
                    NameTosetTo = Nametext;
                }
                if (isEditing && GUI.GetNameOfFocusedControl() != "FloatField")
                {
                    if (float.TryParse(floatString, out float result))
                        myFloat = result;
                    else
                    {
                        floatString = myFloat.ToString();
                    }
                        
                    
                    isEditing = false;


                }
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

        private void ChangeName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                return;
            }

            try
            {
                string trimmedName = newName.Trim();
                PhotonNetwork.NickName = trimmedName;

                if (PhotonNetwork.LocalPlayer != null)
                {
                    PhotonNetwork.LocalPlayer.NickName = trimmedName;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to change name: {ex.Message}");
            }
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
    }
}