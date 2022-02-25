using UnityEngine;
using UnityEngine.Scripting;
using Zenject;

namespace AudioBox.ASF
{
	public class ASFDoubleTrack : ASFTrack<ASFDoubleClip>
	{
		[Preserve]
		public class Factory : PlaceholderFactory<ASFDoubleTrack> { }

		UIDoubleRenderer Context { get; }

		float Padding => Context.GetLocalRect().width / 4 * 0.5f;

		int m_MinIndex = -1;
		int m_MaxIndex = -1;

		[Inject]
		public ASFDoubleTrack(UIDoubleRenderer _Context)
		{
			Context = _Context;
		}

		protected override (int minIndex, int maxIndex) GetRange(double _MinTime, double _MaxTime)
		{
			Rect rect = Context.GetLocalRect();
			
			float padding = Padding;
			
			double minTime = PositionToTime(rect.yMin - padding, rect.yMin, rect.yMax, _MinTime, _MaxTime);
			double maxTime = PositionToTime(rect.yMax + padding, rect.yMin, rect.yMax, _MinTime, _MaxTime);
			
			return base.GetRange(minTime, maxTime);
		}

		public override void Sample(double _Time, double _MinTime, double _MaxTime)
		{
			(int minIndex, int maxIndex) = GetRange(_MinTime, _MaxTime);
			
			for (int i = Mathf.Max(0, minIndex); i <= maxIndex; i++)
				Clips[i].Sample(_Time);
			
			Reposition(minIndex, maxIndex, _MinTime, _MaxTime);
		}

		void Reposition(int _MinIndex, int _MaxIndex, double _MinTime, double _MaxTime)
		{
			if (Context == null)
				return;
			
			Rect rect = Context.GetLocalRect();
			
			float padding = Padding;
			
			for (int i = Mathf.Max(0, m_MinIndex); i <= m_MaxIndex; i++)
			{
				if (i < _MinIndex || i > _MaxIndex)
					Context.RemoveClip(Clips[i], GetFullRect(Clips[i], rect, _MinTime, _MaxTime, padding), GetTimeRect(Clips[i], rect, _MinTime, _MaxTime));
			}
			
			for (int i = Mathf.Max(0, _MinIndex); i <= _MaxIndex; i++)
			{
				if (i < m_MinIndex || i > m_MaxIndex)
					Context.AddClip(Clips[i], GetFullRect(Clips[i], rect, _MinTime, _MaxTime, padding), GetTimeRect(Clips[i], rect, _MinTime, _MaxTime));
				else
					Context.ProcessClip(Clips[i], GetFullRect(Clips[i], rect, _MinTime, _MaxTime, padding), GetTimeRect(Clips[i], rect, _MinTime, _MaxTime));
			}
			
			m_MinIndex = _MinIndex;
			m_MaxIndex = _MaxIndex;
		}
	}
}