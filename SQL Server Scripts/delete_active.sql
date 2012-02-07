delete from notifications where result_id in (select id from results where acknowledgment_time is null)
delete from result_contexts where result_id in (select id from results where acknowledgment_time is null)
delete from results where acknowledgment_time is null
delete from notifications where state='New' or state='New_Escalated'