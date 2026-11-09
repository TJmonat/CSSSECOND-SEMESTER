USE PROJECT;

GO

CREATE TABLE EventCategoryOptions

(

    Event_ID NVARCHAR(13) NOT NULL,

    Category_ID NVARCHAR(13) NOT NULL,

    CONSTRAINT PK_EventCategoryOptions

    PRIMARY KEY (Event_ID, Category_ID),

    CONSTRAINT FK_EventCategoryOptions_Event

    FOREIGN KEY (Event_ID)

    REFERENCES RaceEvent(Event_ID),

    CONSTRAINT FK_EventCategoryOptions_Category

    FOREIGN KEY (Category_ID)

    REFERENCES Event_Category(Category_ID)

);

GO

CREATE TABLE RegistrationCategories

(

    Registration_ID NVARCHAR(13) PRIMARY KEY NOT NULL,

    Category_ID NVARCHAR(13) NOT NULL,

    CONSTRAINT FK_RegistrationCategories_Registration

    FOREIGN KEY (Registration_ID)

    REFERENCES Registration(Registration_ID),

    CONSTRAINT FK_RegistrationCategories_Category

    FOREIGN KEY (Category_ID)

    REFERENCES Event_Category(Category_ID)

);

GO

SELECT * FROM Registration;

SELECT * FROM RegistrationCategories;