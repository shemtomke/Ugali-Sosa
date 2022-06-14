using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.MobileTools
{
	[RequireComponent(typeof(Button))]
	public class ClickableListItem : MonoBehaviour
	{
		#region Member Variables

		private Button uiButton;

		#endregion

		#region Properties

		public int							Index				{ get; set; }
		public object						Data				{ get; set; }
		public System.Action<int, object>	OnListItemClicked	{ get; set; }

		/// <summary>
		/// Gets the Button component attached to this GameObject
		/// </summary>
		private Button UIButton
		{
			get
			{
				if (uiButton == null)
				{
					uiButton = gameObject.GetComponent<Button>();
				}

				return uiButton;
			}
		}

		#endregion

		#region Unity Methods

		private void Start()
		{
			if (UIButton != null)
			{
				UIButton.onClick.AddListener(OnButtonClicked);
			}
			else
			{
				Debug.LogError("[ClickableListItem] There is no Button component on this GameObject.");
			}
		}

		#endregion

		#region Private Methods

		private void OnButtonClicked()
		{
			if (OnListItemClicked != null)
			{
				OnListItemClicked(Index, Data);
			}
			else
			{
				Debug.LogWarning("[ClickableListItem] OnListItemClicked has not been set on object " + gameObject.name);
			}
		}

		#endregion
	}
}
