using AudioBox.UI;
using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace AudioBox.ASF
{
	public class UITapNote : UIEntity
	{
		[Preserve]
		public class Pool : MonoMemoryPool<RectTransform, Rect, UITapNote>
		{
			protected override void Reinitialize(RectTransform _Container, Rect _Rect, UITapNote _Item)
			{
				Vector2 pivot = _Container.pivot;
				
				_Item.RectTransform.SetParent(_Container, false);
				_Item.RectTransform.anchorMin        = pivot;
				_Item.RectTransform.anchorMax        = pivot;
				_Item.RectTransform.pivot            = new Vector2(0.5f, 0.5f);
				_Item.RectTransform.anchoredPosition = _Rect.center;
				_Item.RectTransform.sizeDelta        = _Rect.size;
			}
		}
	}
}