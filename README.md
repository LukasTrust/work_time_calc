# Work Time Calculator

Ein einfacher Arbeitszeitrechner geschrieben in C# mit WPF, um meine Arbeitsstunden während meines Praktikums zu verfolgen.

## Inhaltsverzeichnis

- [Hintergrund](#hintergrund)
- [Funktionsweise](#funktionsweise)
- [Installation](#installation)
- [Nutzung](#nutzung)
- [Screenshots](#screenshots)

## Hintergrund

Ich habe dieses Projekt gestartet, als ich ein Praktikum begonnen habe, bei dem ich mit C# arbeiten musste. Um meine Kenntnisse in C# zu vertiefen, habe ich dieses Projekt in meiner Freizeit entwickelt. Es half mir, meine Arbeitsstunden im Praktikum zu verfolgen und gleichzeitig meine Programmierfähigkeiten zu verbessern.

## Funktionsweise

- Beim Starten der Anwendung öffnet sich ein Fenster, das den aktuellen Monat anzeigt.
- Eine Liste zeigt die Tage des Monats und die jeweiligen Arbeitsstunden an.
- Die Gesamtarbeitszeit für den Monat wird summiert und angezeigt.
- Man kann zwischen vorherigen und nächsten Monaten wechseln.
- Es gibt Eingabefelder für:
  - Arbeitsbeginn
  - Pausenbeginn
  - Pausenende
  - Arbeitsende
- Die Pausenzeit wird von der Arbeitszeit abgezogen.
- Bereits erstellte Einträge können bearbeitet werden.
- Durch Klicken auf "Speichern" werden die Ergebnisse in eine Textdatei geschrieben.
- Beim erneuten Öffnen der Anwendung werden die gespeicherten Daten wieder geladen.

## Installation

1. Klone das Repository:
    ```sh
    git clone https://github.com/dein-benutzername/work-time-calculator.git
    ```
2. Öffne das Projekt in Visual Studio.
3. Stelle sicher, dass du alle notwendigen Abhängigkeiten installiert hast.
4. Baue und starte die Anwendung.

## Nutzung

1. Starte die Anwendung.
2. Gib die Arbeitszeiten für den jeweiligen Tag ein.
3. Klicke auf "Speichern", um die Daten zu speichern.
4. Wechsle zwischen den Monaten, um vergangene oder zukünftige Arbeitszeiten zu sehen.
5. Bearbeite bereits erstellte Einträge bei Bedarf.

## Screenshots

![Screenshot](screenshot.png)
