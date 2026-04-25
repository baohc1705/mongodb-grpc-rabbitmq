create table OutboxMessage(
	outbox_id UNIQUEIDENTIFIER PRIMARY KEY DEFAUlt newsequentialid(),
    outbox_event_type NVARCHAR(200),
    outbox_payload NVARCHAR(MAX),
    outbox_created_at DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    outbox_processed_at DATETIME2 NULL,
    outbox_status NVARCHAR(50) NOT NULL DEFAULT 'Pending'
);


CREATE INDEX IX_OutboxMessage_Status
ON OutboxMessage (outbox_status, outbox_created_at)
INCLUDE (outbox_event_type, outbox_payload)
WHERE outbox_status = 'Pending';