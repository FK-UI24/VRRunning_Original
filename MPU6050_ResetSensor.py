import smbus2 as smbus
from mpu6050 import mpu6050
import time
import math

# MPU6050のI2Cアドレス（0x68）
sensor = mpu6050(0x68)

# 初期化
pitch_data = None  # Pitchのデータを格納する変数

# レジスタアドレス
PWR_MGMT_1 = 0x6B

# I2Cバスの作成
bus = smbus.SMBus(1)

# MPU6050をリセットする関数
def reset_mpu6050():
    try:
        # レジスタに値0x80を書き込むことでリセットを実行
        bus.write_byte_data(0x68, PWR_MGMT_1, 0x80)
        time.sleep(0.1)  # リセット後の待機時間

        # リセット後にセンサーが適切に動作するためには、再度PWR_MGMT_1レジスタを操作してリセットを解除する必要がある
        bus.write_byte_data(0x68, PWR_MGMT_1, 0x00)
        time.sleep(0.1)
        
        # センサー安定待ち
        time.sleep(5)

        print("MPU6050 reset successful!")
    except Exception as e:
        print(f"Error during MPU6050 reset: {e}")

reset_mpu6050()
