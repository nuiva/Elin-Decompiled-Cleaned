using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayerEmbark : ELayer
{
	public GameBlueprint bp
	{
		get
		{
			return ELayer.game.bp;
		}
	}

	public LayerTitle title
	{
		get
		{
			return LayerTitle.Instance;
		}
	}

	public override void OnAfterAddLayer()
	{
		LayerEmbark.Instance = this;
		if (ELayer.game == null)
		{
			Game.Create(null);
		}
		if (this.bp.map != null)
		{
			this.mapPreview.SetMap(this.bp.map);
		}
		if (!LayerTitle.actor)
		{
			LayerTitle.actor = Util.Instantiate<EmbarkActor>(this.moldActor, null);
		}
		this.RefreshMembers();
		this.SwitchMode(LayerEmbark.Mode.top);
		ELayer.ui.AddLayer<LayerEditBio>();
	}

	public void OnClickBack()
	{
		if (this.mode == LayerEmbark.Mode.previewMap && this.lastMode != LayerEmbark.Mode.map)
		{
			SE.Click();
		}
		this.SwitchMode(LayerEmbark.Mode.top);
	}

	public void SwitchMode(int i)
	{
		this.SwitchMode(i.ToEnum<LayerEmbark.Mode>());
	}

	public void SwitchMode(LayerEmbark.Mode _mode)
	{
		bool flag = this.lastMode == LayerEmbark.Mode.map;
		if (_mode == LayerEmbark.Mode.top && this.mode == LayerEmbark.Mode.previewMap && flag)
		{
			_mode = LayerEmbark.Mode.map;
		}
		this.lastMode = this.mode;
		this.mode = _mode;
		this.goPreviewMap.SetActive(this.mode == LayerEmbark.Mode.previewMap);
		this.goTop.SetActive(this.mode == LayerEmbark.Mode.top);
		this.goMembers.SetActive(this.mode == LayerEmbark.Mode.member);
		this.buttonEmbark.SetActive(this.mode == LayerEmbark.Mode.top);
		this.goMap.SetActive(this.mode == LayerEmbark.Mode.map);
		switch (this.mode)
		{
		case LayerEmbark.Mode.top:
			this.mapPreview.transform.SetParent(this.mapHolder1, false);
			ELayer.ui.hud.hint.Show("hintEmbarkTop".lang(), false);
			this.selector.WriteNote(ELayer.player.zone);
			return;
		case LayerEmbark.Mode.previewMap:
			SE.Play("click_paper");
			ELayer.ui.hud.hint.Show("hintEmbarkPreview".lang(), false);
			this.RerollPreviewMap();
			return;
		case LayerEmbark.Mode.member:
			ELayer.ui.AddLayer<LayerEditBio>().SetChara(null, delegate
			{
				this.RefreshMembers();
				this.SwitchMode(LayerEmbark.Mode.top);
			});
			return;
		case LayerEmbark.Mode.map:
			this.mapPreview.transform.SetParent(this.mapHolder2, false);
			ELayer.ui.hud.hint.Show("hintEmbarkMap".lang(), false);
			return;
		default:
			return;
		}
	}

	public override bool OnBack()
	{
		if (this.mode == LayerEmbark.Mode.map)
		{
			return false;
		}
		if (this.mode != LayerEmbark.Mode.top)
		{
			this.SwitchMode(LayerEmbark.Mode.top);
			return false;
		}
		return base.OnBack();
	}

	private void Update()
	{
		if (this.mode == LayerEmbark.Mode.map)
		{
			ActionMode.Title.InputMovement();
		}
	}

	public override void OnKill()
	{
	}

	public void RerollPreviewMap()
	{
		if (this.firstPreview)
		{
			for (int i = 0; i < 8; i++)
			{
				UIMapPreview p = Util.Instantiate<UIMapPreview>(this.moldPreview, this.gridPreview);
				this.previews.Add(p);
				p.button.onClick.AddListener(delegate()
				{
					if (!p.thread.done)
					{
						SE.Beep();
						return;
					}
					this.OnClickPreview(p);
				});
			}
			this.firstPreview = false;
		}
		foreach (UIMapPreview uimapPreview in this.previews)
		{
			uimapPreview.GenerateMap(this.bp);
		}
	}

	public void OnClickPreview(UIMapPreview preview)
	{
		this.bp.map = preview.map;
		this.bp.genSetting = preview.thread.bp.genSetting;
		this.mapPreview.SetMap(this.bp.map);
		this.SwitchMode(LayerEmbark.Mode.top);
	}

	public void RefreshMembers()
	{
		this.listMembers.Clear();
		this.listMembers.callbacks = new UIList.Callback<Chara, ButtonChara>
		{
			onInstantiate = delegate(Chara a, ButtonChara b)
			{
				b.SetChara(a, ButtonChara.Mode.Embark);
				a.elements.ListBestSkills();
				b.item.button2.SetActive(a == ELayer.player.chara);
				b.item.button1.SetActive(a == ELayer.player.chara);
				b.item.button1.onClick.AddListener(delegate()
				{
					ELayer.ui.AddLayer<LayerEditBio>().SetChara(null, delegate
					{
						this.RefreshMembers();
					});
				});
			}
		};
		foreach (Chara chara in this.bp.charas)
		{
			if (chara.IsPCC)
			{
				this.listMembers.Add(chara);
			}
		}
		this.listMembers.Refresh(false);
	}

	public void RerollPC()
	{
		this.bp.charas.Remove(ELayer.player.chara);
		this.bp.charas.Insert(0, ELayer.player.chara);
		(this.listMembers.buttons[0].component as ButtonChara).SetChara(ELayer.player.chara, ButtonChara.Mode.Embark);
	}

	public void RerollMembers()
	{
		this.bp.RerollChara();
		this.RefreshMembers();
	}

	public string GetAlias()
	{
		if (ELayer.rnd(4) == 0)
		{
			return ELayer.player.title;
		}
		return this.bp.charas.RandomItem<Chara>().Name;
	}

	public void RerollPlayerAlias()
	{
		ELayer.player.title = WordGen.Get("title");
	}

	public void RerollPlayerName()
	{
		ELayer.player.chara.c_altName = NameGen.getRandomName();
	}

	public void OnEndEditPlayerName()
	{
		ELayer.player.chara.c_altName = this.inputPlayerName.text;
	}

	public void ListPlayerAlias()
	{
		ELayer.ui.AddLayer<LayerList>().SetStringList(() => WordGen.GetList("title"), delegate(int a, string b)
		{
			ELayer.player.title = b;
		}, true);
	}

	public static LayerEmbark Instance;

	public UIMapPreview mapPreview;

	public GameObject goTop;

	public GameObject goPreviewMap;

	public GameObject goMembers;

	public GameObject goMap;

	public UICharaMaker maker;

	public UIMapSelector selector;

	public UIList listMembers;

	public InputField inputHomeName;

	public InputField inputPlayerAlias;

	public InputField inputSupplyType;

	public InputField inputRegion;

	public InputField inputEra;

	public InputField inputPlayerName;

	public UICardInfo cardInfo;

	public UIButton buttonRerollSupply;

	public UIButton toggleSkipQuests;

	public UIButton buttonEmbark;

	public UISelectableGroup groupGameMode;

	public RectTransform mapHolder1;

	public RectTransform mapHolder2;

	public LayerEmbark.Mode mode;

	public LayerEmbark.Mode lastMode;

	public EmbarkActor moldActor;

	public GridLayoutGroup gridPreview;

	public UIMapPreview moldPreview;

	public List<UIMapPreview> previews;

	private bool firstPreview = true;

	public enum Mode
	{
		top,
		previewMap,
		member,
		map
	}
}
