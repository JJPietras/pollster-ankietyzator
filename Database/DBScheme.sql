/************************************
 *                                  *
 *    PROJEKT ANKIETYZACJA 2.0      *
 *     Ankietyzator + Reaktor       *
 *  Systemy Przetwarzania w Chmurze *
 *                                  *
 *  Definicja schematu bazy danych  *
 *                                  *
 *  Konta                           *
 *  Klucze Uprawnień                *
 *  Wzorce Ankiet                   *
 *  Pytania                         *
 *  Odpowiedzi                      *
 *  Statystyki Pytania              *
 *  Statystyki Ankiety              *
 *                                  *
 *  Autorzy:                        *
 *  Jarosław Drzymalski             *
 *  Jakub Pietras                   *
 *  Patryk Amsolik                  *
 *  Kamil Dembowski                 *
 *  Norbert Rudzki                  *
 *                                  *
 ************************************/

CREATE TABLE [dbo].[Accounts]
(
    [AccountId] [int] IDENTITY (1,1) NOT NULL,
    [Name]      [nvarchar](200)      NOT NULL,
    [EMail]     [nvarchar](200)      NOT NULL,
    [Tags]      [nvarchar](400)      NOT NULL,
    [UserType]  [int]                NOT NULL,

    CONSTRAINT [account_user_primary_key]
        PRIMARY KEY ([AccountId]),
    CONSTRAINT [account_user_type_range]
        CHECK ([UserType] < 3 AND [UserType] > -1)
) ON [PRIMARY]
GO


/* Klucze podnoszenia uprawnień konta
   umożliwiają zmianę typu użytkownika.
   
   Ciąg znaków zwany kluczem - definiuje klucz
   umożliwiający zmianę typu konta po wpisaniu.
   
   EMail - określa przypisany mail do kodu.
   Jeśli klucz ma przypisany mail, to może go
   użyć jedynie osoba o tym mailu. Jeśli mail
   jest łańcuchem pustym, to może go użyć każdy
   użytkownik.
   
   Typ użytkownika - zwykły(0), ankieter(1),
   admin(2)
*/
CREATE TABLE [dbo].[UpgradeKeys]
(
    [KeyId]    [int] IDENTITY (1,1) NOT NULL,
    [Key]      [nvarchar](200)      NOT NULL,
    [EMail]    [nvarchar](200)      NOT NULL,
    [UserType] [int]                NOT NULL,

    CONSTRAINT [pollster_keys_primary_key]
        PRIMARY KEY ([KeyId]),
    CONSTRAINT [pollster_keys_key_format]
        CHECK (LEN([Key]) > 3),
    CONSTRAINT [pollster_keys_email_format]
        CHECK ((LEN([EMail]) > 4 AND [EMail] LIKE '%@%.%') OR LEN([EMail]) = 0),
    CONSTRAINT [pollster_keys_user_non_standard]
        CHECK ([UserType] < 3 AND [UserType] > -1)
) ON [PRIMARY]
GO


/* Wzorzec ankiety - tworzy go Pollster. Posiada 
   unikalny identyfikator, klucz obcy stanowiący
   referencję do użytkownika-autora ankiety.
   
   Tagi - można ich określić wiele. Określają
   grupy docelowe np: PracownicyHaliZ38/Pł_A/Uł3
   
   Adresy EMail - poza tagami istnieje ręczna 
   możliwość przypisania ankiety poszczególnym 
   użytkownikom np: zdzislaw@op.pl/szymo13@wp.pl
   
   Tryb nieanonimowy - można jawnie ustawić 
   nieanonimowość ankiety - użytkownik widzi to.
*/
CREATE TABLE [dbo].[PollForms]
(
    [PollId]       [int] IDENTITY (1,1) NOT NULL,
    [AuthorId]     [int]                NOT NULL,
    [Tags]         [nvarchar](1000)     NOT NULL,
    [EMails]       [nvarchar](1000)     NOT NULL,
    [NonAnonymous] [bit]                NOT NULL,
    [Archived]     [bit]                NOT NULL,

    CONSTRAINT [poll_forms_primary_key]
        PRIMARY KEY ([PollId]),
    CONSTRAINT [poll_forms_author_id_foreign_key]
        FOREIGN KEY ([AuthorId]) REFERENCES [dbo].[Accounts] ([AccountId]) ON DELETE CASCADE 
) ON [PRIMARY]
GO


