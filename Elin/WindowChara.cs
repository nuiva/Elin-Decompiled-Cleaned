using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using PrimitiveUI.Examples;
using UnityEngine;
using UnityEngine.UI;

public class WindowChara : WindowController
{
	public Biography bio
	{
		get
		{
			return this.chara.bio;
		}
	}

	public void SetChara(Chara c)
	{
		WindowChara.Instance = this;
		this.chara = c;
		this.portrait.interactable = this.chara.IsPC;
		this.OnSwitchContent(this.window);
		if (this.window.setting.tabs.Count > 6)
		{
			this.window.setting.tabs[6].button.SetActive(this.chara.IsPC);
		}
		if (this.window.idTab == 6 && !this.chara.IsPC)
		{
			this.window.SwitchContent(0);
		}
	}

	private void Update()
	{
		this.rectEquip.anchoredPosition = new Vector2(this.scrollSkill.content.anchoredPosition.x + this.equipPos.x, this.equipPos.y);
	}

	public override void OnSwitchContent(Window window)
	{
		if (this.chara == null)
		{
			return;
		}
		this.RefreshStatic();
		int idTab = window.idTab;
		if (idTab == 0)
		{
			this.RefreshInfo();
			this.RefreshProfile();
		}
		else
		{
			this.RefreshSkill(idTab);
		}
		bool flag = idTab == 3 || idTab == 4;
		this.scrollSkill.horizontal = flag;
		this.scrollSkill.content.anchoredPosition = new Vector2(0f, 0f);
		this.imageView.sprite = (flag ? this.maskResist : this.mask);
		if (flag)
		{
			this.RefreshEquipment(this.listEquipment2, true);
		}
		this.listEquipment2.transform.parent.SetActive(flag);
		this.buttonFeatMode.SetActive(idTab == 2 && this.chara.IsPC);
	}

	public void SetPortraitBG(int a)
	{
		string text = this.chara.GetStr(23, null) ?? "BG_1";
		text = Portrait.modPortraitBGs.GetNextID(text, a, true);
		if (text == "BG_1")
		{
			text = null;
		}
		this.chara.SetStr(23, text);
		this.RefreshStatic();
	}

	public void SetPortraitFrame(int a)
	{
		string text = this.chara.GetStr(24, null) ?? "BGF_1";
		text = Portrait.modPortraitBGFs.GetNextID(text, a, true);
		if (text == "BGF_1")
		{
			text = null;
		}
		this.chara.SetStr(24, text);
		this.RefreshStatic();
	}

	public void ToggleFeatMode()
	{
		this.featMode = !this.featMode;
		this.RefreshSkill(this.window.idTab);
	}

	public void RefreshFeatMode()
	{
		this.buttonFeatMode.mainText.text = "featMode".lang(EClass.pc.feat.ToString() ?? "", null, null, null, null);
		ColorBlock colors = this.buttonFeatMode.colors;
		colors.normalColor = colors.normalColor.SetAlpha((float)(this.featMode ? 1 : 0));
		this.buttonFeatMode.colors = colors;
	}

	public void Refresh()
	{
		this.OnSwitchContent(this.window);
	}

	public void RefreshStatic()
	{
		this.textHealth.text = this.chara.hp.ToString() + "/" + this.chara.MaxHP.ToString();
		this.textMood.text = this.chara.mana.value.ToString() + "/" + this.chara.mana.max.ToString();
		this.portrait.SetChara(this.chara, null);
		Sprite @object = Portrait.modPortraitBGs.GetItem(this.chara.GetStr(23, null) ?? "BG_1", false).GetObject(null);
		@object.texture.filterMode = FilterMode.Bilinear;
		this.portrait.image.sprite = @object;
		@object = Portrait.modPortraitBGFs.GetItem(this.chara.GetStr(24, null) ?? "BGF_1", false).GetObject(null);
		@object.texture.filterMode = FilterMode.Bilinear;
		this.portrait.imageFrame.sprite = @object;
		string text = Lang.GetList("difficulties_title")[EClass.game.idDifficulty];
		this.window.SetCaption(((text.IsEmpty() || !this.chara.IsPC) ? "" : (text + " - ")) + this.chara.NameBraced);
		this.listAttaribute.Clear();
		this.listAttaribute.callbacks = new UIList.Callback<Element, ButtonElement>
		{
			onClick = delegate(Element a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
				this.RefreshStatic();
			},
			onInstantiate = delegate(Element a, ButtonElement b)
			{
				b.SetElement(a, this.chara.elements, ButtonElement.Mode.Attribute);
			}
		};
		List<Element> list = this.chara.elements.ListElements((Element a) => a.HasTag("primary"), null);
		list.Sort((Element a, Element b) => a.source.sort - b.source.sort);
		foreach (Element o in list)
		{
			this.listAttaribute.Add(o);
		}
		this.listAttaribute.Refresh(false);
	}

