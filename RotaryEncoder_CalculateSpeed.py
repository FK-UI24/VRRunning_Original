from gpiozero import RotaryEncoder
import time

#Unity側から0.1秒間隔でよびださないと使えない

#ロータリーエンコーダ初期化
encoder=RotaryEncoder(a=25,b=24,max_steps=1000000000000000000000000000000000000000000000000000)
prev_steps=encoder.steps

def calculate_speed(base_speed,base_avg_steps):
	"""
	既定速度と対応するステップ数からロータリーエンコーダの速度をリアルタイムで取得する
	base_speed：基準速度（km/h）
	base_avg_steps：基準速度の平均ステップ差（0.1秒単位）
	"""
	
	global prev_steps,encoder
	
	#現在のステップ数取得
	current_steps=encoder.steps
	step_diff=current_steps-prev_steps
	prev_steps=current_steps
	
	#回転していなかったら0を返す
	if step_diff==0:
		return 0.0
	
	return abs((step_diff/base_avg_steps)*base_speed)
	
speed=calculate_speed(5,4)
print(f"現在の速度：{speed}")
