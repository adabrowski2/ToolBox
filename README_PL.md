README – Instrukcja obsługi systemu ToolBox
--------------------------------------------------------------
1. ZAWARTOŚĆ
--------------------------------------------------------------
ToolBox/ – projekt głównej aplikacji WPF  
Generator_kont/ – aplikacja konsolowa do tworzenia kont pracowników  
ToolBox_DB.bacpac – plik do zaimportowania bazy danych
--------------------------------------------------------------
2. IMPORT BAZY DANYCH
--------------------------------------------------------------
Żeby zaimportować bazę danych należy:

1. Połączyć się z bazą danych
2. Kliknąć prawym przyciskiem myszy na "Databases" => Import Data-tier Application...
3. Następnie kliknąć next
4. Kliknąć Browse i wskazać plik ToolBox_DB.bacpac
5. Kliknąć Next, next, import
6. Po zakończeniu baza powinna być widoczna w Databases pod nazwą "ToolBox_DB"
--------------------------------------------------------------
3. KONFIGURACJA BAZY DANYCH W APLIKACJI
--------------------------------------------------------------
Żeby skonfigurować odpowiednio bazę należy:

1. Przejść do Praca Inżynierska v1 => App.config
2. Po kliknięciu powinno otworzyć się rozwiązanie na zakładce App.config w Visual Studio
3. W miejscu:

connectionString="Server=(localdb)\localdb;Initial Catalog=ToolBox_DB;Integrated Security=True;"

należy dostosować connection string do nazwy serwera lokalnego komputera, czyli nazwę serwera lokalnego należy podstawić za (localdb)\localdb

4. Następnie należy zapisać zmiany w rozwiązaniu
--------------------------------------------------------------
5. TWORZENIE KONT – APLIKACJA KONSOLOWA
--------------------------------------------------------------
W bazie są stworzone 4 konta użytkowników z nadanymi rolami. Aplikacja jest wyłącznie pomocnicza, służy za opcję dodania konta nowego użytkownika do systemu (np. dla administratora) i używa szyfrowania hasła. Tym sposobem zapisani użytkownicy w bazie mają zaszyfrowane hasło.

Aplikację można włączyć przechodząc do Generator_kont => Generator_kont.sln oraz klikając F5

Ważna informacja przy tworzeniu konta!
Przy wpisywaniu roli, aby aplikacja działała poprawnie należy wpisać jedną z 4 ról zgodnie z tym zapisem:

doradca_handlowy
magazynier
kierownik_dzialu_handlowego
kierownik_magazynu

--------------------------------------------------------------
6. URUCHOMIENIE APLIKACJI GŁÓWNEJ
--------------------------------------------------------------
Żeby uruchomić główną aplikację ToolBox należy:

1. Przenieść projekt Praca inżynierska v1 do reporzytorium z którego otwierane są rozwiązania w zainstalowanym Visual Studio
2. Włączyć Visual Stduio najlepiej w trybie administratora
3. Załadować projekt ToolBox
4. Zbudować rozwiązanie przy pomocy Ctrl+Shit+B
5. Upewnić się że projekt startowy to Praca
6. Uruchomić aplikację klikając F5
--------------------------------------------------------------
7. LOGOWANIE
--------------------------------------------------------------
Po uruchomieniu aplikacji pokaże się okno logowania. W bazie danych mamy konta 4 użytkowników z 4 nadanymi różnymi rolami. Poniżej znajdują się loginy oraz hasła do kont testowych:

DORADCA HANDLOWY - Uwaga! U doradcy handlowego jako jedynego hasło to test1234, a nie jak u innych test123
Login: doradca1
Hasło: test1234

MAGAZYNIER
Login: magazynier1
Hasło: test123

KIEROWNIK DZIAŁU HANDLOWEGO
Login: kierownikDH
Hasło: test123

KIEROWNIK MAGAZYNU
Login: kierownikMG
Hasło: test123
