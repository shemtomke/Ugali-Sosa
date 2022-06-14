using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BBG.MobileTools
{
	public static class Utils
	{
		public static string SaveFolderPath { get { return Application.persistentDataPath + "/SaveFiles"; } }

		public static void SaveToFile(object data, string folderPath, string filename)
		{
			// Create the save filder if it does not exist
			if (!System.IO.Directory.Exists(folderPath))
			{
				System.IO.Directory.CreateDirectory(folderPath);
			}

			string saveFilePath = folderPath + "/" + filename + ".json";
			string fileContents = EncryptDecrypt(ConvertToJsonString(data), 52486); // Encrypt using a random key

			try
			{
				System.IO.File.WriteAllText(saveFilePath, fileContents);
			}
			catch (System.Exception ex)
			{
				Debug.LogError("An exception occured while saving to " + saveFilePath);
				Debug.LogException(ex);
			}
		}

		public static JSONNode LoadSaveFile(string folderPath, string filename)
		{
			string saveFilePath = folderPath + "/" + filename + ".json";

			if (System.IO.File.Exists(saveFilePath))
			{
				string fileContents = EncryptDecrypt(System.IO.File.ReadAllText(saveFilePath), 52486);

				return JSON.Parse(fileContents);
			}

			return null;
		}

		public static string ConvertToJsonString(object data, bool addQuoteEscapes = false)
		{
			string jsonString = "";
			
			if (data is IDictionary)
			{
				string dictionaryItems = "";

				foreach (DictionaryEntry item in (data as IDictionary))
				{
					if (!string.IsNullOrEmpty(dictionaryItems))
					{
						dictionaryItems += ",";
					}

					if (addQuoteEscapes)
					{
						dictionaryItems += string.Format("\\\"{0}\\\":{1}", item.Key, ConvertToJsonString(item.Value, addQuoteEscapes));
					}
					else
					{
						dictionaryItems += string.Format("\"{0}\":{1}", item.Key, ConvertToJsonString(item.Value, addQuoteEscapes));
					}
				}

				jsonString += "{" + dictionaryItems + "}";
			}
			else if (data is IList)
			{
				IList list = data as IList;
				
				jsonString += "[";
				
				for (int i = 0; i < list.Count; i++)
				{
					if (i != 0)
					{
						jsonString += ",";
					}
					
					jsonString += ConvertToJsonString(list[i], addQuoteEscapes);
				}
				
				jsonString += "]";
			}
			else if (data is string || data is char)
			{
				// If the data is a string then we need to inclose it in quotation marks
				if (addQuoteEscapes)
				{
					jsonString += "\\\"" + data + "\\\"";
				}
				else
				{
					jsonString += "\"" + data + "\"";
				}
			}
			else if (data is bool)
			{
				jsonString += (bool)data ? "true" : "false";
			}
			else
			{
				// Else just return what ever data is as a string
				jsonString += data.ToString();
			}
			
			return jsonString;
		}

		public static string EncryptDecrypt(string inText, int key)
		{
			string outText = "";

			for (int i = 0; i < inText.Length; i++)
			{
				char c = inText[i];

				c = (char)(c ^ key);

				outText += c;
			}

			return outText;
		}

		public static void SwapValue<T>(ref T value1, ref T value2)
		{
			T temp = value1;
			value1 = value2;
			value2 = temp;
		}

		public static float EaseOut(float t)
		{
			return 1.0f - (1.0f - t) * (1.0f - t) * (1.0f - t);
		}

		public static float EaseIn(float t)
		{
			return t * t * t;
		}
	}
}
