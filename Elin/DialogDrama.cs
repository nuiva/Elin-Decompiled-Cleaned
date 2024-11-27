using System;
using Applibot;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogDrama : EMono
{
	private void Awake()
	{
		this.moldButton = this.transChoices.CreateMold(null);
	}

	public virtual UIButton AddChoice(DramaChoice choice, string text, Action func = null, bool deactivateOnChoice = true)
	{
		if (!this.transChoices.gameObject.activeSelf)
		{
			this.transChoices.gameObject.SetActive(true);
		}
		UIButton uibutton = Util.Instantiate<UIButton>(this.moldButton, this.transChoices);
		uibutton.mainText.text = text;
		if (deactivateOnChoice)
		{
			uibutton.onClick.AddListener(new UnityAction(this.Deactivate));
		}
		if (func != null)
		{
			uibutton.onClick.AddListener(delegate()
			{
				func();
			});
		}
		uibutton.RebuildLayout(false);
		choice.button = uibutton;
		return uibutton;
	}

	public void ClearChoice()
	{
		this.transChoices.DestroyChildren(false, true);
		this.transChoices.gameObject.SetActive(false);
	}

	public virtual void SetText(string detail = "", bool center = false)
	{
		if (this.fontRune)
		{
			if (detail.StartsWith("#rune"))
			{
				detail = detail.Replace("#rune", "");
			}
			else
			{
				this.textMain.ApplySkin();
			}
		}
		this.textMain.SetText(detail);
		this.textMain.RebuildLayoutTo<LayerDrama>();
	}

	public void Deactivate()
	{
	}

	[Header("Dialog")]
	public Text textName;

	public Text textBio;

	public Text textAffinity;

	public Text textNoInterest;

	public Text textFav;

	public Text textRank;

	public GameObject transInterest;

	public GameObject transAffinity;

	public GameObject transFav;

	public GameObject transRank;

	public UIText textMain;

	public HyphenationJpn hypen;

	public Transform transChoices;

	private UIButton moldButton;

	public GameObject iconNext;

	public GameObject goAffinity;

	public Portrait portrait;

	public LayoutGroup layoutInterest;

	public Transform moldInterest;

	public Font fontRune;

	public Glitch glitch;

	private bool warned;
}
