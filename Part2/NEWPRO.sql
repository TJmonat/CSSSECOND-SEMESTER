USE PROJECT;
GO
CREATE TABLE UserAccount
(
UserAccount_ID INT IDENTITY(1,1) PRIMARY KEY,
Email NVARCHAR(50) NOT NULL UNIQUE,
PasswordHash NVARCHAR(500) NOT NULL,
UserRole NVARCHAR(20) NOT NULL,
Participant_ID NVARCHAR(13) NULL,
Event_Organizer_ID NVARCHAR(13) NULL,
CONSTRAINT CK_UserAccount_Role
CHECK (UserRole IN ('Participant', 'Organiser')),
CONSTRAINT CK_UserAccount_Link
CHECK (
(UserRole = 'Participant'
AND Participant_ID IS NOT NULL
AND Event_Organizer_ID IS NULL)
OR
(UserRole = 'Organiser'
AND Event_Organizer_ID IS NOT NULL
AND Participant_ID IS NULL)
),
CONSTRAINT FK_UserAccount_Participant
FOREIGN KEY (Participant_ID)
REFERENCES Participant(Participant_ID),
CONSTRAINT FK_UserAccount_Organizer
FOREIGN KEY (Event_Organizer_ID)
REFERENCES Event_Organizer(Event_Organizer_ID)
);

GO

select * from UserAccount
