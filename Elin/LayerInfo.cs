using UnityEngine;

public class LayerInfo : ELayer
{
	public UICardInfo info;

	public UINote note;

	public bool examine;

	public override void OnAfterInit()
	{
		base.OnAfterInit();
		TooltipManager.Instance.HideTooltips(immediate: true);
	}

	public void SetElement(Element e)
	{
		windows[0].SetCaption(e.Name);
		info.SetElement(e);
	}

	public void Set(object o, bool _examine = false)
	{
		if (o is Thing)
		{
			SetThing(o as Thing, _examine);
		}
	}

	public void SetThing(Thing t, bool _examine = false)
	{
		examine = _examine;
		windows[0].SetCaption(t.NameSimple.ToTitleCase());
		info.SetThing(t);
	}

	public void SetBlock(Cell cell)
	{
		windows[0].SetCaption(cell.GetBlockName());
		info.SetBlock(cell);
	}

	public void SetFloor(Cell cell)
	{
		windows[0].SetCaption(cell.GetFloorName());
		info.SetFloor(cell);
	}

	public void SetLiquid(Cell cell)
	{
		windows[0].SetCaption(cell.GetLiquidName());
		info.SetLiquid(cell);
	}

	public void SetZone(Zone z)
	{
		note.Clear();
		note.AddHeader(z.Name);
		note.AddText(z.source.GetDetail());
		note.Build();
	}

	public void SetObj(Cell cell)
	{
		windows[0].SetCaption(cell.sourceObj.GetName());
		info.SetObj(cell);
	}

	public override void OnKill()
	{
		base.OnKill();
		TweenUtil.Tween(0.2f, delegate
		{
			UIButton.TryShowTip<UIButton>(null, highlight: true, ignoreWhenRightClick: false);
		});
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (examine && ELayer.core.config.input.altExamine)
		{
			if (!Input.GetKey(EInput.keys.examine.key))
			{
				Close();
			}
		}
		else if (Input.GetKeyDown(EInput.keys.examine.key))
		{
			Close();
		}
	}
}
