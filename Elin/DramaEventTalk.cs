using System;
using System.Collections.Generic;
using UnityEngine;

public class DramaEventTalk : DramaEvent
{
	public DramaEventTalk()
	{
	}

	public DramaEventTalk(string _idActor, Func<string> func)
	{
		if (_idActor.Contains("?"))
		{
			this.unknown = true;
		}
		this.idActor = _idActor.Replace("?", "");
		this.funcText = func;
	}

	public DramaEventTalk(string _idActor, string _text, List<DramaChoice> listChoice = null)
	{
		if (_idActor.Contains("?"))
		{
			this.unknown = true;
		}
		this.idActor = _idActor.Replace("?", "");
		this.text = _text;
		if (listChoice != null)
		{
			foreach (DramaChoice choice in listChoice)
			{
				this.AddChoice(choice);
			}
		}
	}

	public void AddChoice(DramaChoice choice)
	{
		this.choices.Add(choice);
	}

	public override bool Play()
	{
		if (this.progress == 0)
		{
			this.InitDialog();
			if (this.sequence.firstTalk == null)
			{
				this.sequence.firstTalk = this;
			}
			this.timer = 0f;
			this.sequence.dialog.SetActive(true);
			base.actor.Talk(this.sequence.message + ((this.funcText != null) ? this.funcText() : this.text), this.choices, this.center, this.unknown);
			this.sequence.message = "";
			this.progress++;
		}
		else
		{
			this.timer += Time.deltaTime;
			if (this.timer < 0.08f)
			{
				return false;
			}
			if (!this.canCancel || (!EInput.rightMouse.down && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.LeftShift)))
			{
				int num = 0;
				foreach (DramaChoice dramaChoice in this.choices)
				{
					if (dramaChoice.activeCondition == null || dramaChoice.activeCondition())
					{
						string inputString = Input.inputString;
						if (dramaChoice.button && inputString == ((num + 1).ToString() ?? ""))
						{
							if (dramaChoice.button.soundClick)
							{
								SoundManager.current.Play(dramaChoice.button.soundClick);
							}
							dramaChoice.button.onClick.Invoke();
							return false;
						}
						num++;
					}
				}
				if (num != 0 || (!EInput.leftMouse.down && !EInput.rightMouse.down && !Input.GetKeyDown(KeyCode.KeypadEnter) && !Input.GetKey(KeyCode.LeftControl) && EInput.action != EAction.Confirm && EInput.action != EAction.Wait && !Input.GetKeyDown(KeyCode.Return) && !Input.GetKeyDown(KeyCode.Escape)))
				{
					return false;
				}
				this.ResetDialog();
				if (!(EClass.ui.GetTopLayer() == base.layer))
				{
					return false;
				}
				Typewriter typewriter = this.sequence.dialog.textMain.typewriter;
				if (typewriter && !typewriter.IsFinished)
				{
					typewriter.Skip();
					return false;
				}
				base.actor.gameObject.SetActive(false);
				EClass.Sound.Play("click_chat");
				if (!this.temp)
				{
					if (this.idJump.IsEmpty())
					{
						this.sequence.PlayNext();
					}
					else
					{
						this.sequence.Play(this.idJump);
					}
					return false;
				}
				this.sequence.tempEvents.Clear();
				if (!this.idJump.IsEmpty())
				{
					this.sequence.Play(this.idJump);
					return false;
				}
				return true;
			}
			this.ResetDialog();
			if (this.temp)
			{
				this.sequence.tempEvents.Clear();
			}
			EClass.Sound.Play("click_chat");
			if (this.idCancelJump == "back")
			{
				return true;
			}
			if (this.idCancelJump.IsEmpty())
			{
				this.sequence.Exit();
			}
			else
			{
				this.sequence.Play(this.idCancelJump);
			}
			return false;
		}
		return false;
	}

	public void InitDialog()
	{
		if (this.idActor == "*")
		{
			base.manager.SetDialog("Mono");
		}
		if (this.idActor == "*2")
		{
			base.manager.SetDialog("Default2");
		}
		base.manager.imageBG.color = ((this.idActor == "*") ? new Color(0.6f, 0.6f, 0.6f) : Color.white);
	}

	public void ResetDialog()
	{
		base.manager.imageBG.color = Color.white;
		if (this.idActor == "*" || this.idActor == "*2")
		{
			base.manager.SetDialog("Default");
		}
	}

	public string text;

	public string idCancelJump;

	public List<DramaChoice> choices = new List<DramaChoice>();

	public bool center;

	public bool canCancel;

	public bool unknown;

	public float timer;

	public Func<string> funcText;
}
