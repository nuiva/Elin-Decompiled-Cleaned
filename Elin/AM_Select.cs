using System;
using System.Collections.Generic;
using UnityEngine;

public class AM_Select : AM_BaseTileSelect
{
	public override string idSound
	{
		get
		{
			return null;
		}
	}

	public bool ForceInnerBlockMode()
	{
		if (!base.IsActive)
		{
			return false;
		}
		using (List<InspectGroup>.Enumerator enumerator = base.Summary.groups.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current is InspectGroupBlock)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnUpdateInput()
	{
		this.RenderHighlights();
		if (EInput.rightMouse.down)
		{
			base.Deactivate();
			return;
		}
		if (!Input.GetMouseButton(0) && !EClass.ui.isPointerOverUI)
		{
			if (base.Summary.groups.Count > 0 && base.Summary.groups.Count != 1)
			{
				UIContextMenu uicontextMenu = EClass.ui.CreateContextMenu("ContextMenu");
				uicontextMenu.onUpdate = new Action(this.RenderHighlights);
				uicontextMenu.Show();
			}
			base.Deactivate();
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
		foreach (IInspect inspect in point.ListInspectorTargets())
		{
			if (!(inspect is Area) && inspect.CanInspect)
			{
				Type type = inspect.GetType();
				InspectGroup inspectGroup = null;
				foreach (InspectGroup inspectGroup2 in summary.groups)
				{
					if (inspectGroup2.type.Equals(type))
					{
						inspectGroup = inspectGroup2;
						break;
					}
				}
				if (inspectGroup == null)
				{
					inspectGroup = InspectGroup.Create(inspect);
					summary.groups.Add(inspectGroup);
				}
				else if (!inspectGroup.Contains(inspect))
				{
					inspectGroup.targets.Add(inspect);
				}
				summary.targets.Add(inspect);
			}
		}
	}

	public override ref string SetMouseInfo(ref string s)
	{
		foreach (InspectGroup inspectGroup in base.Summary.groups)
		{
			s = s + inspectGroup.GetName() + "\n";
		}
		return base.SetMouseInfo(ref s);
	}
}
