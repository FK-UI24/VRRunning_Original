from gpiozero import RotaryEncoder
import time

#このプログラムでは既定速度での平均ステップ数が返る
#ステップ数とは、ロータリーエンコーダ１回の「かちっ」みたいなやつ

#ロータリーエンコーダの初期化
#a,b：エンコーダの２相信号を接続したGPIOピン
#max_steps：オーバフロー防止のため非常に大きな数字にする
encoder=RotaryEncoder(a=25,b=24,max_steps=100000000000000000000000000)

#設定モードで使用する変数
#保持するステップ差の数（リストの要素数）
#これが多ければ安定しているかを厳密に判断できる
sample_window=30
#過去のステップ差を保持するリスト
step_diffs=[]
#前回のステップ数
prev_steps=encoder.steps

#設定モード関数
def measure_reference(target_speed,stability_threshold=2):
	
	"""
	安定した基準回転数を取得する関数
	-target_speed：仮の規定速度（将来的にはunityから送信する）
	-stability_threshold：ステップ差の変動幅がこの値以内なら安定とみなす（閾値）
	return：基準回転数（平均ステップ差）
	"""
	
	global prev_steps,step_diffs
	
	#設定モード開始通知（コンソール表示）
	#将来的にはUnityに送信する
	print(f"設定モード開始：既定速度{target_speed}km/hで動かしてください")
	
	while True:
		#0.1sごとに測定する
		time.sleep(0.1)
		
		current_steps=encoder.steps
		#今回のステップ増分
		delta_steps=current_steps-prev_steps
		prev_steps=current_steps
		
		#delta_stepsが0の場合は回転していないので無視する
		if delta_steps==0:
			continue
		
		#過去のステップ差をリストに追加する
		step_diffs.append(delta_steps)
		if len(step_diffs)>sample_window:
			#sample_window分だけを保持する
			step_diffs.pop(0)
		#安定判定：sample_window回の最大と最小の差が閾値以下なら安定とする
		if len(step_diffs)==sample_window:
			max_diff=max(step_diffs)
			min_diff=min(step_diffs)
			if(max_diff-min_diff)<=stability_threshold:
				#平均ステップ数
				avg_steps=sum(step_diffs)/sample_window
				print(f"\n回転安定：平均ステップ差{avg_steps:.2f}/0.1秒")
				
				#グローバルなので呼び出されたときに前回の中身が残っている可能性があるので初期化する
				prev_steps=encoder.steps
				step_diffs.clear()
				
				return avg_steps
		
		#デバッグ表示（回転中のステップ差）
		print(f"回転中：ステップ差{delta_steps}",end='\r')

#実行部分（ラズパイ単体確認用）
reference_steps=measure_reference(target_speed=5.0)
print(f"設定完了：基準回転数{reference_steps:.2f}/0.1秒")
