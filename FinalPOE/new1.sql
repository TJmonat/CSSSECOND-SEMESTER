USE PROJECT;

GO

CREATE TABLE RaceResults

(

    RaceResult_ID NVARCHAR(13) PRIMARY KEY NOT NULL,

    Registration_ID NVARCHAR(13) NOT NULL,

    Finishing_Position INT NOT NULL,

    Finish_Time TIME NOT NULL,

    CONSTRAINT FK_RaceResults_Registration

    FOREIGN KEY (Registration_ID)

    REFERENCES Registration(Registration_ID),

    CONSTRAINT UQ_RaceResults_Registration

    UNIQUE (Registration_ID),

    CONSTRAINT CK_RaceResults_Position

    CHECK (Finishing_Position > 0)

);

GO