using System;

public class TraitDrawingPaper : TraitCanvas
{
	public override bool PointFilter
	{
		get
		{
			return true;
		}
	}

	public override float Scale
	{
		get
		{
			return 2f;
		}
	}

	public override TraitPainter.Type CanvasType
	{
		get
		{
			return TraitPainter.Type.Paper;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (p.altAction)
		{
			p.TrySetAct("actPaint", delegate()
			{
				EClass.ui.AddLayer<LayerPixelPaint>().SetCanvas(this);
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}
}
