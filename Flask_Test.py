from flask import Flask
app = Flask(__name__)

#「Hello World!」を返す
@app.route("/")
def hello():
    return "Hello World!"

@app.route("/ping")
def ping():
	return "pong"

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000)
