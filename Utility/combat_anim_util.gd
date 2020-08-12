extends Node

# Constants
enum Anim_States {
	NORMAL,
	EYES_CLOSED,
	HURT,
}
const Anim_State_Region_Pos_X = {
	Anim_States.NORMAL: 0,
	Anim_States.EYES_CLOSED: 32,
	Anim_States.HURT: 64,
}
const BATTLE_TEXTURE_POS_Y = 0
const BATTLE_TEXTURE_WIDTH = 32
const BATTLE_TEXTURE_HEIGHT = 32
