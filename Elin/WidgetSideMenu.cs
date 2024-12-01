using UnityEngine;

public class WidgetSideMenu : Widget
{
	public enum Mode
	{
		Stock,
		Mob,
		Exp
	}

	public Mode mode;

	public GameObject goMob;

	public GameObject goExp;

	public override void OnActivate()
	{
		ChangeMode(0);
	}

	public void ChangeMode(int i)
	{
		ChangeMode(i.ToEnum<Mode>());
	}

	public void ChangeMode(Mode m)
	{
		mode = m;
		goMob.SetActive(mode == Mode.Mob);
		goExp.SetActive(mode == Mode.Exp);
	}

	public override void OnSetContextMenu(UIContextMenu m)
	{
		m.AddChild("style").AddSlider("toggleButtonBG", (float a) => a.ToString() ?? "", base.config.skin.button, delegate(float a)
		{
			base.config.skin.button = (int)a;
			ApplySkin();
		}, 0f, base.config.skin.Skin.buttons.Count - 1, isInt: true);
		SetBaseContextMenu(m);
	}
}
