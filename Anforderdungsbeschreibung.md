

**Anforderungsdokument: Audio-Pegelwarnung für Focusrite Scarlett**

**Version:** 1.2 (Fokus auf optimale Codebase und Performance)
**Datum:** 27. Mai 2025

## 1. Einleitung und Zielsetzung

* **1.1 Projektübersicht:** Diese Anwendung soll Benutzer eines Focusrite Scarlett Audio-Interfaces unter Windows 11 in Echtzeit warnen, wenn ihr Mikrofoneingangspegel bestimmte, konfigurierbare Schwellenwerte überschreitet. Die Warnung erfolgt durch ein visuelles Overlay am Rand jedes Monitors und optional durch eine akustische Signalisierung. **Ein Kernziel bei der Entwicklung ist die Erreichung höchstmöglicher Performance und einer exzellenten, wartbaren Codebasis.**
* **1.2 Ziel:** Das Hauptziel ist es, dem Benutzer eine sofortige, präzise und ressourcenschonende Rückmeldung über Eingangspegel zu geben, um die Audioqualität zu sichern. Die Softwarearchitektur soll auf Langlebigkeit, Stabilität und einfache Weiterentwicklung ausgelegt sein.
* **1.3 Zielgruppe:** Musiker, Podcaster, Streamer und andere Content Creator, die ein Focusrite Scarlett Audio-Interface unter Windows 11 verwenden und Wert auf eine hochperformante und zuverlässige Monitoring-Lösung legen.

---

## 2. Funktionale Anforderungen (Was soll die Software tun?)

* **FA01: Auswahl des Audio-Eingangsgeräts**
    * Die Anwendung muss dem Benutzer ermöglichen, das spezifische Focusrite Scarlett Audio-Interface und den gewünschten Eingangskanal (falls zutreffend und technisch umsetzbar) als Audioquelle auszuwählen.
    * Eine automatische Erkennung des angeschlossenen Focusrite-Geräts wäre wünschenswert.
    * Falls mehrere Focusrite-Geräte oder andere kompatible Mikrofone angeschlossen sind, muss eine klare Auswahlmöglichkeit bestehen.
* **FA02: Echtzeit-Audiopegelmessung**
    * Die Anwendung muss den Audiopegel des ausgewählten Mikrofoneingangs kontinuierlich und in Echtzeit überwachen.
    * Der Pegel soll in dBFS (Dezibel relativ zu Full Scale) gemessen werden.
* **FA03: Mehrstufige, konfigurierbare Warnschwellenwerte mit Farbverlauf**
    * Der Benutzer muss mehrere Pegelstufen definieren können (z.B. "Sicher", "Achtung", "Kritisch/Rot").
    * Jeder Stufe muss ein dBFS-Schwellenwert zugeordnet werden können.
    * Diesen Stufen entsprechend soll ein Farbverlauf für das visuelle Overlay definiert werden (z.B. von Grün über Gelb nach Rot). Die genauen Farben des Verlaufs sollten idealerweise konfigurierbar sein, mindestens aber ein sinnvoller Standardverlauf (Grün-Gelb-Rot) implementiert sein.
    * Die Einstellungen müssen persistent gespeichert werden.
* **FA04: Visuelles Overlay als Warnung**
    * **FA04.1: Design und Position:**
        * Das Overlay soll sich als **schmaler Streifen am Rand jedes angeschlossenen Monitors** befinden. Der Benutzer sollte den Rand auswählen können (z.B. oberer, unterer, linker oder rechter Rand).
        * Das Overlay visualisiert den aktuellen Pegel durch einen **Farbverlauf**.
        * _Anmerkung:_ Die Implementierung muss hochgradig optimiert sein, um flüssige visuelle Updates ohne Beeinträchtigung der Systemleistung oder anderer Anwendungen zu gewährleisten, selbst bei mehreren hochauflösenden Displays.
    * **FA04.2: Dynamische Transparenz:**
        * Bei niedrigem Pegel (z.B. im "grünen" Bereich) ist das Overlay **nahezu vollständig transparent**.
        * Mit steigendem Pegel wird das Overlay **kontinuierlich weniger transparent** und die Farbe ändert sich entsprechend dem definierten Farbverlauf (siehe FA03).
    * **FA04.3: Verhalten im kritischen Bereich:**
        * Sobald der Pegel den für den "roten" (kritischen) Bereich definierten Schwellenwert erreicht oder überschreitet, soll das Overlay in der entsprechenden kritischen Farbe (z.B. Rot) **mindestens zwei Sekunden lang sichtbar bleiben**, auch wenn der Pegel innerhalb dieser zwei Sekunden wieder sinkt. Danach richtet es sich wieder nach dem aktuellen Pegel.
    * **FA04.4: Konfigurierbarkeit des Overlays (Wünschenswert):**
        * Dicke/Breite des Overlay-Streifens.
        * Auswahl des Monitorrands für die Anzeige.
