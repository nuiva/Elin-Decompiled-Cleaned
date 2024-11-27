using System;

public class TerrainMenu : EMono
{
	public void Show(ActionMode _am)
	{
		bool useSubMenu = _am.UseSubMenu;
		this.am = _am;
		base.gameObject.SetActive(useSubMenu);
		if (!useSubMenu)
		{
			return;
		}
		this.sliderRadius.value = (float)this.am.brushRadius;
		this.sliderRadius.transform.parent.SetActive(this.am.UseSubMenuSlider);
		this.group.checkbox = !this.am.SubMenuAsGroup;
		foreach (UIButton c in this.group.GetComponentsInDirectChildren(true))
		{
			c.SetActive(true);
		}
		this.group.Init(this.am.SubMenuModeIndex, delegate(int a)
		{
			this.am.OnClickSubMenu(a);
		}, false);
		for (int i = 0; i < this.group.list.Count; i++)
		{
			UIButton uibutton = this.group.list[i];
			string text = this.am.OnSetSubMenuButton(i, uibutton);
			uibutton.SetActive(text != null);
			if (text != null)
			{
				uibutton.mainText.SetText(text.lang());
			}
		}
		this.RebuildLayout(true);
	}

	public void OnChangeRadius(float a)
	{
		this.am.brushRadius = (int)a;
		this.sliderRadius.textMain.text = "radius".lang() + ": " + this.am.brushRadius.ToString();
	}

	public UISlider sliderRadius;

	public ActionMode am;

	public UISelectableGroup group;

	public int radius;
}