	public void RefreshStatus()
	{
		this.layoutStatus.DestroyChildren(false, true);
		foreach (Stats st in new List<Stats>
		{
			this.chara.hunger,
			this.chara.burden,
			this.chara.stamina,
			this.chara.depression,
			this.chara.bladder,
			this.chara.hygiene
		})
		{
			this.AddStatus(st);
		}
		foreach (Condition st2 in this.chara.conditions)
		{
			this.AddStatus(st2);
		}
		if (this.layoutStatus.transform.childCount == 1)
		{
			this.AddStatus(null);
		}
		this.layoutStatus.RebuildLayout(false);
	}

	public void AddNeeds(Stats st)
	{
		UIItem uiitem = Util.Instantiate<UIItem>(this.moldNeeds, this.layoutNeeds);
		uiitem.text1.SetText(st.source.GetName());
		if (st.GetText().IsEmpty())
		{
			uiitem.text2.SetActive(false);
		}
		else
		{
			st.SetText(uiitem.text2, null);
		}
		UIBar componentInChildren = uiitem.GetComponentInChildren<UIBar>();
		componentInChildren.Refresh((float)st.value, (float)st.max);
		componentInChildren.image.color = st.GetColor().Multiply(this.barColor1, this.barColor2);
	}

	public void AddStatus(BaseStats st)
	{
		BaseStats st2 = st;
		string text = ((st2 != null) ? st2.GetText() : null) ?? "noItem".lang();
		if (text.IsEmpty())
		{
			return;
		}
		UIItem uiitem = Util.Instantiate<UIItem>(this.moldStats, this.layoutStatus);
		if (st == null)
		{
			uiitem.text1.SetText(text, FontColor.Passive);
			uiitem.button1.interactable = false;
		}
		else if (text.IsEmpty())
		{
			uiitem.text1.SetText(st.source.GetName());
		}
		else
		{
			st.SetText(uiitem.text1, null);
			uiitem.image1.sprite = st.GetSprite();
			uiitem.image1.SetNativeSize();
		}
		if (st != null)
		{
			uiitem.button1.SetTooltip(delegate(UITooltip t)
			{
				st.WriteNote(t.note, null);
			}, true);
		}
	}

	public void RefreshProfile()
	{
		this.RefreshStatus();
		this.textName.text = this.chara.NameSimple;
		this.textAlias.text = this.chara.Aka;
		this.textTitle.text = (this.chara.IsPC ? EClass.player.title : "-");
		this.textCurrentZone.text = ((this.chara.currentZone == null) ? "???" : this.chara.currentZone.Name);
		this.textHomeZone.text = ((this.chara.homeZone == null) ? "???" : this.chara.homeZone.Name);
		this.textBio.text = this.chara.job.GetText("name", false).ToTitleCase(false);
		this.textBio2.text = this.bio.TextBio(this.chara);
		this.textBio3.text = this.bio.TextBio2(this.chara);
		this.textDV.text = "_DV".lang(this.chara.DV.ToString() ?? "", this.chara.PV.ToString() ?? "", null, null, null);
		this.textStyle.text = "_style".lang(Lang._weight(this.chara.body.GetWeight(true), true, 0) ?? "", this.chara.elements.GetOrCreateElement(this.chara.GetArmorSkill()).Name, ("style" + this.chara.body.GetAttackStyle().ToString()).lang(), null, null);
		this.textKarma.text = (this.chara.IsPC ? (EClass.player.karma.ToString() ?? "") : "???");
		this.textSAN.text = (this.chara.SAN.value.ToString() ?? "");
		this.textFame.text = (this.chara.IsPC ? (EClass.player.fame.ToString() ?? "") : "???");
		this.textMoney.text = Lang._currency(this.chara.GetCurrency("money"), true, 14);
		this.textDeposit.text = (this.chara.IsPC ? Lang._currency(EClass.game.cards.container_deposit.GetCurrency("money"), true, 14) : "???");
		string text = "deepestLv2".lang((this.chara.IsPCFaction ? EClass.player.stats.deepest : this.chara.LV).ToString() ?? "", null, null, null, null);
		if (this.chara.IsPCFaction && EClass.player.CountKeyItem("license_void") > 0)
		{
			text = text + " " + "deepestLv3".lang(Mathf.Abs(EClass.game.spatials.Find("void").GetDeepestLv()).ToString() ?? "", null, null, null, null);
		}
		this.textDeepest.text = text;
		this.textAssets.text = (this.chara.IsPC ? "tGameTime".lang(EClass.player.stats.days.ToFormat(), EClass.player.stats.turns.ToFormat(), null, null, null) : "???");
		this.textTerritory.text = (this.chara.IsPC ? (EClass.pc.faction.CountTerritories().ToString() ?? "") : "???");
		this.buttonLife.SetElement(this.chara.elements.GetOrCreateElement(60), this.chara.elements, ButtonElement.Mode.OnlyValue);
		this.buttonMana.SetElement(this.chara.elements.GetOrCreateElement(61), this.chara.elements, ButtonElement.Mode.OnlyValue);
		this.buttonVigor.SetElement(this.chara.elements.GetOrCreateElement(62), this.chara.elements, ButtonElement.Mode.OnlyValue);
		this.buttonSpeed.SetElement(this.chara.elements.GetOrCreateElement(79), this.chara.elements, ButtonElement.Mode.OnlyValue);
		this.RefreshEquipment(this.listEquipment, true);
	}

