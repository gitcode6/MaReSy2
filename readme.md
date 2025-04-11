# Konfigurationshinweis API für Datenbankanbindung mittels geheime Benutzerschlüssel
---
### Voraussetungen
- **.NET SDK 9.0** installiert
- Entwicklungsumgebung Visual Studio 2022 v17.12 oder neuer
- Workload \"ASP.NET und Webentwicklung" in Visual Studio installiert


## Einrichtung von User Secrets
1. **API Projekt öffnen** <i>(in \<Repo-Pfad\>\MaReSy2\MaReSy2_API)</i> durch klick auf die <i>MaReSy2_Api.sln</i> Projektdatei
<br>

2. **Visual Studio Console öffnen** (Menüleiste: Ansicht / Terminal) und folgenden Befehl eingeben:
    ```cmd
    dotnet user-secrets init
    ```
<br>

3. **Pfad zur Datenbank-Datei** (database1_maresy2.mdf) **kopieren**
<i>(befindet sich in \<Repo-Pfad\>\MaReSy2\Datenbank\database_maresy2)</i>


<br>

4. **User-Secret für Datenbankverbindung hinzufügen**
<b>Achtung:</b> \<Pfad\> mit dem im Schritt 3 kopierten Pfad ersetzten <br>
    Durch Befehl in Visual Studio Console:
    ```cmd
    dotnet user-secrets set "ConnectionStrings:Datenbank" "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='<Pfad>';Integrated Security=True;Connect Timeout=30;Encrypt=True" ""
    ```

    Sollte das nicht funktionieren, kann jederzeit durch den Rechtsklick auf das Projekt und geheime Benutzerschlüssel verwalten der ConnectionString überprüft werden.

    Einen Beispielhaften ConnectionString finden Sie im API Ordner in `Datenbankverbindungsstring_Tobias.txt`