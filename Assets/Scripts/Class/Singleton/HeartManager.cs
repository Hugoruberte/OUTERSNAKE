using My.Tools;
using My.Events;

public class HeartManager : Singleton<HeartManager>
{
	public _Transform heart = new _Transform();
	private _Transform cache = new _Transform();
	private _Transform previous = new _Transform();

	public _Transform_TransformEvent onRotate;

	protected override void OnAwake()
	{
		this.previous.Copy(this.cache);
		this.cache.Copy(this.heart);
		this.heart.onRotate += this.OnRotateEventLauncher;
	}

	private void OnRotateEventLauncher()
	{
		this.previous.Copy(this.cache);
		this.cache.Copy(this.heart);

		this.onRotate?.Invoke(this.previous, this.heart);
	}
}
