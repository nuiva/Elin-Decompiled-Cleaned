using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : ELayer
{
	public override void OnAfterInit()
	{
		ELayer.ui.hud.textMouseHintRight.transform.parent.SetActive(false);
		this.textDetail.SetActive(!this.textDetail.text.IsEmpty());
		this.listGrid.SetActive(this.gridItems.Count > 0);
		if (this.gridItems.Count >= 8)
		{
			GridLayoutGroup gridLayoutGroup = this.listGrid.layoutItems as GridLayoutGroup;
			gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			gridLayoutGroup.constraintCount = 8;
		}
		if (this.gridItems.Count > 0)
		{
			this.listGrid.Clear();
			BaseList baseList = this.listGrid;
			UIList.Callback<GridItem, ButtonGrid> callback = new UIList.Callback<GridItem, ButtonGrid>();
			callback.onInstantiate = delegate(GridItem a, ButtonGrid b)
			{
				b.SetItem(a);
			};
			callback.onClick = delegate(GridItem a, ButtonGrid b)
			{
				a.OnClick(b);
			};
			baseList.callbacks = callback;
			foreach (GridItem o in this.gridItems)
			{
				this.listGrid.Add(o);
			}
			this.listGrid.Refresh(false);
		}
		this.list.Refresh(false);
		this.RebuildLayout(true);
		GraphicRaycaster g = this.windows[0].GetComponent<GraphicRaycaster>();
		g.enabled = false;
		TweenUtil.Delay(0.3f, delegate
		{
			if (g)
			{
				g.enabled = true;
			}
		});
		if (this.input)
		{
			this.input.Focus();
		}
		EInput.WaitReleaseKey();
	}

	public void AddButton(string text, Action onClick = null, bool close = true)
	{
		this.list.AddButton(null, text, delegate
		{
			if (onClick != null)
			{
				onClick();
			}
			if (close)
			{
				this.Close();
			}
		}, null);
	}

	private void Update()
	{
		if (this.input && this.option.canClose && Input.GetKeyDown(KeyCode.Escape))
		{
			this.Close();
		}
	}

	public override void OnUpdateInput()
	{
		if (this.input && this.option.canClose && Input.GetKeyDown(KeyCode.Return))
		{
			this.isInputEnter = true;
			this.Close();
			return;
		}
		if (!this.input.gameObject.activeInHierarchy && this.list != null)
		{
			foreach (UIList.ButtonPair buttonPair in this.list.buttons)
			{
				UIButton uibutton = buttonPair.component as UIButton;
				if (uibutton && uibutton.interactable && !EInput.waitReleaseAnyKey && uibutton.keyText && !uibutton.keyText.text.IsEmpty() && uibutton.keyText.text == Input.inputString)
				{
					uibutton.onClick.Invoke();
					return;
				}
			}
		}
		if (this.keymap != null)
		{
			List<KeyCode> list = new List<KeyCode>
			{
				KeyCode.Mouse0,
				KeyCode.Mouse1,
				KeyCode.Mouse2,
				KeyCode.Mouse3,
				KeyCode.Mouse4,
				KeyCode.Escape,
				KeyCode.Return,
				KeyCode.LeftShift,
				KeyCode.Delete,
				KeyCode.Backspace
			};
			if (Input.GetKeyDown(KeyCode.Delete))
			{
				if (this.keymap.required)
				{
					SE.Beep();
					return;
				}
				SE.Tab();
				this.keymap.key = KeyCode.None;
				this.Close();
				return;
			}
			else
			{
				foreach (object obj in Enum.GetValues(typeof(KeyCode)))
				{
					KeyCode keyCode = (KeyCode)obj;
					if (!list.Contains(keyCode) && Input.GetKey(keyCode))
					{
						foreach (EInput.KeyMap keyMap in ELayer.core.config.input.keys.List())
						{
							if (keyMap.key == keyCode && keyMap.GetGroup() == this.keymap.GetGroup())
							{
								keyMap.key = this.keymap.key;
							}
						}
						this.keymap.key = keyCode;
						SE.Tab();
						this.Close();
						return;
					}
				}
			}
		}
		base.OnUpdateInput();
	}

	public void OnEnterInput()
	{
		this.isInputEnter = true;
		this.Close();
	}

	public override void OnKill()
	{
		ELayer.ui.hud.textMouseHintRight.transform.parent.SetActive(true);
		if (this.input && this.onEnterInput != null)
		{
			this.onEnterInput(!this.isInputEnter, this.input.Text);
		}
		if (this.input)
		{
			this.input.field.DeactivateInputField();
		}
		EInput.WaitReleaseKey();
	}

	public static Dialog CreateNarration(string idImage, string idText)
	{
		Dialog dialog = Layer.Create<Dialog>("DialogNarration");
		dialog.image.SetActive(true);
		dialog.image.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Dialog/" + idImage);
		dialog.image.SetNativeSize();
		dialog.textDetail.SetText(IO.LoadText(CorePath.CorePackage.TextNarration + idText + ".txt"));
		return dialog;
	}

	public static Dialog Ok(string langDetail, Action action = null)
	{
		Dialog dialog = Layer.Create<Dialog>();
		dialog.textDetail.SetText(langDetail.lang() + " ");
		dialog.list.AddButton(null, Lang.Get("ok"), new Action(dialog.Close), null);
		if (action != null)
		{
			dialog.SetOnKill(action);
		}
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog Choice(string langDetail, Action<Dialog> action)
	{
		Dialog dialog = Layer.Create<Dialog>();
		dialog.textDetail.SetText(langDetail.lang() + " ");
		action(dialog);
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog YesNo(string langDetail, Action actionYes, Action actionNo = null, string langYes = "yes", string langNo = "no")
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(langDetail.lang() + " ");
		d.list.AddButton(null, Lang.Get(langYes), delegate
		{
			if (actionYes != null)
			{
				actionYes();
			}
			d.Close();
		}, null);
		d.list.AddButton(null, Lang.Get(langNo), delegate
		{
			if (actionNo != null)
			{
				actionNo();
			}
			d.Close();
		}, null);
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog List<TValue>(string langDetail, ICollection<TValue> items, Func<TValue, string> getString, Func<int, string, bool> onSelect, bool canCancel = false)
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(langDetail.lang() + " ");
		int num = 0;
		using (IEnumerator<TValue> enumerator = items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				TValue item = enumerator.Current;
				int _i = num;
				d.list.AddButton(null, getString(item).lang(), delegate
				{
					if (onSelect(_i, getString(item)))
					{
						d.Close();
					}
				}, null);
				num++;
			}
		}
		d.option.canClose = canCancel;
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static void TryWarn(string lang, Action action, bool yes = true)
	{
		Dialog d = Layer.Create<Dialog>();
		d.textDetail.SetText(lang.lang() + " ");
		d.SetOnKill(delegate
		{
			ELayer.core.actionsNextFrame.Add(delegate
			{
				ActPlan.warning = false;
			});
		});
		d.list.AddButton(null, Lang.Get("yes"), delegate
		{
			d.Close();
			base.<TryWarn>g__Commit|0();
		}, null);
		if (yes)
		{
			d.list.AddButton(null, Lang.Get("yes_dontask"), delegate
			{
				string lang2 = lang;
				if (!(lang2 == "warn_crime"))
				{
					if (!(lang2 == "warn_mana"))
					{
						if (lang2 == "warn_disassemble")
						{
							ELayer.core.config.game.ignoreWarnDisassemble = true;
						}
					}
					else
					{
						ELayer.core.config.game.ignoreWarnMana = true;
					}
				}
				else
				{
					ELayer.core.config.game.ignoreWarnCrime = true;
				}
				d.Close();
				base.<TryWarn>g__Commit|0();
			}, null);
		}
		d.list.AddButton(null, Lang.Get("no"), delegate
		{
			d.Close();
		}, null);
		if (!yes)
		{
			d.list.AddButton(null, Lang.Get("no_dontask"), delegate
			{
				string lang2 = lang;
				if (!(lang2 == "warn_parallels"))
				{
					if (lang2 == "warn_linuxMod")
					{
						ELayer.core.config.ignoreLinuxModWarning = true;
					}
				}
				else
				{
					ELayer.core.config.ignoreParallelsWarning = true;
				}
				d.Close();
			}, null);
		}
		ELayer.ui.AddLayer(d);
	}

	public static void TryWarnCrime(Action action)
	{
		if (!ELayer.core.config.game.warnCrime || ELayer.core.config.game.ignoreWarnCrime)
		{
			Dialog.warned = true;
			action();
			Dialog.warned = false;
			ActPlan.warning = false;
			return;
		}
		Dialog.TryWarn("warn_crime", action, true);
	}

	public static void TryWarnMana(Action action)
	{
		if (!ELayer.core.config.game.warnMana || ELayer.core.config.game.ignoreWarnMana)
		{
			Dialog.warned = true;
			action();
			Dialog.warned = false;
			ActPlan.warning = false;
			return;
		}
		Dialog.TryWarn("warn_mana", action, true);
	}

	public static void TryWarnDisassemble(Action action)
	{
		if (!ELayer.core.config.game.warnDisassemble || ELayer.core.config.game.ignoreWarnDisassemble)
		{
			Dialog.warned = true;
			action();
			Dialog.warned = false;
			return;
		}
		Dialog.TryWarn("warn_disassemble", action, true);
	}

	public static Dialog Gift(string langHeader, bool autoAdd, params Card[] cards)
	{
		return Dialog.Gift(langHeader, autoAdd, new List<Card>(cards));
	}

	public static Dialog Gift(string langHeader, bool autoAdd, List<Card> list)
	{
		List<GridItem> list2 = new List<GridItem>();
		foreach (Card c in list)
		{
			list2.Add(new GridItemCard
			{
				c = c
			});
		}
		return Dialog.Gift(langHeader, autoAdd, list2);
	}

	public static Dialog Gift(string langHeader, bool autoAdd, List<GridItem> list)
	{
		Dialog d = Layer.Create<Dialog>();
		d.spacer.SetActive(false);
		d.note.AddHeader(langHeader.IsEmpty("headerGift").lang(), null);
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			if (autoAdd)
			{
				foreach (GridItem gridItem in list)
				{
					gridItem.AutoAdd();
				}
			}
			d.Close();
		}, null);
		d.option.soundActivate = null;
		d.option.canClose = false;
		d.gridItems = list;
		ELayer.Sound.Play("good");
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Recipe(List<RecipeSource> list)
	{
		Dialog d = Layer.Create<Dialog>();
		d.spacer.SetActive(false);
		d.note.AddHeader("giftRecipe".lang(), null);
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		}, null);
		d.option.soundActivate = null;
		for (int i = 0; i < list.Count; i++)
		{
			RecipeSource recipeSource = list[i];
			List<Recipe.Ingredient> ingredients = recipeSource.GetIngredients();
			if (ingredients.Count > 0 && ELayer.sources.cards.map.ContainsKey(ingredients[0].id))
			{
				ELayer.sources.cards.map[ingredients[0].id].GetName();
			}
			d.note.AddText("・" + recipeSource.Name.ToTitleCase(false), FontColor.DontChange);
			if (i >= 9 && list.Count > 10)
			{
				d.note.Space(6, 1);
				d.note.AddText("moreRecipes".lang((list.Count - 10).ToString() ?? "", null, null, null, null), FontColor.DontChange);
				break;
			}
		}
		d.SetOnKill(new Action(SE.Click));
		ELayer.Sound.Play("idea");
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Confetti(string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		ELayer.Sound.Play("confetti");
		return Dialog._Confetti("DialogConfetti", langTitle, langDetail, langConfetti);
	}

	public static Dialog ConfettiSimple(string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		ELayer.Sound.Play("confettiSimple");
		return Dialog._Confetti("DialogConfettiSimple", langTitle, langDetail, langConfetti);
	}

	public static Dialog _Confetti(string idPrefab, string langTitle, string langDetail, string langConfetti = "Grats!")
	{
		Dialog d = Layer.Create(idPrefab) as Dialog;
		d.textConfetti.text = langConfetti.lang();
		d.textDetail.SetText(langDetail.lang());
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			d.Close();
		}, null);
		ELayer.ui.AddLayer(d);
		return d;
	}

	public static Dialog Keymap(EInput.KeyMap keymap)
	{
		Dialog dialog = Layer.Create<Dialog>("DialogKeymap");
		dialog.textDetail.SetText("dialog_keymap".lang(("key_" + keymap.action.ToString()).lang(), null, null, null, null));
		dialog.keymap = keymap;
		ELayer.ui.AddLayer(dialog);
		return dialog;
	}

	public static Dialog InputName(string langDetail, string text, Action<bool, string> onClose, Dialog.InputType inputType = Dialog.InputType.Default)
	{
		Dialog d = Layer.Create<Dialog>("DialogInput");
		d.inputType = inputType;
		d.langHint = langDetail;
		d.note.AddText(langDetail.lang(), FontColor.DontChange).text1.alignment = TextAnchor.MiddleCenter;
		switch (inputType)
		{
		case Dialog.InputType.Password:
			d.input.field.characterLimit = 8;
			d.input.field.contentType = InputField.ContentType.Alphanumeric;
			break;
		case Dialog.InputType.Item:
			d.input.field.characterLimit = 30;
			break;
		case Dialog.InputType.DistributionFilter:
			d.input.field.characterLimit = 100;
			break;
		}
		d.input.Text = text;
		d.onEnterInput = onClose;
		d.list.AddButton(null, Lang.Get("ok"), delegate
		{
			onClose(false, d.input.Text);
			d.Close();
		}, null);
		d.list.AddButton(null, Lang.Get("cancel"), delegate
		{
			onClose(true, "");
			d.Close();
		}, null);
		ELayer.ui.AddLayer(d);
		return d;
	}

	public Text textConfetti;

	public Image image;

	public UIText textDetail;

	public UINote note;

	public UIButtonList list;

	public LayoutGroup layout;

	public ScreenEffect effect;

	public UIList listGrid;

	public Transform spacer;

	public List<GridItem> gridItems = new List<GridItem>();

	public UIInputText input;

	public Dialog.InputType inputType;

	[NonSerialized]
	public EInput.KeyMap keymap;

	[NonSerialized]
	public bool isInputEnter;

	public Action<bool, string> onEnterInput;

	public static bool warned;

	public enum InputType
	{
		None,
		Default,
		Password,
		Item,
		DistributionFilter
	}
}
