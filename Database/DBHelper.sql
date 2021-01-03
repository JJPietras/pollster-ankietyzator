select * from PollForms
    

select * from PollStats
select * from QuestionStats

select * from Questions

select * from PollForms

-- delete from Questions where Poll not in (select PollId from PollForms)
-- delete from QuestionStats where QuestionID not in (select Questions.QuestionId from Questions)

select Poll from Questions where QuestionId = 26

select * from Answers
select * from Accounts where AccountId = 4

delete from Answers where AccountId = 4