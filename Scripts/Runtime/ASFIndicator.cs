using AudioBox.UI;
using UnityEngine;

namespace AudioBox.ASF
{
	public class ASFIndicator : UIEntity
	{
		public float Value
		{
			get => m_Value;
			set
			{
				if (Mathf.Approximately(m_Value, value))
					return;
				
				m_Value = value;
				
				ProcessValue();
			}
		}

		float m_Value;

		protected override void Awake()
		{
			base.Awake();
			
			ProcessValue();
		}

		void ProcessValue()
		{
			Vector2 size = RectTransform.sizeDelta;
			size.x                  = Mathf.Max(10, 600 * m_Value);
			RectTransform.sizeDelta = size;
		}
	}
}