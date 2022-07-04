using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

namespace MultiPlay.Editor
{
    [InitializeOnLoad]
    public class DualPlayEditor : UnityEditor.EditorWindow
    {
        #region privateMembers
        private string sourcePath;

        //Format
        private Texture2D bgTexture;
        private Texture2D headerTexture;
        private Texture2D bodyTexture;
        private const int windowMinWidth = 180;
        private const int windowMinHeight = 180;
        private const int windowMaxWidthExpanded = 420;
        private const int windowMinHeightExpanded = 320;

        private Color bgColor;
        private Color bodyColor;

        private Rect fullRect;
        private Rect headerRect;
        private Rect bodyRect;

        private float headerTexScale = 0.20f;
        private GUISkin skin;
        private float pad = 5f;

        private bool isCreatingReferences;
        private bool hasChanged;
        private static bool isClient;
        private string sceneFilePath;
        private DateTime lastSyncTime;
        private DateTime lastWriteTime;
        private static bool autoSync;

        private Int32 clientIndex;
        private string headerText;
        private GUIStyle headerStyle;
        private Color defaultFontColor;
        private string clientHeaderText;

        private static string myPubID = "46749";
        private float timer;
        private Vector2 scrollPos;
        private bool showSettings;
        private static DualPlayEditor window;

        //Settings: Hard coded
        private DualPlaySettings dualPlaySettingsAsset;

        //Settings Preferences
        private static int maxNumberOfClients;
        private static string clonesPath;
        private bool copyLibrary;
        private bool hasLibrary;
		public string LibraryPath { get; private set; }

        #region License Setup
        private const Licence productLicence = Licence.Default;
        private const string licenseMenuCaption = productLicence == Licence.Full ? "MultiPlay" : "DualPlay";
        #endregion
        #endregion
        #region menus
        [MenuItem("Tools/" + licenseMenuCaption + "/Client Manager &C", false, 10)]

