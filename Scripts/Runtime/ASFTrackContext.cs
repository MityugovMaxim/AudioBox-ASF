using System.Collections.Generic;
using AudioBox.UI;
using AudioBox.Logging;
using UnityEngine;

namespace AudioBox.ASF
{
	public abstract class ASFTrackContext<TClip, TView> : UIEntity where TClip : ASFClip where TView : UIEntity
	{
		readonly Dictionary<TClip, TView> m_Views = new Dictionary<TClip, TView>();

		public abstract void AddClip(TClip _Clip, Rect _Rect, Rect _View);

		public abstract void RemoveClip(TClip _Clip, Rect _Rect, Rect _View);

		public abstract void ProcessClip(TClip _Clip, Rect _Rect, Rect _View);

		protected bool AddView(TClip _Clip, TView _View)
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

		protected TView GetView(TClip _Clip)
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