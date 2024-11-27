using System;
using System.Collections.Generic;
using UnityEngine;

public class LayerEditPCC : ELayer
{
	public bool UniformMode
	{
		get
		{
			return this.mode == UIPCC.Mode.UniformM || this.mode == UIPCC.Mode.UniformF;
		}
	}

	public bool IsMale
	{
		get
		{
			return this.mode == UIPCC.Mode.UniformM;
		}
	}

	public void Activate(Chara _chara, UIPCC.Mode m, string idUniform = null, Action _onKill = null)
	{
		this.chara = (_chara.IsPCC ? _chara : ELayer.pc);
		this.mode = m;
		this.uiPCC.exportPath = CorePath.user + "PCC/";
		this.uiPCC.onShowPalette = delegate(PCC pcc, string id, PCC.Part part, TestActor actor)
		{
			LayerColorPicker layerColorPicker = ELayer.ui.AddLayer<LayerColorPicker>();
			Color color = pcc.data.GetColor(id);
			layerColorPicker.SetColor(color, color, delegate(PickerState state, Color _c)
			{
				pcc.data.SetColor(id, _c);
				actor.Reset();
			});
		};
		PCCData newData = new PCCData();
		newData.Set(this.chara.pccData);
		this.backup = IO.DeepCopy<PCCData>(newData);
		this.backupPortrait = this.chara.c_idPortrait;
		if (this.UniformMode)
		{
			ELayer.game.uniforms.Apply(newData, idUniform, this.IsMale, false);
			this.windows[0].SetCaption("_uniform".lang(ELayer.sources.jobs.map[idUniform].GetName().ToTitleCase(false), this.IsMale ? "male".lang() : "female".lang(), null, null, null));
		}
		else
		{
			ELayer.game.uniforms.Apply(newData, PCCUniformManager.dummy);
		}
		this.uiPCC.Init(newData, this.mode);
		this.SetPortraitSlider();
		this.windows[0].AddBottomButton("removeAllCloth", delegate
		{
			this.uiPCC.RemoveAll();
			ELayer.Sound.Play("offering");
		}, false);
		this.windows[0].AddBottomButton("reset", delegate
		{
			if (this.UniformMode)
			{
				ELayer.game.uniforms.RemoveUniform(newData);
			}
			this.uiPCC.Reset(this.backup);
			this.chara.c_idPortrait = this.backupPortrait;
			this.SetPortraitSlider();
			ELayer.Sound.Play("offering");
			newData.colors.Clear();
		}, false);
		if (this.UniformMode)
		{
			PCCData otherUni = ELayer.game.uniforms.GetUniform(idUniform, !this.IsMale);
			if (otherUni != null)
			{
				this.windows[0].AddBottomButton("copyUniform", delegate
				{
					PCCData pccdata = new PCCData();
					pccdata.Set(this.chara.pccData);
					ELayer.game.uniforms.Apply(pccdata, otherUni);
					this.uiPCC.Reset(pccdata);
					ELayer.Sound.Play("offering");
				}, false);
			}
		}
		this.windows[2].SetActive(true);
		this.portrait.SetActive(!this.UniformMode);
		if (_onKill != null)
		{
			base.SetOnKill(_onKill);
		}
	}

	public void Apply()
	{
		PCCData data = this.uiPCC.pcc.data;
		ELayer.game.uniforms.Apply(data, this.chara.job.id, this.IsMale, true);
		this.chara.pccData.Set(data);
		PCC.Get(this.chara.pccData).Build(false);
		this.chara.SetInt(105, IntColor.ToInt(this.chara.pccData.GetHairColor(false)));
		this.applied = true;
	}

	public override void OnKill()
	{
		this.Apply();
	}

	private void Update()
	{
		Color hairColor = this.uiPCC.actor.data.GetHairColor(false);
		if (hairColor != this.lastHair)
		{
			this.portrait.SetChara(this.chara, this.uiPCC.actor.data);
			this.lastHair = hairColor;
		}
		int wheel = EInput.wheel;
		if (InputModuleEX.IsPointerOver(this.windows[1].transform))
		{
			if (wheel < 0)
			{
				this.uiPCC.actor.provider.NextDir(15f);
			}
			if (wheel > 0)
			{
				this.uiPCC.actor.provider.PrevDir();
			}
		}
	}

	public void SetPortraitSlider()
	{
		if (!this.sliderPortrait)
		{
			return;
		}
		List<ModItem<Sprite>> list = Portrait.ListPlayerPortraits(this.chara.bio.gender, this.chara != ELayer.pc);
		list.Sort((ModItem<Sprite> a, ModItem<Sprite> b) => Lang.comparer.Compare(a.id, b.id));
		this.sliderPortrait.SetList<ModItem<Sprite>>(list.Find((ModItem<Sprite> a) => a.id == this.chara.c_idPortrait), list, delegate(int a, ModItem<Sprite> b)
		{
			this.chara.c_idPortrait = b.id;
			this.portrait.SetChara(this.chara, this.uiPCC.actor.data);
		}, delegate(ModItem<Sprite> a)
		{
			if (!a.id.IsEmpty())
			{
				return a.id;
			}
			return "None";
		});
	}

	public UIPCC.Mode mode;

	public UIPCC uiPCC;

	public Portrait portrait;

	public Chara chara;

	public UISlider sliderPortrait;

	public PCCData backup;

	public string backupPortrait;

	public bool applied;

	private Color lastHair;
}
