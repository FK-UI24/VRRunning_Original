from flask import Flask, request
from gpiozero import RotaryEncoder
import threading
import time
import smbus2 as smbus
from mpu6050 import mpu6050
import math

#Flaskアプリの初期化
app = Flask(__name__)

#キャリブレーション状態を保存するグローバル変数
calibration_status = "待機中"
#キャリブレーションの実行フラグ
calibration_running = False
#キャリブレーションの結果用変数
avg_steps = None

#MPU6050の初期化
#累積や蓄積がないのでサーバーがたった段階でまとめて初期化しちゃう
MPU6050_Sensor = mpu6050(0x68)
#配線の抜き差しをするとレジスタが狂うためハードリセットとソフトリセットを行う
#ハードリセット
#PWR_MGMT_1に0x80を書き込み、内部レジスタを初期化する
#その後クロックソースを設定してスリープ解除
#DEVICE_RESETビットセット
MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x80)
#リセット完了待ち
time.sleep(0.1)
#クロックソースをx軸ジャイロに；ｐ設定してスリープ解除
MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x01)
#安定化待ち
time.sleep(0.05)

#ソフトリセット
#スリープ状態にする
MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x40)
time.sleep(0.05)
#クロックソースをx軸ジャイロに設定してスリープ解除
MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x01)
time.sleep(0.05)

#MPU6050の状態を保持するグローバル変数
MPU6050_status = "待機中"
#MPU6050の実行フラグ
MPU6050_running = False
#MPU6050の結果用変数
MPU6050_Pitch = None

#ロータリーエンコーダ速度取得用グローバル変数
#ロータリーエンコーダインスタンス
rotary_encoder=None
#現在のステップ数
rotary_current_steps=0
#前回のステップ数
rotary_last_steps=0
#Unityから送られた既定速度
rotary_ref_speed=None
#Unityから送られた平均ステップ数
rotary_ref_steps=None
#計算した速度
rotary_speed=0
#速度取得の実行フラグ
rotary_running=False

#接続テスト用関数
@app.route("/connecttest")
def ConnectTest():
    return "Connecting"

"""
ロータリーエンコーダ
	ステップ数：ロータリーエンコーダを回したときの１回の「かちっ」みたいなやつ
	ステップ数の数え方：基本的に合計を返す　-max_steps~max_stepsまでで、初期化しないとずっと残る
"""

#速度のキャリブレーション用関数
#0.1秒ごとにステップ数を取得して、それと前回との差を計算する
#その後、差をリストに保存してリストに保存する
#リストがすべて埋まった状態で最大値と最小値が閾値以下なら
#センサーからの取得を終了して、リストの平均値をとる
#その結果を既定速度の平均ステップ数とする
def RotaryEncoder_Calibration(target_speed, stability_threshold):
    global calibration_status, calibration_running
    encoder = RotaryEncoder(a=25, b=24, max_steps=10**27)
    sample_window = 30
    step_diffs = []
    prev_steps = encoder.steps
    calibration_status = f"キャリブレーション開始：{target_speed}km/hで動かしてください"
    calibration_running = True

    while calibration_running:
        time.sleep(0.1)
        current_steps = encoder.steps
        delta_steps = current_steps - prev_steps
        prev_steps = current_steps

        if delta_steps == 0:
            continue

        step_diffs.append(delta_steps)
        if len(step_diffs) > sample_window:
            step_diffs.pop(0)

        calibration_status = f"回転中：ステップ差　{delta_steps}"

        if len(step_diffs) == sample_window:
            max_diff = max(step_diffs)
            min_diff = min(step_diffs)
            if (max_diff - min_diff) <= stability_threshold:
                global avg_steps
                avg_steps = sum(step_diffs) / sample_window
                calibration_status = f"キャリブレーション終了：{target_speed}km/hのときの平均ステップ数　{avg_steps:.2f}/0.1s"
                calibration_running = False
                return avg_steps

    calibration_status = "キャリブレーション中断"

