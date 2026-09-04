Alter Table Participant
Add Constraint FK_Participant
Foreign key (RegistrationID) References Registration(Registration_ID);
GO

Alter Table Registration
Add Constraint FK_Registration
Foreign key (Participant_Id) References Participant(ParticipantID),
Foreign key (EventID) References RaceEvent(Event_ID);
GO

Alter Table Event_Category
Add Constraint FK_Category
Foreign key (Event_Id ) references RaceEvent(Event_ID);
GO

Alter Table Event_Organizer
Add Constraint FK_Organizer
Foreign key (Event_Id ) references RaceEvent(Event_ID);
GO

Alter Table Results
Add Constraint FK_Results
Foreign Key (Participant_ID) references Participant(ParticipantID),
Foreign Key (EventID ) references RaceEvent(Event_ID);
GO

Alter table RaceEvent
Add constraint FK_Event
Foreign Key (RegistrationID) references Registration(Registration_ID),
Foreign Key ( CategoryID) references Event_Category(Category_ID),
Foreign Key (EventOrginizerID) references  Event_Organizer(Event_Organizer_ID ),
Foreign Key (VenueID ) references Venue(Venue_ID);
GO