        public static void OpenWindow()
        {
            try
            {
                string windowTitle = (productLicence == Licence.Full) ? "MultiPlay" : "DualPlay";
                if(window == null)
                    window = GetWindow<DualPlayEditor>(windowTitle, typeof(SceneView));
                window.titleContent = new GUIContent(windowTitle, EditorGUIUtility.ObjectContent(CreateInstance<DualPlayEditor>(), typeof(DualPlayEditor)).image);
                if (isClient)
                {
                    window.minSize = new Vector2(windowMinWidth, windowMinHeight);
                }
                else
                {
                    window.minSize = new Vector2(windowMinWidth, windowMinHeight);
                    window.maxSize = new Vector2(350, windowMinHeight * 1.5f);
                }

                window.Show();
            }
            catch (Exception) { }
        }



        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", false, 11)]
        static void Menu_Cleanup()
        {
            int clientsFound = DoLinksExist();

            string msg = $"Clearing cached references to {clientsFound} clients, are you sure you want to proceed?";


            if (clientsFound == 0)
                msg = $"No Clients were found in {clonesPath}, Try clear references anyway?";

            if (DoLinksLive())
            {
                Debug.Log("WARNING: Live Clients were detected! You Should close them before clearing references; Otherwise, Unity may crash!");
                msg = "WARNING!! Make sure ALL CLIENTS are CLOSED before proceeding!!";
            }

            if (EditorUtility.DisplayDialog("Clearing References", msg, "Proceed", "Cancel"))
            {
                try
                {
                    Debug.Log("Cleaning cache...");
                    EditorUtility.DisplayProgressBar("Processing..", "Shows a progress", 0.9f);
                    PurgeAllClients();
                    EditorUtility.ClearProgressBar();
                    Debug.Log("MultiPlay: References cleared successfully");
                    RemoveFromHub();
                    window?.Repaint();
                }
                catch (Exception e) { Debug.LogError(e.Message); }
            }
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Clean Up", true, 11)]
        static bool Validate_Menu_Cleanup()
        {
            int cnt = Application.dataPath.Split('/').Length;
            string appFolderName = Application.dataPath.Split('/')[cnt - 2];
            isClient = appFolderName.EndsWith("___Client");

            return !Application.isPlaying && !isClient;
        }

        [MenuItem("Tools/" + licenseMenuCaption + "/Rate Please :)", false, 30)]
        public static void MenuRate() => Application.OpenURL($"https://assetstore.unity.com/packages/tools/utilities/dual-play-multiplayer-testing-without-builds-164345?aid=1011lds77&utm_source=aff#reviews");

        [MenuItem("Tools/" + licenseMenuCaption + "/Help", false, 30)]
        public static void MenuHelp()
        {
            string helpFilePath = Application.dataPath + @"/Plugins/PanettoneGames/DualPlay/DualPlay Read Me.pdf";
            Debug.Log($"Help file is in: {helpFilePath}");
            Application.OpenURL(helpFilePath);
            Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
        }
        #endregion

        private void Awake() => InitializeTextures();
        private void OnEnable()
        {
            try
            {
                EditorApplication.playModeStateChanged += HandleOnPlayModeChanged;

                sceneFilePath = Application.dataPath + "/../" + SceneManager.GetActiveScene().path;
                sceneFilePath = sceneFilePath.Replace(@"/", @"\");

                isCreatingReferences = false;

                //headerColor = new Color(0, 0, 0);

                sourcePath = $"{ Application.dataPath }/..";
                sourcePath = sourcePath.Replace(@"/", @"\");


                headerText = (productLicence == Licence.Full) ? "MultiPlay" : "DualPlay";
                headerStyle = (productLicence == Licence.Full) ? skin.GetStyle("PanHeaderFull") : skin.GetStyle("PanHeaderDefault");

                defaultFontColor = GUI.contentColor;

                int cnt = Application.dataPath.Split('/').Length;
                string appFolderName = Application.dataPath.Split('/')[cnt - 2];
                isClient = appFolderName.EndsWith("___Client");
                if (isClient)
                {
                    bool indexHasParsed = Int32.TryParse(appFolderName.Substring(appFolderName.IndexOf('[') + 1, 1), out clientIndex);
                    LibraryPath = Application.dataPath + "/../Library";
                }

                clientHeaderText = (productLicence == Licence.Full) ? $"Client [{clientIndex}]" : $"Client";

                //reset status
                hasChanged = false;
                lastSyncTime = DateTime.Now;

                InitializeTextures();
                RemoveFromHub();
                //Debug.Log($"lastWrite: {lastWriteTime}, lastSync: {lastSyncTime}");
                EditorApplication.update += OnEditorUpdate;

                dualPlaySettingsAsset = Resources.Load<DualPlaySettings>("settings/DualPlaySettings");//there's already one scriptable object asset provided and you don't actually need to create another one, just find it and change its variables
                InitialiseSettings();
            }
            catch (Exception ex) { Debug.LogError($"{ex.Message}"); }
        }

        protected virtual void OnDisable()
        {
            SaveSettings();
            EditorApplication.update -= OnEditorUpdate;
            EditorApplication.playModeStateChanged -= HandleOnPlayModeChanged;
            
        }

        private void OnDestroy()
        {
            SaveSettings();
            EditorApplication.playModeStateChanged -= HandleOnPlayModeChanged;
            EditorApplication.update -= OnEditorUpdate;
        }

        private void InitialiseSettings()
        {
            if (string.IsNullOrEmpty(clonesPath))
            {
                clonesPath = dualPlaySettingsAsset.clonesPath;
            }
            maxNumberOfClients = productLicence == Licence.Default? 1 : dualPlaySettingsAsset.maxNumberOfClients;
            copyLibrary = dualPlaySettingsAsset.copyLibrary;

            if (string.IsNullOrEmpty(clonesPath))
            {
                clonesPath = clonesPath.Replace(@"/", @"\");
            }
        }

        private void SaveSettings()
        {
            maxNumberOfClients = productLicence == Licence.Default ? 1 : dualPlaySettingsAsset.maxNumberOfClients;
            dualPlaySettingsAsset.clonesPath = clonesPath;
            dualPlaySettingsAsset.maxNumberOfClients = maxNumberOfClients;
            dualPlaySettingsAsset.copyLibrary = copyLibrary;
        }


        private void HandleOnPlayModeChanged(PlayModeStateChange obj)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode && isClient && !autoSync)
            {
                //CheckIfSceneChanged();
                //if (hasChanged)
                {
                    ReloadScene();
                    hasChanged = false;
                }
            }
        }


        private void OnEditorUpdate()
        {
            if (isClient) CheckIfSceneChanged();
        }

        private void CheckIfSceneChanged()
        {
            try
            {
                lastWriteTime = File.GetLastWriteTime(sceneFilePath);
                if (lastWriteTime > lastSyncTime) //scene changed
                {

                    if (autoSync)
                    {
                        try
                        {
                            //if (clientIndex > 1)
                            {
                                //Debug.Log("Lib: " + copyLibrary + ". Refreshing...");
                                System.Threading.Thread.Sleep(clientIndex * 50); //<< inducing some delay here to prevent Editor crashing

                                hasChanged = false;
                                lastSyncTime = DateTime.Now;
                            }
                            ReloadScene();

                        }
                        catch (Exception)
                        {
                            if (autoSync)
                            {
                                Debug.LogError("Error reloading Scene. Switching to Manual Sync...");
                                autoSync = false;
                            }
                        }
                    }
                    hasChanged = false;

                    //Debug.LogWarning($"Changes made on {lastWriteTime}. Make sure to Sync before running the game");
                    Repaint();
                }
            }
            catch (Exception e) { Debug.LogError($"{e.Message}"); }
        }

        private void InitializeTextures()
        {
            try
            {
                headerTexture = (productLicence == Licence.Full) ? Resources.Load<Texture2D>("icons/MP_EditorHeader") : Resources.Load<Texture2D>("icons/DP_EditorHeader");
                skin = Resources.Load<GUISkin>("guiStyles/Default");

                bgTexture = new Texture2D(1, 1);
                bgTexture.SetPixel(0, 0, bgColor);
                bgTexture.Apply();

                bodyTexture = new Texture2D(1, 1);
                bodyTexture.SetPixel(0, 0, bodyColor);
                bodyTexture.Apply();
            }
            catch (Exception e) { Debug.LogError($"{e.Message}"); }
        }
        private void OnGUI()
        {
            DrawLayout();
        }

        private void DrawLayout()
        {
            #region Header Formatting

            if (bgTexture == null || headerTexture == null || skin == null)
                InitializeTextures();

            fullRect = new Rect(0, 0, Screen.width, Screen.height);
            GUI.DrawTexture(fullRect, bgTexture);

            //Header
            headerRect = new Rect(Screen.width - headerTexture.width * headerTexScale, 0, headerTexture.width * headerTexScale, headerTexture.height * headerTexScale);
            GUI.DrawTexture(headerRect, headerTexture);

            //Body
            bodyRect = new Rect(pad, headerRect.height + pad, Screen.width - pad * 2, Screen.height - headerRect.height - pad * 2);
            GUI.DrawTexture(bodyRect, bodyTexture);

            #region Draw Header
            GUILayout.BeginArea(fullRect);
            if (isClient)
                GUILayout.Label(clientHeaderText, headerStyle);
            else
                GUILayout.Label(headerText, headerStyle);
            GUILayout.EndArea();
            #endregion
            #endregion

            #region EditorPlaying
            if (EditorApplication.isPlaying)
            {
                GUILayout.BeginArea(bodyRect);
                EditorGUILayout.HelpBox($"Control panel is disabled while playing.", MessageType.Info);
                ShowNotification(new GUIContent("Playing..."), 1);
                if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
                {
                    Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                    Application.OpenURL("https://panettonegames.com/");
                }
                GUILayout.EndArea();
                return;
            }
            #endregion

            if (isClient) //Client
            {
                GUILayout.BeginArea(bodyRect);
                if (GUILayout.Button("Sync"))
                {
                    hasChanged = false;
                    lastSyncTime = DateTime.Now;
                    ShowNotification(new GUIContent("Syncing..."));
                    ReloadScene();
                }

                hasLibrary = !IsSymbolic(LibraryPath);
                string autoSyncCaption = !hasLibrary ? "Auto Sync" : "Auto Sync - Client created without [Link Library]";
                GUI.enabled = !hasLibrary;
                autoSync = GUILayout.Toggle(!hasLibrary ? autoSync : false, autoSyncCaption);
                GUI.enabled = true;

                if (hasChanged) EditorGUILayout.HelpBox("Changes from original build were detected. Make sure to Sync before running", MessageType.Warning);
                else EditorGUILayout.HelpBox($"You're Good to Go!\nLast Changed:\t{lastWriteTime}\nLast Synced:\t{lastSyncTime}", MessageType.Info);

                if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
                {
                    Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                    Application.OpenURL("https://panettonegames.com/");
                }
                GUILayout.EndArea();
            }

            else //Original Copy
            {
                GUILayout.BeginArea(bodyRect);////////////////////1
                GUILayout.BeginVertical();//////////////2

                if (isCreatingReferences)
                {
                    isCreatingReferences = false;
                    ShowNotification(new GUIContent("Creating Client..."));
                }
                else //Create References Or Launch
                {
                    scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(90));

                    for (int i = 1; i < maxNumberOfClients + 1; i++)
                    {
                        string destinationPath = $"{ clonesPath }/{Application.productName}_[{i}]___Client".Replace(@"/", @"\");
                        string btnCaption = Directory.Exists(destinationPath) ? $"Launch Client [{i}]" : $"Create Client [{i}]";
                        GUI.enabled = !Directory.Exists(destinationPath + "\\Temp");

                        GUILayout.BeginHorizontal();
                        if (Directory.Exists(destinationPath)) GUI.contentColor = Color.green;
                        if (GUILayout.Button(btnCaption, GUILayout.Height(25)))
                        {
                            Debug.Log($"creating Client {i} in {destinationPath.Replace("\\\\", "\\")}");

                            /////////////
                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            if (!Directory.Exists(destinationPath))
                            {
                                isCreatingReferences = false;
                                CreateLink(destinationPath, "Assets");
                                CreateLink(destinationPath, "ProjectSettings");
                                CreateLink(destinationPath, "Packages");

                                if (copyLibrary)
                                    CreateLink(destinationPath, "Library"); //kills auto sync.
                            }

                            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                            hasChanged = false;
                            LaunchClient(destinationPath);
                            RemoveFromHub();
                            //Close();

                            GUI.enabled = true;
                            ///
                        }
                        if (Directory.Exists(destinationPath))
                        {
                            GUI.contentColor = Color.red;
                            if (GUILayout.Button("X", GUILayout.Height(25), GUILayout.Width(25)))
                            {
                                Debug.Log($"Deleting [{new DirectoryInfo(destinationPath).Name}]");
                                ClearClient(destinationPath);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUI.contentColor = defaultFontColor;
                    }

                    EditorGUILayout.EndScrollView();
                    GUI.enabled = true;
                }

                #region store link inside settings
                if (GUILayout.Button("More cool tools...", skin.GetStyle("PanStoreLink")))
                {
                    Application.OpenURL($"https://assetstore.unity.com/publishers/" + myPubID);
                    Application.OpenURL("https://panettonegames.com/");
                }
                //GUILayout.Space(5);
                #endregion
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }

        private bool IsSymbolic(string path)
        {
            FileInfo pathInfo = new FileInfo(path);
            return pathInfo.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        private static int DoLinksExist()
        {
            int cnt = 0;

            for (int i = 1; i < maxNumberOfClients + 1; i++)
            {
                string destinationPath = $"{ clonesPath }/{Application.productName}_[{i}]___Client".Replace(@"/", @"\");
                //if (i == 1) result = Directory.Exists(destinationPath);
                //result = result || Directory.Exists(destinationPath);

                if (Directory.Exists(destinationPath)) cnt++;
            }

            return cnt;
        }

        private static bool DoLinksLive()
        {
            bool result = false;
            for (int i = 1; i < maxNumberOfClients + 1; i++)
            {
                string destinationPath = $"{ clonesPath }/{Application.productName}_[{i}]___Client".Replace(@"/", @"\");
                if (i == 1) result = Directory.Exists(destinationPath + "\\Temp");

                result = result || Directory.Exists(destinationPath + "\\Temp");
            }

            return result;
        }

        private static void ReloadScene()
        {
            try
            {
                EditorSceneManager.OpenScene(SceneManager.GetActiveScene().path);

            }
            catch (Exception)
            {
                if (autoSync)
                {
                    Debug.LogError("Error reloading Scene. Switching to Manual Sync...");
                    autoSync = false;
                }
            }
        }

        private static void RemoveFromHub()
        {
            try
            {
                string kFound = string.Empty;
                string keyName = @"Software\Unity Technologies\Unity Editor 5.x";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (key == null)
                    {
                        //Debug.Log("Editor version not found");
                    }
                    else
                    {
                        string[] kvals = key.GetValueNames();
                        foreach (string k in kvals)
                        {
                            //if (k.Contains("RecentlyUsedProjectPaths-0"))
                            if (k.Contains("RecentlyUsedProjectPaths-"))
                            {


                                if (key.GetValueKind(k) == RegistryValueKind.Binary)
                                {
                                    var value = (byte[])key.GetValue(k);
                                    var valueAsString = Encoding.ASCII.GetString(value);

                                    if (valueAsString.EndsWith("Client"))
                                    {
                                        //Debug.Log($"key deleted: {k} with value {valueAsString}");
                                        key.DeleteValue(k);
                                        //kFound = k;
                                        //break;
                                    }

                                }
                            }
                        }
                        // Debug.Log($"{kFound} deleted");
                    }
                }
            }
            catch (Exception e) { Debug.LogError($"Unable to clear system cache due to unsufficient User Priviliges. Please contact your system administrator. \nDetails: {e.Message}"); }
        }

        private void CopyStartupConfig(string destPath)
        {
            try
            {
                string startupFile = "LastSceneManagerSetup.txt";
                string args = $"/c copy /y {sourcePath}\\Library\\{startupFile} {startupFile}\\Library\\{startupFile}";
                var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                thread.Start();
            }
            catch (Exception exx)
            {
                Debug.LogError($"Links failed. You do not have sufficient previliges to write to windows temporary files. Please contact your system administrator\n{exx.Message}");

            }
        }
        private void CreateLink(string destPath, string subDirectory)
        {
            if (!Directory.Exists(destPath))
                Directory.CreateDirectory(destPath);

            string cmd, args = String.Empty;
            try
            {
                switch (Application.platform)
                {
                    case RuntimePlatform.WindowsEditor:
                        cmd = "cmd";
                        args = $"/c mklink /j \"{destPath}\\{subDirectory}\" \"{sourcePath}\\{subDirectory}\"";
                        break;

                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.LinuxEditor:

                        cmd = "/bin/bash";
                        args = $"ln -s \"{destPath}\\{subDirectory}\" \"{sourcePath}\\{subDirectory}\"";
                        args = "-c \"" + args + "\"";
                        break;

                    default:
                        throw new NotImplementedException("Platform not supported!");
                }


                var thread = new Thread(delegate () { ExcuteCMD(cmd, args); });
                thread.Start();
            }
            catch (Exception e)
            {

                Debug.LogWarning($"Could not link {subDirectory}, trying again...\n{e.Message}");
                try
                {
                    args = $"/c mklink /d {destPath}\\{subDirectory} {sourcePath}\\{subDirectory}";
                    var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                    thread.Start();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Attempt 2 failed.. attempting one last time\n{ex.Message}");

                    try
                    {
                        args = $"/c xcopy /s /y {sourcePath}\\{subDirectory} {destPath}\\{subDirectory}";
                        var thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                        thread.Start();
                        ClearConsole();
                    }
                    catch (Exception exx)
                    {
                        Debug.LogError($"Links failed. You do not have sufficient previliges to write to windows temporary files. Please contact your system administrator\n{exx.Message}");

                    }
                    //this.Close();
                }

            }
        }

        private void LaunchClient(string destPath)
        {
            try
            {
                string currentUnityVersion = Application.unityVersion;
                string editorPath = GetAppPath(Application.platform);
                string editorArgs = $"-DisableDirectoryMonitor -disable-assembly-updater -silent-crashes";
                string projectPath = $" -projectPath \"{destPath}\"";

                var thread = new Thread(delegate () { ExcuteCMD($"\"{editorPath}\"", editorArgs + projectPath); });
                //Debug.Log();

                thread.Start();
                //RemoveFromHub();
                ClearConsole();
            }
            catch (Exception e) { Debug.LogError($"Unable to read temporary files due to unsufficient User Priviliges. Please contact your system administrator. \nDetails: {e.Message}"); }
        }

        private string GetAppPath(RuntimePlatform currentPlatform)
        {
            switch (currentPlatform)
            {
                case RuntimePlatform.WindowsEditor:
                    return EditorApplication.applicationPath;
                case RuntimePlatform.OSXEditor:
                    return EditorApplication.applicationPath + "/Contents/MacOS/Unity";
                case RuntimePlatform.LinuxEditor:
                    return EditorApplication.applicationPath;
                default:
                    throw new NotImplementedException("Platform not supported!");
            }
        }

        private static void ClearClient(string destPath)
        {
            if (!Directory.Exists(destPath))
                return;

            Thread thread = null;
            try
            {
                string args = $"/c rd /s /q \"{destPath}\"";
                thread = new Thread(delegate () { ExcuteCMD("cmd", args); });
                thread?.Start();
            }
            catch (Exception e) { Debug.LogError($"Error resetting clients\n{e.Message}"); }
        }

        private static void PurgeAllClients()
        {
            try
            {
                var tmpPath = new DirectoryInfo( //Path.GetTempPath());
                $"{ clonesPath }");

                foreach (var d in tmpPath.EnumerateDirectories("*Client*"))
                {
                    //Debug.Log(d.FullName);
                    ClearClient(d.FullName);

                    //if all failed, run this from windows command prompt
                    //for / d % x in (% tmp %\*Client) do rd / s / q " % x"

                }
            }
            catch (Exception e) { Debug.LogError($"Error resetting clients\n{e.Message}"); }
        }


        private static void ClearConsole()
        {
            var logEntries = System.Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }
        static void ExcuteCMD(string prog, string args)
        {
            if (prog == null) return;
            try
            {
                Process process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo();
                bool isCleaningUp = args.StartsWith("/c rd");
                startInfo.WindowStyle = isCleaningUp? ProcessWindowStyle.Hidden : ProcessWindowStyle.Maximized;

                startInfo.FileName = prog;
                startInfo.Arguments = args;
                string tmp = prog + args;
                //Debug.LogError(tmp);
                process.StartInfo = startInfo;
                process.Start();

                process.WaitForExit();

                process.Close();
            }
            catch (Exception) { }
            finally { RemoveFromHub(); }
        }

        private IEnumerator Sleep(float timer)
        { yield return new WaitForSeconds(timer); }

        private enum Licence { Default, Full }
    }
}

