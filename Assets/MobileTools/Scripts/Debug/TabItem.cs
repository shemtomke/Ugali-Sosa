using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBG.MobileTools
{
	[RequireComponent(typeof(Button))]
	public class TabItem : MonoBehaviour
	{
		#region Inspector Variables
		
		[SerializeField] private Text	tabText			= null;
		[SerializeField] private Image	bkgImage		= null;
		[SerializeField] private Color	normalColor		= Color.white;
		[SerializeField] private Color	selectedColor	= Color.white;
		
		#endregion // Inspector Variables

		#region Properties
		
		public int					Index			{ get; private set; }
		public bool					IsSelected		{ get; private set; }
		public System.Action<int>	OnTabSelected	{ get; set; }
		
		#endregion // Properties

		#region Public Methods
		
		public void Setup(int index, string text)
		{
			Index = index;
			tabText.text = text;

			gameObject.GetComponent<Button>().onClick.AddListener(OnClicked);
		}

		public void SetSelected(bool isSelected)
		{
			IsSelected = isSelected;
			bkgImage.color = isSelected ? selectedColor : normalColor;
		}

		#endregion // Public Methods

		#region Private Methods
		
		private void OnClicked()
		{
			if (!IsSelected)
			{
				OnTabSelected?.Invoke(Index);
			}
		}
		
		#endregion // Private Methods
	}
}
