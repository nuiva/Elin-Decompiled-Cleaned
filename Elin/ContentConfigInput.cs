using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

public class ContentConfigInput : ContentConfigGame
{
	public override void OnInstantiate()
	{
		this.toggleAutorun.SetToggle(base.config.input.autorun, delegate(bool on)
		{
			base.config.input.autorun = on;
		});
		this.toggleAltKeyAxis.SetToggle(base.config.input.altKeyAxis, delegate(bool on)
		{
			base.config.input.altKeyAxis = on;
		});
		this.toggleIgnoreNPCs.SetToggle(base.config.input.ignoreNPCs, delegate(bool on)
		{
			base.config.input.ignoreNPCs = on;
		});
		this.toggleKeepRunning.SetToggle(base.config.input.keepRunning, delegate(bool on)
		{
			base.config.input.keepRunning = on;
		});
		this.toggleRightClickExitBuildMode.SetToggle(base.config.input.rightClickExitBuildMode, delegate(bool on)
		{
			base.config.input.rightClickExitBuildMode = on;
		});
		this.toggleAltExamine.SetToggle(base.config.input.altExamine, delegate(bool on)
		{
			base.config.input.altExamine = on;
		});
		this.toggleAltChangeHeight.SetToggle(base.config.input.altChangeHeight, delegate(bool on)
		{
			base.config.input.altChangeHeight = on;
		});
		this.toggleSmoothFollow.SetToggle(base.config.camera.smoothFollow, delegate(bool on)
		{
			base.config.camera.smoothFollow = on;
		});
		this.toggleSmoothMove.SetToggle(base.config.camera.smoothMove, delegate(bool on)
		{
			base.config.camera.smoothMove = on;
			this.sliderMoveFrame.SetActive(!on);
		});
		this.toggleZoomToMouse.SetToggle(base.config.camera.zoomToMouse, delegate(bool on)
		{
			base.config.camera.zoomToMouse = on;
		});
		this.toggleZoomMin.SetToggle(base.config.camera.extendZoomMin, delegate(bool on)
		{
			base.config.camera.extendZoomMin = on;
		});
		this.toggleZoomMax.SetToggle(base.config.camera.extendZoomMax, delegate(bool on)
		{
			base.config.camera.extendZoomMax = on;
		});
		this.toggleEdge.SetToggle(base.config.camera.edgeScroll, delegate(bool on)
		{
			base.config.camera.edgeScroll = on;
		});
		this.toggleInvertX.SetToggle(base.config.camera.invertX, delegate(bool on)
		{
			base.config.camera.invertX = on;
		});
		this.toggleInvertY.SetToggle(base.config.camera.invertY, delegate(bool on)
		{
			base.config.camera.invertY = on;
		});
		this.toggleLinearZoom.SetToggle(base.config.camera.linearZoom, delegate(bool on)
		{
			base.config.camera.linearZoom = on;
		});
		base.SetSlider(this.sliderMoveFrame, (float)base.config.camera.moveframe, delegate(float a)
		{
			base.config.camera.moveframe = (int)a;
			return Lang.Get("moveframe") + "(" + ((int)a).ToString() + ")";
		});
		this.sliderMoveFrame.SetActive(!base.config.camera.smoothMove);
		base.SetSlider(this.sliderKeyboardScroll, base.config.camera.senseKeyboard, delegate(float a)
		{
			base.config.camera.senseKeyboard = a;
			return Lang.Get("speed") + "(" + ((int)(a * 100f)).ToString() + ")";
		});
		base.SetSlider(this.sliderDragScroll, base.config.camera.sensDrag, delegate(float a)
		{
			base.config.camera.sensDrag = a;
			return Lang.Get("speed") + "(" + ((int)(a * 100f)).ToString() + ")";
		});
		base.SetSlider(this.sliderEdgeScroll, base.config.camera.sensEdge, delegate(float a)
		{
			base.config.camera.sensEdge = a;
			return Lang.Get("speed") + "(" + ((int)(a * 100f)).ToString() + ")";
		});
		List<CoreConfig.GameFunc> list = Util.EnumToList<CoreConfig.GameFunc>();
		this.ddMiddleClick.SetList<CoreConfig.GameFunc>((int)base.config.input.middleClick, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.middleClick = b;
		}, true);
		this.ddMiddleClickLong.SetList<CoreConfig.GameFunc>((int)base.config.input.middlePressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.middlePressLong = b;
		}, true);
		this.ddMouse3Click.SetList<CoreConfig.GameFunc>((int)base.config.input.mouse3Click, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse3Click = b;
		}, true);
		this.ddMouse3Long.SetList<CoreConfig.GameFunc>((int)base.config.input.mouse3PressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse3PressLong = b;
		}, true);
		this.ddMouse4Click.SetList<CoreConfig.GameFunc>((int)base.config.input.mouse4Click, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse4Click = b;
		}, true);
		this.ddMouse4Long.SetList<CoreConfig.GameFunc>((int)base.config.input.mouse4PressLong, list, (CoreConfig.GameFunc a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFunc b)
		{
			base.config.input.mouse4PressLong = b;
		}, true);
		List<CoreConfig.GameFuncBuild> list2 = Util.EnumToList<CoreConfig.GameFuncBuild>();
		this.b_ddMiddleClick.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_middleClick, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_middleClick = b;
		}, true);
		this.b_ddMiddleClickLong.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_middlePressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_middlePressLong = b;
		}, true);
		this.b_ddMouse3Click.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_mouse3Click, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse3Click = b;
		}, true);
		this.b_ddMouse3Long.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_mouse3PressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse3PressLong = b;
		}, true);
		this.b_ddMouse4Click.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_mouse4Click, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse4Click = b;
		}, true);
		this.b_ddMouse4Long.SetList<CoreConfig.GameFuncBuild>((int)base.config.input.b_mouse4PressLong, list2, (CoreConfig.GameFuncBuild a, int b) => a.ToString().lang(), delegate(int a, CoreConfig.GameFuncBuild b)
		{
			base.config.input.b_mouse4PressLong = b;
		}, true);
		this.listMovement.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				this.<OnInstantiate>g___onInstantiate|38_45(a, b);
			},
			onList = delegate(UIList.SortMode m)
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uilist = this.listMovement;
				uilist.Add(keys.axisUp);
				uilist.Add(keys.axisDown);
				uilist.Add(keys.axisLeft);
				uilist.Add(keys.axisRight);
				uilist.Add(keys.axisUpLeft);
				uilist.Add(keys.axisUpRight);
				uilist.Add(keys.axisDownLeft);
				uilist.Add(keys.axisDownRight);
				uilist.Add(keys.wait);
			}
		};
		this.listGeneral.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				this.<OnInstantiate>g___onInstantiate|38_45(a, b);
			},
			onList = delegate(UIList.SortMode m)
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uilist = this.listGeneral;
				uilist.Add(keys.mouseLeft);
				uilist.Add(keys.mouseMiddle);
				uilist.Add(keys.mouseRight);
				uilist.Add(keys.fire);
				uilist.Add(keys.autoCombat);
				uilist.Add(keys.emptyHand);
			}
		};
		this.listMenu.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				this.<OnInstantiate>g___onInstantiate|38_45(a, b);
			},
			onList = delegate(UIList.SortMode m)
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uilist = this.listMenu;
				uilist.Add(keys.chara);
				uilist.Add(keys.inventory);
				uilist.Add(keys.ability);
				uilist.Add(keys.journal);
				uilist.Add(keys.log);
				uilist.Add(keys.report);
				uilist.Add(keys.search);
			}
		};
		this.listEtc.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				this.<OnInstantiate>g___onInstantiate|38_45(a, b);
			},
			onList = delegate(UIList.SortMode m)
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uilist = this.listEtc;
				uilist.Add(keys.switchHotbar);
				uilist.Add(keys.quickSave);
				uilist.Add(keys.quickLoad);
			}
		};
		this.listAdvanced.callbacks = new UIList.Callback<EInput.KeyMap, ItemKeymap>
		{
			onInstantiate = delegate(EInput.KeyMap a, ItemKeymap b)
			{
				this.<OnInstantiate>g___onInstantiate|38_45(a, b);
			},
			onList = delegate(UIList.SortMode m)
			{
				EInput.KeyMapManager keys = base.config.input.keys;
				UIList uilist = this.listAdvanced;
				uilist.Add(keys.examine);
				uilist.Add(keys.getAll);
				uilist.Add(keys.dump);
				uilist.Add(keys.mute);
				uilist.Add(keys.meditate);
			}
		};
		this.<OnInstantiate>g___refreshList|38_44();
	}

	[CompilerGenerated]
	private void <OnInstantiate>g___refreshList|38_44()
	{
		this.listMovement.List(false);
		this.listGeneral.List(false);
		this.listMenu.List(false);
		this.listEtc.List(false);
		this.listAdvanced.List(false);
	}

	[CompilerGenerated]
	private void <OnInstantiate>g___onInstantiate|38_45(EInput.KeyMap a, ItemKeymap b)
	{
		b.text.text = ("key_" + a.action.ToString()).lang();
		string text = a.key.ToString() ?? "";
		b.buttonKey.mainText.text = text;
		b.buttonKey.SetOnClick(delegate
		{
			Dialog.Keymap(a).SetOnKill(new Action(this.<OnInstantiate>g___refreshList|38_44));
		});
	}

	public UIList listMovement;

	public UIList listGeneral;

	public UIList listMenu;

	public UIList listEtc;

	public UIList listAdvanced;

	public UIButton toggleAutorun;

	public UIButton toggleAltKeyAxis;

	public UIButton toggleKeepRunning;

	public UIButton toggleRightClickExitBuildMode;

	public UIButton toggleIgnoreNPCs;

	public UIButton toggleAltExamine;

	public UIButton toggleZoomToMouse;

	public UIButton toggleZoomMin;

	public UIButton toggleZoomMax;

	public UIButton toggleSmoothFollow;

	public UIButton toggleSmoothMove;

	public Slider sliderKeyboardScroll;

	public Slider sliderDragScroll;

	public Slider sliderEdgeScroll;

	public Slider sliderRunDistance;

	public Slider sliderMoveFrame;

	public UIButton toggleEdge;

	public UIButton toggleInvertX;

	public UIButton toggleInvertY;

	public UIButton toggleLinearZoom;

	public UIButton toggleAltChangeHeight;

	public UIDropdown ddMiddleClick;

	public UIDropdown ddMiddleClickLong;

	public UIDropdown ddMouse3Click;

	public UIDropdown ddMouse3Long;

	public UIDropdown ddMouse4Click;

	public UIDropdown ddMouse4Long;

	public UIDropdown b_ddMiddleClick;

	public UIDropdown b_ddMiddleClickLong;

	public UIDropdown b_ddMouse3Click;

	public UIDropdown b_ddMouse3Long;

	public UIDropdown b_ddMouse4Click;

	public UIDropdown b_ddMouse4Long;
}
