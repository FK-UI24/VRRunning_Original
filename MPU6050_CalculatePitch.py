import smbus2 as smbus
from mpu6050 import mpu6050
import time
import math
import threading
import collections

# MPU6050のI2Cアドレス（0x68）
sensor = mpu6050(0x68)

# 初期化
pitch_data = None  # Pitchのデータを格納する変数
pitch_history = collections.deque(maxlen=10)  # 移動平均用のデータ履歴（10個のデータを保持）

# レジスタアドレス
PWR_MGMT_1 = 0x6B

# I2Cバスの作成
bus = smbus.SMBus(1)

# ピッチを取得し、移動平均でフィルタリングする関数
def get_pitch():
    global pitch_data  # グローバル変数を使用してピッチデータを返す
    accel_data = sensor.get_accel_data()

    # 加速度（X,Y,Z軸）
    ax = accel_data['x']
    ay = accel_data['y']
    az = accel_data['z']
    
    # ピッチ（X,Y軸の傾き計算）
    pitch = (180.0 / math.pi) * math.atan(ay / math.sqrt(ax**2 + az**2))

    # 最初の10個のデータが揃うまでそのまま加える
    if len(pitch_history) < 10:
        pitch_history.append(pitch)
        smoothed_pitch = pitch  # 10個揃うまでは移動平均を使わずそのまま返す
    else:
        # 10個目以降で移動平均を計算
        pitch_history.append(pitch)
        smoothed_pitch = sum(pitch_history) / len(pitch_history)  # 最新10個の平均値を使用

    pitch_data = smoothed_pitch  # 最新のフィルタリングされたPitchデータを更新
    
    return smoothed_pitch


print(get_pitch())
