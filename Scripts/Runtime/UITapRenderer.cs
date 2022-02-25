using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AudioBox.ASF
{
	public class UITapRenderer : ASFTrackContext<ASFTapClip, UITapNote>
	{
		[Inject] UITapNote.Pool m_ItemPool;

		readonly Dictionary<ASFTapClip, UITapNote> m_Items = new Dictionary<ASFTapClip, UITapNote>();

		public override void AddClip(ASFTapClip _Clip, Rect _Rect, Rect _View)
		{
			if (m_Items.ContainsKey(_Clip))
				return;
			
			UITapNote item = m_ItemPool.Spawn(RectTransform, _Rect);
			
			m_Items[_Clip] = item;
		}

		public override void RemoveClip(ASFTapClip _Clip, Rect _Rect, Rect _View)
		{
			if (!m_Items.ContainsKey(_Clip))
				return;
			
			UITapNote item = m_Items[_Clip];
			
			item.RectTransform.anchoredPosition = _Rect.center;
			item.RectTransform.sizeDelta        = _Rect.size;
			
			m_Items.Remove(_Clip);
			
			m_ItemPool.Despawn(item);
		}

		public override void ProcessClip(ASFTapClip _Clip, Rect _Rect, Rect _View)
		{
			if (!m_Items.ContainsKey(_Clip))
				return;
			
			UITapNote item = m_Items[_Clip];
			
			item.RectTransform.anchoredPosition = _Rect.center;
			item.RectTransform.sizeDelta        = _Rect.size;
		}
	}
}