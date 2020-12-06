CREATE TABLE Users
(
    Id int not null Identity(1,1),
    FirstName varchar(255),
    LastName varchar(255)
)

INSERT INTO dbo.Users
VALUES 
    ('Krzysztof', 'Radwan'),
    ('Admin', 'Admin'),
    ('user first name','user last name')


SELECT LastName, FirstName from dbo.Users
WHERE Id = 1