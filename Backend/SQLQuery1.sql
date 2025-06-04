ALTER TABLE dbo.Servicerequests
DROP CONSTRAINT CK_Status;

ALTER TABLE dbo.Servicerequests
ADD CONSTRAINT CK_Status CHECK (Status IN ('Pending', 'InProgress', 'Completed'));