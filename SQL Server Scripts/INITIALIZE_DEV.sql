
DECLARE @BossyUser int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'drbossy@partners.org','Benjamin','Bossy','','MD','',-1 )
SET @BossyUser = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@BossyUser, @SenderRole)
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@BossyUser, @ClinRole)
INSERT INTO user_entries([user_id], [type], [entry_key], [value]) VALUES (@BossyUser, 'QueryStringSecurityExtension', 'Centricity', 'lprevedello')
INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@BossyUser, 'AuthExt', 'Default', 'bossy', 1)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@BossyUser, @ManualTransport, '')


--Modified: 2009-08-07, John Morgan - added password to test user
DECLARE @SidetrackedUser int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'drsidetracked@partners.org','Sam','Sidetracked','','MD','',-1 )
SET @SidetrackedUser = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@SidetrackedUser, @SenderRole)
INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@SidetrackedUser, 'AuthExt', 'Default', 'sidetracked', 1)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@SidetrackedUser, @ManualTransport, '')


--Modified: 2009-08-07, Jeremy Richardson - Added Dr. Busy
DECLARE @BusyUser int
INSERT INTO [users] 
([user_name],[first_name],[last_name],[middle_name],[credentials],[title],[is_homo_sapien]) 
VALUES ( 'drbusy@partners.org','Barbara','Busy','','MD','',-1 )
SET @BusyUser = SCOPE_IDENTITY()
INSERT INTO user_roles ([user_id], [role_id]) VALUES (@BusyUser, @ReceiverRole)
INSERT INTO user_entries([user_id], [type], [entry_key], [value], [restricted_access]) VALUES (@BusyUser, 'AuthExt', 'Default', 'busy', 1)
INSERT INTO user_transports([user_id], [transport_id], [address]) VALUES (@BusyUser, @ManualTransport, '')
