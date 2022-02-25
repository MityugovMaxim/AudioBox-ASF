using System;
using System.Collections.Generic;
using UnityEngine;

namespace AudioBox.ASF
{
	public abstract class ASFTrack
	{
		public abstract void Sample(double _Time, double _MinTime, double _MaxTime);

		protected static float TimeToPosition(double _Time, double _MinTime, double _MaxTime, float _MinPosition, float _MaxPosition)
		{
			double phase = ASFMath.Remap(_Time, _MinTime, _MaxTime);
			
			return (float)ASFMath.LerpUnclamped(_MinPosition, _MaxPosition, phase);
		}

		protected static double PositionToTime(float _Position, float _MinPosition, float _MaxPosition, double _MinTime, double _MaxTime)
		{
			double phase = ASFMath.Remap(_Position, _MinPosition, _MaxPosition);
			
			return ASFMath.LerpUnclamped(_MinTime, _MaxTime, phase);
		}

		protected static Rect GetTimeRect(ASFClip _Clip, Rect _Rect, double _MinTime, double _MaxTime)
		{
			float minPosition = TimeToPosition(_Clip.MinTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax);
			float maxPosition = TimeToPosition(_Clip.MaxTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax);
			
			return new Rect(
				_Rect.x,
				minPosition,
				_Rect.width,
				maxPosition - minPosition
			);
		}

		protected static Rect GetFullRect(ASFClip _Clip, Rect _Rect, double _MinTime, double _MaxTime, float _Padding = 0)
		{
			float minPosition = TimeToPosition(_Clip.MinTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax) - _Padding;
			float maxPosition = TimeToPosition(_Clip.MaxTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax) + _Padding;
			
			return new Rect(
				_Rect.x,
				minPosition,
				_Rect.width,
				maxPosition - minPosition
			);
		}

		protected static Rect GetCullRect(ASFClip _Clip, Rect _Rect, double _MinTime, double _MaxTime, float _Padding = 0)
		{
			float minPosition = TimeToPosition(_Clip.MinTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax) - _Padding;
			float maxPosition = TimeToPosition(_Clip.MaxTime, _MinTime, _MaxTime, _Rect.yMin, _Rect.yMax) + _Padding;
			
			minPosition = Mathf.Clamp(minPosition, _Rect.yMin, _Rect.yMax);
			maxPosition = Mathf.Clamp(maxPosition, _Rect.yMin, _Rect.yMax);
			
			return new Rect(
				_Rect.x,
				minPosition,
				_Rect.width,
				maxPosition - minPosition
			);
		}
	}

	public abstract class ASFTrack<T> : ASFTrack where T : ASFClip
	{
		public IReadOnlyList<T> Clips => m_Clips;

		readonly List<T> m_Clips = new List<T>();

		public void AddClip(T _Clip)
		{
			m_Clips.Add(_Clip);
		}

		public void RemoveClip(T _Clip)
		{
			m_Clips.Remove(_Clip);
		}

		protected virtual (int minIndex, int maxIndex) GetRange(double _MinTime, double _MaxTime)
		{
			int anchor = FindAnchor(_MinTime, _MaxTime);
			
			if (anchor < 0)
				return (anchor, anchor);
			
			int minIndex = FindMin(anchor, _MinTime);
			int maxIndex = FindMax(anchor, _MaxTime);
			
			return (minIndex, maxIndex);
		}

		int FindMin(int _Anchor, double _MinTime)
		{
			int index = _Anchor;
			while (index > 0)
			{
				ASFClip clip = Clips[index - 1];
				
				if (clip.MaxTime < _MinTime)
					break;
				
				index--;
			}
			return index;
		}

		int FindMax(int _Anchor, double _MaxTime)
		{
			int index = _Anchor;
			while (index < Clips.Count - 1)
			{
				ASFClip clip = Clips[index + 1];
				
				if (clip.MinTime > _MaxTime)
					break;
				
				index++;
			}
			return index;
		}

		int FindAnchor(double _MinTime, double _MaxTime)
		{
			int i = 0;
			int j = Clips.Count - 1;
			while (i <= j)
			{
				int k = (i + j) / 2;
				
				ASFClip clip = Clips[k];
				
				if (clip.MaxTime < _MinTime)
					i = k + 1;
				else if (clip.MinTime > _MaxTime)
					j = k - 1;
				else
					return k;
			}
			return -1;
		}
	}
}