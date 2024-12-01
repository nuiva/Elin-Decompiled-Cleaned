using UnityEngine;
using UnityEngine.UI;

public class ItemGachaResult : EMono
{
	public UIButton buttonGet;

	public UIButton buttonDump;

	public Portrait portrait;

	public Chara chara;

	public UIText textName;

	public UIText textBio;

	public UIText textJob;

	public UIText textDetail;

	public UIText textResult;

	public UIText textHobby;

	public UIText textWork;

	public UIText textLifeStyle;

	public LayerGachaResult layer;

	public LayoutGroup layoutTag;

	public void SetChara(Chara c, LayerGachaResult _layer)
	{
		layer = _layer;
		chara = c;
		portrait.SetChara(c);
		if (c.IsPCC)
		{
			portrait.imageChara.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
			portrait.imageChara.rectTransform.anchoredPosition = new Vector2(45f, -120f);
		}
		textName.text = c.NameBraced;
		textJob.text = c.job.GetName();
		textBio.text = c.bio.TextBio(c);
		foreach (Element e in c.elements.ListBestSkills())
		{
			HintIcon hintIcon = Util.Instantiate<HintIcon>("UI/Element/Item/Tag Skill", layoutTag);
			hintIcon.text.SetText(e.Name);
			hintIcon.text2.SetText(e.Value.ToString() ?? "");
			hintIcon.tooltip.onShowTooltip = delegate(UITooltip t)
			{
				e.WriteNote(t.note, chara.elements);
			};
			hintIcon.RebuildLayout();
		}
		textResult.SetActive(enable: false);
		buttonGet.onClick.AddListener(delegate
		{
			EMono.Sound.Play("good");
			Confirm(add: true);
		});
		buttonDump.tooltip.text = "gachaDump".lang(GetMedal().ToString() ?? "");
		buttonDump.onClick.AddListener(delegate
		{
			EMono.Sound.Play("pay");
			Confirm(add: false);
		});
		textHobby.text = c.GetTextHobby();
		textWork.text = c.GetTextWork();
		textLifeStyle.text = "lifestyle".lang() + ": " + ("lifestyle_" + c.idTimeTable).lang();
	}

	public int GetMedal()
	{
		return 1;
	}

	public void Confirm(bool add)
	{
		textResult.SetActive(enable: true);
		textResult.SetText(add ? "Get!" : "Discarded", add ? FontColor.Good : FontColor.Bad);
		buttonDump.SetActive(enable: false);
		buttonGet.SetActive(enable: false);
		layer.items.Remove(this);
		if (add)
		{
			EMono.Home.AddReserve(chara);
			Msg.Say("gachaAdd", chara);
		}
		else
		{
			EMono.pc.ModCurrency(GetMedal(), "medal");
		}
		if (layer.items.Count == 0)
		{
			layer.Close();
		}
	}
}
