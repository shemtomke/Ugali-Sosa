using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace BBG.MobileTools
{
	public static class EditorUtilities
	{
		/// <summary>
		/// Checks if the given namespace string exists in the project and can be used
		/// </summary>
		public static bool CheckNamespacesExists(string ns)
		{
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

					if (ns == type.Namespace)
					{
						return true;
					}
				}
			}

			// Namespace not found
			return false;
		}
		/// <summary>
		/// Checks if the given namespace string exists in the project and can be used
		/// </summary>
		public static bool CheckClassExists(string ns, string className)
		{
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

					if (ns == type.Namespace && className == type.Name)
					{
						return true;
					}
				}
			}

			// Namespace not found
			return false;
		}

		/// <summary>
		/// Syncs the scripting define symbols by either adding or removing it
		/// </summary>
		public static void SyncScriptingDefineSymbols(string defineSymbol, bool enableDefineSymbol)
		{
			SyncScriptingDefineSymbols(BuildTargetGroup.Android, defineSymbol, enableDefineSymbol);
			SyncScriptingDefineSymbols(BuildTargetGroup.iOS, defineSymbol, enableDefineSymbol);
		}

		/// <summary>
		/// Adds or removes the scripting define symbols
		/// </summary>
		public static void SyncScriptingDefineSymbols(BuildTargetGroup buildTarget, string defineSymbol, bool enableDefineSymbol)
		{
			List<string> defineSymbols = new List<string>(PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Split(';'));

			bool isDefineSymbolsDirty = false;

			if (enableDefineSymbol && !defineSymbols.Contains(defineSymbol))
			{
				defineSymbols.Add(defineSymbol);

				isDefineSymbolsDirty = true;
			}
			else if (!enableDefineSymbol && defineSymbols.Contains(defineSymbol))
			{
				defineSymbols.Remove(defineSymbol);

				isDefineSymbolsDirty = true;
			}

			if (isDefineSymbolsDirty)
			{
				string symbols = "";

				for (int i = 0; i < defineSymbols.Count; i++)
				{
					if (i != 0)
					{
						symbols += ";";
					}

					symbols += defineSymbols[i];
				}

				PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTarget, symbols);
			}
		}
	}
}
