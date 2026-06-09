import smbus2 as smbus
from mpu6050 import mpu6050
import time
import math

#設定値
#MPU6050のI2Cアドレス
#ADO=GNDなら0x68,Vccなら0x69になる
I2C_ADDR=0x68
#取得間隔（秒）
LOOP_DT=1.0

#センサー初期化
#MPU6050ライブラリのコンストラクタ
#I2C上の該当アドレスに接続して
#必要な初期設定（電源管理レジスタ等）を
#内部で行う実装が一般的
sensor=mpu6050(I2C_ADDR)

#メインループ
while True:
    #１．センサーから生データを取得する
    #get_accel_data():加速度[g](重力加速度1gを基準とした無次元)をdictで返す
    accel=sensor.get_accel_data()
    #get_gyro_data():角速度[deg/s]をdictで返す
    gyro=sensor.get_gyro_data()
    
    #２．軸成分を取り出す（可読性のため変数に束ねる）
    #加速度：静止時は重力ベクトルが主成分。姿勢角は基本的にこの重力ベクトルの向きから求める
    ax=accel['x']
    ay=accel['y']
    az=accel['z']
    #ジャイロ：角速度（今回は表示のみ。姿勢推定の補正に使うなら後段で融合する）
    gx=gyro['x']
    gy=gyro['y']
    gz=gyro['z']
    
    #３．傾き計算（加速度ベースの姿勢角）
    #一般式
    #roll=atan2(ax,sqrt(ay^2+az^2))
    #pitch=atan2(ay,sqrt(ax^2+az^2))
    #atan2を使うことで分母が0になっても安全（ゼロ除算なし）
    #出力はラジアンなのでdegress()で度に変換する
    #ここでのrollはx軸回り、pitchはy軸回りを想定
    #ラジアン
    roll_rad=math.atan2(ax,math.sqrt(ay*ay+az*az))
    #ラジアン
    pitch_rad=math.atan2(ay,math.sqrt(ax*ax+az*az))
    #度数法に変換
    roll=math.degrees(roll_rad)
    #度数法に変換
    pitch=math.degrees(pitch_rad)
    
    #４．データを整形して表示する
    # :.2f→小数点以下2桁
    # :.3f→小数点異ｋ3桁
    print(f"Roll:{roll:.2f}°　Pitch:{pitch:.2f}°")
    print(f"Accel X:{ax:.3f} y:{ay:.3f} z:{az:.3f}")
    print(f"Gyro x:{gx:.3f} y:{gy:.3f} z:{gz:.3f}")
    
    #５．指定時間待機する
    time.sleep(LOOP_DT)
