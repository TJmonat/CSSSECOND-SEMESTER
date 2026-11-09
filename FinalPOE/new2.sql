USE PROJECT;

GO

CREATE TABLE EventDetails

(

    EventDetails_ID NVARCHAR(13) PRIMARY KEY NOT NULL,

    Event_ID NVARCHAR(13) NOT NULL,

    Event_Name VARCHAR(100) NOT NULL,

    Event_Description VARCHAR(500),

    Event_Distance VARCHAR(20),

    Event_Type VARCHAR(30),

    CONSTRAINT FK_EventDetails_RaceEvent

    FOREIGN KEY (Event_ID)

    REFERENCES RaceEvent(Event_ID),

    CONSTRAINT UQ_EventDetails_Event

    UNIQUE (Event_ID)

);

GO