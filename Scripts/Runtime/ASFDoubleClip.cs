using System.Collections.Generic;
using AudioBox.Compression;

namespace AudioBox.ASF
{
	public class ASFDoubleClip : ASFClip
	{
		public ASFDoubleClip(double _Time) : base(_Time, _Time) { }

		public override object Serialize()
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			
			data["time"] = MinTime;
			
			return data;
		}

		public override void Deserialize(object _Data)
		{
			Dictionary<string, object> data = _Data as Dictionary<string, object>;
			
			if (data == null)
				return;
			
			double time = data.GetDouble("time");
			MinTime = time;
			MaxTime = time;
		}
	}
}