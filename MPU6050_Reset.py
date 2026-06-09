import smbus
import time

# MPU6050のI2Cアドレス
MPU6050_ADDR = 0x68

# レジスタアドレス
PWR_MGMT_1 = 0x6B

# I2Cバスの作成
bus = smbus.SMBus(1)

# MPU6050をリセットする関数
def reset_mpu6050():
    # レジスタに値0x80を書き込むことでリセットを実行
    bus.write_byte_data(MPU6050_ADDR, PWR_MGMT_1, 0x80)
    time.sleep(0.1)  # リセット後の待機時間
    # リセット後にセンサーが適切に動作するためには、再度PWR_MGMT_1レジスタを操作してリセットを解除する必要がある
    bus.write_byte_data(MPU6050_ADDR, PWR_MGMT_1, 0x00)

# センサーのリセット
reset_mpu6050()
print("MPU6050 has been reset")
