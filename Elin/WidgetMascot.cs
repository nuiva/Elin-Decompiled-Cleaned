using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class WidgetMascot : Widget
{
	public override bool ShowStyleMenu
	{
		get
		{
			return false;
		}
	}

	public override void OnActivate()
	{
		string path = CorePath.coreWidget + base.config.id + "/" + "default.png";
		string str = Lang.setting.dir + "Widget/" + base.config.id + "/";
		string path2 = str + "default.txt";
		string path3 = str + "siege.txt";
		string path4 = str + "shutup.txt";
		if (File.Exists(path2))
		{
			this.linesDefault = IO.LoadTextArray(path2);
		}
		if (File.Exists(path3))
		{
			this.linesSiege = IO.LoadTextArray(path3);
		}
		if (File.Exists(path4))
		{
			this.linesShutup = IO.LoadTextArray(path4);
		}
		if (File.Exists(path))
		{
			this.image.texture = IO.LoadPNG(path, FilterMode.Point);
		}
		this.Say("");
		base.InvokeRepeating("_Update", 1f, 1f);
	}

	private void Update()
	{
		if (this.isShut && InputModuleEX.IsPointerOver(base.transform))
		{
			SE.Play("teleport");
			base.transform.position = new Vector3((float)EMono.rnd(Screen.width), (float)EMono.rnd(Screen.height), 0f);
			base.OnChangePosition();
			base.ClampToScreen();
		}
	}

	public void _Update()
	{
		if (this.nextSay > 0)
		{
			this.nextSay--;
			return;
		}
		if (this.isShut)
		{
			return;
		}
		this.isSiege = (EMono._zone.events.GetEvent<ZoneEventSiege>() != null);
		string[] array = this.isSiege ? this.linesSiege : this.linesDefault;
		if (array == null)
		{
			return;
		}
		string str = (EMono.rnd(2) == 0) ? "" : array.RandomItem<string>();
		if (this.first)
		{
			str = array[0];
			this.first = false;
		}
		this.Say(str);
	}

	public override void OnFlip()
	{
		this.image.transform.localScale = new Vector3((float)(this.flip ? 1 : -1), 1f, 1f);
	}

	public void Say(string[] lines)
	{
		this.Say(lines.RandomItem<string>());
	}

	public void Say(string str)
	{
		Transform parent = this.text.transform.parent;
		if (str.IsEmpty())
		{
			parent.SetActive(false);
		}
		else
		{
			parent.SetActive(true);
			this.text.text = str;
			base.ClampToScreenEnsured(parent, this.textPos);
		}
		this.nextSay = this.intervalSay + EMono.rnd(this.intervalSay);
		if (this.isSiege)
		{
			this.nextSay /= 2;
		}
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddToggle("shutup", this.isShut, delegate(bool a)
		{
			this.isShut = a;
			base.config.annoyPlayer = this.isShut;
			if (this.isShut)
			{
				this.Say(this.linesShutup);
			}
		});
		base.SetBaseContextMenu(m);
	}

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
}
