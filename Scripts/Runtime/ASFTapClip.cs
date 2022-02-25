using System.Collections;
using System.Collections.Generic;
using AudioBox.Compression;

namespace AudioBox.ASF
{
	public class ASFHoldKey
	{
		public double Time     { get; set; }
		public float  Position { get; set; }

		public ASFHoldKey() { }

		public ASFHoldKey(double _Time, float _Position)
		{
			Time     = _Time;
			Position = _Position;
		}
	}

	public class ASFHoldClip : ASFClip
	{
		public List<ASFHoldKey> Keys { get; }

		public ASFHoldClip(double _MinTime, double _MaxTime, params ASFHoldKey[] _Keys) : base(_MinTime, _MaxTime)
		{
			Keys = new List<ASFHoldKey>(_Keys);
		}

		public override object Serialize()
		{
			IList data = new List<object>();
			foreach (ASFHoldKey key in Keys)
			{
				Dictionary<string, object> keyData = new Dictionary<string, object>();
				keyData["time"]     = key.Time;
				keyData["position"] = key.Position;
				data.Add(keyData);
			}
			return data;
		}

		public override void Deserialize(object _Data)
		{
			IList data = _Data as IList;
			
			if (data == null)
				return;
			
			foreach (object entry in data)
			{
				Dictionary<string, object> keyData = entry as Dictionary<string, object>;
				
				if (keyData == null)
					continue;
				
				ASFHoldKey key = new ASFHoldKey();
				
				key.Time     = keyData.GetDouble("time");
				key.Position = keyData.GetFloat("position");
				
				Keys.Add(key);
			}
		}
	}

	public class ASFDoubleClip : ASFClip
	{
		public ASFDoubleClip(double _Time) : base(_Time, _Time) { }

		public override object Serialize()
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			
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

	public class ASFTapClip : ASFClip
	{
		public float Position { get; }

		public ASFTapClip(double _Time, float _Position) : base(_Time, _Time)
		{
			Position = _Position;
		}

		public override object Serialize()
		{
			return MinTime;
		}

		public override void Deserialize(object _Data)
		{
			double time = (double)_Data;
			MinTime = time;
			MaxTime = time;
		}
	}
}