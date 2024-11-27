using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInspector : ELayer
{
	public bool IsActive
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public void SetVisible(bool enable)
	{
		base.gameObject.SetActive(enable && this.group != null);
		if (enable && this.group != null)
		{
			this.Refresh();
		}
	}

	public void Hide()
	{
		this.group = null;
		this.point = null;
		base.gameObject.SetActive(false);
	}

	public bool InspectUnderMouse()
	{
		if (!Scene.HitPoint.IsValid)
		{
			return false;
		}
		List<IInspect> list = ELayer.scene.mouseTarget.pos.ListInspectorTargets();
		this.point = Scene.HitPoint;
		IList<IInspect> source = list;
		InspectGroup inspectGroup = this.group;
		this.Inspect(source.NextItem((inspectGroup != null) ? inspectGroup.FirstTarget : null));
		return true;
	}

	public void Inspect(IInspect newTarget)
	{
		InspectGroup inspectGroup = this.group;
		if (((inspectGroup != null) ? inspectGroup.FirstTarget : null) == newTarget)
		{
			return;
		}
		this.Inspect(InspectGroup.Create(newTarget));
	}

	public void Inspect(InspectGroup g)
	{
		base.gameObject.SetActive(true);
		if (!this.initialized)
		{
			this.windows.Add(this.windowChara.window);
			this.initialized = true;
			this.mold = this.layoutButtons.CreateMold(null);
			IUISkin[] componentsInChildren = base.transform.GetComponentsInChildren<IUISkin>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ApplySkin();
			}
			base.InitInspector();
		}
		this.group = g;
		this.soundPop.Play();
		this.Refresh();
	}

	public void OnUpdate()
	{
		if (this.group == null || !this.IsActive)
		{
			return;
		}
		if (!this.group.CanInspect())
		{
			this.Hide();
			return;
		}
		foreach (IInspect i in this.group.targets)
		{
			this.DrawHighlight(i, true);
		}
	}

	public void Refresh()
	{
		if (!this.group.CanInspect())
		{
			this.Hide();
			return;
		}
		this.group.SetActions();
		this.layoutButtons.DestroyChildren(false, true);
		using (List<InspectGroup.Item>.Enumerator enumerator = this.group.actions.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				InspectGroup.Item item = enumerator.Current;
				if (this.group.Solo || item.multi)
				{
					UIButton uibutton = Util.Instantiate<UIButton>(this.mold, this.layoutButtons);
					uibutton.icon.sprite = (SpriteSheet.Get(item.idSprite.IsEmpty("icon_" + item.text)) ?? uibutton.icon.sprite);
					uibutton.mainText.SetText((item.textFunc != null) ? item.textFunc() : item.text.lang());
					uibutton.onClick.AddListener(delegate()
					{
						if (this.group.CanInspect())
						{
							if (item.sound)
							{
								this.soundPop.Play();
							}
							foreach (IInspect obj in this.group.targets)
							{
								item.action(obj);
							}
						}
						if (this.group == null || !this.group.CanInspect())
						{
							this.Hide();
							return;
						}
						this.Refresh();
					});
					uibutton.RebuildLayout(true);
				}
			}
		}
		Chara chara = this.group.FirstTarget as Chara;
		bool flag = this.group.Solo && chara != null && chara.IsHomeMember();
		Room room = null;
		if (!flag && ELayer.core.IsGameStarted && this.point != null)
		{
			room = this.point.cell.room;
		}
		bool flag2 = room != null;
		this.windowChara.SetActive(flag);
		this.rectInfo.SetActive(!flag);
		this.rectRoom.SetActive(flag2);
		this.note.Clear();
		if (flag)
		{
			this.windowChara.SetChara(chara);
		}
		else if (this.group.Solo)
		{
			this.group.FirstTarget.WriteNote(this.note, null, IInspect.NoteMode.Default, null);
		}
		else
		{
			this.note.AddHeader(this.group.GetName(), null);
			this.note.Build();
		}
		if (flag2)
		{
			UINote uinote = this.noteRoom;
			uinote.Clear();
			uinote.AddHeader("Room", null);
			uinote.AddText(room.points.Count.ToString() ?? "", FontColor.DontChange);
			uinote.Build();
		}
		this.layoutButtons.RebuildLayout(true);
		this.layoutBody.RebuildLayout(false);
		this.group.FirstTarget.OnInspect();
	}

	public unsafe void DrawHighlight(IInspect i, bool selected = false)
	{
		UIInspector.<>c__DisplayClass25_0 CS$<>8__locals1 = new UIInspector.<>c__DisplayClass25_0();
		if (i is Area || i is EloPos)
		{
			return;
		}
		CS$<>8__locals1.v = i.InspectPosition;
		Point inspectPoint = i.InspectPoint;
		if (inspectPoint == null)
		{
			return;
		}
		MeshPass meshPass = (inspectPoint.HasNonWallBlock || inspectPoint.HasBlockRecipe) ? ELayer.screen.guide.passGuideBlock : ELayer.screen.guide.passGuideFloor;
		if (i is Chara)
		{
			CS$<>8__locals1.v += ELayer.screen.pcOrbit.crystal.transform.position;
			ELayer.screen.tileMap.passIcon.Add(ref CS$<>8__locals1.v, 1f, 0f);
			return;
		}
		if (i is Thing)
		{
			Thing thing = i as Thing;
			if (thing.IsInstalled && thing.sourceCard.multisize)
			{
				if (selected)
				{
					thing.ForeachPoint(delegate(Point pos, bool main)
					{
						CS$<>8__locals1.v = *pos.Position();
						CS$<>8__locals1.v.z = CS$<>8__locals1.v.z - 0.01f;
						ELayer.screen.guide.passGuideFloor.Add(ref CS$<>8__locals1.v, 7f, 0f);
					});
				}
				return;
			}
			if (thing.TileType.IsBlockMount)
			{
				meshPass = ELayer.screen.guide.passGuideBlock;
			}
		}
		else if (i is TaskBuild)
		{
			Recipe recipe = (i as TaskBuild).recipe;
			if (recipe.MultiSize)
			{
				if (selected)
				{
					i.InspectPoint.ForeachMultiSize(recipe.W, recipe.H, delegate(Point pos, bool main)
					{
						CS$<>8__locals1.v = *pos.Position();
						CS$<>8__locals1.v.z = CS$<>8__locals1.v.z - 0.01f;
						ELayer.screen.guide.passGuideBlock.Add(ref CS$<>8__locals1.v, 7f, 0f);
					});
				}
				return;
			}
		}
		UIInspector.<>c__DisplayClass25_0 CS$<>8__locals2 = CS$<>8__locals1;
		CS$<>8__locals2.v.z = CS$<>8__locals2.v.z - 0.01f;
		meshPass.Add(ref CS$<>8__locals1.v, 7f, 0f);
	}

	public InspectGroup group;

	public LayoutGroup layoutBody;

	public LayoutGroup layoutButtons;

	public UIButton mold;

	public Vector3 offset;

	public Vector3 modPos;

	public SoundData soundPop;

	public WindowChara windowChara;

	public RectTransform rectInfo;

	public RectTransform rectRoom;

	public UINote note;

	public UINote noteRoom;

	public UIText textName;

	public UIImage bg;

	public Point point;

	private bool initialized;
}