* **FA05: Akustische Warnung**
    * Die Anwendung muss eine Option für eine **akustische Warnung** bieten, wenn ein bestimmter (vom Benutzer konfigurierbarer, z.B. der kritische) Schwellenwert überschritten wird.
    * Das Warnsignal soll eine **kurze Sinuswelle** sein.
    * Der Benutzer muss die akustische Warnung aktivieren/deaktivieren können.
    * Wünschenswert: Einstellbare Lautstärke für die akustische Warnung.
* **FA06: Einstellungsmenü/Konfigurationsoberfläche**
    * Die Anwendung benötigt eine Benutzeroberfläche, über die der Benutzer:
        * Das Audio-Eingangsgerät auswählen kann (FA01).
        * Die Pegelstufen und den Farbverlauf konfigurieren kann (FA03).
        * Einstellungen für das Overlay (Position, Verhalten) vornehmen kann (FA04).
        * Die akustische Warnung aktivieren/deaktivieren und konfigurieren kann (FA05).
        * Die Autostart-Funktion aktivieren/deaktivieren kann (FA07).
        * Die Anwendung starten/stoppen oder in den System-Tray minimieren kann.
* **FA07: Anwendung im Hintergrund/System-Tray und Autostart**
    * Die Anwendung sollte ressourcenschonend im Hintergrund laufen können.
    * Ein Symbol im System-Tray (Infobereich der Taskleiste) wäre wünschenswert, um schnellen Zugriff auf Einstellungen oder das Beenden der Anwendung zu ermöglichen. Das Tray-Icon könnte den Status anzeigen (z.B. grün = aktiv und unter Schwelle, rot = aktiv und über Schwelle).
    * Die Anwendung muss eine **Option bieten, um automatisch mit Windows zu starten.**
* **FA08: Persistente Einstellungen**
    * Alle Benutzereinstellungen (ausgewähltes Gerät, Schwellenwerte, Overlay-Präferenzen, Autostart-Einstellung) müssen gespeichert werden, sodass sie beim nächsten Start der Anwendung erhalten bleiben.

---

## 3. Nicht-Funktionale Anforderungen (Wie gut soll die Software sein?)

* **NFA01: Performance (Höchste Priorität)**
    * **NFA01.1: Minimale Latenz:** Die Verzögerung zwischen dem Auftreten eines Pegelereignisses und der entsprechenden visuellen/akustischen Reaktion muss für das menschliche Auge/Ohr nicht wahrnehmbar sein (Ziel: <<50ms).
    * **NFA01.2: Extrem geringe CPU-Auslastung:** Die Anwendung muss im Ruhezustand (Pegelüberwachung ohne Warnung) eine vernachlässigbare CPU-Last erzeugen. Auch während aktiver Warnungen (Overlay-Animation, Ton) darf die CPU-Last das System nur minimal beanspruchen, um keine anderen Anwendungen (Recording-Software, Spiele, Streaming-Tools) zu beeinträchtigen.
    * **NFA01.3: Effiziente Speichernutzung:** Der Arbeitsspeicherbedarf muss gering sein und es dürfen keine Speicherlecks auftreten.
    * **NFA01.4: Flüssige Animationen/Übergänge:** Alle visuellen Änderungen am Overlay (Farbverlauf, Transparenz) müssen absolut flüssig und ohne Ruckeln erfolgen.
    * **NFA01.5: Optimierte Rendering-Pipeline:** Für das Overlay ist eine effiziente Rendering-Methode zu wählen, die die GPU nutzt, wo sinnvoll (z.B. über DirectX-Interops mit WPF/WinUI), um die CPU zu entlasten.
    * **NFA01.6: Kein unnötiges Aufwecken der CPU:** Hintergrundprozesse müssen so gestaltet sein, dass sie die CPU nur dann aktiv beanspruchen, wenn es unbedingt notwendig ist (Event-gesteuerte Verarbeitung bevorzugen).
