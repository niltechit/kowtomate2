ALTER TABLE Management_TeamMember
ADD IsSupportingMember BIT DEFAULT 0;

ALTER TABLE Order_AssignedImageEditor
ADD IsSupport BIT DEFAULT 0;


ALTER TABLE Order_AssignedImageEditor
ADD EditorPrimaryTeamId int;


CREATE TABLE ManageTeamMemberChangelog (
    TeamId INT,
    MemberContactId INT,
    AssignByContactId INT,
    AssignTime DATETIME,
    CancelByContactId INT,
    CancelTime DATETIME,
    AssignNote NVARCHAR(500),
    CancelNote NVARCHAR(500),
    IsSupportingMember BIT,
    
    -- Foreign key relationship for MemberContactId
    FOREIGN KEY (MemberContactId) REFERENCES Security_Contact(ID),
    
    -- Foreign key relationship for AssignByContactId
    FOREIGN KEY (AssignByContactId) REFERENCES Security_Contact(ID),
    
    -- Foreign key relationship for CancelByContactId
    FOREIGN KEY (CancelByContactId) REFERENCES Security_Contact(ID)
);
