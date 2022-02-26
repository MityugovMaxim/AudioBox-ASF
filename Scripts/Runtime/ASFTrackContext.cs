using System.Collections.Generic;
using AudioBox.UI;
using AudioBox.Logging;
using UnityEngine;

namespace AudioBox.ASF
{
	public abstract class ASFTrackContext<TClip> : UIEntity where TClip : ASFClip
	{
		readonly Dictionary<TClip, ASFClipContext<TClip>> m_Views = new Dictionary<TClip, ASFClipContext<TClip>>();

		public abstract void AddClip(TClip _Clip, Rect _ClipRect, Rect _ViewRect);

		public abstract void RemoveClip(TClip _Clip, Rect _ClipRect, Rect _ViewRect);

		public abstract void ProcessClip(TClip _Clip, Rect _ClipRect, Rect _ViewRect);

		protected bool AddView(TClip _Clip, ASFClipContext<TClip> _View)
		{
			if (_Clip == null)
			{
				Log.Error(this, "Add view failed. Clip is null.");
				return false;
			}
			
			if (_View == null)
			{
				Log.Error(this, "Add view failed. View is null for clip '{0}'.", _Clip);
				return false;
			}
			
			if (m_Views.ContainsKey(_Clip))
			{
				Log.Error(this, "Add view failed. View for clip '{0}' already exists.", _Clip);
				return false;
			}
			
			m_Views[_Clip] = _View;
			
			return true;
		}

		protected bool RemoveView(TClip _Clip)
		{
			if (_Clip == null)
			{
				Log.Error(this, "Remove view failed. Clip is null.");
				return false;
			}
			
			if (!m_Views.ContainsKey(_Clip))
			{
				Log.Error(this, "Remove view failed. View for clip '{0}' not found.", _Clip);
				return false;
			}
			
			m_Views.Remove(_Clip);
			
			return true;
		}

		protected bool ContainsView(TClip _Clip)
		{
			return m_Views.ContainsKey(_Clip) && m_Views[_Clip] != null;
		}

		protected ASFClipContext<TClip> GetView(TClip _Clip)
		{
			if (_Clip == null)
			{
				Log.Error(this, "Get view failed. Clip is null.");
				return null;
			}
			
			if (!m_Views.ContainsKey(_Clip))
			{
				Log.Error(this, "Get view failed. View for clip '{0}' not found.", _Clip);
				return null;
			}
			
			return m_Views[_Clip];
		}
	}
}