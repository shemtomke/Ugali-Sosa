using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.MobileTools
{
	public class GameDebugManager : MonoBehaviour
	{
		#region Inspector Variables

		[SerializeField] private bool					enableDebugging				= false;
		[SerializeField] private Canvas					canvas						= null;
		[Header("Tabs")]
		[SerializeField] private Transform				tabContainer				= null;
		[SerializeField] private TabItem				tabItemTemplate				= null;
		[SerializeField] private List<DebugConsoleItem> debugConsoleItems			= null;
		[SerializeField] private GameObject				debugConsoleItemContainer	= null;
		[Header("Logs")]
		[SerializeField] private Text					logText 					= null;
		[SerializeField] private bool					enableTextColor				= false;
		[SerializeField] private Color					warningColor				= Color.white;
		[SerializeField] private Color					errorColor					= Color.white;

		#endregion

		#region Member Variables

		private static GameDebugManager instance;

		private List<TabItem> tabItems;

		#endregion

		#region Properties

		private static GameDebugManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = FindObjectOfType<GameDebugManager>();

					if (instance != null)
					{
						instance.Initialize();
					}
				}

				return instance;
			}
		}

		#endregion

		#region Unity Methods

		private void Awake()
		{
			if (instance == null)
			{
				instance = this;

				Initialize();
			}
			else if (instance != this)
			{
				Debug.LogWarning("[GameDebugManager] There is more than one GameDebugManager instance in the scene.");
			}
		}

		private void Start()
		{
			if (instance == this)
			{
				CreateConsoleItems();
			}
		}

		private void OnDestroy()
		{
			Application.logMessageReceived -= UnityLogCallback;
		}

		#if UNITY_EDITOR
		private void OnValidate()
		{
			gameObject.name = string.Format("DebugManager [{0}]", enableDebugging ? "Enabled" : "Disabled");
		}
		#endif

		#endregion

		#region Public Methods

		/// <summary>
		/// Logs a message if enableDebugLogging is true
		/// </summary>
		public static void Log(string tag, string message)
		{
			if (Instance != null && Instance.enableDebugging)
			{
				string log = string.Format("[{0}] {1}", tag, message);
				
				Debug.Log(log);

				Instance.AddLogToText(log);
			}
		}

		/// <summary>
		/// Logs an warning message if enableDebugLogging is true
		/// </summary>
		public static void LogWarning(string tag, string message)
		{
			if (Instance != null && Instance.enableDebugging)
			{
				string log = string.Format("[{0}] {1}", tag, message);
				
				Debug.LogWarning(log);

				Instance.AddWarningLogToText(log);
			}
		}

		/// <summary>
		/// Logs a error message if enableDebugLogging is true
		/// </summary>
		public static void LogError(string tag, string message)
		{
			if (Instance != null && Instance.enableDebugging)
			{
				string log = string.Format("[{0}] {1}", tag, message);
				
				Debug.LogError(log);

				Instance.AddErrorLogToText(log);
			}
		}

		#endregion

		#region Private Methods

		private void Initialize()
		{
			if (enableDebugging)
			{
				canvas.gameObject.SetActive(true);

				tabItemTemplate.gameObject.SetActive(false);

				Application.logMessageReceived += UnityLogCallback;
			}
		}

		private void CreateConsoleItems()
		{
			tabItems = new List<TabItem>();

			CreateTabItem("Logs", 0);

			for (int i = 0; i < debugConsoleItems.Count; i++)
			{
				DebugConsoleItem consoleItem = debugConsoleItems[i];

				CreateTabItem(consoleItem.GetTabName(), i + 1);

				consoleItem.Initialize();

				consoleItem.gameObject.SetActive(false);
			}
		}

		private TabItem CreateTabItem(string text, int index)
		{
			TabItem tabItem = Instantiate(tabItemTemplate, tabContainer, false);

			tabItem.Setup(index, text);
			tabItem.OnTabSelected = OnTabSelected;
			tabItem.SetSelected(index == 0);

			tabItems.Add(tabItem);

			tabItem.gameObject.SetActive(true);

			return tabItem;
		}

		private void OnTabSelected(int index)
		{
			for (int i = 0; i < tabItems.Count; i++)
			{
				tabItems[i].SetSelected(i == index);
			}

			debugConsoleItemContainer.gameObject.SetActive(index > 0);

			if (index > 0)
			{
				index--;

				for (int i = 0; i < debugConsoleItems.Count; i++)
				{
					debugConsoleItems[i].gameObject.SetActive(i == index);
				}
			}
		}

		private void CreateMainContainer(GameObject root)
		{
			GameObject container = new GameObject("");
			Image bkgImage = root.GetComponent<Image>();
			bkgImage.color = new Color(0, 0, 0, 0.8f);
		}

		private void AddLogToText(string log)
		{
			if (logText != null)
			{
				if (!string.IsNullOrEmpty(logText.text))
				{
					logText.text += "\n";
				}

				logText.text += log;
			}
		}

		private void AddWarningLogToText(string log)
		{
			if (logText != null)
			{
				if (enableTextColor)
				{
					log = AddColorTags(log, warningColor);
				}

				AddLogToText(log);
			}
		}

		private void AddErrorLogToText(string log)
		{
			if (enableTextColor)
			{
				log = AddColorTags(log, errorColor);
			}

			AddLogToText(log);
		}

		private string AddColorTags(string text, Color color)
		{
			string colorStr = ColorUtility.ToHtmlStringRGBA(color);

			return string.Format("<color=#{0}>{1}</color>", colorStr, text);
		}

		private void UnityLogCallback(string condition, string stackTrace, LogType logType)
		{
			if (logType == LogType.Exception)
			{
				if (!string.IsNullOrEmpty(stackTrace))
				{
					stackTrace = stackTrace.Remove(stackTrace.Length - 1, 1);
				}

				AddErrorLogToText(condition + "\n" + stackTrace);
			}
		}

		#endregion
	}
}
