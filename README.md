# 🌐 Botnet-With-Minecraft (PoC)

<p align="center">
  <img src="https://img.shields.io/badge/Language-Java%2021-orange?style=for-the-badge&logo=openjdk" alt="Java 21" />
  <img src="https://img.shields.io/badge/Backend-Python%20Flask-blue?style=for-the-badge&logo=flask" alt="Flask" />
  <img src="https://img.shields.io/badge/GUI%20Tool-C%23%20%2F%20.NET-purple?style=for-the-badge&logo=dotnet" alt=".NET" />
  <img src="https://img.shields.io/badge/License-MIT-green?style=for-the-badge" alt="MIT License" />
</p>

---

## 📌 Language / Dil Seçimi
- [English](#-english)
- [Türkçe](#-türkçe)

---

# 🇬🇧 English

## 📖 Overview
**Botnet-With-Minecraft** is an experimental educational **Proof-of-Concept (PoC)** demonstrating how distributed Minecraft game servers can act as coordinated network nodes via custom game plugins. 

The project highlights the critical importance of supply-chain security and plugin auditing in gaming ecosystems, showing how an innocent-looking Bukkit/Spigot plugin can expose internal HTTP endpoints and communicate with a central command server.

> [!WARNING]
> **DISCLAIMER & LEGAL NOTICE:**
> This repository was developed strictly for **educational, security research, and personal laboratory testing** purposes. The author does not condone and is not responsible for any malicious use or unauthorized deployment. Do not run or distribute this code across networks without explicit written permission.

---

## 🏗️ Architecture

```
                      +-----------------------------+
                      |   Flask Admin Dashboard     |
                      |   (Command & Monitor API)   |
                      +--------------+--------------+
                                     |
               +---------------------+---------------------+
               | Broadcast Target                          | Broadcast Target
               v                                           v
    +----------------------+                    +----------------------+
    |  Minecraft Server A  |                    |  Minecraft Server B  |
    |  - Spark HTTP (8080) |                    |  - Spark HTTP (8080) |
    |  - Async HTTP Client |                    |  - Async HTTP Client |
    +----------+-----------+                    +----------+-----------+
               |                                           |
               +---------------------+---------------------+
                                     | Concurrent HTTP Pings
                                     v
                        +-------------------------+
                        |       Target Host       |
                        +-------------------------+
```

### Components
1. **Admin Server (`Admin Server/`):**
   - Built with **Python / Flask**.
   - Handles node registration (`/register`), status monitoring (`/online`), and task dispatching (`/ping`).
   - Provides a real-time web dashboard for node management.
2. **Minecraft Plugin (`Minecraft Plugin/`):**
   - Built with **Java 21** using Paper/Spigot APIs and **Spark Java**.
   - Spawns an internal HTTP micro-server on port `8080`.
   - Listens for `/ping?url=` triggers and performs non-blocking asynchronous HTTP requests.
3. **Plugin Compiler (`Plugin Compiler/`):**
   - Standalone **C# / .NET WinForms** utility.
   - Automates `.env` injection and Maven build orchestration for non-technical users.

---

## 🚀 Getting Started

### 1. Prerequisites
- **Python 3.10+**
- **Java Development Kit (JDK) 21**
- **Apache Maven 3.9+**
- **Static IP** or port forwarding service (e.g., [ngrok](https://ngrok.com/))

---

### 2. Admin Server Setup
```bash
# Clone the repository
git clone https://github.com/DarkMirrorq/Botnet-With-Minecraft.git
cd Botnet-With-Minecraft/Admin\ Server

# Install dependencies
pip install -r requirements.txt

# Start the dashboard
python app.py
```
The dashboard will be accessible at `http://localhost:5000`.

---

### 3. Minecraft Plugin Build & Configuration

#### Method A: Manual Compilation (Maven)
1. Navigate to the plugin directory:
   ```bash
   cd "Minecraft Plugin"
   ```
2. Edit `src/main/resources/.env`:
   ```env
   API_URL=http://<YOUR_ADMIN_IP_OR_NGROK_DOMAIN>:5000
   ```
3. (Optional) In `src/main/java/me/plugin/Main.java`, toggle `debugMode` (`true` for sandbox logging, `false` for silent operation).
4. Compile the JAR file:
   ```bash
   mvn clean install
   ```
5. Retrieve the output JAR from `target/aktiviteplugin-1.0-SNAPSHOT.jar` and place it in your Minecraft server's `plugins/` directory.

#### Method B: Automated GUI Compiler (.NET)
1. Open `Plugin Compiler/Plugin Derleyici.sln` in **Visual Studio**.
2. Build or publish the desktop utility.
3. Place `maven` and `source_code` in the application root, enter your API URL into the GUI, and click **Derle (Compile)**.

---

# 🇹🇷 Türkçe

## 📖 Genel Bakış
**Botnet-With-Minecraft**, üçüncü parti oyun eklentilerinin arka planda dağıtık bir ağ düğümü (node) gibi nasıl çalışabileceğini inceleyen deneysel bir **Proof-of-Concept (PoC / Konsept Kanıtı)** projesidir.

Bu proje, oyun sunucusu ekosistemlerindeki tedarik zinciri (supply-chain) güvenlik açıklarına ve güvenilmeyen eklentilerin doğurabileceği risklere dikkat çekmek amacıyla geliştirilmiştir. Masum görünen bir Bukkit/Spigot eklentisinin nasıl bağımsız bir HTTP dinleyicisi açabileceğini ve merkezi bir API ile koordine olabileceğini gösterir.

> [!WARNING]
> **YASAL UYARI VE SORUMLULUK REDDİ:**
> Bu yazılım yalnızca **eğitim, siber güvenlik araştırmaları ve izole laboratuvar ortamlarında test yapılmak üzere** geliştirilmiştir. Geliştirici, yazılımın yetkisiz, izinsiz veya kötüye kullanımından hiçbir şekilde sorumlu tutulamaz. Yazılımı yalnızca izin aldığınız kendi sunucularınızda test ediniz.

---

## 🏗️ Sistem Mimarisi

- **Admin Server (Yönetim Paneli):** Flask ile hazırlanmış merkezi yönetim konsolu.
  - `/register`: Yeni sunucuların IP ve port (8080) bilgilerini kaydeder.
  - `/online`: Düğümlerin anlık erişilebilirliğini doğrular.
  - `/ping`: Online durumdaki tüm düğümlere eşzamanlı HTTP istek komutu iletir.
- **Minecraft Plugin (İstemci Eklenti):** Java 21 ve Spark framework kullanılarak kodlanmıştır.
  - Sunucu açıldığında Admin API'ye kendi IP'sini bildirir.
  - 8080 portunda bir HTTP micro-server başlatır.
  - Panelden gelen hedef URL isteklerini arka planda asenkron (`HttpClient`) olarak hedefe gönderir.
- **Plugin Compiler (Derleyici Arayüzü):** C# ile yazılmış masaüstü aracı.
  - Manuel Maven komutlarıyla uğraşmak istemeyen kullanıcılar için `.env` dosyasındaki API URL'sini güncelleyip otomatik derleme yapar.

---

## 🛠️ Kurulum ve Çalıştırma

### 1. Gereksinimler
- **Python 3.10+**
- **Java JDK 21**
- **Apache Maven 3.9+**
- Sabit IP adresi veya tünelleme aracı (örn: [ngrok](https://ngrok.com/))

---

### 2. Admin Server'ı Başlatma
```bash
# Depoyu klonlayın
git clone https://github.com/DarkMirrorq/Botnet-With-Minecraft.git
cd Botnet-With-Minecraft/Admin\ Server

# Gereksinimleri yükleyin
pip install -r requirements.txt

# Paneli çalıştırın
python app.py
```
Panel varsayılan olarak `http://localhost:5000` adresinde çalışacaktır.

---

### 3. Eklentiyi Derleme ve Yapılandırma

#### Yöntem 1: Maven ile Manuel Derleme
1. Eklenti dizinine geçin:
   ```bash
   cd "Minecraft Plugin"
   ```
2. `src/main/resources/.env` dosyasını açıp panel adresinizi yazın:
   ```env
   API_URL=http://<IP_VEYA_NGROK_ADRESINIZ>:5000
   ```
3. Test loglarını görmek veya kapatmak isterseniz `Main.java` içerisindeki `debugMode` değerini (`true` / `false`) ayarlayın.
4. Eklentiyi paketleyin:
   ```bash
   mvn clean install
   ```
5. Oluşan `target/aktiviteplugin-1.0-SNAPSHOT.jar` dosyasını Minecraft sunucunuzun `plugins/` klasörüne atıp sunucuyu başlatın.

#### Yöntem 2: C# Derleyici Aracı ile
1. `Plugin Compiler/Plugin Derleyici.sln` projesini **Visual Studio** ile açın ve Publish edin.
2. Uygulama arayüzünden Admin Server adresinizi girerek tek tıkla JAR çıktısı alın.

---

## 📄 Lisans
Bu proje [MIT Lisansı](LICENSE) ile lisanslanmıştır.
