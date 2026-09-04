USE Race;

Create Table Participant(
ParticipantID nvarchar(13) not null Primary Key,
Participant_Name varchar(25) not null,
Participant_Surname varchar(25),
Participant_Age int,
Participant_Email varchar(25) Unique,
RegistrationID nvarchar(13)
);
GO
Create Table Registration(
Registration_ID nvarchar(13) Primary key not null,
Registration_Status varchar(20) Default 'Pending',
Registration_Date date,
Registration_Time time,
Participant_Id nvarchar(13),
EventID nvarchar(13)
);
GO
Create Table Event_Category(
Category_ID nvarchar(13) Primary key not null,
Event_Category_Type varchar(25),
Event_Category_Name varchar(25),
Event_Id nvarchar(13)
);
GO
Create Table Event_Organizer(
Event_Organizer_ID nvarchar(13) Primary key not null,
Event_Organizer_Name varchar(25),
Event_Organizer_Email nvarchar(13),
Event_ID nvarchar(13)
);
GO
Create Table Venue(
Venue_ID nvarchar(13) Primary key not null,
Venue_Name varchar(30),
Venue_Location varchar(30)
);
GO
Create Table RaceEvent(
Event_ID nvarchar(13) Primary key not null,
Event_Location nvarchar(100),
Event_Date Date,
RegistrationID nvarchar(13),
CategoryID nvarchar(13),
EventOrginizerID nvarchar(13),
VenueID nvarchar(13)
);
GO
Create Table Results(
Results_ID nvarchar(13) Primary key not null,
Results_Type varchar(23),
Participant_ID nvarchar(13),
EventID nvarchar(13)
);
Go