#そのときのセンサー自体のピッチを返す
#初回(MPU6050_ResetCount=0)と30分(MPU6050_ResetCount=18000)に
#１回リセットする
def MPU6050_Get_Pitch():
    global MPU6050_status, MPU6050_running, MPU6050_Pitch
    MPU6050_ResetCount = 0
    MPU6050_running = True
    MPU6050_status = "計測開始"

    pitch_buffer = []
    window_size = 5
    filtered_pitch = 0
    alpha = 0.1

    while MPU6050_running:
        time.sleep(0.1)
        accel = MPU6050_Sensor.get_accel_data()
        ax = accel['x']
        ay = accel['y']
        az = accel['z']
        pitch_rad = math.atan2(ay, math.sqrt(ax*ax + az*az))
        pitch = math.degrees(pitch_rad)

        # -----------------------------
        # 振動対策：移動平均
        # -----------------------------
        pitch_buffer.append(pitch)
        if len(pitch_buffer) > window_size:
            pitch_buffer.pop(0)
        avg_pitch = sum(pitch_buffer) / len(pitch_buffer)

        # -----------------------------
        # ローパスフィルタで平滑化
        # -----------------------------
        filtered_pitch = filtered_pitch * (1 - alpha) + avg_pitch * alpha

        MPU6050_Pitch = filtered_pitch
        MPU6050_status = f"計測中：ピッチ　{filtered_pitch:.2f}°"

        if MPU6050_ResetCount == 0 or MPU6050_ResetCount == 36000:
            MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x80)
            time.sleep(0.1)
            MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x01)
            time.sleep(0.05)
            print(f"ハードリセット：{MPU6050_ResetCount}")
            MPU6050_ResetCount = 0

        if MPU6050_ResetCount % 9000 == 0:
            MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x40)
            time.sleep(0.05)
            MPU6050_Sensor.bus.write_byte_data(MPU6050_Sensor.address, 0x6B, 0x01)
            time.sleep(0.05)
            print(f"ソフトリセット：{MPU6050_ResetCount}")

        print(f"{MPU6050_ResetCount}")
        MPU6050_ResetCount += 1
        
#Unityから送られてくる既定速度と平均ステップ数を保持し、速度計算を開始するフラグをオンにする関数
#速度取得を開始する前にこれを実行する
def RotarySpeed_Set(ref_speed,ref_steps):
    
    global rotary_encoder,rotary_current_steps,rotary_last_steps
    global rotary_ref_speed,rotary_ref_steps,rotary_speed,rotary_running
    
    #Unityから送られた値をグローバル変数に保存する
    rotary_ref_speed=ref_speed
    rotary_ref_steps=ref_steps
    
    #速度取得開始前に毎回この関数を実行するので、ここでロータリーエンコーダのリセットを毎回行う
    rotary_encoder=RotaryEncoder(a=25,b=24,max_steps=10**27)
    
    #グローバル変数の初期化とフラグの変更
    rotary_current_steps=rotary_encoder.steps
    rotary_last_steps=rotary_current_steps
    rotary_speed=0
    rotary_running=True
    return "速度取得開始"

#Unityから呼ばれたときに速度取得を停止して、保存データをリセットする
def RotarySpeed_Stop():
    global rotary_encoder,rotary_current_steps,rotary_last_steps
    global rotary_ref_speed,rotary_ref_steps,rotary_speed,rotary_running
    
    #グローバル変数のリセットとフラグの変更
    rotary_running=False
    rotary_encoder=None
    rotary_current_steps=0
    rotary_last_steps=0
    rotary_ref_speed=None
    rotary_ref_steps=None
    rotary_speed=0
    
    return "速度取得終了"

#Unityから0.1秒ごとに呼び出す
#保存している既定速度と平均ステップ数から速度を計算して返す
def RotarySpeed_Get():
    global rotary_encoder,rotary_current_steps,rotary_last_steps
    global rotary_ref_speed,rotary_ref_steps,rotary_speed,rotary_running
    
    #もしフラグがオフだったら
    if not rotary_running:
        return None
    
    #もし既定速度か平均ステップ数がなかったら（キャリブレーションが行われていなかったら）
    if rotary_ref_speed is None or rotary_ref_steps is None:
        return None
    
    #現在のステップ数を取得する
    rotary_current_steps=rotary_encoder.steps
    
    #前回との差分を計算する
    delta_steps=rotary_current_steps-rotary_last_steps
    
    #前回のステップ数を今回のステップ数に更新する
    rotary_last_steps=rotary_current_steps
    
    #速度計算
    rotary_speed=rotary_ref_speed*(delta_steps/rotary_ref_steps)
    
    return rotary_speed

#キャリブレーション開始用エンドポイント
@app.route("/start_calibration", methods=["POST"])
def start_calibration():
    global calibration_running
    if calibration_running:
        return "すでにキャリブレーション中です"
    target_speed = float(request.form.get("target_speed"))
    stability_threshold = float(request.form.get("stability_threshold"))
    thread = threading.Thread(target=RotaryEncoder_Calibration, args=(target_speed, stability_threshold))
    thread.start()
    return "キャリブレーション開始"

