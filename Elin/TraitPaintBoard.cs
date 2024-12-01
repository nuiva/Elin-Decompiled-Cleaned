using System.Collections.Generic;

public class TraitPaintBoard : TraitItem
{
	public PaintPosition paintPos;

	public override bool IsHomeItem => true;

	public override bool ShowContextOnPick => true;

	public override bool RenderExtra
	{
		get
		{
			if (owner.refVal != 0)
			{
				if (!owner.IsInstalled)
				{
					return owner.isRoofItem;
				}
				return true;
			}
			return false;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (!EClass.debug.enable && (!EClass._zone.IsPCFaction || !p.altAction))
		{
			return;
		}
		p.TrySetAct("actChangePaint", delegate
		{
			List<int> list = new List<int>();
			foreach (SourceThing.Row row in EClass.sources.things.rows)
			{
				if (!row.isOrigin && row.model.trait is TraitPaint)
				{
					list.Add(row._tiles[0]);
				}
			}
			int num = ((owner.refVal != 0) ? list.IndexOf(owner.refVal) : 0);
			UIContextMenu uIContextMenu = EClass.ui.CreateContextMenuInteraction();
			uIContextMenu.AddSlider("sliderPaint", (float a) => a.ToString() ?? "", num, delegate(float b)
			{
				owner.refVal = list[(int)b];
				paintPos = null;
			}, 0f, list.Count - 1, isInt: true, hideOther: false);
			uIContextMenu.Show();
			return false;
		}, owner);
	}

	public override void OnRenderExtra(RenderParam p)
	{
		if (owner != EClass.pc.held)
		{
			if (paintPos == null)
			{
				paintPos = EClass.setting.render.paintPos.TryGetValue(owner.id) ?? EClass.setting.render.paintPos.FirstItem();
			}
			int num = ((!owner.flipX) ? 1 : (-1)) * ((!paintPos.flip) ? 1 : (-1));
			p.x += paintPos.pos.x * (float)num;
			p.y += paintPos.pos.y;
			p.z += paintPos.pos.z;
			p.tile = owner.refVal * num;
			p.matColor = 104025f;
			EClass.core.refs.renderers.obj_paint.Draw(p);
		}
	}
}
