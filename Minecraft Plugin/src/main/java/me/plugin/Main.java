package me.plugin;

import org.bukkit.plugin.java.JavaPlugin;
import static spark.Spark.*;
import java.io.IOException;
import java.net.InetAddress;
import java.net.URI;
import java.net.URLEncoder;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import java.nio.charset.StandardCharsets;
import java.util.concurrent.CompletableFuture;
import io.github.cdimascio.dotenv.Dotenv;

public class Main extends JavaPlugin {
    private String apiUrl; // Flask API URL'si
    private final boolean debugMode = true; // Debug modu değişkeni

    @Override
    public void onEnable() {
        getLogger().warning("Plugin aktif!");

        // .env dosyasından API URL'sini oku
        Dotenv dotenv = Dotenv.load();
        apiUrl = dotenv.get("API_URL");

        if (apiUrl == null || apiUrl.isEmpty()) {
            if (debugMode) {
                getLogger().warning("API_URL .env dosyasında ayarlanmamış! Varsayılan URL kullanılacak.");
            }
            apiUrl = ""; // Varsayılan URL
        }

        if (debugMode) {
            getLogger().warning("Debug modu etkin!");
            getLogger().warning("API URL: " + apiUrl);
        }

        // Sunucunun kendi IP'sini al ve kaydet
        CompletableFuture.runAsync(this::registerServer);

        // HTTP dinleme sunucusunu başlat
        port(8080);

        // favicon.ico yolunu ekle
        get("/favicon.ico", (req, res) -> {
            res.status(204); // No Content
            return "";
        });

        get("/ping", (req, res) -> {
            String targetUrl = req.queryParams("url");
            if (targetUrl != null && !targetUrl.isEmpty()) {
                CompletableFuture.runAsync(() -> pingSite(targetUrl));
                return "PİNG BAŞARILI."; // Return a response to the HTTP request
            } else {
                return "Url Eksik Gönderildi"; // Return an error message if the URL is missing
            }
        });

        get("/online", (req, res) -> {
            res.status(200);
            return "";
        });

        // Tanımlanmayan rotaları işle
        notFound((req, res) -> {
            res.status(404);
            return "Sayfa Bulunamadı";
        });
    }

    private void registerServer() {
        String serverIp = getServerIp();
        String registerApiUrl = apiUrl + "/register?url=" + URLEncoder.encode(serverIp, StandardCharsets.UTF_8);

        try {
            HttpClient client = HttpClient.newHttpClient();
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(registerApiUrl))
                    .build();

            client.sendAsync(request, HttpResponse.BodyHandlers.ofString())
                    .thenApply(HttpResponse::statusCode)
                    .thenAccept(responseCode -> {
                        if (responseCode == 200) {
                            if (debugMode) {
                                getLogger().warning("Sunucu başarıyla API'ye kaydedildi!");
                            }
                        } else {
                            if (debugMode) {
                                getLogger().warning("Sunucu kaydedilemedi, API Hatası. Response Code: " + responseCode);
                            }
                        }
                    })
                    .join();

        } catch (Exception e) {
            if (debugMode) {
                getLogger().warning("Kayıt sırasında hata oluştu: " + e.getMessage());
            }
        }
    }

    private void pingSite(String targetUrl) {
        try {
            HttpClient client = HttpClient.newHttpClient();
            HttpRequest request = HttpRequest.newBuilder()
                    .uri(URI.create(targetUrl))
                    .build();

            client.sendAsync(request, HttpResponse.BodyHandlers.ofString())
                    .thenApply(HttpResponse::statusCode)
                    .thenAccept(statusCode -> {
                        if (debugMode) {
                            getLogger().warning("Ping Sonuçları: Status Code: " + statusCode);
                        }
                    })
                    .join();

        } catch (Exception e) {
            if (debugMode) {
                getLogger().warning("Ping sırasında hata oluştu: " + e.getMessage());
            }
        }
    }

    private String getServerIp() {
        try {
            return InetAddress.getLocalHost().getHostAddress();
        } catch (IOException e) {
            if (debugMode) {
                getLogger().warning("IP alınırken hata oluştu: " + e.getMessage());
            }
            return "Unknown IP";
        }
    }
}
