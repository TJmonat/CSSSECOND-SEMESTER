SELECT
R.Registration_ID,
P.Participant_ID,
P.Participant_Name,
P.Participant_Surname,
E.Event_ID,
E.Event_Location,
C.Event_Category_Type,
V.Venue_Name
FROM Registration R
INNER JOIN Participant P
ON R.ParticipantID = P.Participant_ID
INNER JOIN RaceEvent E
ON R.EventID = E.Event_ID
INNER JOIN Event_Category C
ON E.CategoryID = C.Category_ID
INNER JOIN Event_Organizer O
ON E.EventOrganizerID = O.Event_Organizer_ID
INNER JOIN Venue V
ON E.VenueID = V.Venue_ID;