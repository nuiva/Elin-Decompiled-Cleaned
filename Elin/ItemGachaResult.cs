using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGachaResult : EMono
{
	public void SetChara(Chara c, LayerGachaResult _layer)
	{
		this.layer = _layer;
		this.chara = c;
		this.portrait.SetChara(c, null);
		if (c.IsPCC)
		{
			this.portrait.imageChara.transform.localScale = new Vector3(1.4f, 1.4f, 1f);
			this.portrait.imageChara.rectTransform.anchoredPosition = new Vector2(45f, -120f);
		}
		this.textName.text = c.NameBraced;
		this.textJob.text = c.job.GetName();
		this.textBio.text = c.bio.TextBio(c);
		using (List<Element>.Enumerator enumerator = c.elements.ListBestSkills().GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Element e = enumerator.Current;
				HintIcon hintIcon = Util.Instantiate<HintIcon>("UI/Element/Item/Tag Skill", this.layoutTag);
				hintIcon.text.SetText(e.Name);
				hintIcon.text2.SetText(e.Value.ToString() ?? "");
				hintIcon.tooltip.onShowTooltip = delegate(UITooltip t)
				{
					e.WriteNote(t.note, this.chara.elements, null);
				};
				hintIcon.RebuildLayout(false);
			}
		}
		this.textResult.SetActive(false);
		this.buttonGet.onClick.AddListener(delegate()
		{
			EMono.Sound.Play("good");
			this.Confirm(true);
		});
		this.buttonDump.tooltip.text = "gachaDump".lang(this.GetMedal().ToString() ?? "", null, null, null, null);
		this.buttonDump.onClick.AddListener(delegate()
		{
			EMono.Sound.Play("pay");
			this.Confirm(false);
		});
		this.textHobby.text = c.GetTextHobby(false);
		this.textWork.text = c.GetTextWork(false);
		this.textLifeStyle.text = "lifestyle".lang() + ": " + ("lifestyle_" + c.idTimeTable).lang();
	}

	public int GetMedal()
	{
		return 1;
	}

	public void Confirm(bool add)
	{
		this.textResult.SetActive(true);
		this.textResult.SetText(add ? "Get!" : "Discarded", add ? FontColor.Good : FontColor.Bad);
		this.buttonDump.SetActive(false);
		this.buttonGet.SetActive(false);
		this.layer.items.Remove(this);
		if (add)
		{
			EMono.Home.AddReserve(this.chara);
			Msg.Say("gachaAdd", this.chara, null, null, null);
		}
		else
		{
			EMono.pc.ModCurrency(this.GetMedal(), "medal");
		}
		if (this.layer.items.Count == 0)
		{
			this.layer.Close();
		}
	}

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
}
