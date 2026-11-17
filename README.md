# Zadanie 2.1 — Kalkulator Desktopowy i Karta Obserwacji

**Autor:** Julia Kluga (`estr311a`)  
**Narzędzie:** Antigravity IDE (Google DeepMind)  
**Model:** Gemini 3.8 Flash (High)  

---

## 📌 Zawartość projektu
- `KalkulatorDesktop.exe` — gotowa, skompilowana natywna aplikacja desktopowa dla systemu Windows (.NET C#).
- `src/Program.cs` — pełny kod źródłowy aplikacji desktopowej (Windows Forms).
- `obserwacje/pierwszy-agent.md` — uzupełniona karta obserwacji pracy agenta AI zgodnie z wytycznymi zadania.
- `web/` — alternatywna wersja przeglądarkowa aplikacji (HTML5, CSS3, JavaScript) do podglądu w przeglądarce.
- `run.bat` — skrypt szybkiego uruchomienia aplikacji desktopowej.
- `run-web.bat` — skrypt uruchomienia wersji webowej.
- `2-1-kalkulator-desktop-julia-kluga/` — kompletna kopia projektu w folderze zgodnym ze wzorem nazewnictwa (`nr-zadania-nazwa-aplikacji-klasa-imie-nazwisko`).

---

## 🚀 Jak uruchomić?

### 1. Wersja desktopowa (Windows)
Wystarczy dwukrotnie kliknąć plik:
- **`run.bat`** lub bezpośrednio **`KalkulatorDesktop.exe`**

### 2. Ponowna kompilacja ze źródeł (opcjonalnie)
Jeśli chcesz skompilować kod ponownie, w wierszu poleceń PowerShell lub CMD uruchom:
```cmd
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe /target:winexe /out:KalkulatorDesktop.exe /r:System.dll,System.Drawing.dll,System.Windows.Forms.dll src\Program.cs
```

### 3. Wersja przeglądarkowa
Uruchom plik **`run-web.bat`** lub otwórz w dowolnej przeglądarce plik `web/index.html`.

---

## 📋 Karta obserwacji
Pełna dokumentacja przebiegu pracy z agentem AI znajduje się w pliku:  
👉 [obserwacje/pierwszy-agent.md](file:///c:/Users/kluga.julia/Desktop/2.1/obserwacje/pierwszy-agent.md)
