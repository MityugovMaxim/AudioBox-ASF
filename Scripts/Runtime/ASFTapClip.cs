using System.Collections.Generic;

namespace AudioBox.ASF
{
	public class ASFTapClip : ASFClip
	{
		public float Position { get; set; }

		public ASFTapClip(double _Time, float _Position) : base(_Time, _Time)
		{
			Position = _Position;
		}

		public override object Serialize()
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			
			data["time"] = MinTime;
			
			return data;
		}

		public override void Deserialize(object _Data)
		{
			double time = (double)_Data;
			MinTime = time;
			MaxTime = time;
		}
	}
}