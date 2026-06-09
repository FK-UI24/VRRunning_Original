#ロータリーエンコーダの回転数（1000まで）を表示するプログラム
#CLK:25 DT:24 SW:23 +:3.3V GND:GND

import gpiozero
import signal

encorder=gpiozero.RotaryEncoder(a=25,b=24,max_steps=1000)

def rotated():
	print (f"Steps:{encorder.steps}")

encorder.when_rotated=rotated

signal.pause()
