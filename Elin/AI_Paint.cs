using System.Collections.Generic;

public class AI_Paint : AIAct
{
	public TraitPainter painter;

	public TraitCanvas canvas;

	public byte[] data;

	public bool IsValidTarget()
	{
		return true;
	}

	public override IEnumerable<Status> Run()
	{
		bool isCamera = painter.PaintType == TraitPainter.Type.Camera;
		if (painter.ExistsOnMap)
		{
			owner.LookAt(painter.owner);
		}
		Progress_Custom seq = new Progress_Custom
		{
			canProgress = () => IsValidTarget(),
			onProgressBegin = delegate
			{
			},
			onProgress = delegate
			{
				owner.PlaySound(isCamera ? "camera" : painter.owner.material.GetSoundImpact());
				painter.owner.renderer.PlayAnime(AnimeID.Shiver);
			},
			onProgressComplete = delegate
			{
				Thing thing = canvas.owner.Split(1);
				thing.c_textureData = data;
				thing.isModified = true;
				EClass.pc.TryHoldCard(thing);
			}
		}.SetDuration(isCamera ? 2 : 10);
		yield return Do(seq);
	}
}
