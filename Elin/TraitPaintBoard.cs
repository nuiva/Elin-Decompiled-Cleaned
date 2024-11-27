using System;
using System.Collections.Generic;

public class TraitPaintBoard : TraitItem
{
	public override bool IsHomeItem
	{
		get
		{
			return true;
		}
	}

	public override bool ShowContextOnPick
	{
		get
		{
			return true;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (EClass.debug.enable || (EClass._zone.IsPCFaction && p.altAction))
		{
			p.TrySetAct("actChangePaint", delegate()
			{
				List<int> list = new List<int>();
				foreach (SourceThing.Row row in EClass.sources.things.rows)
				{
					if (!row.isOrigin && row.model.trait is TraitPaint)
					{
						list.Add(row._tiles[0]);
					}
				}
				int num = (this.owner.refVal == 0) ? 0 : list.IndexOf(this.owner.refVal);
				UIContextMenu uicontextMenu = EClass.ui.CreateContextMenuInteraction();
				uicontextMenu.AddSlider("sliderPaint", (float a) => a.ToString() ?? "", (float)num, delegate(float b)
				{
					this.owner.refVal = list[(int)b];
					this.paintPos = null;
				}, 0f, (float)(list.Count - 1), true, false, false);
				uicontextMenu.Show();
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}

	public override bool RenderExtra
	{
		get
		{
			return this.owner.refVal != 0;
		}
	}

	public override void OnRenderExtra(RenderParam p)
	{
		if (this.owner == EClass.pc.held)
		{
			return;
		}
		if (this.paintPos == null)
		{
			this.paintPos = (EClass.setting.render.paintPos.TryGetValue(this.owner.id, null) ?? EClass.setting.render.paintPos.FirstItem<string, PaintPosition>());
		}
		int num = (this.owner.flipX ? -1 : 1) * (this.paintPos.flip ? -1 : 1);
		p.x += this.paintPos.pos.x * (float)num;
		p.y += this.paintPos.pos.y;
		p.z += this.paintPos.pos.z;
		p.tile = (float)(this.owner.refVal * num);
		p.matColor = 104025f;
		EClass.core.refs.renderers.obj_paint.Draw(p);
	}

	public PaintPosition paintPos;
}
