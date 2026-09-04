Insert Into Participant
Values
('P001','Tshegofatso','Modise','tshego@icloud.com'),
('P002','Lesedi','Nkuna','LesediNkuna@gmail.com');
GO

Insert Into Event_Category
Values
('CAT001','Cycling','Charity Cycling'),
('CAT002','Running','Comrades Marathon');
GO

Insert Into Event_Organizer
Values 
('ORG001','Grant','Francis','francis@fifa.co.za'),
('ORG002','Micheal','Kors','mich@comradesMarathon.co.za');
GO

Insert Into Venue
Values
('VEN001','DHL Stadium','CapeTown'),
('VEN002','Moses Mbida Stadium','Durban');
GO

Insert Into RaceEvent
Values 
('EVE001','CapeTown','2026-10-10','CAT001','ORG001','VEN001'),
('EVE002','Durban','2026-09-11','CAT002','ORG002','VEN002');
GO

Insert Into Registration
Values 
('REG001','Pending','2026-10-11','09:00:30','P002','EVE001'),
('REG002','Registered','2026-12-04','12:30:11','P001','EVE002');
GO