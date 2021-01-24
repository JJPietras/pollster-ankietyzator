select * from PollForms
    

select * from PollStats
select * from QuestionStats

select * from Questions

select * from PollForms
select * from answers

-- delete from Questions where Poll not in (select PollId from PollForms)
-- delete from QuestionStats where QuestionID not in (select Questions.QuestionId from Questions)

select * from Questions where Poll = 91

select * from Answers
select * from Accounts where AccountId = 4


select * from QuestionStats where QuestionID > 214

update Questions set Options = '10/100/10' where QuestionId = 217
update Questions set Options = '10/500' where QuestionId = 219


select * from Accounts
update Accounts set UserType = 2 where AccountId = 4