/* Pytania - każde pytanie ma unikalny
   identyfikator. Posiada klucz obcy, stanowiący
   referencję do ankiety, w której się znajduje.
   Pytanie posiada swój numer porządkowy
   w ankiecie. Posiada tytuł i tytuły opcji np:
   
   Tytuł: Jaki lubisz kolor
   Opcje: Zielony/Niebieski/Brązowy
   
   Istnieje możliwość określenia czy pytanie
   może zostać pozostawione bez jakiejkolwiek 
   odpowiedzi.
   
   Można także określić długość każdego pytania
   (stała dla wszystkich), np: 50 znaków.
   W przypadku maksymalnej długości dajemy 0.
   
   Typ - wyróżniamy 5 typów pytań:
   SingleChoice: kobieta/mężczyzna
   MultipleChoice: duży/płaski/czerwony/drogi
   Slider: b.słaby/słaby/średni/dobry/b.dobry
   Text: Co sądzisz o dziekanie/Wymień osiągnięcia
   Number: Ile masz lat/Ile lat pracujesz w IT
*/
CREATE TABLE [dbo].[Questions]
(
    [QuestionId] [int] IDENTITY (1,1) NOT NULL,
    [Poll]       [int]                NOT NULL,
    [Position]   [int]                NOT NULL,
    [Title]      [nvarchar](150)      NOT NULL,
    [Options]    [nvarchar](500)      NOT NULL,
    [AllowEmpty] [bit]                NOT NULL,
    [MaxLength]  [smallint]           NOT NULL,
    [Type]       [int]                NOT NULL,

    CONSTRAINT [questions_primary_key]
        PRIMARY KEY ([QuestionId]),
    CONSTRAINT [questions_poll_foreign_key]
        FOREIGN KEY ([Poll]) REFERENCES [dbo].[PollForms] ([PollId]) ON DELETE CASCADE,
    CONSTRAINT [questions_poll_max_length]
        CHECK ([MaxLength] > -1 AND [MaxLength] < 2001),
    CONSTRAINT [questions_type_non_standard]
        CHECK ([Type] > -1 AND [Type] < 5),
    CONSTRAINT [questions_position_positive]
        CHECK ([Position] > -1)
) ON [PRIMARY]
GO

select * from Accounts
select * from PollForms
/* Odpowiedź na dane pytanie pewnej ankiety 
   jest identyfikowane przez identyfikator
   konta użytkownika wypełniającego ankietę,
   a także przez identyfikator pytania,
   który to jest unikalny wśród wszystkich
   pytań wszystkich ankiet.
   
   Odpowiedź - przedzielone stringi np:
   dla singleChoice: 1 albo 0 albo 11
   dla multipleChoice: 1/3/2 albo 5/7
   dla slider: 7 albo 3 albo 0
   dla text: Nie lubię IO/Chcę spać
   dla number: 13 lub 53/112/4553/10
*/
CREATE TABLE [dbo].[Answers]
(
    [AccountId]  [int]            NOT NULL,
    [QuestionId] [int]            NOT NULL,
    [Content]    [nvarchar](2000) NOT NULL,

    CONSTRAINT [answers_primary_key]
        PRIMARY KEY CLUSTERED ([AccountId], [QuestionId])
) ON [PRIMARY]
GO


/* Statystyki danego pytania - App Funkcji
   po dodaniu ankiety aktualizuje statystyki
   każdego pytania tej ankiety

   Liczby odpowiedzi ilość odpowiedzi dla
   danych opcji danego pytania np:
   dla pytania single choice 
   (zielone, czerwone, żółte) różni
   użytkownicy mogli odpowiedzieć odpowiednio
   żółte(14), czerwone(23), żółte(0)
   
   Format:
   Odpowiedzi oddzielone '/' np. 14/23/0 
*/
CREATE TABLE [dbo].[QuestionStats]
(
    [QuestionID]   [int]          NOT NULL,
    [AnswerCounts] [varchar](500) NOT NULL,

    CONSTRAINT [question_stats_primary_key]
        PRIMARY KEY ([QuestionID]),
    CONSTRAINT [question_stats_foreign_key]
        FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[Questions] ([QuestionId]) ON DELETE CASCADE
)
GO


/* Statystyki ankiety
   Liczba wypełnionych ankiet - App Funkcji 
   zwiększa o 1 przy umieszczeniu w bazie
   nowo wypełnionej ankiety.
   
   Procentowy udział - App Funkcji uwzględnia
   wszystkie ankiety dla danego Ankietera.
   Jeśli ankieter utworzył 3 ankiety np:
   Ulubiony samochód, Na kogo głosujesz
   i Znienawidzona piosenka, na które 
   przypisani użytkownicy odpowiedzą w ilości
   20, 30, 0, to udział procentowy dla ankiety
   pierwszej (tej ankiety w bazie) dla tego 
   ankietera wynosi 40.0%. 
*/
CREATE TABLE [dbo].[PollStats]
(
    [PollId]      [int]   NOT NULL,
    [Completions] [int]   NOT NULL,
    [Percentage]  [float] NOT NULL,

    CONSTRAINT [poll_stats_primary_key]
        PRIMARY KEY ([PollId]),
    CONSTRAINT [poll_stats_foreign_key]
        FOREIGN KEY ([PollId]) REFERENCES [dbo].[PollForms] ([PollId]) ON DELETE CASCADE,
    CONSTRAINT [poll_stats_completions]
        CHECK ([Completions] > -1),
    CONSTRAINT [poll_stats_percentage]
        CHECK ([Percentage] >= 0.0 AND [Percentage] <= 100.0)
)
GO


/*select *
from cloud.dbo.Accounts*/

/*UPDATE cloud..Accounts
SET UserType = 2
WHERE EMail = 'jacubeus@gmail.com'*/

/*DELETE
FROM Accounts
WHERE EMail = 'jacubeus@gmail.com'*/

/*SELECT * 
FROM PollForms p
inner join Questions q ON p.PollId = q.Poll
inner join Answers A on q.QuestionId = A.QuestionId
*/

/*delete from PollForms where PollId = 29 
select * from PollForms
delete from Questions*/
delete from Accounts where EMail = 'jacubeus@gmail.com'
update Accounts set UserType = 2 where EMail = 'jacubeus@gmail.com'
select * from PollForms
select * from questions