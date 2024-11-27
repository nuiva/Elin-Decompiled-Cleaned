using System;
using System.Collections.Generic;
using UnityEngine;

public class AI_Paint : AIAct
{
	public bool IsValidTarget()
	{
		return true;
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		bool isCamera = this.painter.PaintType == TraitPainter.Type.Camera;
		if (this.painter.ExistsOnMap)
		{
			this.owner.LookAt(this.painter.owner);
		}
		Progress_Custom progress_Custom = new Progress_Custom();
		progress_Custom.canProgress = (() => this.IsValidTarget());
		progress_Custom.onProgressBegin = delegate()
		{
		};
		progress_Custom.onProgress = delegate(Progress_Custom p)
		{
			this.owner.PlaySound(isCamera ? "camera" : this.painter.owner.material.GetSoundImpact(null), 1f, true);
			this.painter.owner.renderer.PlayAnime(AnimeID.Shiver, default(Vector3), false);
		};
		progress_Custom.onProgressComplete = delegate()
		{
			Thing thing = this.canvas.owner.Split(1);
			thing.c_textureData = this.data;
			thing.isModified = true;
			EClass.pc.TryHoldCard(thing, -1, false);
		};
		Progress_Custom seq = progress_Custom.SetDuration(isCamera ? 2 : 10, 2);
		yield return base.Do(seq, null);
		yield break;
	}

	public TraitPainter painter;

	public TraitCanvas canvas;

	public byte[] data;
}
