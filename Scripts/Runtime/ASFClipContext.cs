using AudioBox.UI;

namespace AudioBox.ASF
{
	public abstract class ASFClipContext<TClip> : UIEntity where TClip : ASFClip
	{
		public abstract TClip Clip { get; protected set; }

		public abstract void Setup(TClip _Clip);
	}
}