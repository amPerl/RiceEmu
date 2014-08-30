-- Adminer 4.0.3 MySQL dump

SET NAMES utf8;
SET foreign_key_checks = 0;
SET time_zone = '-04:00';
SET sql_mode = 'NO_AUTO_VALUE_ON_ZERO';

DROP DATABASE IF EXISTS `RiceDB`;
CREATE DATABASE `RiceDB` /*!40100 DEFAULT CHARACTER SET latin1 */;
USE `RiceDB`;

DROP TABLE IF EXISTS `Characters`;
CREATE TABLE `Characters` (
  `CID` bigint(20) NOT NULL AUTO_INCREMENT,
  `UID` bigint(20) NOT NULL,
  `Name` varchar(21) NOT NULL,
  `Mito` bigint(20) NOT NULL DEFAULT '1000',
  `Avatar` int(11) NOT NULL DEFAULT '1',
  `Level` int(11) NOT NULL DEFAULT '1',
  `City` int(11) NOT NULL DEFAULT '1',
  `CurrentCarID` int(11) NOT NULL DEFAULT '1',
  `GarageLevel` int(11) NOT NULL DEFAULT '1',
  `TID` bigint(20) NOT NULL DEFAULT '0',
  PRIMARY KEY (`CID`),
  KEY `UID` (`UID`),
  CONSTRAINT `Characters_ibfk_1` FOREIGN KEY (`UID`) REFERENCES `Users` (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

INSERT INTO `Characters` (`CID`, `UID`, `Name`, `Mito`, `Avatar`, `Level`, `City`, `CurrentCarID`, `GarageLevel`, `TID`) VALUES
(1, 1,  'Administrator',  1000, 1,  99, 1,  1,  1,  0),
(2, 1,  'Admin',  123456, 2,  15, 1,  2,  1,  0);

DROP TABLE IF EXISTS `Users`;
CREATE TABLE `Users` (
  `UID` bigint(20) NOT NULL AUTO_INCREMENT,
  `Username` varchar(21) NOT NULL,
  `PasswordHash` varchar(32) NOT NULL,
  `Status` tinyint(4) NOT NULL DEFAULT '1',
  `CreateIP` varchar(15) NOT NULL DEFAULT '127.0.0.1',
  `CreateDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`UID`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

INSERT INTO `Users` (`UID`, `Username`, `PasswordHash`, `Status`, `CreateIP`, `CreateDate`) VALUES
(1, 'admin',  '21232f297a57a5a743894a0e4a801fc3', 1,  '127.0.0.1',  '2014-05-01 11:55:42');

-- 2014-08-30 07:50:59