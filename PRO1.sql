USE PROJECT;
Create Table Participant(
Participant_ID nvarchar(13) Primary Key not null,
Participant_Name varchar(25),
Participant_Surname varchar(25),
Participant_Email varchar(50) Unique
);
GO
Create Table Event_Category(
Category_ID nvarchar(13) Primary Key not null,
Event_Category_Type varchar(25),
Event_Category_Name varchar (25)
);
GO

Create Table Event_Organizer(
Event_Organizer_ID nvarchar(13) Primary Key not null,
Event_Organizer_Name varchar(25),
Event_Organizer_Surname varchar(25),
Event_Organizers_Email varchar(50)
);
GO

Create table Venue(
Venue_ID nvarchar(13) Primary Key not null,
Venue_Name varchar(30),
Venue_Location varchar(30)
);
GO
Create table Results(
Results_ID nvarchar(13) Primary Key not null,
Result_Type varchar(25),
);
GO

create table RaceEvent(
Event_ID nvarchar(13) Primary Key not null,
Event_Location nvarchar(100),
Event_Date date,
CategoryID nvarchar(13),
EventOrganizerID nvarchar(13),
VenueID nvarchar(13),
Constraint FK_RaceEvent_Category
Foreign Key (CategoryID) references Event_Category(Category_ID),
Constraint FK_RaceEvent_Organizer
Foreign Key (EventOrganizerID) references Event_Organizer(Event_Organizer_ID),
Constraint FK_RaceEvent_Venue
Foreign Key (VenueID) references Venue(Venue_ID)
);
GO
Create Table Registration (
Registration_ID nvarchar(13) Primary KEY not null,
Registration_Status varchar(25) Default 'Pending',
Registration_Date date,
Registration_Time time,
ParticipantID nvarchar(13),
EventID nvarchar(13),
Constraint FK_Registartion_Participant
Foreign key (ParticipantID) references  Participant(Participant_ID),
Constraint FK_Registration_Event
Foreign Key (EventID) references RaceEvent(Event_ID)
);
go