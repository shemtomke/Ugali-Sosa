using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBG.MobileTools
{
	public static class MobileAdsUtils
	{
		#region Classes

		public class NetworkInfo
		{
			public string DefineSymbol		{ get; private set; }
			public string SdkNamespace		{ get; private set; }

			public NetworkInfo(string defineSymbol, string sdkNamespace = "")
			{
				DefineSymbol	= defineSymbol;
				SdkNamespace	= sdkNamespace;
			}
		}

		#endregion

		#region Member Variables

		// Ad network SDKs
		public static readonly NetworkInfo AdMobNetworkInfo			= new NetworkInfo("BBG_ADMOB", "GoogleMobileAds");
		public static readonly NetworkInfo UnityAdsNetworkInfo		= new NetworkInfo("BBG_UNITYADS", "UnityEngine.Monetization");

		/// <summary>
		/// Array of all the network infos
		/// </summary>
		public static readonly List<NetworkInfo> NetworkInfos = new List<NetworkInfo>()
		{
			AdMobNetworkInfo,
			UnityAdsNetworkInfo
		};

		#endregion

		#region Public Methods

		public static void DetectSDKs()
		{ 
			HashSet<string> existingNetworkIds = CheckNamespacesExists(NetworkInfos);

			List<string> androidDefineSymbols	= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android);
			List<string> iosDefineSymbols		= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS);

			bool anySdkExists = false;

			for (int i = 0; i < NetworkInfos.Count; i++)
			{
				NetworkInfo networkInfo = NetworkInfos[i];

				// Check if we found the indentifier for the sdk in the project
				bool sdkExists = existingNetworkIds.Contains(networkInfo.SdkNamespace);

				anySdkExists |= sdkExists;

				SyncScriptingDefineSymbols(sdkExists, androidDefineSymbols, iosDefineSymbols, networkInfo.DefineSymbol);
			}

			// Symbol for any sdk existing
			SyncScriptingDefineSymbols(anySdkExists, androidDefineSymbols, iosDefineSymbols, "BBG_MT_ADS");

			SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android, androidDefineSymbols);
			SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS, iosDefineSymbols);
		}

		public static void ClearNetworkInfo(NetworkInfo networkInfo)
		{
			List<string> androidDefineSymbols	= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android);
			List<string> iosDefineSymbols		= GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS);

			androidDefineSymbols.Remove(networkInfo.DefineSymbol);
			iosDefineSymbols.Remove(networkInfo.DefineSymbol);

			bool anyAdsEnabled = false;

			for (int i = 0; i < NetworkInfos.Count; i++)
			{
				NetworkInfo nInfo = NetworkInfos[i];

				if (androidDefineSymbols.Contains(nInfo.DefineSymbol) || iosDefineSymbols.Contains(nInfo.DefineSymbol))
				{
					anyAdsEnabled = true;
				}
			}

			if (!anyAdsEnabled)
			{
				SyncScriptingDefineSymbols(false, androidDefineSymbols, iosDefineSymbols, "BBG_MT_ADS");
			}

			SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.Android, androidDefineSymbols);
			SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup.iOS, iosDefineSymbols);
		}

		public static bool CheckAdNetwork(string adNetworkId)
		{
			switch (adNetworkId)
			{
				case MobileAdsSettings.AdMobNetworkId:
					#if BBG_ADMOB
					return true;
					#else
					return false;
					#endif
				case MobileAdsSettings.UnityAdsNetworkId:
					#if BBG_UNITYADS
					return true;
					#else
					return false;
					#endif
			}

			return false;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Adds or removes the scripting define symbols
		/// </summary>
		private static void SyncScriptingDefineSymbols(bool exists, List<string> androidDefineSymbols, List<string> iosDefineSymbols, string symbol)
		{
			if (exists)
			{
				if (!androidDefineSymbols.Contains(symbol))
				{
					androidDefineSymbols.Add(symbol);
				}

				if (!iosDefineSymbols.Contains(symbol))
				{
					iosDefineSymbols.Add(symbol);
				}
			}
			else
			{
				androidDefineSymbols.Remove(symbol);
				iosDefineSymbols.Remove(symbol);
			}
		}

		/// <summary>
		/// Checks if the given namespace string exists in the project and can be used
		/// </summary>
		private static HashSet<string> CheckNamespacesExists(List<NetworkInfo> networkInfos)
		{
			HashSet<string> existingIdentifiers = new HashSet<string>();

			// Get an array of all the assembiles that are currently compiled
			System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();

			for (int i = 0; i < assemblies.Length; i++)
			{
				System.Reflection.Assembly assembly = assemblies[i];

				// Get all the system types for the assembly
				System.Type[] types = assembly.GetTypes();

				for (int j = 0; j < types.Length; j++)
				{
					System.Type type = types[j];

					// Get the namespace string for the current type
					string typeNamespace = type.Namespace;

					// Check for a NetworkInfo that has either the same Namespace or Class name
					for (int k = 0; k < networkInfos.Count; k++)
					{
						NetworkInfo networkInfo = networkInfos[k];

						if (!string.IsNullOrEmpty(networkInfo.SdkNamespace) && networkInfo.SdkNamespace == typeNamespace)
						{
							if (!existingIdentifiers.Contains(typeNamespace))
							{
								existingIdentifiers.Add(typeNamespace);
							}

							break;
						}
					}
				}
			}

			// Namespace not found
			return existingIdentifiers;
		}

		/// <summary>
		/// Gets the Scripting Define Symbols in Player Settings
		/// </summary>
		private static List<string> GetScriptingDefineSymbols(UnityEditor.BuildTargetGroup buildTargetGroup)
		{
			return new List<string>(UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup).Split(';'));
		}

		/// <summary>
		/// Sets teh Scripting Define Symbols in Player Settings
		/// </summary>
		private static void SetScriptingDefineSymbols(UnityEditor.BuildTargetGroup buildTargetGroup, List<string> scriptingDefineSymbols)
		{
			string symbols = "";

			for (int i = 0; i < scriptingDefineSymbols.Count; i++)
			{
				if (i != 0)
				{
					symbols += ";";
				}

				symbols += scriptingDefineSymbols[i];
			}

			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, symbols);
		}

		#endregion
	}
}
