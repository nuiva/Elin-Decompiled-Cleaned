using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AI_PlayMusic : AIAct
{
	public static void CancelKeepPlaying()
	{
		if (AI_PlayMusic.keepPlaying)
		{
			SE.CancelAction();
			AI_PlayMusic.keepPlaying = false;
			AI_PlayMusic.playingTool = null;
		}
	}

	public override bool ShowProgress
	{
		get
		{
			return false;
		}
	}

	public override bool CancelWhenDamaged
	{
		get
		{
			return !AI_PlayMusic.ignoreDamage;
		}
	}

	public override TargetType TargetType
	{
		get
		{
			if (this.tool != null && this.tool != EClass.pc.Tool)
			{
				return TargetType.Any;
			}
			return TargetType.Self;
		}
	}

	public override IEnumerable<AIAct.Status> Run()
	{
		if (this.tool == null)
		{
			this.tool = (this.owner.IsPC ? this.owner.Tool : this.owner.things.Find<TraitToolMusic>());
		}
		if (this.tool == null && !this.owner.IsPCFaction && EClass.rnd(20) == 0)
		{
			this.tool = this.owner.AddThing(ThingGen.Create("lute", -1, -1), true, -1, -1);
		}
		if (this.tool == null)
		{
			yield return this.Cancel();
		}
		if (this.tool.parent is Zone)
		{
			yield return base.DoGoto(this.tool.pos, this.tool.pos.IsBlocked ? 1 : 0, false, null);
		}
		if (this.owner.IsPCC && EClass.game.config.preference.keepPlayingMusic)
		{
			AI_PlayMusic.keepPlaying = true;
			AI_PlayMusic.playingTool = this.tool;
			EInput.Consume(true, 10);
		}
		this.toolLv = 1;
		string idSong = null;
		KnownSong song = null;
		this.playing = null;
		this.score = 40;
		this.gold = 0;
		if (this.owner.IsPC && EClass.player.playingSong != null && EClass.player.playingSong.idTool == this.tool.id)
		{
			this.playing = EClass.player.playingSong;
			idSong = EClass.player.playingSong.id;
			song = EClass.player.knownSongs[EClass.player.playingSong.id];
		}
		if (this.playing == null)
		{
			string id = this.tool.id;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
			if (num <= 3078472054U)
			{
				if (num <= 1172811742U)
				{
					if (num <= 390330866U)
					{
						if (num != 63985367U)
						{
							if (num != 390330866U)
							{
								goto IL_627;
							}
							if (!(id == "harpsichord"))
							{
								goto IL_627;
							}
							idSong = "harpsichord_goldberg";
							goto IL_637;
						}
						else
						{
							if (!(id == "taiko"))
							{
								goto IL_627;
							}
							idSong = "taiko";
							goto IL_637;
						}
					}
					else if (num != 773192040U)
					{
						if (num != 1172811742U)
						{
							goto IL_627;
						}
						if (!(id == "piano"))
						{
							goto IL_627;
						}
						idSong = "piano_kanon";
						goto IL_637;
					}
					else
					{
						if (!(id == "guitar_ash"))
						{
							goto IL_627;
						}
						idSong = "guitar_caccini";
						goto IL_637;
					}
				}
				else if (num <= 2565162886U)
				{
					if (num != 2166422584U)
					{
						if (num != 2565162886U)
						{
							goto IL_627;
						}
						if (!(id == "trumpet"))
						{
							goto IL_627;
						}
						idSong = "trumpet_practice";
						goto IL_637;
					}
					else if (!(id == "instrument_violin"))
					{
						goto IL_627;
					}
				}
				else if (num != 2938588943U)
				{
					if (num != 3078472054U)
					{
						goto IL_627;
					}
					if (!(id == "guitar_efrond"))
					{
						goto IL_627;
					}
					idSong = "guitar_dusk";
					goto IL_637;
				}
				else
				{
					if (!(id == "guitar"))
					{
						goto IL_627;
					}
					idSong = "guitar_air";
					goto IL_637;
				}
			}
			else if (num <= 3559073827U)
			{
				if (num <= 3438029316U)
				{
					if (num != 3163500768U)
					{
						if (num != 3438029316U)
						{
							goto IL_627;
						}
						if (!(id == "piano_gould"))
						{
							goto IL_627;
						}
						idSong = "piano_gould";
						goto IL_637;
					}
					else
					{
						if (!(id == "mic"))
						{
							goto IL_627;
						}
						idSong = "mic_rachmaninoff";
						goto IL_637;
					}
				}
				else if (num != 3502455904U)
				{
					if (num != 3559073827U)
					{
						goto IL_627;
					}
					if (!(id == "panty"))
					{
						goto IL_627;
					}
				}
				else
				{
					if (!(id == "harp"))
					{
						goto IL_627;
					}
					idSong = "harp_komori";
					goto IL_637;
				}
			}
			else if (num <= 3841196189U)
			{
				if (num != 3758837070U)
				{
					if (num != 3841196189U)
					{
						goto IL_627;
					}
					if (!(id == "lute"))
					{
						goto IL_627;
					}
					idSong = "guitar_sad";
					goto IL_637;
				}
				else
				{
					if (!(id == "mokugyo"))
					{
						goto IL_627;
					}
					idSong = "mokugyo";
					goto IL_637;
				}
			}
			else if (num != 3981110062U)
			{
				if (num != 4124199536U)
				{
					if (num != 4156158084U)
					{
						goto IL_627;
					}
					if (!(id == "piano2"))
					{
						goto IL_627;
					}
					idSong = "piano_neko";
					goto IL_637;
				}
				else
				{
					if (!(id == "stradivarius"))
					{
						goto IL_627;
					}
					idSong = "violin_furusato";
					goto IL_637;
				}
			}
			else
			{
				if (!(id == "cello"))
				{
					goto IL_627;
				}
				idSong = "cello_prelude";
				goto IL_637;
			}
			idSong = "violin_chaconne";
			goto IL_637;
			IL_627:
			idSong = "violin_chaconne";
			IL_637:
			if (this.owner.IsPC && EClass.player.knownSongs.ContainsKey(idSong))
			{
				song = EClass.player.knownSongs[idSong];
			}
			if (song == null)
			{
				song = new KnownSong();
				if (this.owner.IsPC)
				{
					EClass.player.knownSongs[idSong] = song;
				}
			}
			this.playing = new PlayingSong
			{
				id = idSong,
				idTool = this.tool.id
			};
			if (this.owner.IsPC)
			{
				EClass.player.playingSong = this.playing;
			}
		}
		if (this.owner.IsPC)
		{
			(EClass.Sound.GetData("Instrument/" + idSong) as BGMData).song.index = this.playing.index;
		}
		if (Application.isEditor && this.owner.IsPC)
		{
			song.lv += 10;
			Debug.Log(song.lv);
		}
		List<Chara> reacted = new List<Chara>();
		Progress_Custom seq = new Progress_Custom
		{
			maxProgress = 15,
			cancelWhenMoved = false,
			showProgress = false,
			canProgress = (() => this.tool != null && !this.tool.isDestroyed),
			onProgressBegin = delegate()
			{
				this.owner.Say("music_start", this.owner, this.tool, null, null);
				this.owner.ShowEmo(Emo.happy, 3f, false);
				if (this.tool != null && this.tool.ExistsOnMap)
				{
					this.owner.LookAt(this.tool);
				}
				this.owner.PlayAnime(AnimeID.PlayMusic, false);
				if (this.owner.IsPC)
				{
					bool flag = false;
					if (this.playing.mistakes == 0 && this.playing.source && this.playing.source.isPlaying && this.playing.source.data.name == idSong)
					{
						SoundSource source = this.playing.source;
						if (Mathf.Abs(source.source.time - (source.data as BGMData).song.parts[this.playing.index].start) < 2f && this.playing.source.source.volume >= 0.1f)
						{
							source.KeepPlay();
							flag = true;
							Debug.Log("keep:" + source.source.time.ToString());
						}
						else
						{
							EClass.Sound.Stop(source.data, 0.1f);
						}
						Debug.Log(this.playing.source);
					}
					if (!flag)
					{
						this.playing.source = this.owner.PlaySound("Instrument/" + idSong, 1f, true);
						Debug.Log(this.playing.source);
					}
					this.playing.bgm = (this.playing.source.data as BGMData);
					BGMData.Part part = this.playing.bgm.song.parts[this.playing.index];
					if (!UISong.Instance)
					{
						Util.Instantiate<UISong>("UI/Util/UISong", EClass.ui.rectDynamicEssential);
					}
					UISong.Instance.SetSong(this.playing.source, this.playing.bgm, part);
					if (EClass.Sound.currentBGM != null)
					{
						float num2 = 0.5f - 0.1f * (float)this.playing.index;
						if (num2 < 0f)
						{
							num2 = 0f;
						}
						if (EClass.Sound.sourceBGM.volume > EClass.Sound.currentBGM.data.volume * num2)
						{
							EClass.Sound.sourceBGM.DOFade(EClass.Sound.currentBGM.data.volume * num2, 3f);
						}
						SoundManager.jingleTimer = part.duration / this.playing.bgm.pitch + this.playing.bgm.song.fadeIn + 2f;
					}
				}
			},
			onProgress = delegate(Progress_Custom p)
			{
				Msg.SetColor(Msg.colors.Ono);
				this.owner.Say(Lang.GetList("music").RandomItem<string>(), null, null);
				Msg.SetColor();
				if (EClass.debug.enable && EClass.pc.job.id == "pianist")
				{
					song.lv = 10000;
				}
				if (p.progress > 2 && (EClass.rnd(100 + 50 * this.owner.Evalue(1405)) == 0 || EClass.rnd(4 + (int)MathF.Max(0f, (float)(song.lv - this.playing.index * 25 - this.playing.index * this.playing.index / 2))) == 0))
				{
					this.playing.mistakes++;
					if (EClass.rnd(2) == 0)
					{
						base.<Run>g__LevelSong|0(2 + 2 * EClass.rnd(this.owner.Evalue(1405) + 1));
					}
					if (this.playing.mistakes >= 10)
					{
						this.playing.index = 0;
					}
					this.Cancel();
					return;
				}
				foreach (Chara chara in EClass._map.ListCharasInCircle(this.owner.pos, 4f, true))
				{
					if (chara.conSleep != null && chara.ResistLv(957) <= 0)
					{
						chara.conSleep.Kill(false);
						chara.ShowEmo(Emo.angry, 0f, true);
					}
				}
				List<Chara> list = this.owner.pos.ListWitnesses(this.owner, 4, WitnessType.music, null);
				int num2 = this.owner.Evalue(241) * (100 + this.toolLv) / 100;
				int num3 = 0;
				foreach (Chara chara2 in list)
				{
					if (this.owner == null)
					{
						break;
					}
					if (!reacted.Contains(chara2) && EClass.rnd(5) == 0)
					{
						if (this.owner.IsPCParty)
						{
							if (chara2.interest <= 0 || (EClass._zone is Zone_Music && (chara2.IsPCFaction || chara2.IsPCFactionMinion)))
							{
								continue;
							}
							chara2.interest -= EClass.rnd(10);
							if (chara2.interest < 0)
							{
								chara2.Talk("musicBored", null, null, false);
								continue;
							}
						}
						if (EClass.rnd(num3 * num3) <= 30)
						{
							bool isMinion = chara2.IsMinion;
							if (num2 < chara2.LV && EClass.rnd(2) == 0)
							{
								reacted.Add(chara2);
								if (!isMinion)
								{
									this.score -= chara2.LV / 2 - 10;
								}
								if (EClass.rnd(2) == 0)
								{
									chara2.Talk("musicBad", null, null, false);
								}
								else
								{
									chara2.Say("musicBad", chara2, this.owner, null, null);
								}
								chara2.ShowEmo(Emo.sad, 0f, true);
								this.owner.elements.ModExp(241, 10, false);
								if (EClass.rnd(5) == 0)
								{
									this.ThrowReward(chara2, true);
								}
								num3++;
							}
							else if (EClass.rnd(num2 + 5) > EClass.rnd(chara2.LV * 5 + 1))
							{
								reacted.Add(chara2);
								if (!isMinion)
								{
									this.score += EClass.rnd(chara2.LV / 2 + 5) + 5;
								}
								if (EClass.rnd(2) == 0)
								{
									chara2.Talk("musicGood", null, null, false);
								}
								else
								{
									chara2.Say("musicGood", chara2, this.owner, null, null);
								}
								chara2.ShowEmo(Emo.happy, 0f, true);
								chara2.renderer.PlayAnime((EClass.rnd(2) == 0) ? AnimeID.Jump : AnimeID.Fishing, default(Vector3), false);
								this.owner.elements.ModExp(241, EClass._zone.IsUserZone ? 10 : 50, false);
								if (!isMinion)
								{
									this.ThrowReward(chara2, false);
								}
								num3++;
							}
						}
					}
				}
				if (this.owner != null && this.owner.IsPC && EClass.rnd(80) < num3)
				{
					this.owner.stamina.Mod(-1);
				}
			},
			onProgressComplete = delegate()
			{
				if (EClass.rnd(2) == 0)
				{
					base.<Run>g__LevelSong|0(2);
				}
				if (this.playing.bgm)
				{
					this.playing.index++;
					this.playing.mistakes = 0;
					if (this.playing.index >= this.playing.bgm.song.parts.Count)
					{
						this.playing.index = 0;
					}
					this.playing.bgm.song.index = this.playing.index;
				}
				this.Evaluate(true);
			}
		}.SetDuration(26, 2);
		yield return base.Do(seq, null);
		yield break;
	}

	public void Evaluate(bool success)
	{
		if (this.owner.IsPC)
		{
			if (success)
			{
				this.score = this.score * 110 / 100;
			}
			else
			{
				this.score = this.score / 2 - 20;
			}
			int num = Mathf.Clamp(this.score / 20 + 1, 0, 9);
			this.owner.Say(Lang.GetList("music_result")[num], null, null);
			if (this.gold > 0)
			{
				this.owner.Say("music_reward", this.owner, this.gold.ToString() ?? "", null);
			}
			if (EClass.rnd(3) != 0)
			{
				this.owner.stamina.Mod(-1);
			}
			QuestMusic questMusic = EClass.game.quests.Get<QuestMusic>();
			if (questMusic != null)
			{
				questMusic.score += this.score;
				questMusic.sumMoney += this.gold;
				int num2 = num / 2 - 1;
				if (num > 0)
				{
					SE.Play("clap" + num2.ToString());
				}
			}
		}
	}

	public void ThrowReward(Chara c, bool punish)
	{
		Thing thing = null;
		int num = 1;
		string text;
		if (punish)
		{
			text = ((EClass.rnd(5) == 0) ? "rock" : "pebble");
			if (EClass.rnd(8) == 0)
			{
				text = ((EClass.rnd(3) == 0) ? "water_dirty" : "water");
			}
			if (!c.IsPCFactionOrMinion)
			{
				Thing thing2 = c.TryGetThrowable();
				thing = ((thing2 != null) ? thing2.Split(1) : null);
			}
		}
		else if (EClass.rnd(100) == 0 && !EClass._zone.IsUserZone)
		{
			text = "ecopo";
			if (EClass.rnd(4) == 0)
			{
				text = "gacha_coin";
			}
			if (EClass.rnd(4) == 0)
			{
				text = "plat";
			}
			if (EClass.rnd(3) == 0)
			{
				text = "tomato";
			}
			if (EClass.rnd(3) == 0)
			{
				text = "casino_coin";
			}
		}
		else
		{
			num = (EClass.rnd(c.LV * 2 + 1) + 1) * (100 + this.toolLv * 2 + this.owner.Evalue(1405) * 10) / 100;
			if (c.IsUnique)
			{
				num *= 2;
			}
			if (!(EClass._zone is Zone_Music))
			{
				if (num > 25)
				{
					num /= 2;
				}
				if (num > 50)
				{
					num /= 2;
				}
				if (num > 100)
				{
					num /= 2;
				}
				if (EClass._zone.IsUserZone)
				{
					num /= 5;
				}
			}
			if (num < 1)
			{
				num = 1;
			}
			this.gold += num;
			text = "money";
		}
		if (!this.owner.IsPCParty && !punish && text != "money")
		{
			return;
		}
		if (thing == null)
		{
			thing = ThingGen.Create(text, -1, -1).SetNum(num);
		}
		AI_PlayMusic.ignoreDamage = true;
		ActThrow.Throw(c, this.owner.pos, thing, punish ? ThrowMethod.Punish : ThrowMethod.Reward, 0f);
		AI_PlayMusic.ignoreDamage = false;
		if (this.owner != null && thing.ExistsOnMap)
		{
			this.owner.Pick(thing, true, true);
			if (thing.id == "money" && !this.owner.IsPCParty)
			{
				if (thing.GetRootCard() != this.owner)
				{
					thing.Destroy();
					return;
				}
				if (this.owner.GetCurrency("money") >= (this.owner.Evalue(241) * 10 + 100) / ((this.owner.memberType == FactionMemberType.Default) ? 1 : 10))
				{
					this.owner.c_allowance += num;
					this.owner.ModCurrency(-num, "money");
				}
			}
		}
	}

	public override void OnCancel()
	{
		if (this.playing != null && this.playing.bgm)
		{
			this.playing.bgm.song.Fail(this.playing.source, this.playing.bgm);
			if (UISong.Instance)
			{
				UISong.Instance.Kill();
			}
			this.playing.source = null;
			SoundManager.jingleTimer = 1f;
		}
		this.Evaluate(false);
	}

	public static bool keepPlaying;

	public static Thing playingTool;

	public Thing tool;

	public static bool ignoreDamage;

	public PlayingSong playing;

	public int score;

	public int gold;

	public int toolLv;
}
