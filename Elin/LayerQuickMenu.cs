using System;
using UnityEngine;

public class LayerQuickMenu : ELayer
{
	public override void OnInit()
	{
		QuickMenu quickMenu = new QuickMenu();
		quickMenu.Build();
		for (int i = 0; i < quickMenu.items.Count; i++)
		{
			QuickMenu.Item item = quickMenu.items[i];
			UIButton uibutton = this.radial.AddOption(SpriteSheet.Get("icon_" + item.id), delegate
			{
				if (item.action != null)
				{
					item.action();
					this.Close();
					return;
				}
				HotItemActionMode.Execute(item.id);
			});
			if (item.id.IsEmpty())
			{
				uibutton.icon.SetActive(false);
				uibutton.image.enabled = false;
			}
			else
			{
				uibutton.icon.enabled = uibutton.icon.sprite;
				uibutton.tooltip.text = item.id.lang();
			}
		}
		this.radial.transform.position = (this.oriPos = EInput.mpos);
		this.radial.Init();
	}

	public override void OnUpdateInput()
	{
		if (Vector2.Distance(EInput.mpos, this.oriPos) > this.closeDist)
		{
			this.Close();
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!InputModuleEX.IsPointerOver(base.transform))
			{
				this.Close();
				return;
			}
		}
		else if (!Application.isEditor && Input.anyKeyDown)
		{
			this.Close();
			return;
		}
		EInput.ConsumeWheel();
	}

	public CircularRadialButton radial;

	public Vector2 oriPos;

	public float closeDist;
}
