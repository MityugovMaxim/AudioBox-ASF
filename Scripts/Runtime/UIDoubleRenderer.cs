using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace AudioBox.ASF
{
	public class UIDoubleRenderer : ASFTrackContext<ASFDoubleClip, UIDoubleNote>
	{
		[Inject] UIDoubleNote.Pool m_ItemPool;

		readonly Dictionary<ASFDoubleClip, UIDoubleNote> m_Items = new Dictionary<ASFDoubleClip, UIDoubleNote>();

		public override void AddClip(ASFDoubleClip _Clip, Rect _Rect, Rect _View)
		{
			if (m_Items.ContainsKey(_Clip))
				return;
			
			UIDoubleNote item = m_ItemPool.Spawn(RectTransform, _Rect);
			
			m_Items[_Clip] = item;
		}

		public override void RemoveClip(ASFDoubleClip _Clip, Rect _Rect, Rect _View)
		{
			if (!m_Items.ContainsKey(_Clip))
				return;
			
			UIDoubleNote item = m_Items[_Clip];
			
			item.RectTransform.anchoredPosition = _Rect.center;
			item.RectTransform.sizeDelta        = _Rect.size;
			
			m_Items.Remove(_Clip);
			
			m_ItemPool.Despawn(item);
		}

		public override void ProcessClip(ASFDoubleClip _Clip, Rect _Rect, Rect _View)
		{
			if (!m_Items.ContainsKey(_Clip))
				return;
			
			UIDoubleNote item = m_Items[_Clip];
			
			item.RectTransform.anchoredPosition = _Rect.center;
			item.RectTransform.sizeDelta        = _Rect.size;
		}
	}
}