using System.Collections.Generic;
using AudioBox.Compression;

namespace AudioBox.ASF
{
	public class ASFSwipeClip : ASFClip
	{
		public enum Direction
		{
			Left,
			Right,
			Up,
			Down,
		}

		public Direction Type { get; set; }

		public ASFSwipeClip(double _Time, Direction _Type) : base(_Time, _Time)
		{
			Type = _Type;
		}

		public override object Serialize()
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			
			data["time"] = MinTime;
			data["type"] = (int)Type;
			
			return data;
		}

		public override void Deserialize(object _Data)
		{
			IDictionary<string, object> data = _Data as IDictionary<string, object>;
			
			if (data == null)
				return;
			
			double time = data.GetDouble("time");
			MinTime = time;
			MaxTime = time;
			Type    = data.GetEnum<Direction>("type");
		}
	}
}