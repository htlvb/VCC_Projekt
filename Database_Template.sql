-- --------------------------------------------------------
-- Host:                         students-db.htlvb.at
-- Server Version:               8.0.23 - MySQL Community Server - GPL
-- Server Betriebssystem:        Win64
-- HeidiSQL Version:             12.3.0.6589
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetroleclaims
CREATE TABLE IF NOT EXISTS `vcc_aspnetroleclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `RoleId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_vcc_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_vcc_AspNetRoleClaims_vcc_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `vcc_aspnetroles` (`Name`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetroleclaims: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetroles
CREATE TABLE IF NOT EXISTS `vcc_aspnetroles` (
  `Name` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NormalizedName` varchar(256) GENERATED ALWAYS AS (upper(`Name`)) STORED,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Beschreibung` varchar(256) DEFAULT NULL,
  `Id` varchar(256) GENERATED ALWAYS AS (`Name`) VIRTUAL,
  PRIMARY KEY (`Name`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetroles: ~1 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetuserclaims
CREATE TABLE IF NOT EXISTS `vcc_aspnetuserclaims` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `UserId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ClaimType` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ClaimValue` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`Id`),
  KEY `IX_vcc_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_vcc_AspNetUserClaims_vcc_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetuserclaims: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetuserlogins
CREATE TABLE IF NOT EXISTS `vcc_aspnetuserlogins` (
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderKey` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProviderDisplayName` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UserId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_vcc_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_vcc_AspNetUserLogins_vcc_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetuserlogins: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetuserroles
CREATE TABLE IF NOT EXISTS `vcc_aspnetuserroles` (
  `UserId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `RoleId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_vcc_AspNetUserRoles_RoleId` (`RoleId`),
  KEY `FK_vcc_UserId` (`UserId`),
  CONSTRAINT `FK_vcc_AspNetUserRoles_vcc_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `vcc_aspnetroles` (`Name`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_vcc_AspNetUserRoles_vcc_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetuserroles: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetusers
CREATE TABLE IF NOT EXISTS `vcc_aspnetusers` (
  `UserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Firstname` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Lastname` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `EmailConfirmed` tinyint(1) NOT NULL,
  `PasswordHash` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `SecurityStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ConcurrencyStamp` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `AccessFailedCount` int NOT NULL,
  `Id` varchar(256) GENERATED ALWAYS AS (`UserName`) STORED,
  PRIMARY KEY (`UserName`),
  UNIQUE KEY `EmailIndex` (`NormalizedEmail`) USING BTREE,
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetusers: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aspnetusertokens
CREATE TABLE IF NOT EXISTS `vcc_aspnetusertokens` (
  `UserId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `LoginProvider` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Value` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_vcc_AspNetUserTokens_vcc_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aspnetusertokens: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_aufgaben
CREATE TABLE IF NOT EXISTS `vcc_aufgaben` (
  `AufgabenID` int NOT NULL AUTO_INCREMENT,
  `Aufgabennr` int NOT NULL COMMENT 'Bestimmt die Reihenfolge im Level',
  `Input_TXT` mediumblob NOT NULL COMMENT 'Speichern die Aufgabe und das erwartete Ergebnis als Medium Blob',
  `Ergebnis_TXT` mediumblob NOT NULL COMMENT 'Speichern die Aufgabe und das erwartete Ergebnis als Medium Blob',
  `Level_LevelID` int NOT NULL,
  PRIMARY KEY (`AufgabenID`),
  KEY `Aufgaben_Level_FK` (`Level_LevelID`) USING BTREE,
  CONSTRAINT `Aufgaben_Level_FK` FOREIGN KEY (`Level_LevelID`) REFERENCES `vcc_level` (`LevelID`) ON DELETE CASCADE,
  CONSTRAINT `chk_aufgabennr_positive` CHECK ((0 < `Aufgabennr`))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_aufgaben: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_eingeladeneuseringruppe
CREATE TABLE IF NOT EXISTS `vcc_eingeladeneuseringruppe` (
  `Email` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gruppe_GruppenId` int NOT NULL,
  PRIMARY KEY (`Email`,`Gruppe_GruppenId`),
  KEY `FK_vcc_eingeladeneuseringruppe_vcc_gruppe` (`Gruppe_GruppenId`),
  CONSTRAINT `FK_vcc_eingeladeneuseringruppe_vcc_gruppe` FOREIGN KEY (`Gruppe_GruppenId`) REFERENCES `vcc_gruppe` (`GruppenID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_eingeladeneuseringruppe: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_event
CREATE TABLE IF NOT EXISTS `vcc_event` (
  `EventID` int NOT NULL AUTO_INCREMENT,
  `Bezeichnung` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'Eindeutiger Name des Events.',
  `Beginn` timestamp NOT NULL COMMENT 'Startzeitpunkt des Events.',
  `Dauer` int NOT NULL COMMENT 'Dauer in Minuten.',
  `StrafminutenProFehlversuch` int NOT NULL COMMENT 'Strafminuten bei Fehlern.',
  PRIMARY KEY (`EventID`),
  CONSTRAINT `chk_Dauer` CHECK ((`Dauer` > 0)),
  CONSTRAINT `chk_Strafminuten` CHECK ((`StrafminutenProFehlversuch` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_event: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_eventlog
CREATE TABLE IF NOT EXISTS `vcc_eventlog` (
  `EventLogID` int NOT NULL AUTO_INCREMENT,
  `Tabellenname` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Beschreibung` varchar(1000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'Details zum Log-Eintrag.',
  `Zeit` timestamp NOT NULL DEFAULT (now()) COMMENT 'Zeitpunkt des Log-Eintrags.',
  `LogKategorie_KatID` int NOT NULL,
  PRIMARY KEY (`EventLogID`),
  KEY `EventLog_LogKategorie_FK` (`LogKategorie_KatID`),
  CONSTRAINT `EventLog_LogKategorie_FK` FOREIGN KEY (`LogKategorie_KatID`) REFERENCES `vcc_logkategorie` (`KatID`) ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_eventlog: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_gruppe
CREATE TABLE IF NOT EXISTS `vcc_gruppe` (
  `GruppenID` int NOT NULL AUTO_INCREMENT,
  `Gruppenname` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Eindeutiger Name der Gruppe innerhalb eines Events.',
  `Event_EventID` int NOT NULL,
  `Teilnehmertyp` enum('Einzelspieler','Team') CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT 'Typ des Teilnehmers: Einzelspieler oder Team',
  `GruppenleiterId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL COMMENT 'Gruppenleiter',
  `Gesperrt` bit(1) NOT NULL DEFAULT (false),
  PRIMARY KEY (`GruppenID`),
  UNIQUE KEY `Gruppe_Gruppenname_EventID_UN` (`Gruppenname`,`Event_EventID`) USING BTREE,
  UNIQUE KEY `UN_Leader_For_Event` (`GruppenleiterId`,`Event_EventID`) USING BTREE,
  KEY `Gruppe_Event_FK` (`Event_EventID`) USING BTREE,
  CONSTRAINT `FK_vcc_gruppe_vcc_aspnetusers` FOREIGN KEY (`GruppenleiterId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON UPDATE CASCADE,
  CONSTRAINT `Gruppe_Event_FK` FOREIGN KEY (`Event_EventID`) REFERENCES `vcc_event` (`EventID`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_gruppe: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_gruppe_absolviert_level
CREATE TABLE IF NOT EXISTS `vcc_gruppe_absolviert_level` (
  `Gruppe_GruppeID` int NOT NULL,
  `Level_LevelID` int NOT NULL,
  `benoetigteZeit` time DEFAULT NULL,
  `Fehlversuche` int NOT NULL DEFAULT '0',
  PRIMARY KEY (`Gruppe_GruppeID`,`Level_LevelID`) USING BTREE,
  KEY `Gruppe_absolviert_Level_Level_FK` (`Level_LevelID`) USING BTREE,
  CONSTRAINT `Gruppe_absolviert_Level_Gruppe_FK` FOREIGN KEY (`Gruppe_GruppeID`) REFERENCES `vcc_gruppe` (`GruppenID`) ON DELETE CASCADE,
  CONSTRAINT `Gruppe_absolviert_Level_Level_FK` FOREIGN KEY (`Level_LevelID`) REFERENCES `vcc_level` (`LevelID`) ON DELETE CASCADE,
  CONSTRAINT `chk_benoetigteZeit` CHECK ((`benoetigteZeit` > _utf8mb4'00:00:00')),
  CONSTRAINT `chk_Fehlversuche` CHECK ((`Fehlversuche` >= 0))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_gruppe_absolviert_level: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_level
CREATE TABLE IF NOT EXISTS `vcc_level` (
  `LevelID` int NOT NULL AUTO_INCREMENT,
  `Levelnr` int NOT NULL,
  `Angabe_PDF` mediumblob NOT NULL,
  `Event_EventID` int NOT NULL,
  PRIMARY KEY (`LevelID`),
  UNIQUE KEY `Levelnr_Event_Comb` (`Event_EventID`,`Levelnr`),
  KEY `Level_Event_FK` (`Event_EventID`) USING BTREE,
  CONSTRAINT `Level_Event_FK` FOREIGN KEY (`Event_EventID`) REFERENCES `vcc_event` (`EventID`) ON DELETE CASCADE,
  CONSTRAINT `chk_Levelnr` CHECK (((`Levelnr` > 0) and (`Levelnr` <= 5)))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_level: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_logkategorie
CREATE TABLE IF NOT EXISTS `vcc_logkategorie` (
  `KatID` int NOT NULL AUTO_INCREMENT,
  `Beschreibung` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`KatID`),
  UNIQUE KEY `Beschr_UN` (`Beschreibung`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_logkategorie: ~4 rows (ungefähr)
INSERT INTO `vcc_logkategorie` (`KatID`, `Beschreibung`) VALUES
	(2, 'Datenänderung'),
	(1, 'Einfügen'),
	(4, 'Fehler'),
	(3, 'Löschen');

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.vcc_useringruppe
CREATE TABLE IF NOT EXISTS `vcc_useringruppe` (
  `User_UserId` varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Gruppe_GruppenId` int NOT NULL,
  PRIMARY KEY (`User_UserId`,`Gruppe_GruppenId`) USING BTREE,
  KEY `FK_GruppenId` (`Gruppe_GruppenId`) USING BTREE,
  KEY `FK_UserId` (`User_UserId`) USING BTREE,
  CONSTRAINT `FK_GruppenId` FOREIGN KEY (`User_UserId`) REFERENCES `vcc_aspnetusers` (`UserName`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `FK_UserId` FOREIGN KEY (`Gruppe_GruppenId`) REFERENCES `vcc_gruppe` (`GruppenID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.vcc_useringruppe: ~0 rows (ungefähr)

-- Exportiere Struktur von Tabelle 2425_5ahwii_maier.__efmigrationshistory
CREATE TABLE IF NOT EXISTS `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Exportiere Daten aus Tabelle 2425_5ahwii_maier.__efmigrationshistory: ~0 rows (ungefähr)

-- Exportiere Struktur von Prozedur 2425_5ahwii_maier.ShowRangliste
DELIMITER //
CREATE PROCEDURE `ShowRangliste`(
	IN `EventId` INT
)
    READS SQL DATA
BEGIN
    SELECT 
        RANK() OVER (ORDER BY AnzahlLevel DESC, gebrauchteZeit ASC) AS Rang,  
        GruppenID, 
        Gruppenname, 
        GruppenleiterId,
        Teilnehmertyp,
        AbgeschlosseneLevel, 
        AnzahlLevel,
        gesamtFehlversuche,  -- Neue Spalte: Gesamtzahl der Fehlversuche
        maxBenötigteZeit,     -- Neue Spalte: Maximale benötigte Zeit
        gebrauchteZeit        -- Gesamtzeit inkl. Strafzeit
    FROM (
        SELECT 
            gr.GruppenID, 
            gr.Gruppenname, 
            gr.GruppenleiterId,
            gr.Teilnehmertyp,
            GROUP_CONCAT(DISTINCT le.Levelnr ORDER BY le.Levelnr ASC SEPARATOR ', ') AS AbgeschlosseneLevel,  -- Alle Level als String
            COUNT(DISTINCT le.Levelnr) AS AnzahlLevel,  -- Anzahl der abgeschlossenen Level
            SUM(ab.Fehlversuche) AS gesamtFehlversuche,  -- Gesamtzahl der Fehlversuche
            MAX(ab.benoetigteZeit) AS maxBenötigteZeit,  -- Maximale benötigte Zeit
            ADDTIME(
                MAX(ab.benoetigteZeit),  -- Maximale benötigte Zeit
                SEC_TO_TIME(COALESCE(SUM(ab.Fehlversuche) * ev.StrafminutenProFehlversuch * 60, 0))  -- Strafzeit in Sekunden umgerechnet
            ) AS gebrauchteZeit  -- Gesamtzeit im TIME-Format
        FROM 
            vcc_gruppe gr
        LEFT JOIN 
            vcc_gruppe_absolviert_level ab ON gr.GruppenID = ab.Gruppe_GruppeID
        LEFT JOIN 
            vcc_level le ON le.LevelID = ab.Level_LevelID
        LEFT JOIN 
            vcc_event ev ON ev.EventID = le.Event_EventID
        WHERE 
            ab.benoetigteZeit IS NOT NULL 
            AND gr.Event_EventID = EventId
        GROUP BY 
            gr.GruppenID, gr.Gruppenname, gr.GruppenleiterId, gr.Teilnehmertyp
    ) t
    ORDER BY 
        AnzahlLevel DESC,  -- Zuerst nach Anzahl abgeschlossener Level sortieren
        gebrauchteZeit ASC;  -- Falls gleich viele Level, dann nach benötigter Zeit
END//
DELIMITER ;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccAspNetRoles
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccAspNetRoles` AFTER DELETE ON `vcc_aspnetroles` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetroles', 
        CONCAT(
            'Rolle ', old.`Name`, 
            ' wurde gelöscht. (Name: ', old.Name, ';RolleId: ', old.ID, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccAspNetUserRoles
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccAspNetUserRoles` AFTER DELETE ON `vcc_aspnetuserroles` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetuserroles', 
        CONCAT(
            'Person ', old.UserId, 
            ' wurde die Rolle ', old.roleid,' entzogen.'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccAspNetUsers
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccAspNetUsers` AFTER DELETE ON `vcc_aspnetusers` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetuser', 
        CONCAT(
            'Person ', OLD.UserName, 
            ' wurde gelöscht. (Name: ', OLD.Firstname, ' ', OLD.Lastname, 
            ', Email: ', OLD.Email, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccAufgaben
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccAufgaben` AFTER DELETE ON `vcc_aufgaben` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aufgaben', 
        CONCAT(
            'Aufgabe mit ID ', OLD.AufgabenID, 
            ' für LevelID ', OLD.Level_LevelID, 
            ' wurde gelöscht. (Alte Daten: Aufgabennr: ', OLD.Aufgabennr, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccEvent
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccEvent` AFTER DELETE ON `vcc_event` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_event', 
        CONCAT(
            'Event ', OLD.Bezeichnung, 
            ' mit ID ', OLD.EventID, 
            ' wurde gelöscht. (Beginn: ', OLD.Beginn, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccGruppe` AFTER DELETE ON `vcc_gruppe` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
    'vcc_gruppe', 
    CONCAT(
        IF(old.Teilnehmertyp = 'Team', 'Neue Gruppe ', 'Neue Einzelspieler-Gruppe'), 
        IF(old.Gruppenname IS NULL, '', old.Gruppenname), 
        ' für EventID ', old.Event_EventID, 
        ' wurde gelöscht. (GruppenID: ', old.GruppenID, 
        '; Gruppenleiter: ', old.GruppenleiterId, ')'
    ), 
    (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccGruppe_absolviert_level
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccGruppe_absolviert_level` AFTER DELETE ON `vcc_gruppe_absolviert_level` FOR EACH ROW BEGIN
    -- Protokolliere den Löschvorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_gruppe_absolviert_level', 
            CONCAT('Datensatz gelöscht: GruppeID = ', OLD.Gruppe_GruppeID, ', LevelID = ', OLD.Level_LevelID), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccLevel` AFTER DELETE ON `vcc_level` FOR EACH ROW BEGIN
    -- Protokolliere den Löschvorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_Level', 
            CONCAT('Datensatz gelöscht mit ID: ', OLD.LevelId, ' und LevelNr ',OLD.Levelnr), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterDelete_vccUserInGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterDelete_vccUserInGruppe` AFTER DELETE ON `vcc_useringruppe` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_useringruppe', 
        CONCAT(
            'Benutzer ', old.User_UserId, 
            ' wurde aus der Gruppe ' , OLD.Gruppe_GruppenId , ' gelöscht.'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccAspNetRoles
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccAspNetRoles` AFTER INSERT ON `vcc_aspnetroles` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetroles', 
        CONCAT(
            'Rolle ', NEW.`Name`, 
            ' wurde hinzugefügt. (Name: ', NEW.Name, ';RolleId: ', new.ID, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccAspNetUserRoles
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccAspNetUserRoles` AFTER INSERT ON `vcc_aspnetuserroles` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetuserroles', 
        CONCAT(
            'Person ', NEW.UserId, 
            ' hat die Rolle ', NEW.roleid,' bekommen.'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccAspNetUsers
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccAspNetUsers` AFTER INSERT ON `vcc_aspnetusers` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetuser', 
        CONCAT(
            'Person ', new.UserName, 
            ' wurde hinzugefügt. (Name: ', new.Firstname, ' ', new.Lastname, 
            ', Email: ', new.Email, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccAufgaben
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccAufgaben` AFTER INSERT ON `vcc_aufgaben` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aufgaben', 
        CONCAT(
            'Neue Aufgabe mit ID ', NEW.AufgabenID, 
            ' für LevelID ', NEW.Level_LevelID, 
            ' wurde eingefügt. (Aufgabennr: ', NEW.Aufgabennr, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccEvent
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccEvent` AFTER INSERT ON `vcc_event` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_event', 
        CONCAT(
            'Neues Event ', NEW.Bezeichnung, 
            ' wurde eingefügt. (EventID: ', NEW.EventID, 
            ', Beginn: ', NEW.Beginn,', Dauer: ', NEW.Dauer,', StrafminutenProFehlversuch: ', NEW.StrafminutenProFehlversuch, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccGruppe` AFTER INSERT ON `vcc_gruppe` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
    'vcc_gruppe', 
    CONCAT(
        IF(NEW.Teilnehmertyp = 'Team', 'Neue Gruppe ', 'Neue Einzelspieler-Gruppe'), 
        IF(NEW.Gruppenname IS NULL, '', NEW.Gruppenname), 
        ' für EventID ', NEW.Event_EventID, 
        ' wurde eingefügt. (GruppenID: ', NEW.GruppenID, 
        '; Gruppenleiter: ', NEW.GruppenleiterId,'; Gesperrt: ',IF(new.Gesperrt, 'true', 'false'), ')'
    ), 
    (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
);
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccGruppe_absolviert_level
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccGruppe_absolviert_level` AFTER INSERT ON `vcc_gruppe_absolviert_level` FOR EACH ROW BEGIN
    -- Protokolliere den Insert-Vorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_gruppe_absolviert_level', 
            CONCAT('Neuer Datensatz eingefügt: GruppeID = ', NEW.Gruppe_GruppeID, ', LevelID = ', NEW.Level_LevelID), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccLevel` AFTER INSERT ON `vcc_level` FOR EACH ROW BEGIN
    -- Protokolliere den Insert-Vorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_Level', 
            CONCAT('Neuer Level eingefügt mit ID: ', NEW.LevelId,'; LevelNr: ',NEW.Levelnr,'; EventId: ',NEW.Event_Eventid ), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterInsert_vccUserInGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterInsert_vccUserInGruppe` AFTER INSERT ON `vcc_useringruppe` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_useringruppe', 
        CONCAT(
            'Benutzer ', new.User_UserId,
            ' wurde in die Gruppe ' , new.Gruppe_GruppenId , ' hinzugefügt.'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Einfügen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccAspNetRoles
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccAspNetRoles` AFTER UPDATE ON `vcc_aspnetroles` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aspnetroles', 
        CONCAT(
            'Rolle ', old.`Name`, 
            ' wurde geändert. (Name: ', old.Name, ';RolleId: ', old.ID, ') -> neue Daten(Name: ', new.Name, ';RolleId: ', new.ID, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Löschen')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccAspNetUsers
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccAspNetUsers` AFTER UPDATE ON `vcc_aspnetusers` FOR EACH ROW BEGIN
   DECLARE userCount INT;
		SELECT COUNT(*) INTO userCount FROM vcc_gruppe pe WHERE pe.Gruppenname = upper(NEW.NormalizedUserName);
		if userCount > 0 then
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ein User kann nicht gleich wie ein Team heißen!';
		END if;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccAufgaben
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccAufgaben` AFTER UPDATE ON `vcc_aufgaben` FOR EACH ROW BEGIN
    DECLARE log_beschreibung TEXT;

    IF OLD.input_txt != NEW.input_txt THEN
        SET log_beschreibung = CONCAT(
            'Aufgabe mit ID ', OLD.AufgabenID,
            ' für LevelID ', OLD.Level_LevelID,
            ' wurde aktualisiert. (input_txt geändert)'
        );
    
    ELSEIF OLD.ergebnis_txt != NEW.ergebnis_txt THEN
        SET log_beschreibung = CONCAT(
            'Aufgabe mit ID ', OLD.AufgabenID,
            ' für LevelID ', OLD.Level_LevelID,
            ' wurde aktualisiert. (ergebnis_txt geändert)'
        );
    ELSE
        SET log_beschreibung = CONCAT(
            'Aufgabe mit ID ', OLD.AufgabenID, 
            ' für LevelID (Alt): ', OLD.Level_LevelID, ' -> LevelID (Neu): ', NEW.Level_LevelID,  
            ' wurde geändert. Alte Daten: (Aufgabennr: ', OLD.Aufgabennr, ') -> Neue Daten: (Aufgabennr: ', NEW.Aufgabennr, ')'
        );
    END IF;

    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_aufgaben',
        log_beschreibung,
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccEvent
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccEvent` AFTER UPDATE ON `vcc_event` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_event', 
        CONCAT(
            'Event ', OLD.Bezeichnung, 
            ' wurde geändert. Alte Daten: (Beginn: ', OLD.Beginn,'; Dauer: ',OLD.Dauer, ') -> Neue Daten: (Beginn: ', NEW.Beginn,'; Dauer: ',new.Dauer, ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccGruppe` AFTER UPDATE ON `vcc_gruppe` FOR EACH ROW BEGIN
    INSERT INTO `vcc_eventlog` (`Tabellenname`, `Beschreibung`, `LogKategorie_KatID`)
    VALUES (
        'vcc_gruppe', 
        CONCAT(
            'Gruppe ', IFNULL(OLD.GruppenID, 'Einzelperson'), 
            ' wurde geändert. Alte Daten: (Gruppenname: ', IFNULL(NEW.Gruppenname, 'Einzelperson'), '; Event: ',OLD.Event_eventid,'; Gesperrt: ',IF(old.Gesperrt, 'true', 'false'), ') -> Neue Daten: (Gruppenname: ', IFNULL(NEW.Gruppenname, 'Einzelperson'), '; Event: ',NEW.Event_eventid,'; Gesperrt: ',IF(new.Gesperrt, 'true', 'false'), ')'
        ), 
        (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung')
    );
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccGruppe_absolviert_level
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccGruppe_absolviert_level` AFTER UPDATE ON `vcc_gruppe_absolviert_level` FOR EACH ROW BEGIN
    -- Protokolliere den Update-Vorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_gruppe_absolviert_level', 
            CONCAT(
            'GruppeID ', OLD.Gruppe_GruppeID, ', LevelID ', OLD.Level_LevelID, 
            ' wurde geändert. Alte Daten: (Fehlversuche: ', OLD.Fehlversuche, ') -> Neue Daten: (Fehlversuche: ', NEW.Fehlversuche, ')'
        ), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccLevel` AFTER UPDATE ON `vcc_level` FOR EACH ROW BEGIN
    -- Protokolliere den Update-Vorgang in der Tabelle vcc_eventlog
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_Level', 
            CONCAT(
            'Level mit ID ', OLD.LevelId, 
            ' wurde geändert. Alte Daten: (Levelnr: ', OLD.Levelnr,'; EventID: ',OLD.event_eventid, ') -> Neue Daten: (Levelnr: ', NEW.Levelnr,'; EventID: ',new.event_eventid, ')'
        ),
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.afterUpdate_vccUserInGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `afterUpdate_vccUserInGruppe` AFTER UPDATE ON `vcc_useringruppe` FOR EACH ROW BEGIN
    
    INSERT INTO vcc_eventlog (Tabellenname, Beschreibung, LogKategorie_KatID)
    VALUES ('vcc_useringruppe', 
            CONCAT(
            'Daten in UserInGruppe ',
            ' wurde geändert. Alte Daten: (User_UserId: ', OLD.User_Userid,'; Gruppe_GruppenId: ',OLD.Gruppe_GruppenId , ') -> Neue Daten: (User_UserId: ', new.User_Userid,'; Gruppe_GruppenId: ',new.Gruppe_GruppenId , ')'
        ), 
            (SELECT KatID FROM vcc_logkategorie WHERE Beschreibung = 'Datenänderung'));
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccAspNetUsers
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccAspNetUsers` BEFORE INSERT ON `vcc_aspnetusers` FOR EACH ROW BEGIN
   DECLARE userCount INT;
	SELECT COUNT(*) INTO userCount FROM vcc_gruppe gr WHERE upper(gr.Gruppenname) = NEW.NormalizedUserName;
	if userCount > 0 then
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ein Username kann nicht gleich wie ein Team heißen!';
	END if;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccAufgaben
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccAufgaben` BEFORE INSERT ON `vcc_aufgaben` FOR EACH ROW BEGIN
	DECLARE max_reihenfolge INT;

   SELECT COALESCE(MAX(au.Aufgabennr), 0) INTO max_reihenfolge
   FROM vcc_aufgaben au
   WHERE au.Level_LevelID = NEW.Level_LevelID;

   SET NEW.Aufgabennr = max_reihenfolge + 1;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccEvent
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccEvent` BEFORE INSERT ON `vcc_event` FOR EACH ROW BEGIN
   IF NEW.Beginn < NOW() THEN
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Event muss in der Zukunft liegen!';
   END IF;
   

END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccGruppe` BEFORE INSERT ON `vcc_gruppe` FOR EACH ROW BEGIN
	DECLARE userCount INT;
	if NEW.Teilnehmertyp = 'Team' then
		SELECT COUNT(*) INTO userCount FROM vcc_aspnetusers pe WHERE pe.NormalizedUserName = upper(NEW.Gruppenname);
		if userCount > 0 then
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ein Teamname kann nicht gleich wie ein Benutzer heißen!';
		END if;
	ELSE
		set NEW.Gruppenname = NULL;
	END if;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccGruppeabsolviertLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccGruppeabsolviertLevel` BEFORE INSERT ON `vcc_gruppe_absolviert_level` FOR EACH ROW BEGIN
    -- Variablen deklarieren
    DECLARE beginn DATETIME;
    DECLARE dauer INT;
    DECLARE eventID VARCHAR(255);
    DECLARE errorMsg VARCHAR(1000);
    DECLARE previousLevelId INT;
    DECLARE previousLevelCompleted INT;

    -- Überprüfung: Gehören Gruppe und Level zum selben Event?
    IF (
        SELECT gr.Event_EventID 
        FROM vcc_gruppe gr 
        WHERE gr.GruppenID = NEW.Gruppe_GruppeID LIMIT 1
    ) != (
        SELECT le.Event_EventID 
        FROM vcc_level le 
        WHERE le.LevelID = NEW.Level_LevelID LIMIT 1
    ) THEN
        SET errorMsg = CONCAT(
            'Die Gruppe ', NEW.Gruppe_GruppeID,
            ' kann das Level ', NEW.Level_LevelID,
            ' nicht abschließen, da die Gruppe nicht am selben Event teilnimmt ',
            '(GruppenID: ', NEW.Gruppe_GruppeID, '; LevelID: ', NEW.Level_LevelID, ').'
        );
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;

    -- Event-Daten abrufen
    SELECT ev.Beginn, ev.Dauer, ev.EventID
    INTO beginn, dauer, eventID
    FROM vcc_level le
    LEFT JOIN vcc_event ev ON le.Event_EventID = ev.EventID
    WHERE le.LevelID = NEW.Level_LevelID;

    -- Überprüfung: Ist das Event abgelaufen?
    IF NOW() > beginn + INTERVAL dauer MINUTE THEN
        SET errorMsg = CONCAT('Das Event mit der ID ', eventID, ' ist abgelaufen.');
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;

    -- Überprüfung: Hat das Event begonnen?
    IF NOW() < beginn THEN
    	  SET errorMsg = CONCAT('Das Event mit der ID ', eventID, ' hat noch nicht begonnen!.');
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;
    
    -- Hole die Levelnr des aktuellen Levels
    SELECT le.Levelnr INTO @currentLevelNr
    FROM vcc_level le
    WHERE le.LevelID = NEW.Level_LevelID;

    -- Überprüfe, ob es ein vorheriges Level gibt (Levelnr > 1)
    IF @currentLevelNr > 1 THEN
        -- Hole die LevelID des vorherigen Levels
        SELECT le.LevelID INTO previousLevelId
        FROM vcc_level le
        WHERE le.Event_EventID = eventId
          AND le.Levelnr = (@currentLevelNr - 1);

        -- Überprüfe, ob das vorherige Level abgeschlossen wurde
        SELECT COUNT(*) INTO previousLevelCompleted
        FROM vcc_gruppe_absolviert_level gal
        WHERE gal.Gruppe_GruppeID = NEW.Gruppe_GruppeID
          AND gal.Level_LevelID = previousLevelId AND gal.benoetigteZeit IS NOT NULL;

        -- Wenn das vorherige Level nicht abgeschlossen wurde, Fehler auslösen
        IF previousLevelCompleted = 0 THEN
            SET @errorMsg = CONCAT(
                'Die Gruppe ', NEW.Gruppe_GruppeID,
                ' kann das Level ', NEW.Level_LevelID,
                ' nicht abschließen, da das vorherige Level (Levelnr: ', (@currentLevelNr - 1), ') nicht abgeschlossen wurde.'
            );
            SIGNAL SQLSTATE '45000' 
            SET MESSAGE_TEXT = @errorMsg;
        END IF;
    END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccLevel` BEFORE INSERT ON `vcc_level` FOR EACH ROW BEGIN
	DECLARE max_reihenfolge INT;

   SELECT COALESCE(MAX(le.Levelnr), 0) INTO max_reihenfolge
   FROM vcc_level le
   WHERE le.Event_EventID = NEW.Event_EventID;

   SET NEW.Levelnr = max_reihenfolge + 1;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeInsert_vccUserInGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeInsert_vccUserInGruppe` BEFORE INSERT ON `vcc_useringruppe` FOR EACH ROW BEGIN
	DECLARE event_count INT;

   -- Prüfe, ob der User bereits in einer anderen Gruppe ist, die am selben Event teilnimmt
   SELECT COUNT(*)
   INTO event_count
   FROM vcc_useringruppe UIG
   JOIN vcc_gruppe G ON UIG.Gruppe_GruppenId = G.GruppenID
   WHERE UIG.User_UserId = NEW.User_UserId
   AND G.Event_EventID = (SELECT Event_EventID FROM vcc_gruppe WHERE GruppenID = NEW.Gruppe_GruppenId)
   AND UIG.Gruppe_GruppenId != NEW.Gruppe_GruppenId;

   -- Falls bereits eine Gruppe für dasselbe Event existiert, verhindere den Insert
   IF event_count > 0 THEN
      SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'User kann nicht in mehreren Gruppen des gleichen Events sein!';
   END IF;




END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeUpdate_vccEvent
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeUpdate_vccEvent` BEFORE UPDATE ON `vcc_event` FOR EACH ROW BEGIN
   IF NEW.Beginn < NOW() THEN
      SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Event muss in der Zukunft liegen!';
   END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeUpdate_vccGruppe
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeUpdate_vccGruppe` BEFORE UPDATE ON `vcc_gruppe` FOR EACH ROW BEGIN
	DECLARE userCount INT;
	if NEW.Teilnehmertyp = 'Team' then
		SELECT COUNT(*) INTO userCount FROM vcc_aspnetusers pe WHERE pe.NormalizedUserName = upper(NEW.Gruppenname);
		if userCount > 0 then
		SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Ein Teamname kann nicht gleich wie ein Benutzer heißen!';
		END if;
	ELSE
		set NEW.Gruppenname = NULL;
	END if;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Exportiere Struktur von Trigger 2425_5ahwii_maier.beforeUpdate_vccGruppeabsolviertLevel
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='STRICT_TRANS_TABLES,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `beforeUpdate_vccGruppeabsolviertLevel` BEFORE UPDATE ON `vcc_gruppe_absolviert_level` FOR EACH ROW BEGIN
    -- Variablen deklarieren
    DECLARE beginn DATETIME;
    DECLARE dauer INT;
    DECLARE eventID VARCHAR(255);
    DECLARE errorMsg VARCHAR(1000);
    DECLARE previousLevelId INT;
    DECLARE previousLevelCompleted INT;

    -- Überprüfung: Gehören Gruppe und Level zum selben Event?
    IF (
        SELECT gr.Event_EventID 
        FROM vcc_gruppe gr 
        WHERE gr.GruppenID = NEW.Gruppe_GruppeID LIMIT 1
    ) != (
        SELECT le.Event_EventID 
        FROM vcc_level le 
        WHERE le.LevelID = NEW.Level_LevelID LIMIT 1
    ) THEN
        SET errorMsg = CONCAT(
            'Die Gruppe ', NEW.Gruppe_GruppeID,
            ' kann das Level ', NEW.Level_LevelID,
            ' nicht abschließen, da die Gruppe nicht am selben Event teilnimmt ',
            '(GruppenID: ', NEW.Gruppe_GruppeID, '; LevelID: ', NEW.Level_LevelID, ').'
        );
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;

    -- Event-Daten abrufen
    SELECT ev.Beginn, ev.Dauer, ev.EventID
    INTO beginn, dauer, eventID
    FROM vcc_level le
    LEFT JOIN vcc_event ev ON le.Event_EventID = ev.EventID
    WHERE le.LevelID = NEW.Level_LevelID;

    -- Überprüfung: Ist das Event abgelaufen?
    IF NOW() > beginn + INTERVAL dauer MINUTE THEN
        SET errorMsg = CONCAT('Das Event mit der ID ', eventID, ' ist abgelaufen.');
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;

    -- Überprüfung: Hat das Event begonnen?
    IF NOW() < beginn THEN
    	  SET errorMsg = CONCAT('Das Event mit der ID ', eventID, ' hat noch nicht begonnen!.');
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = errorMsg;
    END IF;
    
    -- Hole die Levelnr des aktuellen Levels
    SELECT le.Levelnr INTO @currentLevelNr
    FROM vcc_level le
    WHERE le.LevelID = NEW.Level_LevelID;

    -- Überprüfe, ob es ein vorheriges Level gibt (Levelnr > 1)
    IF @currentLevelNr > 1 THEN
        -- Hole die LevelID des vorherigen Levels
        SELECT le.LevelID INTO previousLevelId
        FROM vcc_level le
        WHERE le.Event_EventID = eventId
          AND le.Levelnr = (@currentLevelNr - 1);

        -- Überprüfe, ob das vorherige Level abgeschlossen wurde
        SELECT COUNT(*) INTO previousLevelCompleted
        FROM vcc_gruppe_absolviert_level gal
        WHERE gal.Gruppe_GruppeID = NEW.Gruppe_GruppeID
          AND gal.Level_LevelID = previousLevelId AND gal.benoetigteZeit IS NOT NULL;

        -- Wenn das vorherige Level nicht abgeschlossen wurde, Fehler auslösen
        IF previousLevelCompleted = 0 THEN
            SET @errorMsg = CONCAT(
                'Die Gruppe ', NEW.Gruppe_GruppeID,
                ' kann das Level ', NEW.Level_LevelID,
                ' nicht abschließen, da das vorherige Level (Levelnr: ', (@currentLevelNr - 1), ') nicht abgeschlossen wurde.'
            );
            SIGNAL SQLSTATE '45000' 
            SET MESSAGE_TEXT = @errorMsg;
        END IF;
    END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
