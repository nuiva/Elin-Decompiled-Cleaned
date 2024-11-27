using System;

public class TraitCamera : TraitPainter
{
	public override TraitPainter.Type PaintType
	{
		get
		{
			return TraitPainter.Type.Camera;
		}
	}
}
