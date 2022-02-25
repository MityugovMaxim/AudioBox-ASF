using System;

namespace AudioBox.ASF
{
	public static class ASFMath
	{
		public static double Clamp(double _Value, double _Min, double _Max)
		{
			return _Value < _Min ? _Min : _Value > _Max ? _Max : _Value;
		}

		public static double Clamp01(double _Value)
		{
			return _Value < 0d ? 0d : _Value > 1 ? 1d : _Value;
		}

		public static double Lerp(double _A, double _B, double _Time)
		{
			return _A + (_B - _A) * Clamp01(_Time);
		}

		public static double LerpUnclamped(double _A, double _B, double _Time)
		{
			return _A + (_B - _A) * _Time;
		}

		public static double Remap(double _Value, double _A, double _B)
		{
			return Math.Abs(_A - _B) > double.Epsilon * 2 ? (_Value - _A) / (_B - _A) : 0d;
		}

		public static double RemapClamped(double _Value, double _A, double _B)
		{
			return Clamp(Remap(_Value, _A, _B), 0d, 1d);
		}

		public static double Remap(double _Value, double _A0, double _B0, double _A1, double _B1)
		{
			return Math.Abs(_A0 - _B0) > double.Epsilon * 2 ? _A1 + (_Value - _A0) * (_B1 - _A1) / (_B0 - _A0) : 0d;
		}

		public static double RemapClamped(double _Value, double _A0, double _B0, double _A1, double _B1)
		{
			return Clamp(Remap(_Value, _A0, _B0, _A1, _B1), _A1, _B1);
		}
	}
}