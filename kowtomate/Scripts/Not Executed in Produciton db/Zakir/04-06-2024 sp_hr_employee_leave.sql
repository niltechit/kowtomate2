CREATE PROCEDURE sp_employee_leave_insert
	@Id int out,
    @LeaveForContactId INT,
    @LeaveTypeId INT,
    @LeaveSubTypeId INT = NULL,
    @LeaveStartRequestDate DATETIME,
    @LeaveEndRequestDate DATETIME,
    @LeaveApprovedStartDate DATETIME = NULL,
    @LeaveApprovedEndDate DATETIME = NULL,
    @EmployeeNote NVARCHAR(MAX),
    @HRNote NVARCHAR(MAX),
    @LeaveStatus NVARCHAR(50),
    @LeaveRequestByContactId INT,
    @LeaveApprovalByContactId INT = NULL
AS
BEGIN
    INSERT INTO HR_EmployeeLeave (
        LeaveForContactId,
        LeaveTypeId,
        LeaveSubTypeId,
        LeaveStartRequestDate,
        LeaveEndRequestDate,
        LeaveApprovedStartDate,
        LeaveApprovedEndDate,
        EmployeeNote,
        HRNote,
        LeaveStatus,
        LeaveRequestByContactId,
        LeaveApprovalByContactId
    )
    VALUES (
        @LeaveForContactId,
        @LeaveTypeId,
        ISNULL(@LeaveSubTypeId, null),  -- Assuming 0 as a default value
        @LeaveStartRequestDate,
        @LeaveEndRequestDate,
        ISNULL(@LeaveApprovedStartDate, '1900-01-01'),  -- Assuming '1900-01-01' as a default value
        ISNULL(@LeaveApprovedEndDate, '1900-01-01'),    -- Assuming '1900-01-01' as a default value
        ISNULL(@EmployeeNote, ''),  -- Assuming empty string as a default value
        ISNULL(@HRNote, ''),        -- Assuming empty string as a default value
        @LeaveStatus,
        @LeaveRequestByContactId,
        ISNULL(@LeaveApprovalByContactId, null)  -- Assuming 0 as a default value
    );

    -- Return the newly inserted ID
    SELECT @Id = SCOPE_IDENTITY()
END;
