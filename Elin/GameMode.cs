using System;

[Flags]
public enum GameMode
{
	Reserved = 1,
	Adv = 2,
	Sim = 4,
	Build = 8,
	NoMap = 0x10,
	EloMap = 0x20,
	Bird = 0x40
}
