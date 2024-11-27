using System;
using System.Collections.Generic;
using System.Linq;

public class WidgetTracker : Widget
{
	public override object CreateExtra()
	{
		return new WidgetTracker.Extra();
	}

	public WidgetTracker.Extra extra
	{
		get
		{
			return base.config.extra as WidgetTracker.Extra;
		}
	}

	public static void Toggle(Element e)
	{
		if (e.owner != EMono.pc.elements)
		{
			SE.Beep();
			return;
		}
		HashSet<int> trackedElements = EMono.player.trackedElements;
		if (trackedElements.Contains(e.id))
		{
			trackedElements.Remove(e.id);
			if (trackedElements.Count == 0 && WidgetTracker.Instance)
			{
				WidgetTracker.Instance.Close();
			}
		}
		else
		{
			trackedElements.Add(e.id);
			if (!WidgetTracker.Instance)
			{
				EMono.ui.widgets.ActivateWidget("Tracker");
			}
		}
		SE.ClickGeneral();
		if (WidgetTracker.Instance)
		{
			WidgetTracker.Instance.Refresh();
		}
	}

	public override void OnActivate()
	{
		WidgetTracker.Instance = this;
		this.Refresh();
	}

	private void OnEnable()
	{
		base.InvokeRepeating("Refresh", 0.5f, 0.5f);
	}

	private void OnDisable()
	{
		base.CancelInvoke();
	}

	public void Refresh()
	{
		this.sb.Clear();
		HashSet<int> trackedElements = EMono.player.trackedElements;
		if (trackedElements.Count == 0)
		{
			this.sb.Append("none".lang());
		}
		else
		{
			int num = trackedElements.Last<int>();
			foreach (int num2 in trackedElements)
			{
				Element element = EMono.pc.elements.GetElement(num2);
				if (element != null)
				{
					string text = element.Name + "  " + element.Value.ToString();
					if (element.ShowXP)
					{
						text = text + "." + (element.vExp / 10).ToString("D2");
					}
					if (this.extra.potential)
					{
						text += (" (" + element.Potential.ToString() + ")").TagSize(13);
					}
					if (num2 != num)
					{
						text += "\n";
					}
					this.sb.Append(text);
				}
			}
		}
		if (this.sb.IsEmpty())
		{
			this.sb.Append("none".lang());
		}
		if (this.sb.Equals(this.lastSb))
		{
			return;
		}
		this.text.text = this.sb.ToString();
		this.lastSb.Set(this.sb);
		this.RebuildLayout(false);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("showPotential", this.extra.potential, delegate(bool a)
		{
			this.extra.potential = a;
			this.Refresh();
		});
		m.AddButton("clear", delegate()
		{
			EMono.player.trackedElements.Clear();
			EMono.ui.widgets.DeactivateWidget(this);
		}, true);
		base.SetBaseContextMenu(m);
	}

	public static WidgetTracker Instance;

	private FastString sb = new FastString(32);

	private FastString lastSb = new FastString(32);

	public UIText text;

	public class Extra
	{
		public bool potential;
	}
}
