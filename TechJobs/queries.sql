--Part 1

SELECT column_name, data_type 
FROM information_schema.columns
WHERE table_name = "Jobs";

--Part 2

SELECT * FROM employers 
WHERE Location = "St. Louis";

--Part 3

SELECT Name, Description FROM Skills 
WHERE Id IN (SELECT SkillID
FROM JobSkills)
ORDER BY Name ASC;