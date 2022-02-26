using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AudioBox.Compression;

namespace AudioBox.ASF
{
	public class ASFHoldClip : ASFClip
	{
		public class Key
		{
			public double Time     { get; set; }
			public float  Position { get; set; }

			public Key() { }

			public Key(double _Time, float _Position)
			{
				Time     = _Time;
				Position = _Position;
			}
		}

		public List<Key> Keys { get; }

		public ASFHoldClip(double _MinTime, double _MaxTime) : base(_MinTime, _MaxTime)
		{
			Keys = new List<Key>()
			{
				new Key(_MinTime, 0.5f),
				new Key(_MaxTime, 0.5f),
			};
		}

		public override object Serialize()
		{
			IDictionary<string, object> data = new Dictionary<string, object>();
			
			data["min_time"] = MinTime;
			data["max_time"] = MaxTime;
			
			IList keysData = new List<object>();
			
			foreach (Key key in Keys)
			{
				if (key == null)
					continue;
				
				IDictionary<string, object> keyData = new Dictionary<string, object>();
				
				keyData["time"]     = key.Time;
				keyData["position"] = key.Position;
				
				keysData.Add(keyData);
			}
			
			data["keys"] = keysData;
			
			return data;
		}

		public override void Deserialize(object _Data)
		{
			IDictionary<string, object> data = _Data as IDictionary<string, object>;
			
			if (data == null)
				return;
			
			MinTime = data.GetDouble("min_time");
			MaxTime = data.GetDouble("max_time");
			
			IList keysData = data.GetList("keys");
			foreach (IDictionary<string, object> keyData in keysData.Cast<IDictionary<string, object>>())
			{
				if (keyData == null)
					continue;
				
				Key key = new Key(
					keyData.GetDouble("time"),
					keyData.GetFloat("position")
				);
				
				Keys.Add(key);
			}
		}
	}
}