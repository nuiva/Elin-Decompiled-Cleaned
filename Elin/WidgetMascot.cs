using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMascot : Widget
{
	public RawImage image;

	public UIText text;

	public int intervalSay;

	public string[] linesDefault;

	public string[] linesSiege;

	public string[] linesShutup;

	public Vector2 textPos;

	private int nextSay = 2;

	private bool first = true;

	private bool isSiege;

	private bool isShut;

	public override bool ShowStyleMenu => false;

	public override void OnActivate()
	{
		string path = string.Concat(CorePath.coreWidget + base.config.id + "/", "default.png");
		string obj = Lang.setting.dir + "Widget/" + base.config.id + "/";
		string path2 = obj + "default.txt";
		string path3 = obj + "siege.txt";
		string path4 = obj + "shutup.txt";
		if (File.Exists(path2))
		{
			linesDefault = IO.LoadTextArray(path2);
		}
		if (File.Exists(path3))
		{
			linesSiege = IO.LoadTextArray(path3);
		}
		if (File.Exists(path4))
		{
			linesShutup = IO.LoadTextArray(path4);
		}
		if (File.Exists(path))
		{
			image.texture = IO.LoadPNG(path);
		}
		Say("");
		InvokeRepeating("_Update", 1f, 1f);
	}

	private void Update()
	{
		if (isShut && InputModuleEX.IsPointerOver(base.transform))
		{
			SE.Play("teleport");
			base.transform.position = new Vector3(EMono.rnd(Screen.width), EMono.rnd(Screen.height), 0f);
			OnChangePosition();
			ClampToScreen();
		}
	}

	public void _Update()
	{
		if (nextSay > 0)
		{
			nextSay--;
		}
		else
		{
			if (isShut)
			{
				return;
			}
			isSiege = EMono._zone.events.GetEvent<ZoneEventSiege>() != null;
			string[] array = (isSiege ? linesSiege : linesDefault);
			if (array != null)
			{
				string str = ((EMono.rnd(2) == 0) ? "" : array.RandomItem());
				if (first)
				{
					str = array[0];
					first = false;
				}
				Say(str);
			}
		}
	}

	public override void OnFlip()
	{
		image.transform.localScale = new Vector3(flip ? 1 : (-1), 1f, 1f);
	}

	public void Say(string[] lines)
	{
		Say(lines.RandomItem());
	}

	public void Say(string str)
	{
		Transform parent = text.transform.parent;
		if (str.IsEmpty())
		{
			parent.SetActive(enable: false);
		}
		else
		{
			parent.SetActive(enable: true);
			text.text = str;
			ClampToScreenEnsured(parent, textPos);
		}
		nextSay = intervalSay + EMono.rnd(intervalSay);
		if (isSiege)
		{
			nextSay /= 2;
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("shutup", isShut, delegate(bool a)
		{
			isShut = a;
			base.config.annoyPlayer = isShut;
			if (isShut)
			{
				Say(linesShutup);
			}
		});
		SetBaseContextMenu(m);
	}
}
