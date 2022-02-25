using UnityEngine;
using UnityEngine.Scripting;
using Zenject;
using AudioBox.UI;

namespace AudioBox.ASF
{
	public class UIDoubleNote : UIEntity
	{
		[Preserve]
		public class Pool : MonoMemoryPool<RectTransform, Rect, UIDoubleNote>
		{
			protected override void Reinitialize(RectTransform _Container, Rect _Rect, UIDoubleNote _Item)
			{
				Vector2 pivot = _Container.pivot;
				
				_Item.RectTransform.SetParent(_Container, false);
				_Item.RectTransform.anchorMin        = pivot;
				_Item.RectTransform.anchorMax        = pivot;
				_Item.RectTransform.pivot            = pivot;
				_Item.RectTransform.anchoredPosition = _Rect.center;
				_Item.RectTransform.sizeDelta        = _Rect.size;
			}
		}
	}
}