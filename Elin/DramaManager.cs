using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class DramaManager : EMono
{
	private void Update()
	{
		if (this.sequence == null)
		{
			return;
		}
		this.sequence.OnUpdate();
	}

	public DramaSequence Play(DramaSetup setup)
	{
		this.setup = setup;
		this.cgCover.SetActive(false);
		this.endroll.SetActive(false);
		this.tg = setup.person;
		this.SetDialog("Default");
		this.sequence = new DramaSequence
		{
			setup = setup,
			manager = this,
			id = setup.sheet
		};
		this.Load();
		this.sequence.lastStep = (this.sequence.lastlastStep = setup.step);
		if (!setup.forceJump.IsEmpty())
		{
			this.sequence.Play(setup.forceJump);
		}
		else
		{
			this.sequence.Play(0);
		}
		return this.sequence;
	}

	public DramaSequence Load()
	{
		this.sequence.Clear();
		this.customEvents.Clear();
		this.customTalkTopics.Clear();
		this.moldActor = this.actorPos.CreateMold(null);
		if (this.tg != null)
		{
			this.sequence.AddActor("tg", this.tg);
		}
		this.sequence.AddActor("pc", new Person(EMono.pc));
		string text = CorePath.DramaData + this.setup.book + ".xlsx";
		ExcelData excelData = DramaManager.dictCache.TryGetValue(text, null);
		if (excelData != null && excelData.IsModified())
		{
			excelData = null;
		}
		if (excelData == null)
		{
			excelData = new ExcelData();
		}
		excelData.maxEmptyRows = 10;
		excelData.path = text;
		List<Dictionary<string, string>> list = excelData.BuildList(this.setup.sheet);
		if (!Lang.isBuiltin && this.dictLocalize.Count == 0)
		{
			foreach (Dictionary<string, string> dictionary in new ExcelData
			{
				maxEmptyRows = 10,
				path = CorePath.DramaDataLocal + this.setup.book + ".xlsx"
			}.BuildList(this.setup.sheet))
			{
				string text2 = dictionary["id"];
				if (!text2.IsEmpty() && !dictionary["text"].IsEmpty())
				{
					this.dictLocalize.Add(text2, dictionary["text"]);
				}
			}
		}
		DramaManager.dictCache[text] = excelData;
		this.idDefault = this.setup.step;
		this.lastTalk = null;
		this.enableTone = (this.customEventsAdded = (this.idDefaultPassed = false));
		this.countLine = 0;
		for (int i = 0; i < list.Count; i++)
		{
			this.ParseLine(list[i]);
			this.countLine++;
		}
		this.AddCustomEvents("Resident");
		this.sequence.steps["end"] = 99999;
		this.sequence.AddActor("narrator", new Person("narrator", null));
		return this.sequence;
	}

	public void AddCustomEvents(string idCustom = "Resident")
	{
		if (this.customEventsAdded)
		{
			return;
		}
		DramaCustomSequence dramaCustomSequence = new DramaCustomSequence
		{
			manager = this,
			idDefault = this.idDefault,
			idCustom = idCustom,
			events = this.customEvents,
			setup = this.sequence.setup,
			sequence = this.sequence
		};
		this.CustomEvent(delegate
		{
			this.sequence.Exit();
		}, null, 0f, false);
		if (this.tg != null && this.tg.hasChara)
		{
			DramaManager.TG = this.tg.chara;
			dramaCustomSequence.Build(this.tg.chara);
		}
		if (!this.sequence.setup.textData.IsEmpty())
		{
			dramaCustomSequence.BuildTextData();
		}
		foreach (DramaEvent e in this.customEvents)
		{
			this.AddEvent(e);
		}
		this.customEventsAdded = true;
	}

	public unsafe void ParseLine(Dictionary<string, string> item)
	{
		DramaManager.<>c__DisplayClass37_0 CS$<>8__locals1 = new DramaManager.<>c__DisplayClass37_0();
		CS$<>8__locals1.<>4__this = this;
		string[] array = item.ContainsKey("action") ? item["action"].Split('/', StringSplitOptions.None) : null;
		CS$<>8__locals1.action = ((array != null) ? array[0] : null);
		string text = item.ContainsKey("step") ? item["step"] : null;
		if (text == "//")
		{
			return;
		}
		if (text == this.idDefault)
		{
			this.idDefaultPassed = true;
		}
		CS$<>8__locals1.actor = (item.ContainsKey("actor") ? item["actor"] : "#1");
		CS$<>8__locals1.p = (item.ContainsKey("param") ? item["param"].Split(',', StringSplitOptions.None) : new string[0]);
		CS$<>8__locals1.p0 = ((CS$<>8__locals1.p.Length != 0) ? CS$<>8__locals1.p[0] : "");
		CS$<>8__locals1.p1 = ((CS$<>8__locals1.p.Length > 1) ? CS$<>8__locals1.p[1] : "");
		CS$<>8__locals1.p2 = ((CS$<>8__locals1.p.Length > 2) ? CS$<>8__locals1.p[2] : "");
		float.TryParse(CS$<>8__locals1.p0, out CS$<>8__locals1.p0f);
		float num;
		float.TryParse(CS$<>8__locals1.p1, out num);
		bool flag = !item["text_JP"].IsEmpty();
		item.TryGetValue("text_JP", null);
		CS$<>8__locals1.text = null;
		if (flag)
		{
			if (!Lang.isBuiltin)
			{
				string key = item["id"];
				if (this.dictLocalize.ContainsKey(key))
				{
					CS$<>8__locals1.text = this.dictLocalize[key];
				}
				else
				{
					CS$<>8__locals1.text = item.TryGetValue("text_EN", null);
				}
			}
			else
			{
				CS$<>8__locals1.text = item["text_" + Lang.langCode];
			}
		}
		if (flag && CS$<>8__locals1.text.StartsWith("$") && this.tg != null && this.tg.hasChara)
		{
			string text2 = CS$<>8__locals1.text.Split(' ', StringSplitOptions.None)[0];
			CS$<>8__locals1.text = CS$<>8__locals1.text.Replace(text2, this.tg.chara.GetTalkText(text2.Remove(0, 1), false, true));
		}
		CS$<>8__locals1.jump = (item.ContainsKey("jump") ? item["jump"] : null);
		string text3 = item.ContainsKey("if") ? item["if"] : null;
		string @if = item.ContainsKey("if2") ? item["if2"] : null;
		string check = item.ContainsKey("check") ? item["check"] : null;
		bool flag2 = false;
		if (text != null && !this.sequence.steps.ContainsKey(text) && CS$<>8__locals1.action != "choice" && CS$<>8__locals1.action != "cancel")
		{
			this.sequence.steps.Add(text, this.sequence.events.Count);
		}
		if (text3 == "*")
		{
			text3 = this.lastIF;
			@if = this.lastIF2;
		}
		else
		{
			this.lastIF = text3;
			this.lastIF2 = @if;
		}
		if (!this.CheckIF(text3) || !this.CheckIF(@if))
		{
			if (CS$<>8__locals1.action == "reload")
			{
				string id = "flag" + this.countLine.ToString();
				this.sequence.AddStep(id);
			}
			return;
		}
		string text4 = CS$<>8__locals1.action;
		uint num2 = <PrivateImplementationDetails>.ComputeStringHash(text4);
		if (num2 <= 1915422415U)
		{
			if (num2 <= 691248415U)
			{
				if (num2 <= 290138826U)
				{
					if (num2 <= 92252806U)
					{
						if (num2 <= 75475187U)
						{
							if (num2 != 59586952U)
							{
								if (num2 != 75475187U)
								{
									goto IL_1F6B;
								}
								if (!(text4 == "refAction1"))
								{
									goto IL_1F6B;
								}
								this.AddEvent(delegate()
								{
									LayerDrama.refAction1();
								}, 0f, false);
								goto IL_1FC4;
							}
							else
							{
								if (!(text4 == "addResource"))
								{
									goto IL_1F6B;
								}
								this.AddEvent(delegate()
								{
									EMono.BranchOrHomeBranch.resources.Get(CS$<>8__locals1.p0).Mod(CS$<>8__locals1.p1.ToInt(), true);
								}, 0f, false);
								goto IL_1FC4;
							}
						}
						else if (num2 != 85301960U)
						{
							if (num2 != 92252806U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "refAction2"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								LayerDrama.refAction2();
							}, 0f, false);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "glitch"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								DramaActor.useGlitch = true;
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 <= 142925988U)
					{
						if (num2 != 107912219U)
						{
							if (num2 != 142925988U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "fadeInOut"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.imageCover.color = ((CS$<>8__locals1.p2 != null && CS$<>8__locals1.p2 == "white") ? Color.white : Color.black);
								CS$<>8__locals1.<>4__this.cgCover.SetActive(true);
								CS$<>8__locals1.<>4__this.cgCover.alpha = 0f;
								CS$<>8__locals1.<>4__this.cgCover.DOFade(1f, CS$<>8__locals1.p0f).SetDelay(0.1f);
							}, CS$<>8__locals1.p0f, true);
							this.AddEvent(new DramaEventWait((num == 0f) ? 0.1f : num, null));
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.imageCover.color = ((CS$<>8__locals1.p2 != null && CS$<>8__locals1.p2 == "white") ? Color.white : Color.black);
								CS$<>8__locals1.<>4__this.cgCover.SetActive(true);
								CS$<>8__locals1.<>4__this.cgCover.alpha = 1f;
								CS$<>8__locals1.<>4__this.cgCover.DOFade(0f, CS$<>8__locals1.p0f).SetDelay(0.1f);
							}, CS$<>8__locals1.p0f, false);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "cancel"))
							{
								goto IL_1F6B;
							}
							this.lastTalk.canCancel = true;
							this.lastTalk.idCancelJump = CS$<>8__locals1.jump;
							goto IL_1FC4;
						}
					}
					else if (num2 != 235771284U)
					{
						if (num2 != 260921166U)
						{
							if (num2 != 290138826U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "setBG2"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								Util.Instantiate<Transform>("Media/Graphics/Image/Drama/" + CS$<>8__locals1.p0, CS$<>8__locals1.<>4__this.layer).SetAsFirstSibling();
							}, 0f, false);
							goto IL_1FC4;
						}
						else if (!(text4 == "bout_win"))
						{
							goto IL_1F6B;
						}
					}
					else
					{
						if (!(text4 == "sound"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							if (CS$<>8__locals1.p1 != "")
							{
								SoundManager.current.MuteBGMFor(float.Parse(CS$<>8__locals1.p1, CultureInfo.InvariantCulture));
							}
							EMono.Sound.Play(CS$<>8__locals1.p0);
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 <= 434279500U)
				{
					if (num2 <= 310765516U)
					{
						if (num2 != 307805908U)
						{
							if (num2 != 310765516U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "haltPlaylist"))
							{
								goto IL_1F6B;
							}
							LayerDrama.haltPlaylist = true;
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "addKeyItem"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								EMono.player.ModKeyItem(CS$<>8__locals1.p0, 1, true);
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 != 337658899U)
					{
						if (num2 != 381584877U)
						{
							if (num2 != 434279500U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "completeQuest"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								EMono.game.quests.Complete(CS$<>8__locals1.p0.IsEmpty() ? LayerDrama.currentQuest : EMono.game.quests.Get(CS$<>8__locals1.p0));
								LayerDrama.currentQuest = null;
							}, 0f, false);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "addActor"))
							{
								goto IL_1F6B;
							}
							if (CS$<>8__locals1.actor == "god")
							{
								this.sequence.AddActor(CS$<>8__locals1.actor, new Person(LayerDrama.currentReligion));
								goto IL_1FC4;
							}
							DramaActor dramaActor = this.sequence.AddActor(CS$<>8__locals1.actor, new Person(CS$<>8__locals1.actor, null));
							if (!CS$<>8__locals1.text.IsEmpty())
							{
								dramaActor.owner.tempName = CS$<>8__locals1.text;
								goto IL_1FC4;
							}
							goto IL_1FC4;
						}
					}
					else
					{
						if (!(text4 == "focus"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							Point pos = CS$<>8__locals1.<>4__this.sequence.setup.person.chara.pos.Copy();
							EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
							{
								pos = pos
							};
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 <= 588029017U)
				{
					if (num2 != 441554552U)
					{
						if (num2 != 588029017U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "bout_lose"))
						{
							goto IL_1F6B;
						}
					}
					else
					{
						if (!(text4 == "alphaIn"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							DOTween.Kill(CS$<>8__locals1.<>4__this.cg, false);
							CS$<>8__locals1.<>4__this.cg.alpha = 0f;
							CS$<>8__locals1.<>4__this.cg.DOFade(1f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 != 652943243U)
				{
					if (num2 != 681154065U)
					{
						if (num2 != 691248415U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "effectEmbarkIn"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							EMono._map.RevealAll(true);
							Util.Instantiate("UI/Layer/LayerEmbark/EmbarkActor_crystal", null);
						}, 0f, false);
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "new"))
						{
							goto IL_1F6B;
						}
						goto IL_1FC4;
					}
				}
				else
				{
					if (!(text4 == "Playlist"))
					{
						goto IL_1F6B;
					}
					this.AddEvent(delegate()
					{
						LayerDrama.haltPlaylist = false;
						EMono.Sound.StopBGM(0f, false);
						EMono.Sound.currentBGM = null;
						CS$<>8__locals1.<>4__this.bgmChanged = true;
					}, 0f, false);
					goto IL_1FC4;
				}
				this.AddEvent(delegate()
				{
					Layer instance = LayerDrama.Instance;
					Action onKill;
					if ((onKill = CS$<>8__locals1.<>9__68) == null)
					{
						onKill = (CS$<>8__locals1.<>9__68 = delegate()
						{
							Zone zone = EMono.game.spatials.Find(EMono._zone.instance.uidZone);
							if (CS$<>8__locals1.action == "bout_win")
							{
								zone.events.AddPreEnter(new ZonePreEnterBoutWin
								{
									target = CS$<>8__locals1.<>4__this.tg.chara
								}, true);
							}
							EMono.pc.MoveZone(zone, ZoneTransition.EnterState.Auto);
						});
					}
					instance.SetOnKill(onKill);
				}, 0f, false);
				goto IL_1FC4;
			}
			if (num2 <= 1425576060U)
			{
				if (num2 <= 895014094U)
				{
					if (num2 <= 767425313U)
					{
						if (num2 != 718790086U)
						{
							if (num2 == 767425313U)
							{
								if (text4 == "acceptQuest")
								{
									this.AddEvent(delegate()
									{
										IEnumerable<Quest> globalList = EMono.game.quests.globalList;
										Func<Quest, bool> predicate;
										if ((predicate = CS$<>8__locals1.<>9__66) == null)
										{
											predicate = (CS$<>8__locals1.<>9__66 = ((Quest a) => a.source.id == CS$<>8__locals1.p0));
										}
										Quest quest = globalList.Where(predicate).First<Quest>();
										EMono.game.quests.globalList.Remove(quest);
										EMono.game.quests.Start(quest);
									}, 0f, false);
									goto IL_1FC4;
								}
							}
						}
						else if (text4 == "forceBGM")
						{
							this.AddEvent(delegate()
							{
								SoundManager.ForceBGM();
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 != 797646213U)
					{
						if (num2 != 826615426U)
						{
							if (num2 == 895014094U)
							{
								if (text4 == "unfocus")
								{
									this.AddEvent(delegate()
									{
										EMono.scene.screenElin.focusOption = null;
									}, 0f, false);
									goto IL_1FC4;
								}
							}
						}
						else if (text4 == "keepBGM")
						{
							LayerDrama.keepBGM = true;
							goto IL_1FC4;
						}
					}
					else if (text4 == "endroll")
					{
						this.AddEvent(new DramaEventEndRoll());
						goto IL_1FC4;
					}
				}
				else if (num2 <= 998101119U)
				{
					if (num2 != 912332847U)
					{
						if (num2 == 998101119U)
						{
							if (text4 == "nextPhase")
							{
								this.AddEvent(delegate()
								{
									EMono.game.quests.Get(CS$<>8__locals1.p0).NextPhase();
								}, 0f, false);
								goto IL_1FC4;
							}
						}
					}
					else if (text4 == "tutorial")
					{
						LayerDrama.Instance.SetOnKill(delegate
						{
							Tutorial.Play(CS$<>8__locals1.p0);
						});
						goto IL_1FC4;
					}
				}
				else if (num2 != 1073903299U)
				{
					if (num2 != 1225822769U)
					{
						if (num2 == 1425576060U)
						{
							if (text4 == "setBG")
							{
								this.AddEvent(delegate()
								{
									if (CS$<>8__locals1.p0.IsEmpty())
									{
										CS$<>8__locals1.<>4__this.imageBG.enabled = false;
										return;
									}
									CS$<>8__locals1.<>4__this.imageBG.enabled = true;
									CS$<>8__locals1.<>4__this.imageBG.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Drama/" + CS$<>8__locals1.p0);
								}, 0f, false);
								goto IL_1FC4;
							}
						}
					}
					else if (text4 == "hideDialog")
					{
						this.AddEvent(delegate()
						{
							CS$<>8__locals1.<>4__this.dialog.SetActive(false);
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (text4 == "updateJournal")
				{
					this.AddEvent(delegate()
					{
						EMono.game.quests.UpdateJournal();
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (num2 <= 1672383624U)
			{
				if (num2 <= 1505339517U)
				{
					if (num2 != 1487154227U)
					{
						if (num2 == 1505339517U)
						{
							if (text4 == "alphaOut")
							{
								this.AddEvent(delegate()
								{
									DOTween.Kill(CS$<>8__locals1.<>4__this.cg, false);
									CS$<>8__locals1.<>4__this.cg.alpha = 1f;
									CS$<>8__locals1.<>4__this.cg.DOFade(0f, CS$<>8__locals1.p0f).SetDelay(0.1f);
								}, CS$<>8__locals1.p0f, true);
								goto IL_1FC4;
							}
						}
					}
					else if (text4 == "slap")
					{
						this.AddEvent(delegate()
						{
							Layer instance = LayerDrama.Instance;
							Action onKill;
							if ((onKill = CS$<>8__locals1.<>9__67) == null)
							{
								onKill = (CS$<>8__locals1.<>9__67 = delegate()
								{
									CS$<>8__locals1.<>4__this.tg.chara.PlaySound("whip", 1f, true);
									CS$<>8__locals1.<>4__this.tg.chara.Say("slap", CS$<>8__locals1.<>4__this.tg.chara, EMono.pc, null, null);
									EMono.pc.PlayAnime(AnimeID.Shiver, false);
									EMono.pc.DamageHP(5 + EClass.rndHalf(EMono.pc.MaxHP / 3), 919, 100, AttackSource.Condition, null, true);
									EMono.pc.OnInsulted();
								});
							}
							instance.SetOnKill(onKill);
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 != 1563407972U)
				{
					if (num2 != 1611900348U)
					{
						if (num2 == 1672383624U)
						{
							if (text4 == "addAffinity")
							{
								goto IL_1FC4;
							}
						}
					}
					else if (text4 == "alphaInOut")
					{
						this.AddEvent(delegate()
						{
							DOTween.Kill(CS$<>8__locals1.<>4__this.cg, false);
							CS$<>8__locals1.<>4__this.cg.alpha = 1f;
							CS$<>8__locals1.<>4__this.cg.DOFade(0f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, true);
						this.AddEvent(new DramaEventWait((num == 0f) ? 0.1f : num, null));
						this.AddEvent(delegate()
						{
							DOTween.Kill(CS$<>8__locals1.<>4__this.cg, false);
							CS$<>8__locals1.<>4__this.cg.alpha = 0f;
							CS$<>8__locals1.<>4__this.cg.DOFade(1f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, false);
						goto IL_1FC4;
					}
				}
				else if (text4 == "BGMStay")
				{
					this.AddEvent(delegate()
					{
						EMono.Sound.PlayBGM(EMono.core.refs.dictBGM[CS$<>8__locals1.p0.ToInt()], 0f, 0f);
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (num2 <= 1813383965U)
			{
				if (num2 != 1787721130U)
				{
					if (num2 == 1813383965U)
					{
						if (text4 == "setHour")
						{
							this.AddEvent(delegate()
							{
								EMono.world.date.hour = (int)CS$<>8__locals1.p0f;
								EMono.world.date.min = 0;
								EMono.scene.OnChangeHour();
								EMono.scene.screenElin.RefreshAll();
								EMono.pc.RecalculateFOV();
							}, 0f, false);
							goto IL_1FC4;
						}
					}
				}
				else if (text4 == "end")
				{
					this.AddEvent(new DramaEventExit());
					goto IL_1FC4;
				}
			}
			else if (num2 != 1852738900U)
			{
				if (num2 != 1908317780U)
				{
					if (num2 == 1915422415U)
					{
						if (text4 == "setDialog")
						{
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.SetDialog(CS$<>8__locals1.p0);
							}, 0f, false);
							goto IL_1FC4;
						}
					}
				}
				else if (text4 == "setQuestClient")
				{
					this.AddEvent(delegate()
					{
						if (LayerDrama.currentQuest != null)
						{
							LayerDrama.currentQuest.SetClient(CS$<>8__locals1.<>4__this.tg.chara, false);
						}
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (text4 == "effect")
			{
				this.AddEvent(delegate()
				{
					Point from = new Point(CS$<>8__locals1.p[1].ToInt(), CS$<>8__locals1.p[2].ToInt());
					Effect.Get(CS$<>8__locals1.p0).Play(from, 0f, null, null);
				}, 0f, false);
				goto IL_1FC4;
			}
		}
		else
		{
			if (num2 <= 3002570765U)
			{
				if (num2 <= 2301512864U)
				{
					if (num2 <= 2124469614U)
					{
						if (num2 <= 2027681165U)
						{
							if (num2 != 1960439120U)
							{
								if (num2 != 2027681165U)
								{
									goto IL_1F6B;
								}
								if (!(text4 == "setFlag"))
								{
									goto IL_1F6B;
								}
								this.AddEvent(delegate()
								{
									if (CS$<>8__locals1.p0.StartsWith("*"))
									{
										Quest quest = EMono.game.quests.Get(CS$<>8__locals1.p0.TrimStart('*'));
										if (quest != null)
										{
											quest.ChangePhase(CS$<>8__locals1.p1.ToInt(quest.GetType()));
											return;
										}
									}
									else
									{
										EMono.player.dialogFlags[CS$<>8__locals1.p0] = (CS$<>8__locals1.p1.IsEmpty() ? 1 : int.Parse(CS$<>8__locals1.p1));
									}
								}, 0f, false);
								goto IL_1FC4;
							}
							else
							{
								if (!(text4 == "haltBGM"))
								{
									goto IL_1F6B;
								}
								EMono.Sound.haltUpdate = true;
								goto IL_1FC4;
							}
						}
						else if (num2 != 2066570139U)
						{
							if (num2 != 2076142835U)
							{
								if (num2 != 2124469614U)
								{
									goto IL_1F6B;
								}
								if (!(text4 == "focusChara"))
								{
									goto IL_1F6B;
								}
								this.AddEvent(delegate()
								{
									Point pos = EMono._map.FindChara(CS$<>8__locals1.p0).pos.Copy();
									EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
									{
										pos = pos,
										speed = CS$<>8__locals1.p1.IsEmpty("2").ToFloat()
									};
								}, 0f, false);
								goto IL_1FC4;
							}
							else
							{
								if (!(text4 == "hideUI"))
								{
									goto IL_1F6B;
								}
								this.AddEvent(delegate()
								{
									EMono.ui.Hide(CS$<>8__locals1.p0f);
								}, 0f, false);
								goto IL_1FC4;
							}
						}
						else
						{
							if (!(text4 == "canCancel"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.sequence.canCancel = bool.Parse(CS$<>8__locals1.p0);
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 <= 2194345477U)
					{
						if (num2 != 2137771011U)
						{
							if (num2 != 2194345477U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "fadeOut"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.imageCover.color = ((CS$<>8__locals1.p1 != null && CS$<>8__locals1.p1 == "white") ? Color.white : Color.black);
								CS$<>8__locals1.<>4__this.cgCover.SetActive(true);
								CS$<>8__locals1.<>4__this.cgCover.alpha = 0f;
								CS$<>8__locals1.<>4__this.cgCover.DOFade(1f, CS$<>8__locals1.p0f).SetDelay(0.1f);
							}, CS$<>8__locals1.p0f, true);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "focusPos"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
								{
									pos = new Point(CS$<>8__locals1.p0.ToInt(), CS$<>8__locals1.p1.ToInt()),
									speed = CS$<>8__locals1.p2.IsEmpty("2").ToFloat()
								};
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 != 2212514718U)
					{
						if (num2 != 2225796703U)
						{
							if (num2 != 2301512864U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "wait"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(new DramaEventWait(CS$<>8__locals1.p0f, null));
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "startQuest"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								Quest quest = Quest.Create(CS$<>8__locals1.p0, null, null);
								if (!quest.HasDLC)
								{
									Msg.Say("(Failed DLC check)");
									return;
								}
								EMono.game.quests.Start(quest);
								LayerDrama.currentQuest = quest;
								if (CS$<>8__locals1.<>4__this.tg != null && CS$<>8__locals1.<>4__this.tg.chara != null)
								{
									string[] array2 = new string[6];
									array2[0] = "Starting Quest:";
									int num3 = 1;
									Quest quest2 = quest;
									array2[num3] = ((quest2 != null) ? quest2.ToString() : null);
									array2[2] = "/";
									int num4 = 3;
									Quest quest3 = CS$<>8__locals1.<>4__this.tg.chara.quest;
									array2[num4] = ((quest3 != null) ? quest3.ToString() : null);
									array2[4] = "/";
									array2[5] = (quest == CS$<>8__locals1.<>4__this.tg.chara.quest).ToString();
									Debug.Log(string.Concat(array2));
								}
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else
					{
						if (!(text4 == "propEnter"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							DramaProp component = Util.Instantiate("Media/Drama/Prop/" + CS$<>8__locals1.p0, null).GetComponent<DramaProp>();
							component.name = CS$<>8__locals1.p0;
							if (CS$<>8__locals1.p1.IsEmpty())
							{
								component.transform.position = EMono.scene.cam.transform.position;
							}
							else
							{
								Point point = new Point(CS$<>8__locals1.p[1].ToInt(), CS$<>8__locals1.p[2].ToInt());
								component.transform.position = *point.PositionCenter();
							}
							component.Enter();
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 <= 2704835779U)
				{
					if (num2 <= 2382385783U)
					{
						if (num2 != 2350759358U)
						{
							if (num2 != 2382385783U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "checkAffinity"))
							{
								goto IL_1F6B;
							}
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "%worship"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								LayerDrama.currentReligion.JoinFaith(EMono.pc);
								Tutorial.Reserve("faith", null);
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else if (num2 != 2406701654U)
					{
						if (num2 != 2470639475U)
						{
							if (num2 != 2704835779U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "replace"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								CS$<>8__locals1.<>4__this.textReplace = CS$<>8__locals1.text;
							}, 0f, false);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "showSkip"))
							{
								goto IL_1F6B;
							}
							this.goSkip.SetActive(true);
							goto IL_1FC4;
						}
					}
					else if (!(text4 == "choice"))
					{
						goto IL_1F6B;
					}
				}
				else if (num2 <= 2846199180U)
				{
					if (num2 != 2745269549U)
					{
						if (num2 != 2846199180U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "drop"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							Msg.Say("dropReward");
							CardBlueprint.SetNormalRarity(false);
							Thing t = ThingGen.Create(CS$<>8__locals1.p0, -1, -1);
							EMono._zone.AddCard(t, EMono.pc.pos);
						}, 0f, false);
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "lastBGM"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							EMono.Sound.StopBGM(CS$<>8__locals1.p0f, true);
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 != 2888196486U)
				{
					if (num2 != 2914142528U)
					{
						if (num2 != 3002570765U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "invoke"))
						{
							goto IL_1F6B;
						}
						if (CS$<>8__locals1.jump.IsEmpty())
						{
							this.AddEvent(delegate()
							{
								typeof(DramaOutcome).GetMethod(CS$<>8__locals1.p[0]).Invoke(CS$<>8__locals1.<>4__this.outcome, null);
							}, 0f, false);
							goto IL_1FC4;
						}
						this.AddEvent(delegate()
						{
						}, delegate()
						{
							if (!(bool)typeof(DramaOutcome).GetMethod(CS$<>8__locals1.p[0]).Invoke(CS$<>8__locals1.<>4__this.outcome, null))
							{
								return "";
							}
							return CS$<>8__locals1.jump;
						});
						flag2 = true;
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "fadeIn"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							CS$<>8__locals1.<>4__this.imageCover.color = ((CS$<>8__locals1.p1 != null && CS$<>8__locals1.p1 == "white") ? Color.white : Color.black);
							CS$<>8__locals1.<>4__this.cgCover.SetActive(true);
							CS$<>8__locals1.<>4__this.cgCover.alpha = 1f;
							CS$<>8__locals1.<>4__this.cgCover.DOFade(0f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, false);
						goto IL_1FC4;
					}
				}
				else
				{
					if (!(text4 == "destroyItem"))
					{
						goto IL_1F6B;
					}
					this.AddEvent(delegate()
					{
						EMono.pc.things.Find(CS$<>8__locals1.p0, -1, -1).Destroy();
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (num2 <= 3475118372U)
			{
				if (num2 <= 3244494447U)
				{
					if (num2 <= 3083595508U)
					{
						if (num2 != 3036235398U)
						{
							if (num2 != 3083595508U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "inject"))
							{
								goto IL_1F6B;
							}
							DramaEventTalk dramaEventTalk = this.lastTalk;
							if (this.idDefaultPassed)
							{
								this.AddCustomEvents(CS$<>8__locals1.p0);
							}
							this.lastTalk = dramaEventTalk;
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "enableTone"))
							{
								goto IL_1F6B;
							}
							this.enableTone = true;
							goto IL_1FC4;
						}
					}
					else if (num2 != 3180049141U)
					{
						if (num2 != 3202888471U)
						{
							if (num2 != 3244494447U)
							{
								goto IL_1F6B;
							}
							if (!(text4 == "editPlaylist"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								List<int> list = new List<int>();
								foreach (string s in CS$<>8__locals1.p)
								{
									list.Add(int.Parse(s));
								}
								EMono._zone.SetBGM(list, true);
							}, 0f, false);
							goto IL_1FC4;
						}
						else
						{
							if (!(text4 == "propLeave"))
							{
								goto IL_1F6B;
							}
							this.AddEvent(delegate()
							{
								GameObject.Find(CS$<>8__locals1.p0).GetComponent<DramaProp>().Leave();
							}, 0f, false);
							goto IL_1FC4;
						}
					}
					else
					{
						if (!(text4 == "shake"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							Shaker.ShakeCam("default", 1f);
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 <= 3294324549U)
				{
					if (num2 != 3264522692U)
					{
						if (num2 != 3294324549U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "destroy"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							Chara chara = EMono._map.FindChara(CS$<>8__locals1.p0);
							if (chara != null)
							{
								chara.Destroy();
								return;
							}
							Debug.Log("Drama.destroy chara not found:" + CS$<>8__locals1.p0);
						}, 0f, false);
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "topic"))
						{
							goto IL_1F6B;
						}
						this.customTalkTopics[CS$<>8__locals1.p0] = CS$<>8__locals1.text;
						goto IL_1FC4;
					}
				}
				else if (num2 != 3439296072U)
				{
					if (num2 != 3465848593U)
					{
						if (num2 != 3475118372U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "effectEmbarkOut"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							UnityEngine.Object.FindObjectOfType<EmbarkActor>().Hide();
						}, 0f, false);
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "stopBGM"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							LayerDrama.haltPlaylist = true;
							EMono.Sound.StopBGM(CS$<>8__locals1.p0f, false);
							EMono.Sound.currentBGM = null;
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else
				{
					if (!(text4 == "save"))
					{
						goto IL_1F6B;
					}
					this.AddEvent(delegate()
					{
						EMono.game.Save(false, null, true);
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (num2 <= 3798489521U)
			{
				if (num2 <= 3666392194U)
				{
					if (num2 != 3591781101U)
					{
						if (num2 != 3666392194U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "saveBGM"))
						{
							goto IL_1F6B;
						}
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "disableFullPortrait"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							CS$<>8__locals1.<>4__this.sequence.fullPortrait = false;
						}, 0f, false);
						goto IL_1FC4;
					}
				}
				else if (num2 != 3712584270U)
				{
					if (num2 != 3742116480U)
					{
						if (num2 != 3798489521U)
						{
							goto IL_1F6B;
						}
						if (!(text4 == "BGM"))
						{
							goto IL_1F6B;
						}
						this.AddEvent(delegate()
						{
							LayerDrama.haltPlaylist = true;
							LayerDrama.maxBGMVolume = true;
							EMono.Sound.PlayBGM(EMono.core.refs.dictBGM[CS$<>8__locals1.p0.ToInt()], 0f, 0f);
							CS$<>8__locals1.<>4__this.bgmChanged = true;
						}, 0f, false);
						goto IL_1FC4;
					}
					else
					{
						if (!(text4 == "fadeEnd"))
						{
							goto IL_1F6B;
						}
						if (CS$<>8__locals1.p0f == 0f)
						{
							CS$<>8__locals1.p0f = 1f;
						}
						this.AddEvent(delegate()
						{
							CS$<>8__locals1.<>4__this.imageCover.color = Color.black;
							CS$<>8__locals1.<>4__this.cgCover.SetActive(true);
							CS$<>8__locals1.<>4__this.cgCover.alpha = 0f;
							CS$<>8__locals1.<>4__this.cgCover.DOFade(1f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, true);
						this.AddEvent(delegate()
						{
							CS$<>8__locals1.<>4__this.dialog.SetActive(false);
							CS$<>8__locals1.<>4__this.imageBG.enabled = false;
						}, 0f, false);
						this.AddEvent(delegate()
						{
							DOTween.Kill(CS$<>8__locals1.<>4__this.cg, false);
							CS$<>8__locals1.<>4__this.cg.alpha = 1f;
							CS$<>8__locals1.<>4__this.cg.DOFade(0f, CS$<>8__locals1.p0f).SetDelay(0.1f);
						}, CS$<>8__locals1.p0f, true);
						goto IL_1FC4;
					}
				}
				else
				{
					if (!(text4 == "focusPC"))
					{
						goto IL_1F6B;
					}
					this.AddEvent(delegate()
					{
						EMono.scene.screenElin.focusOption = new BaseGameScreen.FocusOption
						{
							player = true,
							speed = CS$<>8__locals1.p0.IsEmpty("2").ToFloat()
						};
					}, 0f, false);
					goto IL_1FC4;
				}
			}
			else if (num2 <= 3984383372U)
			{
				if (num2 != 3901313050U)
				{
					if (num2 != 3984383372U)
					{
						goto IL_1F6B;
					}
					if (!(text4 == "reload"))
					{
						goto IL_1F6B;
					}
					string __step = "flag" + this.countLine.ToString();
					this.AddEvent(delegate()
					{
						CS$<>8__locals1.<>4__this.Load();
						CS$<>8__locals1.<>4__this.sequence.Play(CS$<>8__locals1.jump.IsEmpty(__step));
					}, 0.01f, true);
					this.sequence.AddStep(__step);
					goto IL_1FC4;
				}
				else
				{
					if (!(text4 == "_choices"))
					{
						goto IL_1F6B;
					}
					using (List<DramaChoice>.Enumerator enumerator = this._choices.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							DramaChoice choice = enumerator.Current;
							this.lastTalk.AddChoice(choice);
						}
						goto IL_1FC4;
					}
				}
			}
			else if (num2 != 4018229058U)
			{
				if (num2 != 4097442686U)
				{
					if (num2 != 4121006852U)
					{
						goto IL_1F6B;
					}
					if (!(text4 == "setAlwaysVisible"))
					{
						goto IL_1F6B;
					}
					this.AddEvent(delegate()
					{
						LayerDrama.alwaysVisible = EMono._map.FindChara(CS$<>8__locals1.p0);
					}, 0f, false);
					goto IL_1FC4;
				}
				else
				{
					if (!(text4 == "screenLock"))
					{
						goto IL_1F6B;
					}
					this.layer.ShowScreenLock(CS$<>8__locals1.p0);
					goto IL_1FC4;
				}
			}
			else
			{
				if (!(text4 == "canSkip"))
				{
					goto IL_1F6B;
				}
				this.AddEvent(delegate()
				{
					CS$<>8__locals1.<>4__this.sequence.skipJump = CS$<>8__locals1.p0;
				}, 0f, false);
				goto IL_1FC4;
			}
			if (this.CheckIF(text3) && this.CheckIF(@if))
			{
				if (array.Length > 1)
				{
					text4 = array[1];
					num2 = <PrivateImplementationDetails>.ComputeStringHash(text4);
					if (num2 <= 1362521689U)
					{
						if (num2 <= 232707651U)
						{
							if (num2 != 164679485U)
							{
								if (num2 == 232707651U)
								{
									if (text4 == "depart")
									{
										CS$<>8__locals1.text = "depart".lang();
										CS$<>8__locals1.jump = "_depart";
									}
								}
							}
							else if (text4 == "quest")
							{
								CS$<>8__locals1.text = "deQuest".lang();
								CS$<>8__locals1.jump = "_quest";
							}
						}
						else if (num2 != 415325326U)
						{
							if (num2 == 1362521689U)
							{
								if (text4 == "trade")
								{
									CS$<>8__locals1.text = "daTrade".lang();
									CS$<>8__locals1.jump = "_trade";
								}
							}
						}
						else if (text4 == "rumor")
						{
							CS$<>8__locals1.text = "letsTalk".lang();
							CS$<>8__locals1.jump = "_rumor";
						}
					}
					else if (num2 <= 1708488555U)
					{
						if (num2 != 1583061253U)
						{
							if (num2 == 1708488555U)
							{
								if (text4 == "buy")
								{
									CS$<>8__locals1.text = "daBuy".lang();
									CS$<>8__locals1.jump = "_buy";
								}
							}
						}
						else if (text4 == "sell")
						{
							CS$<>8__locals1.text = "daSell".lang();
							CS$<>8__locals1.jump = "_sell";
						}
					}
					else if (num2 != 1911791459U)
					{
						if (num2 == 2571906332U)
						{
							if (text4 == "give")
							{
								CS$<>8__locals1.text = "daGive".lang();
								CS$<>8__locals1.jump = "_give";
							}
						}
					}
					else if (text4 == "bye")
					{
						CS$<>8__locals1.text = "bye".lang();
						CS$<>8__locals1.jump = "_bye";
					}
				}
				flag2 = true;
				this.lastTalk.AddChoice(new DramaChoice(CS$<>8__locals1.text, CS$<>8__locals1.jump, CS$<>8__locals1.p0, check, text3));
				goto IL_1FC4;
			}
			goto IL_1FC4;
		}
		IL_1F6B:
		if (flag)
		{
			this.lastTalk = (this.AddEvent(new DramaEventTalk(CS$<>8__locals1.actor, delegate()
			{
				if (!CS$<>8__locals1.<>4__this.textReplace.IsEmpty())
				{
					CS$<>8__locals1.text = CS$<>8__locals1.<>4__this.textReplace;
					CS$<>8__locals1.<>4__this.textReplace = null;
				}
				if (CS$<>8__locals1.<>4__this.tg != null && (CS$<>8__locals1.actor == "tg" || CS$<>8__locals1.actor.IsEmpty()))
				{
					CS$<>8__locals1.text = CS$<>8__locals1.<>4__this.tg.ApplyTone(CS$<>8__locals1.text);
				}
				return CS$<>8__locals1.text;
			})) as DramaEventTalk);
			this.lastTalk.center = (CS$<>8__locals1.p0 == "center");
		}
		IL_1FC4:
		if (!string.IsNullOrEmpty(CS$<>8__locals1.jump) && !flag2)
		{
			this.AddEvent(new DramaEventGoto(CS$<>8__locals1.jump));
		}
	}

	public DramaActor GetActor(string id)
	{
		return this.sequence.GetActor(id);
	}

	public DramaActor tgActor
	{
		get
		{
			return this.sequence.GetActor("tg");
		}
	}

	public DramaEvent AddEvent(DramaEvent e)
	{
		return this.sequence.AddEvent(e);
	}

	public DramaEventMethod AddEvent(Action func, float duration = 0f, bool halt = false)
	{
		return this.AddEvent(new DramaEventMethod(func, duration, halt)) as DramaEventMethod;
	}

	public DramaEventMethod AddEvent(Action func, Func<string> funcJump)
	{
		return this.AddEvent(new DramaEventMethod(func, 0f, false)
		{
			jumpFunc = funcJump
		}) as DramaEventMethod;
	}

	public DramaEvent CustomEvent(Action func, string step = null, float duration = 0f, bool halt = false)
	{
		DramaEventMethod dramaEventMethod = new DramaEventMethod(func, duration, halt)
		{
			step = step
		};
		this.customEvents.Add(dramaEventMethod);
		return dramaEventMethod;
	}

	public void SetDialog(string id = "Default")
	{
		foreach (DialogDrama dialogDrama in base.GetComponentsInChildren<DialogDrama>(true))
		{
			if (dialogDrama.portrait)
			{
				dialogDrama.portrait.imageFull.SetActive(false);
			}
			dialogDrama.SetActive(false);
			if (dialogDrama.name == "Dialog" + id)
			{
				this.dialog = dialogDrama;
			}
		}
	}

	public bool CheckIF(string IF)
	{
		if (IF == null || IF.IsEmpty())
		{
			return true;
		}
		string[] array = IF.Split(',', StringSplitOptions.None);
		string text = array[0];
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 2044268227U)
		{
			if (num <= 439160533U)
			{
				if (num <= 154126619U)
				{
					if (num != 98859158U)
					{
						if (num == 154126619U)
						{
							if (text == "letterTrial")
							{
								return Guild.CurrentQuest is QuestGuildMage && Guild.CurrentQuest.phase == 0 && EMono.pc.things.Find("letter_trial", -1, -1) != null;
							}
						}
					}
					else if (text == "costRecruit")
					{
						return EMono.Branch == null || !EMono.Branch.IsRecruit(this.tg.chara) || EMono.pc.GetCurrency("money2") >= CalcGold.Hire(this.tg.chara);
					}
				}
				else if (num != 358681176U)
				{
					if (num == 439160533U)
					{
						if (text == "merchant")
						{
							return Guild.Current == Guild.Merchant;
						}
					}
				}
				else if (text == "hasDLC")
				{
					return Steam.HasDLC(array[1].ToEnum(true));
				}
			}
			else if (num <= 984623250U)
			{
				if (num != 614694486U)
				{
					if (num == 984623250U)
					{
						if (text == "sister_money")
						{
							return EMono.pc.GetCurrency("money") >= 10000;
						}
					}
				}
				else if (text == "!hasFlag")
				{
					return EMono.player.dialogFlags.TryGetValue(array[1], 0) == 0;
				}
			}
			else if (num != 1239527074U)
			{
				if (num != 2036404098U)
				{
					if (num == 2044268227U)
					{
						if (text == "thief")
						{
							return Guild.Current == Guild.Thief;
						}
					}
				}
				else if (text == "fighter")
				{
					return Guild.Current == Guild.Fighter;
				}
			}
			else if (text == "fiamaPet")
			{
				if (EMono.pc.homeBranch != null)
				{
					foreach (Chara chara in EMono.pc.homeBranch.members)
					{
						if (chara.isDead && chara.GetInt(100, null) != 0)
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}
		else if (num <= 3303594084U)
		{
			if (num <= 2050511405U)
			{
				if (num != 2045191971U)
				{
					if (num == 2050511405U)
					{
						if (text == "hasFlag")
						{
							return EMono.player.dialogFlags.TryGetValue(array[1], 0) != 0;
						}
					}
				}
				else if (text == "guild_promote")
				{
					return Guild.Current.relation.exp >= Guild.Current.relation.ExpToNext;
				}
			}
			else if (num != 2247815201U)
			{
				if (num == 3303594084U)
				{
					if (text == "hasItem")
					{
						return EMono.pc.things.Find(array[1], -1, -1) != null;
					}
				}
			}
			else if (text == "hasMelilithCurse")
			{
				return EMono.player.flags.gotMelilithCurse;
			}
		}
		else if (num <= 3447015858U)
		{
			if (num != 3408510114U)
			{
				if (num == 3447015858U)
				{
					if (text == "costRecruitTicket")
					{
						return EMono.Branch != null && EMono.Branch.IsRecruit(this.tg.chara) && EMono.pc.things.Find("ticket_resident", -1, -1) != null;
					}
				}
			}
			else if (text == "isCompleted")
			{
				return EMono.game.quests.IsCompleted(array[1]);
			}
		}
		else if (num != 3634784398U)
		{
			if (num != 3785939139U)
			{
				if (num == 3914661895U)
				{
					if (text == "guild_maxpromote")
					{
						return Guild.Current.relation.rank >= Guild.Current.relation.MaxRank;
					}
				}
			}
			else if (text == "mage")
			{
				return Guild.Current == Guild.Mage;
			}
		}
		else if (text == "scratch_check")
		{
			return EMono.game.dateScratch > EMono.game.world.date.GetRaw(0);
		}
		if (array.Length > 2)
		{
			int num2;
			bool flag = EMono.player.dialogFlags.TryGetValue(array[1], out num2);
			int num4;
			if (array[1].StartsWith("*"))
			{
				string text2 = array[1].TrimStart('*');
				Quest quest = EMono.game.quests.Get(text2);
				if (text2 == "guild")
				{
					quest = Guild.CurrentQuest;
				}
				flag = (quest != null);
				if (EMono.game.quests.completedIDs.Contains(text2))
				{
					num2 = 999;
					flag = true;
				}
				else if (quest == null)
				{
					num2 = -1;
				}
				else
				{
					num2 = quest.phase;
				}
				int num3;
				if (int.TryParse(array[2], out num3))
				{
					num4 = num3;
				}
				else
				{
					num4 = ((quest != null) ? array[2].ToInt(quest.GetType()) : 0);
				}
			}
			else
			{
				num4 = int.Parse(array[2]);
			}
			text = array[0];
			if (!(text == "="))
			{
				if (!(text == "!"))
				{
					if (!(text == ">"))
					{
						if (!(text == ">="))
						{
							if (!(text == "<"))
							{
								if (text == "<=")
								{
									if (!flag || num2 <= num4)
									{
										return true;
									}
								}
							}
							else if (!flag || num2 < num4)
							{
								return true;
							}
						}
						else if (flag && num2 >= num4)
						{
							return true;
						}
					}
					else if (flag && num2 > num4)
					{
						return true;
					}
				}
				else if (!flag || num2 != num4)
				{
					return true;
				}
			}
			else
			{
				if (!flag && num4 == -1)
				{
					return true;
				}
				if (flag && num2 == num4)
				{
					return true;
				}
			}
			return false;
		}
		return this.setup.tag == IF;
	}

	public void Show()
	{
		SE.PopDrama();
		this.layer.SetActive(true);
		this.layer.cg.alpha = 0f;
		this.layer.cg.DOFade(1f, 0.3f);
	}

	public void Hide()
	{
		this.layer.SetActive(false);
	}

	public static Chara TG;

	public LayerDrama layer;

	public CanvasGroup cg;

	public CanvasGroup cgCover;

	public UIDynamicList listCredit;

	public Transform actorPos;

	public Transform endroll;

	public DramaActor moldActor;

	public DramaSequence sequence;

	public DramaOutcome outcome;

	public DialogDrama dialog;

	public DramaSetup setup;

	public GameObject goSkip;

	public Image imageBG;

	public Image imageCover;

	public Font[] fonts;

	public Person tg;

	public bool bgmChanged;

	private List<DramaEvent> customEvents = new List<DramaEvent>();

	public List<DramaChoice> _choices = new List<DramaChoice>();

	public Dictionary<string, string> customTalkTopics = new Dictionary<string, string>();

	public Dictionary<string, string> dictLocalize = new Dictionary<string, string>();

	public float creditSpeed;

	private string lastIF;

	private string lastIF2;

	public DramaEventTalk lastTalk;

	public bool enableTone;

	public bool customEventsAdded;

	public bool idDefaultPassed;

	public int countLine;

	private string idDefault;

	private string textReplace;

	public static Dictionary<string, ExcelData> dictCache = new Dictionary<string, ExcelData>();
}
