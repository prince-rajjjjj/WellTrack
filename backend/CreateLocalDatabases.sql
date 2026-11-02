-- Run once: sqlcmd -S .\SQLEXPRESS -i scripts\CreateLocalDatabases.sql

IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Identity')
    CREATE DATABASE WellTrack_Identity;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Wellness')
    CREATE DATABASE WellTrack_Wellness;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Participation')
    CREATE DATABASE WellTrack_Participation;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Engagement')
    CREATE DATABASE WellTrack_Engagement;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Compliance')
    CREATE DATABASE WellTrack_Compliance;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Reporting')
    CREATE DATABASE WellTrack_Reporting;
IF NOT EXISTS (SELECT 1 FROM sys.databases WHERE name = 'WellTrack_Notification')
    CREATE DATABASE WellTrack_Notification;

PRINT 'All 7 WellTrack databases are ready.';
