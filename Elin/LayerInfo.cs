using System;
using UnityEngine;

public class LayerInfo : ELayer
{
	public override void OnAfterInit()
	{
		base.OnAfterInit();
		TooltipManager.Instance.HideTooltips(true);
	}

	public void SetElement(Element e)
	{
		this.windows[0].SetCaption(e.Name);
		this.info.SetElement(e);
	}

	public void Set(object o, bool _examine = false)
	{
		if (o is Thing)
		{
			this.SetThing(o as Thing, _examine);
		}
	}

	public void SetThing(Thing t, bool _examine = false)
	{
		this.examine = _examine;
		this.windows[0].SetCaption(t.NameSimple.ToTitleCase(false));
		this.info.SetThing(t);
	}

	public void SetBlock(Cell cell)
	{
		this.windows[0].SetCaption(cell.GetBlockName());
		this.info.SetBlock(cell);
	}

	public void SetFloor(Cell cell)
	{
		this.windows[0].SetCaption(cell.GetFloorName());
		this.info.SetFloor(cell);
	}

	public void SetLiquid(Cell cell)
	{
		this.windows[0].SetCaption(cell.GetLiquidName());
		this.info.SetLiquid(cell);
	}

	public void SetZone(Zone z)
	{
		this.note.Clear();
		this.note.AddHeader(z.Name, null);
		this.note.AddText(z.source.GetDetail(), FontColor.DontChange);
		this.note.Build();
	}

	public void SetObj(Cell cell)
	{
		this.windows[0].SetCaption(cell.sourceObj.GetName());
		this.info.SetObj(cell);
	}

	public override void OnKill()
	{
		base.OnKill();
		TweenUtil.Tween(0.2f, delegate()
		{
			UIButton.TryShowTip<UIButton>(null, true, false);
		}, null);
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (this.examine && ELayer.core.config.input.altExamine)
		{
			if (!Input.GetKey(EInput.keys.examine.key))
			{
				this.Close();
				return;
			}
		}
		else if (Input.GetKeyDown(EInput.keys.examine.key))
		{
			this.Close();
		}
	}

	public UICardInfo info;

	public UINote note;

	public bool examine;
}
