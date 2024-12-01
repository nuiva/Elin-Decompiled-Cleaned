using System.Collections.Generic;
using System.Linq;

public class WidgetTracker : Widget
{
	public class Extra
	{
		public bool potential;
	}

	public static WidgetTracker Instance;

	private FastString sb = new FastString();

	private FastString lastSb = new FastString();

	public UIText text;

	public Extra extra => base.config.extra as Extra;

	public override object CreateExtra()
	{
		return new Extra();
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
			if (trackedElements.Count == 0 && (bool)Instance)
			{
				Instance.Close();
			}
		}
		else
		{
			trackedElements.Add(e.id);
			if (!Instance)
			{
				EMono.ui.widgets.ActivateWidget("Tracker");
			}
		}
		SE.ClickGeneral();
		if ((bool)Instance)
		{
			Instance.Refresh();
		}
	}

	public override void OnActivate()
	{
		Instance = this;
		Refresh();
	}

	private void OnEnable()
	{
		InvokeRepeating("Refresh", 0.5f, 0.5f);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	public void Refresh()
	{
		sb.Clear();
		HashSet<int> trackedElements = EMono.player.trackedElements;
		if (trackedElements.Count == 0)
		{
			sb.Append("none".lang());
		}
		else
		{
			int num = trackedElements.Last();
			foreach (int item in trackedElements)
			{
				Element element = EMono.pc.elements.GetElement(item);
				if (element != null)
				{
					string text = element.Name + "  " + element.Value;
					if (element.ShowXP)
					{
						text = text + "." + (element.vExp / 10).ToString("D2");
					}
					if (extra.potential)
					{
						text += (" (" + element.Potential + ")").TagSize(13);
					}
					if (item != num)
					{
						text += "\n";
					}
					sb.Append(text);
				}
			}
		}
		if (sb.IsEmpty())
		{
			sb.Append("none".lang());
		}
		if (!sb.Equals(lastSb))
		{
			this.text.text = sb.ToString();
			lastSb.Set(sb);
			this.RebuildLayout();
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("showPotential", extra.potential, delegate(bool a)
		{
			extra.potential = a;
			Refresh();
		});
		m.AddButton("clear", delegate
		{
			EMono.player.trackedElements.Clear();
			EMono.ui.widgets.DeactivateWidget(this);
		});
		SetBaseContextMenu(m);
	}
}