	public void RefreshEquipment(UIList list, bool sort = true)
	{
		Element favWeaponSkill = this.chara.GetFavWeaponSkill();
		string text = ((favWeaponSkill != null) ? favWeaponSkill.Name : null) ?? Element.Get(100).GetText("name", false);
		string str = text;
		string str2 = "/";
		Element favArmorSkill = this.chara.GetFavArmorSkill();
		text = str + str2 + (((favArmorSkill != null) ? favArmorSkill.Name : null) ?? Element.Get(120).GetText("name", false));
		this.textFavArmor.SetText(text);
		list.Clear();
		UIList.Callback<BodySlot, UIItem> callback = new UIList.Callback<BodySlot, UIItem>();
		callback.onClick = delegate(BodySlot a, UIItem b)
		{
		};
		callback.onInstantiate = delegate(BodySlot a, UIItem b)
		{
			if (a.elementId == 0)
			{
				b.button1.interactable = false;
				return;
			}
			Thing thing = a.thing;
			b.refObj = thing;
			if (thing != null)
			{
				b.button1.icon.material = this.matItem;
				thing.SetImage(b.button1.icon);
			}
			else
			{
				b.button1.icon.material = null;
				b.button1.icon.sprite = SpriteSheet.Get("Media/Graphics/Icon/Element/", "eq_" + a.element.alias);
				b.button1.icon.color = this.colorUnequipped;
				b.button1.icon.SetNativeSize();
			}
			b.button1.SetTooltip(delegate(UITooltip tt)
			{
				if (a.thing != null)
				{
					a.thing.WriteNote(tt.note, null, IInspect.NoteMode.Default, null);
				}
				else
				{
					tt.note.Clear();
					tt.note.AddHeader(a.name, null);
					tt.note.AddText("noEQ".lang(), FontColor.DontChange);
					if (a.elementId == 35)
					{
						Thing.AddAttackEvaluation(tt.note, this.chara, null);
					}
				}
				tt.note.Build();
			}, true);
		};
		callback.onSort = delegate(BodySlot a, UIList.SortMode m)
		{
			if (a.element.id != 0)
			{
				return this.chara.body.GetSortVal(a);
			}
			return -99999;
		};
		list.callbacks = callback;
		foreach (BodySlot bodySlot in this.chara.body.slots)
		{
			if (bodySlot.elementId != 44)
			{
				list.Add(bodySlot);
			}
		}
		if (list.items.Count < 12)
		{
			int num = 12 - list.items.Count;
			for (int i = 0; i < num; i++)
			{
				list.Add(new BodySlot());
			}
		}
		if (list.items.Count > 18)
		{
			this.gridEquip.cellSize = new Vector2(52f, 44f);
			this.gridEquip.spacing = new Vector2(-6f, -12f);
		}
		if (sort)
		{
			list.Sort();
		}
		list.Refresh(false);
	}

