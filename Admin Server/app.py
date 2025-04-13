from flask import Flask, request, jsonify, render_template
import requests
import os
import time

app = Flask(__name__)

REGISTER_FILE = "register.txt"
request_count = 0
request_history = []
ping_stats = {}

def get_registered_urls():
    if os.path.exists(REGISTER_FILE):
        with open(REGISTER_FILE, 'r') as f:
            return [line.strip() for line in f.readlines()]
    return []

def add_url_to_register(url):
    with open(REGISTER_FILE, 'a') as f:
        f.write(url + '\n')

@app.route('/')
def index():
    registered_urls = get_registered_urls()
    online_count = 0
    online_servers = []

    for server_url in registered_urls:
        try:
            response = requests.get(f"{server_url}/online", timeout=5)
            if response.status_code == 200:
                online_count += 1
                online_servers.append(server_url)
        except requests.exceptions.RequestException:
            continue

    offline_count = len(registered_urls) - online_count

    return render_template('index.html', online_count=online_count, offline_count=offline_count)

@app.route('/register', methods=['POST'])
def register():
    global request_count, request_history
    request_count += 1
    server_url = request.form.get('url')
    if not server_url:
        return jsonify({"error": "URL is required"}), 400

    full_server_url = f"http://{server_url}:8080"
    registered_urls = get_registered_urls()

    if full_server_url in registered_urls:
        return jsonify({"message": "URL already registered"}), 200

    add_url_to_register(full_server_url)
    req_data = {"url": full_server_url, "type": "register", "time": time.strftime("%H:%M:%S"), "server_url": full_server_url, "success": True} # Başarı durumunu ekledik
    request_history.append(req_data)
    return jsonify({"message": f"URL registered: {full_server_url}"}), 201

@app.route('/ping', methods=['POST'])
def ping():
    global request_count, request_history, ping_stats
    request_count += 1
    target_url = request.form.get('url')
    if not target_url:
        return jsonify({"error": "Target URL is required"}), 400

    registered_urls = get_registered_urls()
    results = {}

    online_servers = []
    for server_url in registered_urls:
        try:
            response = requests.get(f"{server_url}/online", timeout=5)
            if response.status_code == 200:
                online_servers.append(server_url)
        except requests.exceptions.RequestException:
            continue

    total_requests = 0
    total_success = 0
    for server_url in online_servers:
        if server_url not in ping_stats:
            ping_stats[server_url] = {"total": 0, "success": 0}
        ping_stats[server_url]["total"] += 1
        total_requests += 1

        try:
            response = requests.get(f"{server_url}/ping?url={target_url}", timeout=5)
            results[server_url] = response.text
            ping_stats[server_url]["success"] += 1
            total_success += 1
            success = True # Başarılı istek
        except requests.exceptions.RequestException as e:
            results[server_url] = str(e)
            success = False # Başarısız istek

        req_data = {"url": target_url, "type": "ping", "time": time.strftime("%H:%M:%S"), "server_url": server_url, "success": success} # Başarı durumunu ekledik
        request_history.append(req_data)
    return render_template('index.html', ping_results=results, online_count=len(online_servers), offline_count=len(registered_urls) - len(online_servers), request_count=request_count, request_history=request_history, ping_stats=ping_stats, total_requests = total_requests, total_success = total_success)

if __name__ == '__main__':
    app.run(port=5000)
