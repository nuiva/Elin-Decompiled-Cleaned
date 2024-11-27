using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UICharaMaker : EMono
{
	public Biography bio
	{
		get
		{
			return this.chara.bio;
		}
	}

	public string TextUnknown
	{
		get
		{
			return "lore_unknown".lang();
		}
	}

	public void SetChara(Chara c)
	{
		this.listMode = Lang.GetList("startMode");
		this.listDifficulties = Lang.GetList("difficulties");
		this.textMode.SetText(this.listMode[EMono.game.idPrologue]);
		this.textDifficulty.SetText(this.listDifficulties[EMono.game.idDifficulty]);
		this.chara = c;
		this.BuildRaces();
		this.SetPortraitSlider();
		this.Refresh();
		this.toggleExtra.SetToggle(this.extraRace, delegate(bool a)
		{
			this.extraRace = a;
			this.BuildRaces();
			SE.Tab();
		});
	}

	public void BuildRaces()
	{
		this.races.Clear();
		this.jobs.Clear();
		bool flag = EMono.core.config.test.extraRace;
		foreach (SourceRace.Row row in EMono.sources.races.rows)
		{
			if (row.playable == 1 || (row.playable <= 6 && this.extraRace) || (flag && row.playable != 9))
			{
				this.races.Add(row);
			}
		}
		foreach (SourceJob.Row row2 in EMono.sources.jobs.rows)
		{
			if (row2.playable == 1 || (row2.playable <= 6 && this.extraRace) || (flag && row2.playable != 9))
			{
				this.jobs.Add(row2);
			}
		}
		this.races.Sort((SourceRace.Row a, SourceRace.Row b) => (a.playable - b.playable) * 10000 + a._index - b._index);
		this.jobs.Sort((SourceJob.Row a, SourceJob.Row b) => (a.playable - b.playable) * 10000 + a._index - b._index);
	}

	private void Update()
	{
		this.RefreshPortraitZoom(false);
	}

	public void RefreshPortraitZoom(bool force = false)
	{
		if (!this.portraitZoom)
		{
			return;
		}
		List<GameObject> hovered = InputModuleEX.GetPointerEventData(-1).hovered;
		bool enable = false;
		using (List<GameObject>.Enumerator enumerator = hovered.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				if (enumerator.Current.transform.IsChildOf(this.portrait.transform))
				{
					enable = true;
					break;
				}
			}
		}
		this.portraitZoom.SetActive(enable);
	}

	public void Refresh()
	{
		this.chara.elements.ApplyPotential(0);
		Prologue prologue = EMono.game.Prologue;
		this.textSign.SetText("signDisembark".lang(prologue.month.ToString() ?? "", prologue.day.ToString() ?? "", prologue.year.ToString() ?? "", null, null));
		this.RefreshPortraitZoom(false);
		this.inputAlias.text = this.chara.Aka;
		this.inputName.text = this.chara.c_altName;
		this.inputRace.text = this.chara.race.GetText("name", false).ToTitleCase(true);
		this.inputJob.text = this.chara.job.GetText("name", false).ToTitleCase(true);
		this.inputGender.text = Lang._gender(this.chara.bio.gender);
		this.inputAge.text = Lang.GetList("ages")[this.ageIndex];
		this.note.Clear();
		this.note.AddTopic("name".lang(), "nameBio".lang(this.chara.NameSimple, this.chara.Aka, null, null, null));
		this.note.AddTopic("birthday".lang(), this.bio.TextBirthDate(this.chara, true) + ((this.ageIndex == 0) ? "" : (" (" + Lang.GetList("ages")[this.ageIndex] + ")")));
		this.note.AddTopic("appearance".lang(), this.bio.TextAppearance());
		this.note.AddTopic("dad".lang(), this.bio.nameDad.ToTitleCase(false));
		this.note.AddTopic("mom".lang(), this.bio.nameMom.ToTitleCase(false));
		this.note.AddTopic("birthLoc".lang(), this.bio.nameBirthplace.ToTitleCase(false));
		this.note.Build();
		this.noteRace.Clear();
		this.noteRace.AddHeaderTopic("race".lang() + ": " + this.chara.race.GetText("name", false).ToTitleCase(true), null);
		this.noteRace.Space(8, 1);
		this.noteRace.AddText("NoteText_long", this.chara.race.GetDetail().IsEmpty(this.TextUnknown), FontColor.DontChange).Hyphenate();
		this.noteRace.Build();
		this.noteJob.Clear();
		this.noteJob.AddHeaderTopic("job".lang() + ": " + this.chara.job.GetText("name", false).ToTitleCase(true), null);
		this.noteJob.Space(8, 1);
		this.noteJob.AddText("NoteText_long", this.chara.job.GetDetail().IsEmpty(this.TextUnknown), FontColor.DontChange).Hyphenate();
		this.AddDomain(this.noteJob, EMono.player.GetDomains(), true);
		this.noteJob.Build();
		this.note2.Clear();
		this.note2.AddHeaderTopic("attributes".lang(), null);
		this.chara.elements.AddNote(this.note2, (Element e) => e.HasTag("primary"), null, ElementContainer.NoteMode.CharaMake, false, null, null);
		this.note2.Space(0, 1);
		this.note2.AddHeaderTopic("skills".lang(), null);
		this.chara.elements.AddNote(this.note2, (Element e) => e.source.category == "skill" && e.ValueWithoutLink > 1 && e.source.categorySub != "weapon", null, ElementContainer.NoteMode.CharaMake, false, null, null);
		this.note2.Space(0, 1);
		this.note2.AddHeaderTopic("feats".lang(), null);
		this.chara.elements.AddNote(this.note2, (Element e) => e is Feat, null, ElementContainer.NoteMode.CharaMake, true, null, null);
		this.note2.Build();
		if (this.addShadow)
		{
			foreach (Text text in this.note2.gameObject.GetComponentsInChildren<Text>())
			{
				Shadow shadow = text.GetComponent<Shadow>();
				if (!shadow)
				{
					shadow = text.gameObject.AddComponent<Shadow>();
				}
				shadow.effectColor = Color.black;
			}
		}
	}

	public void ListModes()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => this.listMode, delegate(int a, string b)
		{
			EMono.game.idPrologue = a;
			Prologue prologue = EMono.game.Prologue;
			EMono.world.date.year = prologue.year;
			EMono.world.date.month = prologue.month;
			EMono.world.date.day = prologue.day;
			EMono.world.weather._currentCondition = prologue.weather;
			this.textMode.SetText(this.listMode[EMono.game.idPrologue]);
			this.Refresh();
		}, true).SetSize(450f, -1f).SetTitles("wStartMode", null);
	}

	public void ListDifficulties()
	{
		TooltipManager.Instance.HideTooltips(true);
		TooltipManager.Instance.disableHide = "note";
		bool first = true;
		EMono.ui.AddLayer<LayerList>().SetPivot(0.5f, 0.2f).SetSize(260f, -1f).SetList2<GameDifficulty>(EMono.setting.start.difficulties, (GameDifficulty a) => a.Name, delegate(GameDifficulty a, ItemGeneral b)
		{
			EMono.game.idDifficulty = a.ID;
			this.textDifficulty.SetText(this.listDifficulties[EMono.game.idDifficulty]);
			this.Refresh();
		}, delegate(GameDifficulty a, ItemGeneral item)
		{
			UIButton b = item.button1;
			b.SetTooltip(delegate(UITooltip t)
			{
				t.note.Clear();
				t.note.AddHeader(a.Name, null);
				t.note.Space(8, 1);
				t.note.AddText("NoteText_medium", "vow_" + a.ID.ToString(), FontColor.DontChange).Hyphenate();
				t.note.Space(8, 1);
				t.note.Build();
			}, true);
			if (first)
			{
				TooltipManager.Instance.GetComponent<CanvasGroup>().alpha = 0f;
				TooltipManager.Instance.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
				EMono.core.actionsNextFrame.Add(delegate
				{
					b.ShowTooltipForced(true);
				});
			}
			first = false;
		}, true).SetTitles("wDifficulty", null);
		RectTransform rectTransform = EMono.ui.GetLayer<LayerList>(false).windows[0].Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = this.posList2;
		TweenUtil.Tween(0.3f, null, delegate()
		{
			UIButton.TryShowTip(null, true, true);
		});
	}

	public void RerollAlias()
	{
		this.chara._alias = AliasGen.GetRandomAlias();
		this.Refresh();
	}

	public void ListAlias()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(delegate
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++)
			{
				list.Add(AliasGen.GetRandomAlias());
			}
			return list;
		}, delegate(int a, string b)
		{
			this.chara._alias = b;
			this.Refresh();
		}, true).SetSize(450f, -1f).EnableReroll().SetTitles("wAlias", null);
	}

	public void RerollName()
	{
		this.chara.c_altName = NameGen.getRandomName();
		this.Refresh();
	}

	public void EditName()
	{
		Dialog.InputName("dialogChangeName", this.chara.c_altName.IsEmpty(this.chara.NameSimple), delegate(bool cancel, string text)
		{
			if (!cancel)
			{
				this.chara.c_altName = text;
				this.Refresh();
			}
		}, Dialog.InputType.Default).SetOnKill(delegate
		{
			EMono.ui.hud.hint.Show("hintEmbarkTop".lang(), false);
		});
	}

	public void OnEndEditName()
	{
		this.chara.c_altName = this.inputName.text;
	}

	public void RerollRace()
	{
		this.chara.ChangeRace(this.races.RandomItem(this.chara.race).id);
		this.RerollBio(true);
	}

	public void ListRace()
	{
		TooltipManager.Instance.HideTooltips(true);
		TooltipManager.Instance.disableHide = "note";
		bool first = true;
		EMono.ui.AddLayer<LayerList>().SetPivot(0.5f, 0.2f).SetSize(260f, -1f).SetList2<SourceRace.Row>(this.races, (SourceRace.Row a) => a.GetText("name", false).ToTitleCase(true), delegate(SourceRace.Row a, ItemGeneral b)
		{
			this.chara.ChangeRace(a.id);
			this.RerollBio(true);
		}, delegate(SourceRace.Row a, ItemGeneral item)
		{
			UIButton b = item.button1;
			b.SetTooltip(delegate(UITooltip t)
			{
				ElementContainer elementContainer = new ElementContainer();
				elementContainer.ApplyElementMap(EMono.pc.uid, SourceValueType.Chara, a.elementMap, 1, false, false);
				elementContainer.ApplyPotential(1);
				t.note.Clear();
				t.note.AddHeader(a.GetText("name", false).ToTitleCase(true), null);
				elementContainer.AddNoteAll(t.note);
				t.note.AddHeader("lore", null);
				t.note.AddText("NoteText_long", a.GetDetail().IsEmpty(this.TextUnknown), FontColor.DontChange).Hyphenate();
				t.note.Space(8, 1);
				t.note.Build();
			}, true);
			if (first)
			{
				TooltipManager.Instance.GetComponent<CanvasGroup>().alpha = 0f;
				TooltipManager.Instance.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
				EMono.core.actionsNextFrame.Add(delegate
				{
					b.ShowTooltipForced(true);
				});
			}
			first = false;
		}, true).SetTitles("wRace", null);
		RectTransform rectTransform = EMono.ui.GetLayer<LayerList>(false).windows[0].Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = this.posList;
		TweenUtil.Tween(0.3f, null, delegate()
		{
			UIButton.TryShowTip(null, true, true);
		});
	}

	public void RerollJob()
	{
		this.chara.ChangeJob(this.jobs.RandomItem(this.chara.job).id);
		EMono.player.RefreshDomain();
		this.Refresh();
	}

	public void ListJob()
	{
		TooltipManager.Instance.HideTooltips(true);
		TooltipManager.Instance.disableHide = "note";
		bool first = true;
		EMono.ui.AddLayer<LayerList>().SetPivot(0.5f, 0.1f).SetSize(260f, -1f).SetList2<SourceJob.Row>(this.jobs, (SourceJob.Row a) => a.GetText("name", false).ToTitleCase(true), delegate(SourceJob.Row a, ItemGeneral b)
		{
			this.chara.ChangeJob(a.id);
			EMono.player.RefreshDomain();
			this.RerollBio(true);
		}, delegate(SourceJob.Row a, ItemGeneral item)
		{
			UIButton b = item.button1;
			b.SetTooltip(delegate(UITooltip t)
			{
				ElementContainer elementContainer = new ElementContainer();
				elementContainer.ApplyElementMap(EMono.pc.uid, SourceValueType.Chara, a.elementMap, 1, false, false);
				elementContainer.ApplyPotential(2);
				t.note.Clear();
				t.note.AddHeader(a.GetText("name", false).ToTitleCase(true), null);
				elementContainer.AddNoteAll(t.note);
				t.note.AddHeader("lore", null);
				t.note.AddText("NoteText_long", a.GetDetail().IsEmpty(this.TextUnknown), FontColor.DontChange).Hyphenate();
				this.AddDomain(t.note, new ElementContainer().ImportElementMap(a.domain), false);
				t.note.Build();
			}, true);
			if (first)
			{
				TooltipManager.Instance.GetComponent<CanvasGroup>().alpha = 0f;
				TooltipManager.Instance.GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
				EMono.core.actionsNextFrame.Add(delegate
				{
					b.ShowTooltipForced(true);
				});
			}
			first = false;
		}, true).SetTitles("wClass", null);
		RectTransform rectTransform = EMono.ui.GetLayer<LayerList>(false).windows[0].Rect();
		rectTransform.pivot = new Vector2(0.5f, 0.5f);
		rectTransform.anchoredPosition = this.posList;
		TweenUtil.Tween(0.3f, null, delegate()
		{
			UIButton.TryShowTip(null, true, true);
		});
	}

	public void AddDomain(UINote n, ElementContainer domains, bool button)
	{
		n.Space(8, 1);
		string text = "";
		foreach (Element element in domains.dict.Values)
		{
			text = text + ((element == domains.dict.Values.First<Element>()) ? "" : ", ") + element.Name;
		}
		UIItem uiitem = n.AddTopic("TopicDomain", "domain".lang(), text);
		uiitem.button1.SetActive(button && EMono.pc.HasElement(1402, 1));
		uiitem.button1.SetOnClick(delegate
		{
			EMono.player.SelectDomain(new Action(this.Refresh));
		});
	}

	public void ListGender()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => Lang.GetList("genders"), delegate(int a, string b)
		{
			if (this.chara.bio.gender != a)
			{
				this.chara.bio.SetGender(a);
				this.chara.c_idPortrait = Portrait.GetRandomPortrait(this.chara.bio.gender, this.chara.GetIdPortraitCat());
				this.portrait.SetChara(this.chara, null);
				this.portraitZoom.SetChara(this.chara, null);
			}
			this.SetPortraitSlider();
			this.Refresh();
		}, true).SetTitles("wGender", null);
	}

	public void ListAge()
	{
		EMono.ui.AddLayer<LayerList>().SetStringList(() => Lang.GetList("ages"), delegate(int a, string b)
		{
			if (this.ageIndex != a)
			{
				this.ageIndex = a;
				this.RerollBio(true);
			}
			this.Refresh();
		}, true).SetTitles("wAge", null);
	}

	public void SetPortraitSlider()
	{
		if (!this.sliderPortrait)
		{
			return;
		}
		List<ModItem<Sprite>> list = Portrait.ListPlayerPortraits(this.chara.bio.gender, false);
		this.sliderPortrait.SetList<ModItem<Sprite>>(list.Find((ModItem<Sprite> a) => a.id == this.chara.c_idPortrait), list, delegate(int a, ModItem<Sprite> b)
		{
			this.chara.c_idPortrait = b.id;
			this.portrait.SetChara(this.chara, null);
			this.portraitZoom.SetChara(this.chara, null);
		}, (ModItem<Sprite> a) => a.id);
	}

	public void RerollPCC()
	{
		this.chara.pccData.Randomize(null, null, false);
		this.portrait.SetChara(this.chara, null);
		this.portraitZoom.SetChara(this.chara, null);
	}

	public void RerollHair()
	{
		this.chara.pccData.SetRandomColor("hair");
		this.portrait.SetChara(this.chara, null);
		this.portraitZoom.SetChara(this.chara, null);
	}

	public void RerollBio()
	{
		this.RerollBio(false);
	}

	public void RerollBio(bool keepParent)
	{
		this.bio.RerollBio(this.chara, this.ageIndex, keepParent);
		this.Refresh();
	}

	public void EditPCC()
	{
		EMono.ui.AddLayer<LayerEditPCC>().Activate(this.chara, UIPCC.Mode.Body, null, delegate
		{
			this.portrait.SetChara(this.chara, null);
			this.SetPortraitSlider();
		});
	}

	public Chara chara;

	public Portrait portrait;

	public Portrait portraitZoom;

	public InputField inputName;

	public InputField inputAlias;

	public InputField inputJob;

	public InputField inputRace;

	public InputField inputGender;

	public InputField inputAge;

	public UISlider sliderPortrait;

	public List<SourceRace.Row> races = new List<SourceRace.Row>();

	public List<SourceJob.Row> jobs = new List<SourceJob.Row>();

	public UIButton toggleExtra;

	public UIButton toggleParentLock;

	public UINote note;

	public UINote note2;

	public UINote noteJob;

	public UINote noteRace;

	public bool addShadow;

	public bool extraRace;

	public UIText textSign;

	public UIText textMode;

	public UIText textDifficulty;

	public int ageIndex;

	public Vector2 posList;

	public Vector2 posList2;

	[NonSerialized]
	public string[] listMode;

	[NonSerialized]
	public string[] listDifficulties;
}
