using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerArea : ELayer
{
	public Area area;

	public Anime animeButton;

	public LayoutGroup layout;

	public UIButton moldButton;

	public RectTransform transMenu;

	public Vector3 offset;

	public Vector3 modPos;

	public override void OnInit()
	{
		BuildMenu.Hide();
		moldButton = layout.CreateMold<UIButton>();
	}

	public void SetArea(Area a)
	{
		area = a;
		ShowPage();
	}

	public override void OnKill()
	{
		BuildMenu.Show();
	}

	public void ShowPage()
	{
		transMenu.position = Input.mousePosition + offset;
		layout.DestroyChildren();
		AddButton();
		AddButton();
		AddButton();
		layout.RebuildLayout(recursive: true);
		layout.enabled = false;
		List<UIButton> componentsInDirectChildren = layout.GetComponentsInDirectChildren<UIButton>();
		for (int i = 0; i < componentsInDirectChildren.Count; i++)
		{
			UIButton uIButton = componentsInDirectChildren[i];
			uIButton.transform.position = uIButton.transform.position + modPos * i;
			animeButton.Play(uIButton.transform);
		}
	}

	public void AddButton()
	{
		Util.Instantiate(moldButton, layout);
	}
}
