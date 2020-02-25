public class KeyPress
{
	public int frame { get; private set; }
	public float timestamp { get; private set; }
	public int keyCode { get; private set; }
	
	public long stopwatch { get; private set; }

	public KeyPress (int key, float time, long stop)
	{
		//this.frame = LowLevelInputManager.Instance.CurrentFrameIdx ();
		this.timestamp = time;
		this.stopwatch = stop;
	}
}