	public void RefreshInfo()
	{
		this.textBirthday.text = this.bio.TextBirthDate(this.chara, false);
		this.textMom.text = this.chara.bio.nameMom.ToTitleCase(false);
		this.textDad.text = this.chara.bio.nameDad.ToTitleCase(false);
		this.textBirthplace.text = this.chara.bio.nameBirthplace.ToTitleCase(false);
		this.textLike.text = EClass.sources.cards.map[this.bio.idLike].GetName();
		this.textHobby.text = EClass.sources.elements.map[this.bio.idHobby].GetText("name", false);
		this.textFaction.text = ((this.chara.faction == null) ? "???" : this.chara.faction.name.ToTitleCase(false));
		this.textFaith.text = this.chara.faith.Name.ToTitleCase(false);
	}

	public void OnClickPortrait()
	{
		if (this.chara.IsPC)
		{
			EClass.ui.AddLayer<LayerEditPCC>().Activate(this.chara, UIPCC.Mode.Body, null, delegate
			{
				this.portrait.SetChara(this.chara, null);
			});
			return;
		}
		SE.Beep();
	}

	public void RefreshSkill(int idTab)
	{
		WindowChara.<>c__DisplayClass93_0 CS$<>8__locals1 = new WindowChara.<>c__DisplayClass93_0();
		CS$<>8__locals1.<>4__this = this;
		CS$<>8__locals1.idTab = idTab;
		this.contentList.DestroyChildren(false, true);
		this.note.Clear();
		this.note.RebuildLayout(false);
		if (CS$<>8__locals1.idTab == 1)
		{
			CS$<>8__locals1.<RefreshSkill>g__List|5("skillsGeneral", "general");
			CS$<>8__locals1.<RefreshSkill>g__List|5("skillsCraft", "craft");
			CS$<>8__locals1.<RefreshSkill>g__List|5("skillsCombat", "combat");
			CS$<>8__locals1.<RefreshSkill>g__List|5("skillsWeapon", "weapon");
			return;
		}
		if (CS$<>8__locals1.idTab == 2)
		{
			this.RefreshFeatMode();
			if (this.featMode)
			{
				CS$<>8__locals1.<RefreshSkill>g__List|6("availableFeats", "general");
				CS$<>8__locals1.<RefreshSkill>g__List|6("availableFeats_special", "special");
				CS$<>8__locals1.<RefreshSkill>g__List|6("availableFeats_skill", "skill");
				CS$<>8__locals1.<RefreshSkill>g__List|6("availableFeats_attribute", "attribute");
				return;
			}
			CS$<>8__locals1.<RefreshSkill>g__Header|0("mutation", null);
			CS$<>8__locals1.<RefreshSkill>g__ListFeat|3();
			CS$<>8__locals1.list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate(UIList.SortMode m)
				{
					foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "mutation" && a.Value != 0, null))
					{
						CS$<>8__locals1.list.Add(o);
					}
				}
			};
			CS$<>8__locals1.list.List(false);
			if (CS$<>8__locals1.list.items.Count == 0)
			{
				this._header.SetActive(false);
				CS$<>8__locals1.list.SetActive(false);
			}
			CS$<>8__locals1.<RefreshSkill>g__Header|0("etherDisease", null);
			CS$<>8__locals1.<RefreshSkill>g__ListFeat|3();
			CS$<>8__locals1.list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate(UIList.SortMode m)
				{
					foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "ether" && a.Value != 0, null))
					{
						CS$<>8__locals1.list.Add(o);
					}
				}
			};
			CS$<>8__locals1.list.List(false);
			if (CS$<>8__locals1.list.items.Count == 0)
			{
				this._header.SetActive(false);
				CS$<>8__locals1.list.SetActive(false);
			}
			CS$<>8__locals1.<RefreshSkill>g__Header|0("innateFeats", null);
			CS$<>8__locals1.<RefreshSkill>g__ListFeat|3();
			CS$<>8__locals1.list.callbacks = new UIList.Callback<Feat, ButtonElement>
			{
				onClick = delegate(Feat a, ButtonElement b)
				{
					WidgetTracker.Toggle(a);
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onInstantiate = delegate(Feat a, ButtonElement b)
				{
					b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
				},
				onList = delegate(UIList.SortMode m)
				{
					foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "feat" && a.HasTag("innate") && a.Value != 0, null))
					{
						CS$<>8__locals1.list.Add(o);
					}
				}
			};
			CS$<>8__locals1.list.List(false);
			CS$<>8__locals1.<RefreshSkill>g__Header|0("feats", null);
			CS$<>8__locals1.<RefreshSkill>g__ListFeat|3();
			BaseList list = CS$<>8__locals1.list;
			UIList.Callback<Feat, ButtonElement> callback = new UIList.Callback<Feat, ButtonElement>();
			callback.onClick = delegate(Feat a, ButtonElement b)
			{
				WidgetTracker.Toggle(a);
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
			};
			callback.onInstantiate = delegate(Feat a, ButtonElement b)
			{
				b.SetElement(a, CS$<>8__locals1.<>4__this.chara.elements, ButtonElement.Mode.Feat);
			};
			callback.onList = delegate(UIList.SortMode m)
			{
				foreach (Element o in CS$<>8__locals1.<>4__this.chara.elements.ListElements((Element a) => a.source.category == "feat" && !a.HasTag("innate") && a.Value != 0, null))
				{
					CS$<>8__locals1.list.Add(o);
				}
			};
			callback.onSort = ((Feat a, UIList.SortMode m) => a.GetSortVal(m));
			list.callbacks = callback;
			CS$<>8__locals1.list.ChangeSort(UIList.SortMode.ByID);
			CS$<>8__locals1.list.List(false);
			return;
		}
		else
		{
			if (CS$<>8__locals1.idTab == 3)
			{
				CS$<>8__locals1.<RefreshSkill>g__Header|0("resistance", null);
				this.headerEquip.SetText("resistance".lang());
				CS$<>8__locals1.<RefreshSkill>g__ListResist|2();
				CS$<>8__locals1.list.callbacks = new UIList.Callback<Element, ButtonElement>
				{
					onInstantiate = delegate(Element a, ButtonElement b)
					{
						b.SetGrid(a, CS$<>8__locals1.<>4__this.chara);
					},
					onList = delegate(UIList.SortMode m)
					{
						IEnumerable<SourceElement.Row> rows = EClass.sources.elements.rows;
						Func<SourceElement.Row, bool> predicate;
						if ((predicate = CS$<>8__locals1.<>9__35) == null)
						{
							predicate = (CS$<>8__locals1.<>9__35 = ((SourceElement.Row a) => a.category == "resist" && ((!a.tag.Contains("hidden") && !a.tag.Contains("high")) || CS$<>8__locals1.<>4__this.chara.Evalue(a.id) != 0)));
						}
						foreach (SourceElement.Row row in rows.Where(predicate))
						{
							CS$<>8__locals1.list.Add(CS$<>8__locals1.<>4__this.chara.elements.GetOrCreateElement(row.id));
						}
					}
				};
				CS$<>8__locals1.list.List(false);
				return;
			}
			if (CS$<>8__locals1.idTab == 4)
			{
				this.chara.elements.GetOrCreateElement(78);
				List<Element> eles = this.chara.elements.ListElements(delegate(Element a)
				{
					if (a.source.tag.Contains("godAbility") || a.source.categorySub == "god")
					{
						return false;
					}
					if (a.IsFlag)
					{
						if (a.Value == 0)
						{
							return false;
						}
					}
					else if ((a.owner == CS$<>8__locals1.<>4__this.chara.elements && a.vLink == 0 && !a.IsFactionElement(CS$<>8__locals1.<>4__this.chara)) || a.source.category == "resist")
					{
						return false;
					}
					return true;
				}, null);
				eles.Sort((Element a, Element b) => a.SortVal(true) - b.SortVal(true));
				string[] skillCats = new string[]
				{
					"general",
					"labor",
					"mind",
					"stealth",
					"combat",
					"craft",
					"weapon"
				};
				CS$<>8__locals1.<RefreshSkill>g__Header|0("enchant", null);
				this.headerEquip.SetText("enchant".lang());
				CS$<>8__locals1.<RefreshSkill>g__ListResist|2();
				CS$<>8__locals1.list.callbacks = new UIList.Callback<Element, ButtonElement>
				{
					onInstantiate = delegate(Element a, ButtonElement b)
					{
						b.SetGrid(a, CS$<>8__locals1.<>4__this.chara);
					},
					onList = delegate(UIList.SortMode m)
					{
						foreach (Element element in eles)
						{
							if (!skillCats.Contains(element.source.categorySub))
							{
								CS$<>8__locals1.list.Add(element);
							}
						}
					}
				};
				CS$<>8__locals1.list.List(false);
				CS$<>8__locals1.<RefreshSkill>g__Header|0("skill", null);
				CS$<>8__locals1.<RefreshSkill>g__ListResist|2();
				CS$<>8__locals1.list.callbacks = new UIList.Callback<Element, ButtonElement>
				{
					onInstantiate = delegate(Element a, ButtonElement b)
					{
						b.SetGrid(a, CS$<>8__locals1.<>4__this.chara);
					},
					onList = delegate(UIList.SortMode m)
					{
						foreach (Element element in eles)
						{
							if (skillCats.Contains(element.source.categorySub))
							{
								CS$<>8__locals1.list.Add(element);
							}
						}
					}
				};
				CS$<>8__locals1.list.List(false);
				return;
			}
			if (CS$<>8__locals1.idTab == 5)
			{
				CS$<>8__locals1.<RefreshSkill>g__Header|0("note", null);
				this.note.transform.SetAsLastSibling();
				WindowChara.RefreshNote(this.chara, this.note, false);
			}
			return;
		}
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus && this.window.idTab == 5)
		{
			WindowChara.RefreshNote(this.chara, this.note, false);
		}
	}

	public static void RefreshNote(Chara chara, UINote n, bool shortMode = false)
	{
		WindowChara.<>c__DisplayClass95_0 CS$<>8__locals1;
		CS$<>8__locals1.n = n;
		CS$<>8__locals1.n.Clear();
		Biography bio = chara.bio;
		if (shortMode)
		{
			CS$<>8__locals1.n.AddText(bio.TextBio(chara) + " " + bio.TextBio2(chara), FontColor.DontChange);
			CS$<>8__locals1.n.Space(0, 1);
		}
		else
		{
			UIItem uiitem = CS$<>8__locals1.n.AddItem("ItemBackground");
			if (chara.IsPC)
			{
				uiitem.text1.SetText(EClass.player.GetBackgroundText());
				uiitem.button1.SetOnClick(delegate
				{
					EClass.player.EditBackgroundText();
				});
			}
			else
			{
				uiitem.text1.SetText("???");
				uiitem.button1.SetActive(false);
			}
			CS$<>8__locals1.n.Space(16, 1);
			CS$<>8__locals1.n.AddTopic("TopicDomain", "profile".lang(), bio.TextBio(chara) + " " + bio.TextBio2(chara));
		}
		string text = "";
		ElementContainer elementContainer = chara.IsPC ? EClass.player.GetDomains() : new ElementContainer().ImportElementMap(chara.job.domain);
		foreach (Element element in elementContainer.dict.Values)
		{
			text = text + ((element == elementContainer.dict.Values.First<Element>()) ? "" : ", ") + element.Name;
		}
		CS$<>8__locals1.n.AddTopic("TopicDomain", "domain".lang(), text).button1.SetActive(false);
		string name = chara.GetFavCat().GetName();
		string name2 = chara.GetFavFood().GetName();
		WindowChara.<RefreshNote>g__Add|95_0("favgift".lang(name.ToLower().ToTitleCase(false), name2, null, null, null), ref CS$<>8__locals1);
		WindowChara.<RefreshNote>g__Add|95_0(chara.GetTextHobby(false), ref CS$<>8__locals1);
		WindowChara.<RefreshNote>g__Add|95_0(chara.GetTextWork(false), ref CS$<>8__locals1);
		if (chara.IsPC)
		{
			CS$<>8__locals1.n.AddTopic("TopicDomain", "totalFeat".lang(), EClass.player.totalFeat.ToString() ?? "");
		}
		Element favWeaponSkill = chara.GetFavWeaponSkill();
		text = (((favWeaponSkill != null) ? favWeaponSkill.Name : null) ?? Element.Get(100).GetText("name", false));
		text = text + " / " + ("style" + chara.GetFavAttackStyle().ToString()).lang();
		CS$<>8__locals1.n.AddTopic("TopicDomain", "attackStyle".lang(), text);
		UINote n2 = CS$<>8__locals1.n;
		string id = "TopicDomain";
		string text2 = "armorStyle".lang();
		Element favArmorSkill = chara.GetFavArmorSkill();
		n2.AddTopic(id, text2, ((favArmorSkill != null) ? favArmorSkill.Name : null) ?? Element.Get(120).GetText("name", false));
		if (chara.IsPC && EClass.pc.c_daysWithGod > 0)
		{
			WindowChara.<RefreshNote>g__AddText|95_1("info_daysWithGod".lang(EClass.pc.c_daysWithGod.ToString() ?? "", EClass.pc.faith.Name, null, null, null), ref CS$<>8__locals1);
		}
		if (chara.ride != null)
		{
			WindowChara.<RefreshNote>g__AddText|95_1("info_ride".lang(chara.ride.NameBraced, null, null, null, null), ref CS$<>8__locals1);
		}
		if (chara.parasite != null)
		{
			WindowChara.<RefreshNote>g__AddText|95_1("info_parasite".lang(chara.parasite.NameBraced, null, null, null, null), ref CS$<>8__locals1);
		}
		if (EClass.player.IsCriminal)
		{
			WindowChara.<RefreshNote>g__AddText|95_1("info_criminal".lang(), ref CS$<>8__locals1);
		}
		if (EClass.debug.showExtra)
		{
			CS$<>8__locals1.n.AddText(string.Concat(new string[]
			{
				"LV:",
				chara.LV.ToString(),
				"  exp:",
				chara.exp.ToString(),
				" next:",
				chara.ExpToNext.ToString()
			}), FontColor.DontChange);
			CS$<>8__locals1.n.AddText("Luck:" + chara.Evalue(78).ToString(), FontColor.DontChange);
		}
		CS$<>8__locals1.n.Build();
	}

	[CompilerGenerated]
	internal static void <RefreshNote>g__Add|95_0(string s, ref WindowChara.<>c__DisplayClass95_0 A_1)
	{
		string[] array = s.Split(':', StringSplitOptions.None);
		A_1.n.AddTopic("TopicDomain", array[0], (array.Length >= 2) ? array[1].TrimStart(' ') : "").button1.SetActive(false);
	}

	[CompilerGenerated]
	internal static void <RefreshNote>g__AddText|95_1(string s, ref WindowChara.<>c__DisplayClass95_0 A_1)
	{
		A_1.n.AddText(" ・ " + s, FontColor.DontChange);
	}

	public static WindowChara Instance;

	public Chara chara;

	public Portrait portrait;

	public UIText textMood;

	public UIText textHealth;

	public UIText textName;

	public UIText textAlias;

	public UIText textTitle;

	public UIText textBio;

	public UIText textBio2;

	public UIText textBio3;

	public UIText textBirthday;

	public UIText textFaction;

	public UIText textFaith;

	public UIText textHome;

	public UIText textMom;

	public UIText textDad;

	public UIText textBirthplace;

	public UIText textLike;

	public UIText textHobby;

	public UIText textKarma;

	public UIText textSAN;

	public UIText textFame;

	public UIText textMoney;

	public UIText textTax;

	public UIText textTerritory;

	public UIText textIncome;

	public UIText textAssets;

	public UIText textDeposit;

	public UIText textDeepest;

	public UIText textCurrentZone;

	public UIText textHomeZone;

	public UIList listAttaribute;

	public UIList listEquipment;

	public UIList listEquipment2;

	public UINote note;

	public PUIExampleRPGStats graph;

	public UIText textStability;

	public UIText textLaw;

	public UIText textAffection;

	public UIText textDominance;

	public UIText textExtroversion;

	public UIText textFavArmor;

	public UIText textDV;

	public UIText textStyle;

	public LayoutGroup layoutNeeds;

	public LayoutGroup layoutStatus;

	public GridLayoutGroup gridEquip;

	public UIScrollView scrollSkill;

	public ButtonElement buttonMana;

	public ButtonElement buttonLife;

	public ButtonElement buttonVigor;

	public ButtonElement buttonSpeed;

	public UIItem moldStats;

	public UIItem moldNeeds;

	public UIHeader moldHeader;

	public UIHeader moldHeader2;

	public UIHeader headerEquip;

	public UIList moldListSkill;

	public UIList moldListResist;

	public UIList moldListFeat;

	public UIList moldListFeatPurchase;

	public Transform contentList;

	public RectTransform rectEquip;

	public UIButton buttonFeatMode;

	public Image imageView;

	public Sprite mask;

	public Sprite maskResist;

	public Material matItem;

	public Color colorUnequipped;

	public Vector2 equipPos;

	public bool featMode;

	public float barColor1;

	public float barColor2;

	private UIHeader _header;
}
