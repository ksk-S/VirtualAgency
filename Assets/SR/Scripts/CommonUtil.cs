using System;

namespace CommonUtil
{
	
	public enum Status : int
	{
		LIVE, STREAM
	}
	
	public enum RenderingMode{
		Spherical, Panoramic
	}
	
	public enum CalibrationMode{
		None, Duplicate, Horizontal, Vertical, Overlay, Frame, FramedBlack
	}

    public enum HybridMode
    {
        None, Horizontal, Vertical
    }

	public enum ScreenMode{
		Live=0, Stream, StreamAlt
	}

	public class DistortionPram{
		public float dk1;
		public float dk2;
		public float dk3;
		public float dk4;
		
		public DistortionPram(float p1, float p2, float p3, float p4)
		{
			dk1 = p1;
			dk2 = p2;
			dk3 = p3;
			dk4 = p4;
		}
	}

	public enum HeadRecordMode{
		None, Record, Replay
	}
}

