using UnityEngine;

public class LayerQuickMenu : ELayer
{
	public CircularRadialButton radial;

	public Vector2 oriPos;

	public float closeDist;

	public override void OnInit()
	{
		QuickMenu quickMenu = new QuickMenu();
		quickMenu.Build();
		for (int i = 0; i < quickMenu.items.Count; i++)
		{
			QuickMenu.Item item = quickMenu.items[i];
			UIButton uIButton = radial.AddOption(SpriteSheet.Get("icon_" + item.id), delegate
			{
				if (item.action != null)
				{
					item.action();
					Close();
				}
				else
				{
					HotItemActionMode.Execute(item.id);
				}
			});
			if (item.id.IsEmpty())
			{
				uIButton.icon.SetActive(enable: false);
				uIButton.image.enabled = false;
			}
			else
			{
				uIButton.icon.enabled = uIButton.icon.sprite;
				uIButton.tooltip.text = item.id.lang();
			}
		}
		radial.transform.position = (oriPos = EInput.mpos);
		radial.Init();
	}

	public override void OnUpdateInput()
	{
		if (Vector2.Distance(EInput.mpos, oriPos) > closeDist)
		{
			Close();
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (!InputModuleEX.IsPointerOver(base.transform))
			{
				Close();
				return;
			}
		}
		else if (!Application.isEditor && Input.anyKeyDown)
		{
			Close();
			return;
		}
		EInput.ConsumeWheel();
	}
}
