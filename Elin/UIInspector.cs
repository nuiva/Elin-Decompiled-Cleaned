using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInspector : ELayer
{
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

	public bool IsActive => base.gameObject.activeSelf;

	public void SetVisible(bool enable)
	{
		base.gameObject.SetActive(enable && group != null);
		if (enable && group != null)
		{
			Refresh();
		}
	}

	public void Hide()
	{
		group = null;
		point = null;
		base.gameObject.SetActive(value: false);
	}

	public bool InspectUnderMouse()
	{
		if (!Scene.HitPoint.IsValid)
		{
			return false;
		}
		List<IInspect> source = ELayer.scene.mouseTarget.pos.ListInspectorTargets();
		point = Scene.HitPoint;
		Inspect(source.NextItem(group?.FirstTarget));
		return true;
	}

	public void Inspect(IInspect newTarget)
	{
		if (group?.FirstTarget != newTarget)
		{
			Inspect(InspectGroup.Create(newTarget));
		}
	}

	public void Inspect(InspectGroup g)
	{
		base.gameObject.SetActive(value: true);
		if (!initialized)
		{
			windows.Add(windowChara.window);
			initialized = true;
			mold = layoutButtons.CreateMold<UIButton>();
			IUISkin[] componentsInChildren = base.transform.GetComponentsInChildren<IUISkin>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].ApplySkin();
			}
			InitInspector();
		}
		group = g;
		soundPop.Play();
		Refresh();
	}

	public void OnUpdate()
	{
		if (group == null || !IsActive)
		{
			return;
		}
		if (!group.CanInspect())
		{
			Hide();
			return;
		}
		foreach (IInspect target in group.targets)
		{
			DrawHighlight(target, selected: true);
		}
	}

	public void Refresh()
	{
		if (!group.CanInspect())
		{
			Hide();
			return;
		}
		group.SetActions();
		layoutButtons.DestroyChildren();
		foreach (InspectGroup.Item item in group.actions)
		{
			if (!group.Solo && !item.multi)
			{
				continue;
			}
			UIButton uIButton = Util.Instantiate(mold, layoutButtons);
			uIButton.icon.sprite = SpriteSheet.Get(item.idSprite.IsEmpty("icon_" + item.text)) ?? uIButton.icon.sprite;
			uIButton.mainText.SetText((item.textFunc != null) ? item.textFunc() : item.text.lang());
			uIButton.onClick.AddListener(delegate
			{
				if (group.CanInspect())
				{
					if (item.sound)
					{
						soundPop.Play();
					}
					foreach (IInspect target in group.targets)
					{
						item.action(target);
					}
				}
				if (group == null || !group.CanInspect())
				{
					Hide();
				}
				else
				{
					Refresh();
				}
			});
			uIButton.RebuildLayout(recursive: true);
		}
		Chara chara = group.FirstTarget as Chara;
		bool flag = group.Solo && chara != null && chara.IsHomeMember();
		Room room = null;
		if (!flag && ELayer.core.IsGameStarted && point != null)
		{
			room = point.cell.room;
		}
		bool flag2 = room != null;
		windowChara.SetActive(flag);
		rectInfo.SetActive(!flag);
		rectRoom.SetActive(flag2);
		note.Clear();
		if (flag)
		{
			windowChara.SetChara(chara);
		}
		else if (group.Solo)
		{
			group.FirstTarget.WriteNote(note);
		}
		else
		{
			note.AddHeader(group.GetName());
			note.Build();
		}
		if (flag2)
		{
			UINote uINote = noteRoom;
			uINote.Clear();
			uINote.AddHeader("Room");
			uINote.AddText(room.points.Count.ToString() ?? "");
			uINote.Build();
		}
		layoutButtons.RebuildLayout(recursive: true);
		layoutBody.RebuildLayout();
		group.FirstTarget.OnInspect();
	}

	public void DrawHighlight(IInspect i, bool selected = false)
	{
		if (i is Area || i is EloPos)
		{
			return;
		}
		Vector3 v = i.InspectPosition;
		Point inspectPoint = i.InspectPoint;
		if (inspectPoint == null)
		{
			return;
		}
		MeshPass meshPass = ((inspectPoint.HasNonWallBlock || inspectPoint.HasBlockRecipe) ? ELayer.screen.guide.passGuideBlock : ELayer.screen.guide.passGuideFloor);
		if (i is Chara)
		{
			v += ELayer.screen.pcOrbit.crystal.transform.position;
			ELayer.screen.tileMap.passIcon.Add(ref v, 1f);
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
						v = pos.Position();
						v.z -= 0.01f;
						ELayer.screen.guide.passGuideFloor.Add(ref v, 7f);
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
						v = pos.Position();
						v.z -= 0.01f;
						ELayer.screen.guide.passGuideBlock.Add(ref v, 7f);
					});
				}
				return;
			}
		}
		v.z -= 0.01f;
		meshPass.Add(ref v, 7f);
	}
}
