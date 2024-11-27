using System;

public class TraitPhoto : TraitCanvas
{
	public override TraitPainter.Type CanvasType
	{
		get
		{
			return TraitPainter.Type.Camera;
		}
	}
}
