using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerArea : ELayer
{
	public override void OnInit()
	{
		BuildMenu.Hide();
		this.moldButton = this.layout.CreateMold(null);
	}

	public void SetArea(Area a)
	{
		this.area = a;
		this.ShowPage();
	}

	public override void OnKill()
	{
		BuildMenu.Show();
	}

	public void ShowPage()
	{
		this.transMenu.position = Input.mousePosition + this.offset;
		this.layout.DestroyChildren(false, true);
		this.AddButton();
		this.AddButton();
		this.AddButton();
		this.layout.RebuildLayout(true);
		this.layout.enabled = false;
		List<UIButton> componentsInDirectChildren = this.layout.GetComponentsInDirectChildren(true);
		for (int i = 0; i < componentsInDirectChildren.Count; i++)
		{
			UIButton uibutton = componentsInDirectChildren[i];
			uibutton.transform.position = uibutton.transform.position + this.modPos * (float)i;
			this.animeButton.Play(uibutton.transform, null, -1f, 0f);
		}
	}

	public void AddButton()
	{
		Util.Instantiate<UIButton>(this.moldButton, this.layout);
	}

	public Area area;

	public Anime animeButton;

	public LayoutGroup layout;

	public UIButton moldButton;

	public RectTransform transMenu;

	public Vector3 offset;

	public Vector3 modPos;
}
