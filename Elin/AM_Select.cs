using System;
using UnityEngine;

public class AM_Select : AM_BaseTileSelect
{
	public override string idSound => null;

	public bool ForceInnerBlockMode()
	{
		if (!base.IsActive)
		{
			return false;
		}
		foreach (InspectGroup group in base.Summary.groups)
		{
			if (group is InspectGroupBlock)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnUpdateInput()
	{
		RenderHighlights();
		if (EInput.rightMouse.down)
		{
			Deactivate();
		}
		else if (!Input.GetMouseButton(0) && !EClass.ui.isPointerOverUI)
		{
			if (base.Summary.groups.Count > 0 && base.Summary.groups.Count != 1)
			{
				UIContextMenu uIContextMenu = EClass.ui.CreateContextMenu();
				uIContextMenu.onUpdate = RenderHighlights;
				uIContextMenu.Show();
			}
			Deactivate();
		}
	}

	public void RenderHighlights()
	{
	}

	public override void OnRefreshSummary(Point point, HitResult result, HitSummary summary)
	{
		if (!point.IsSeen)
		{
			return;
		}
		foreach (IInspect item in point.ListInspectorTargets())
		{
			if (item is Area || !item.CanInspect)
			{
				continue;
			}
			Type type = item.GetType();
			InspectGroup inspectGroup = null;
			foreach (InspectGroup group in summary.groups)
			{
				if (group.type.Equals(type))
				{
					inspectGroup = group;
					break;
				}
			}
			if (inspectGroup == null)
			{
				inspectGroup = InspectGroup.Create(item);
				summary.groups.Add(inspectGroup);
			}
			else if (!inspectGroup.Contains(item))
			{
				inspectGroup.targets.Add(item);
			}
			summary.targets.Add(item);
		}
	}

	public override ref string SetMouseInfo(ref string s)
	{
		foreach (InspectGroup group in base.Summary.groups)
		{
			s = s + group.GetName() + "\n";
		}
		return ref base.SetMouseInfo(ref s);
	}
}
