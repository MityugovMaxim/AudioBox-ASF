using System.Linq;
using AudioBox.UI;
using UnityEngine;

namespace AudioBox.ASF
{
	public class ASFRenderer : UIEntity
	{
		[SerializeField] AudioSource    m_AudioSource;
		[SerializeField] double         m_MinTime;
		[SerializeField] double         m_MaxTime;
		[SerializeField] ASFIndicator[] m_Indicators;

		int     m_MinSamples;
		int     m_MaxSamples;
		int     m_LatencySamples;
		float[] m_Data;

		protected override void Awake()
		{
			base.Awake();
			
			AudioClip audioClip = m_AudioSource.clip;
			
			m_MinSamples = (int)(m_MinTime * audioClip.frequency);
			m_MaxSamples = (int)(m_MaxTime * audioClip.frequency);
			
			m_Data = new float[(m_MaxSamples - m_MinSamples) / m_Indicators.Length];
			
			Sample();
		}

		void LateUpdate()
		{
			Sample();
		}

		void Sample()
		{
			int time = m_AudioSource.timeSamples;
			for (int i = m_Indicators.Length - 1, j = 0; i >= 0; i--, j++)
			{
				int   offset = m_MinSamples + time + m_Data.Length * j;
				float value  = 0;
				if (offset >= 0 && offset < m_AudioSource.clip.samples)
				{
					m_AudioSource.clip.GetData(m_Data, offset);
					value = m_Data.Max(Mathf.Abs);
				}
				m_Indicators[i].Value = value;
			}
		}
	}
}