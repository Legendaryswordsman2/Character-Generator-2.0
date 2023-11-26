using UnityEngine;
using TMPro;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace BlazerTech
{
    [DefaultExecutionOrder(-100000)]
    public class LogManager : MonoBehaviour
    {
        public static LogManager Instance;

        public BTSettingsSO Settings { get; private set; }

        [Space]
        readonly List<LogData> totalLogs = new List<LogData>();
        readonly List<LogData> queuedLogs = new List<LogData>();
        readonly List<Log> logs = new List<Log>();

        [Space]

        [SerializeField] GameObject logConsole;
        [SerializeField] Scrollbar logConsoleScrollbar;
        LogConsole logConsoleComponent;
        [SerializeField] GameObject logConsoleContents;
        [SerializeField] ScrollRect logConsoleScrollRect;
        [SerializeField] GameObject background;
        [SerializeField] Transform SliderBottomPOS;
        [SerializeField] GameObject logPrefab;

        int logIndex;
        int baseLogsCount;

        Transform bottomListTransform;

        public event EventHandler OnDebugConsoleEnabled;
        public event EventHandler OnDebugConsoleDisabled;

        public event EventHandler OnDebugConsoleLogsCleared;

        void Awake()
        {
            Instance = this;

            Settings = BTSettingsSO.Get();

            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.color = Settings.backgroundColor;
            logConsoleComponent = logConsole.GetComponent<LogConsole>();

            logConsoleComponent.OnEnabled += OnLogConsoleEnabled;

            LogBaseInfo("Unity version: " + Application.unityVersion);

            LogBaseInfo("OS: " + SystemInfo.operatingSystem + " (" + SystemInfo.operatingSystemFamily + ")");

            LogBaseInfo("GPU: " + SystemInfo.graphicsDeviceName + " (Running " + SystemInfo.graphicsDeviceType + ")");

            LogBaseInfo("CPU: " + SystemInfo.processorType + " (" + SystemInfo.processorCount + " X " + SystemInfo.processorFrequency + " Mhz)");

            LogBaseInfo("RAM: " + SystemInfo.systemMemorySize + " MB | " + (float)SystemInfo.systemMemorySize / 1024 + " GB");

            LogBaseInfo("Current Directory: " + Directory.GetCurrentDirectory());

            for (int i = 0; i < Settings.logCap; i++)
            {
                Log log = Instantiate(logPrefab, logConsoleContents.transform).GetComponent<Log>();

                log.Init(Settings.fontSize);

                logs.Add(log);
            }

            var newGO = new GameObject();
            newGO.name = "Bottom POS";
            newGO.transform.SetParent(logConsoleContents.transform);
            RectTransform newGORectTransform = newGO.AddComponent<RectTransform>();
            newGORectTransform.sizeDelta = new Vector2(0, 1);
            bottomListTransform = newGO.transform;
            //bottomListTransform = Instantiate(new GameObject(), logConsoleContents.transform).transform;

            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        void Start()
        {
            SceneManager.sceneLoaded += OnNewSceneLoaded;
        }

        void OnDisable()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceived;
        }

        void Update()
        {
            if (Settings.activeInputSystem == ActiveInputSystem.OldInputSystem && Input.GetKeyDown(Settings.toggleConsoleKey))
                ToggleConsole();
        }

        void OnLogMessageReceived(string _condition, string stackTrace, LogType type)
        {
            string condition = "[" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + "] [" + type + "] " + _condition;

            if (totalLogs.Count >= Settings.logCap)
                totalLogs.RemoveAt(0);
            totalLogs.Add(new LogData(condition, stackTrace, type));

            UnityMainThread.wkr.AddJob(() =>
            {
                // Will run on main thread
                if (!logConsole.activeSelf)
                {
                    if (queuedLogs.Count >= Settings.logCap)
                        queuedLogs.RemoveAt(0);
                    queuedLogs.Add(new LogData(condition, stackTrace, type));
                    return;
                }

                switch (type)
                {
                    case LogType.Error:
                        LogError(condition, stackTrace);
                        break;
                    case LogType.Warning:
                        LogWarning(condition, stackTrace);
                        break;
                    case LogType.Log:
                        Log(condition, stackTrace);
                        break;
                    case LogType.Exception:
                        LogException(condition, stackTrace);
                        break;
                }
            });

        }

        void LogBaseInfo(string message)
        {
            Log log = Instantiate(logPrefab, logConsoleContents.transform).GetComponent<Log>();

            log.SetupBaseInfoLog(message);

            totalLogs.Add(new LogData(message, "", LogType.Log));

            baseLogsCount++;
        }
        void Log(string logMessage, string logDetails = "")
        {
            bool isAtBottom;

            if (SliderBottomPOS.position.y <= bottomListTransform.position.y)
                isAtBottom = true;
            else
                isAtBottom = false;

            bool logsfull = CheckLogCap();

            if (logsfull)
                logs[logs.Count - 1].SetupLog(logMessage, logDetails, LogType.Log);
            else
            {
                logs[logIndex].SetupLog(logMessage, logDetails, LogType.Log);
                logIndex++;

                if (isAtBottom)
                    StartCoroutine(GoToBottom());
            }
        }
        void LogWarning(string warningMessage, string warningDetails = "")
        {
            bool isAtBottom;

            if (SliderBottomPOS.position.y <= bottomListTransform.position.y)
                isAtBottom = true;
            else
                isAtBottom = false;

            bool logsfull = CheckLogCap();

            if (logsfull)
                logs[logs.Count - 1].SetupLog(warningMessage, warningDetails, LogType.Warning);
            else
            {
                logs[logIndex].SetupLog(warningMessage, warningDetails, LogType.Warning);
                logIndex++;

                if (isAtBottom)
                    StartCoroutine(GoToBottom());
            }
        }
        void LogError(string errorMessage, string errorDetails = "")
        {
            bool isAtBottom;

            if (SliderBottomPOS.position.y <= bottomListTransform.position.y)
                isAtBottom = true;
            else
                isAtBottom = false;

            bool logsfull = CheckLogCap();

            if (logsfull)
                logs[logs.Count - 1].SetupLog(errorMessage, errorDetails, LogType.Error);
            else
            {
                logs[logIndex].SetupLog(errorMessage, errorDetails, LogType.Error);
                logIndex++;

                if (isAtBottom)
                    StartCoroutine(GoToBottom());
            }
        }
        void LogException(string exception, string exceptionDetails = "")
        {
            bool isAtBottom;

            if (SliderBottomPOS.position.y <= bottomListTransform.position.y)
                isAtBottom = true;
            else
                isAtBottom = false;

            bool logsfull = CheckLogCap();

            if (logsfull)
                logs[logs.Count - 1].SetupLog(exception, exceptionDetails, LogType.Exception);
            else
            {
                logs[logIndex].SetupLog(exception, exceptionDetails, LogType.Exception);
                logIndex++;

                if (isAtBottom)
                    StartCoroutine(GoToBottom());
            }
        }

        IEnumerator GoToBottom()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(logConsole.GetComponent<RectTransform>());
            yield return new WaitForEndOfFrame();
            logConsoleScrollRect.normalizedPosition = new Vector2(0, 0);
        }
        bool CheckLogCap()
        {
            if (logIndex >= Settings.logCap)
            {
                logs[0].transform.SetSiblingIndex(logs.Count + 5);

                Log log = logs[0];

                logs.Remove(log);
                logs.Add(log);

                return true;
            }
            else
                return false;

        }

        void OnLogConsoleEnabled(object sender, EventArgs e)
        {
            for (int i = 0; i < queuedLogs.Count; i++)
            {
                switch (queuedLogs[i].type)
                {
                    case LogType.Error:
                        LogError(queuedLogs[i].condition, queuedLogs[i].stackTrace);
                        break;
                    case LogType.Warning:
                        LogWarning(queuedLogs[i].condition, queuedLogs[i].stackTrace);
                        break;
                    case LogType.Log:
                        Log(queuedLogs[i].condition, queuedLogs[i].stackTrace);
                        break;
                    case LogType.Exception:
                        LogException(queuedLogs[i].condition, queuedLogs[i].stackTrace);
                        break;
                }
            }

            queuedLogs.Clear();

            logConsoleScrollRect.normalizedPosition = new Vector2(0, 0);
        }

        void OnNewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (Settings.clearConsoleOnSceneChange)
                ClearConsole();
        }

        public void ToggleConsole()
        {
            logConsole.SetActive(!logConsole.activeSelf);
            background.SetActive(logConsole.activeSelf);

            if (logConsole.activeSelf)
                OnDebugConsoleEnabled?.Invoke(this, null);
            else
                OnDebugConsoleDisabled?.Invoke(this, null);
        }

        public void SetConsoleActive(bool value)
        {
            logConsole.SetActive(value);
            background.SetActive(value);

            if (value)
                OnDebugConsoleEnabled?.Invoke(this, EventArgs.Empty);
            else
                OnDebugConsoleDisabled?.Invoke(this, EventArgs.Empty);
        }

        public void ClearConsole()
        {
            queuedLogs.Clear();

            foreach (var log in logs)
            {
                log.gameObject.SetActive(false);
            }

            OnDebugConsoleLogsCleared?.Invoke(this, EventArgs.Empty);
        }

        void OnApplicationQuit()
        {
            if (Settings.logCap <= 0) return;

            string logsFolder = Application.persistentDataPath + "/logs";

            if (!Directory.Exists(logsFolder))
                Directory.CreateDirectory(logsFolder);

            DirectoryInfo d = new DirectoryInfo(logsFolder);

            List<FileInfo> logFiles = new List<FileInfo>();
            foreach (var logFile in d.GetFiles("*.txt"))
            {
                logFiles.Add(logFile);
            }


            while (logFiles.Count >= Settings.logFileCap)
            {
                logFiles[0].Delete();
                logFiles.RemoveAt(0);
            }

            TextWriter tw = new StreamWriter(Application.persistentDataPath + "/logs/log" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt");

            tw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + " / " + DateTime.Now);

            for (int i = 0; i < totalLogs.Count; i++)
            {
                if (i > baseLogsCount - 2)
                    tw.WriteLine(totalLogs[i].condition + "\n" + totalLogs[i].stackTrace);
                else
                    tw.WriteLine(totalLogs[i].condition);
            }

            tw.Close();
        }

        [Serializable]
        class LogData
        {
            public string condition;
            public string stackTrace;
            public LogType type;

            public LogData(string condition, string stackTrace, LogType type)
            {
                this.condition = condition;
                this.stackTrace = stackTrace;
                this.type = type;
            }
        }
    }
}