* **NFA02: Codequalität und Wartbarkeit (Hohe Priorität)**
    * **NFA02.1: Modularität und Kohäsion:** Der Code muss in klar definierte, logische und voneinander entkoppelte Module (Assemblies/Namespaces) strukturiert sein (z.B. Audio-Engine, UI-Logik, Overlay-Rendering, Konfigurationsmanagement). Jedes Modul soll eine hohe Kohäsion aufweisen.
    * **NFA02.2: Lesbarkeit und Konsistenz:** Der Code muss durchgängig gut lesbar, verständlich und selbsterklärend sein. Einheitliche Namenskonventionen (z.B. Microsoft C# Coding Conventions) und Formatierungsrichtlinien sind strikt einzuhalten. Kommentare sollen dort verwendet werden, wo der Code nicht trivial verständlich ist ("Warum" statt "Was").
    * **NFA02.3: Testbarkeit:** Kritische Komponenten (insbesondere die Audioverarbeitung, Pegelberechnung, Schwellenwertlogik) müssen so entworfen werden, dass sie durch Unit-Tests und ggf. Integrationstests automatisiert überprüfbar sind. Eine hohe Testabdeckung für die Kernlogik ist anzustreben.
    * **NFA02.4: SOLID-Prinzipien:** Wo anwendbar, sollen die SOLID-Prinzipien des objektorientierten Designs befolgt werden, um eine flexible und wartbare Architektur zu fördern.
    * **NFA02.5: Keine "Magic Numbers" oder Strings:** Hartcodierte Konstanten sind zu vermeiden und stattdessen über benannte Konstanten oder Konfigurationsdateien zu verwalten.
    * **NFA02.6: Effizientes Ressourcenmanagement:** Explizite Freigabe von nicht mehr benötigten Ressourcen (insbesondere bei der Audioverarbeitung und Grafikobjekten durch `IDisposable`).
    * **NFA02.7: Asynchrone Programmierung:** Korrekte Anwendung von asynchronen Mustern (`async`/`await`), um die UI reaktionsfähig zu halten und Thread-Blockaden zu vermeiden, insbesondere bei I/O-Operationen oder länger laufenden Hintergrundaufgaben.
* **NFA03: Benutzerfreundlichkeit (Usability)**
    * Intuitive Bedienung und Konfiguration trotz der technischen Komplexität im Hintergrund.
    * Klares, unmissverständliches Feedback durch Overlay und optionalen Ton.
* **NFA04: Zuverlässigkeit und Stabilität**
    * Die Anwendung muss über lange Zeiträume stabil und ohne Abstürze laufen.
    * Akkurate Pegelmessung unter allen Umständen.
    * Robustes Fehlerhandling: Die Anwendung soll auf unerwartete Zustände (z.B. Trennung des Audiogeräts) kontrolliert reagieren und den Benutzer ggf. informieren, anstatt abzustürzen.
* **NFA05: Kompatibilität**
    * **NFA05.1: Betriebssystem:** Windows 11.
    * **NFA05.2: Audio-Treiber:** Zuverlässige Zusammenarbeit mit Focusrite Scarlett Treibern (ASIO bevorzugt für geringste Latenz, alternativ WASAPI).
    * **NFA05.3: Multi-Monitor-Umgebungen:** Fehlerfreie und performante Funktion auf Systemen mit mehreren Monitoren unterschiedlicher Auflösungen und DPI-Einstellungen.

---

## 4. Anwendungsfälle (Use Cases)

* **UC01: Erstkonfiguration der Anwendung**
    1.  Benutzer startet die Anwendung.
    2.  Benutzer öffnet das Einstellungsmenü.
    3.  Benutzer wählt das Focusrite Scarlett als Eingangsquelle aus.
    4.  Benutzer konfiguriert 2-3 Pegelstufen (z.B. Grün bis -12 dBFS, Gelb bis -6 dBFS, Rot ab -5 dBFS) und passt ggf. den Farbverlauf an.
    5.  Benutzer wählt den oberen Rand für das Overlay.
    6.  Benutzer aktiviert die akustische Warnung (Sinuswelle) für den roten Bereich.
    7.  Benutzer aktiviert die Autostart-Option.
    8.  Benutzer speichert die Einstellungen und aktiviert die Überwachung.
* **UC02: Pegelüberschreitung während der Nutzung**
    1.  Anwendung läuft im Hintergrund. Das Overlay ist am oberen Rand jedes Monitors als sehr transparenter grüner Streifen kaum sichtbar.
    2.  Benutzer spricht lauter. Der Overlay-Streifen wird satter grün und weniger transparent.
    3.  Benutzer wird noch lauter, Pegel erreicht den gelben Bereich. Der Streifen färbt sich gelb und wird deutlicher sichtbar.
    4.  Benutzer ruft ins Mikrofon, Pegel erreicht den roten Bereich (-5 dBFS). Der Streifen wird rot und maximal sichtbar. Eine kurze Sinuswelle ertönt.
    5.  Benutzer reduziert sofort den Eingangspegel. Der rote Streifen bleibt für 2 Sekunden sichtbar und verschwindet dann bzw. passt sich dem nun niedrigeren Pegel an (z.B. wird wieder gelb oder grün und transparenter).
* **UC03: Änderung des Warnschwellenwerts**
    1.  Benutzer öffnet das Einstellungsmenü über das Tray-Icon oder die Programmoberfläche.
    2.  Benutzer ändert den bestehenden Schwellenwert auf einen neuen Wert.
    3.  Benutzer speichert die Änderung. Die Überwachung verwendet nun den neuen Schwellenwert.
* **UC04: Schnelles Aktivieren/Deaktivieren der Überwachung (oder der akustischen Warnung)**
    1.  Benutzer klickt mit der rechten Maustaste auf das Tray-Icon (oder öffnet Einstellungen).
    2.  Benutzer wählt "Überwachung deaktivieren/aktivieren" oder "Akustische Warnung deaktivieren/aktivieren".
    3.  Die Anwendung stoppt/startet die Pegelmessung und Overlay-Anzeige bzw. die akustische Warnung entsprechend.

---

## 5. Zu klärende Punkte / Überlegungen für die Entwicklung

1.  **Overlay-Implementierung:** Die technische Lösung muss von vornherein auf maximale Performance und minimale Systembelastung ausgelegt sein. Benchmarking verschiedener Ansätze ist ggf. notwendig, um die Anforderungen aus NFA01 und NFA05.3 sicherzustellen. Dies betrifft insbesondere die Interaktion mit dem Windows Desktop Window Manager (DWM) und die Handhabung von Vollbildanwendungen.
2.  **Farbverlauf-Konfiguration:** Wie detailliert soll der Benutzer den Farbverlauf einstellen können? (Feste Schemata, Auswahl von Start-/Mittel-/Endfarbe, oder eine Liste von Farbstopps?)
3.  **Monitorrand-Auswahl:** Soll der Benutzer den Rand pro Monitor einzeln wählen können oder gilt die Einstellung global für alle Monitore? (Global ist einfacher zu implementieren).
4.  **ASIO-Implementierung:** Bei Nutzung von ASIO ist die korrekte Lizenzierung und Weitergabe von ASIO-Host-Komponenten zu beachten (falls nicht nur das Steinberg ASIO SDK genutzt wird, sondern Drittanbieter-Wrapper).

---

## 6. Außerhalb des Geltungsbereichs (Out of Scope für V1.0)

* Audioaufnahme oder -bearbeitung innerhalb dieser Anwendung.
* Komplexe Audio-Analyse-Tools (z.B. Spektrogramm, detaillierte Frequenzanalyse).
* Netzwerkfunktionalitäten.
* Unterstützung für andere Betriebssysteme als Windows 11.
* Sprachunterstützung: Primär Deutsch (weitere Sprachen sind Out of Scope für V1.0, können aber bei der Code-Struktur für spätere Internationalisierung berücksichtigt werden – z.B. durch Ressourcen-Dateien für Strings).

---