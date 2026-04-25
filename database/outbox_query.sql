create table OutboxMessage (
	outbox_id uniqueidentifier primary key,
	outbox_event_type nvarchar(200),
	outbox_payload nvarchar(max),
	outbox_created_at datetime2 not null default sysutcdatetime(),
	outbox_is_processed bit default 0,
	outbox_processed_at datetime2 null,
	outbox_status nvarchar(50) not null default 'Pending'
);


CREATE NONCLUSTERED INDEX IX_Outbox_Unprocessed_Filtered
ON OutboxMessage (outbox_created_at)
WHERE outbox_is_processed = 0;


CREATE NONCLUSTERED INDEX IX_Outbox_ProcessedAt
ON OutboxMessage (outbox_processed_at);