#キャリブレーション強制終了用エンドポイント
@app.route("/stop_calibration", methods=["POST"])
def stop_calibration():
    global calibration_running, calibration_status
    calibration_running = False
    calibration_status = "キャリブレーション中断"
    return "キャリブレーションを中断しました"

#キャリブレーションの状態取得用エンドポイント
@app.route("/calibration_status")
def get_status():
    return calibration_status

#キャリブレーションの結果を送る用関数
@app.route("/calibration_result")
def get_result():
    global avg_steps
    result_avg_steps = avg_steps
    avg_steps = None
    return str(result_avg_steps)

#MPU6050でピッチ取得開始用エンドポイント
@app.route("/start_mpu6050_get_pitch", methods=["POST"])
def start_MPU6050_Get_Pitch():
    global MPU6050_running
    if MPU6050_running:
        return "すでに計測中です"
    thread = threading.Thread(target=MPU6050_Get_Pitch)
    thread.start()
    return "計測開始"

#MPU6050でのピッチ取得を終了するエンドポイント
@app.route("/stop_mpu6050_get_pitch", methods=["POST"])
def stop_MPU6050_Get_Pitch():
    global MPU6050_running, MPU6050_status
    MPU6050_running = False
    MPU6050_status = "計測を終了"
    return "計測を終了しました"

#MPU6050の状態取得用エンドポイント
@app.route("/mpu6050_get_pitch_status")
def MPU6050_Get_Pitch_Status():
    return MPU6050_status

#MPU6050の結果を送る用関数
@app.route("/mpu6050_get_pitch_result")
def MPU6050_Get_Pitch_Result():
    global MPU6050_Pitch
    result = MPU6050_Pitch
    return str(result)

#キャリブレーション値保存用エンドポイント
#Unityが送られる既定速度と平均ステップ数を送信する
#それを受け取り、グローバル変数に保存する
#速度取得をする際には必ずこれを最初に呼び出してから、速度取得を行う
#そうしないとキャリブレーション結果を保存できない
#以降は/get_rotary_speedを0.1秒ごとに呼び出すことで速度を取得できる
@app.route("/set_rotary_speed",methods=["POST"])
def Set_Rotary_Speed():
    global rotary_running
    
    print(f"[/set_rotary_speed] が呼び出されました。現在のrotary_runningフラグ: {rotary_running}")
    
    #既に速度取得中であれば重複開始を防止する
    if rotary_running:
        
        print(">>>>> フラグがTrueのため、値の更新をスキップします。")
        
        return "既にキャリブレーション値を取得中です"
    #Unityから送られてきたキャリブレーション値を取得する
    #基準速度[km/h]
    ref_speed=float(request.form.get("ref_speed"))
    #0.1秒あたりの平均ステップ数
    ref_steps=float(request.form.get("ref_steps"))
    #内部関数を読んで初期化と変数保持を実行する
    result=RotarySpeed_Set(ref_speed,ref_steps)
    
    print(f"[/set_rotary_speed] 値をセットしました。rotary_runningフラグは {rotary_running} になりました。")
    
    return result

#速度取得停止エンドポイント
#グローバル変数の初期化とフラグのオフ
#速度取得を終了して、次回開始時には再度キャリブレーション値を送る必要がある
@app.route("/stop_rotary_speed",methods=["POST"])
def Stop_Rotary_Speed():
    
    print(f"[/stop_rotary_speed] が呼び出されました。現在のrotary_runningフラグ: {rotary_running}")
    
    result=RotarySpeed_Stop()
    
    print(f"[/stop_rotary_speed] 停止処理を実行しました。rotary_runningフラグは {rotary_running} になりました。")
    
    return result

#速度取得用エンドポイント
#Unityから0.1秒ごとに呼び出す
#RotarySpeed_Get()で速度計算を実行して返す
@app.route("/get_rotary_speed")
def Get_Rotary_Speed():
    speed=RotarySpeed_Get()
    #キャリブレーションが未設定、または速度取得が開始されていない場合
    if speed is None:
        return "速度取得未開始またはキャリブレーション未設定",400
    #現在の速度を文字列として返す(Unity側でFloatに変換して扱う)
    return str(speed)
        
#サーバー起動
if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
