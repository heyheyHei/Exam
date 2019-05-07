use exam
go
create table Category
(
	Id int identity primary key,
	Name varchar(22) not null,
	[State] int default(0),
	CreateTime datetime default(getdate()),
)
go
create table Question
(
	Id int identity primary key,
	Name text not null,
	[Desc] text null,
	Answer text not null,
	[State] int default(0),
	CategoryId int not null,
	CreateTime datetime default(getdate()),
)
go
create table Choise 
(
	Id int identity primary key,
	Name text not null,
	QuestionId int not null,
	IsCheck int default(1),
	[State] int default(0),
	CreateTime datetime default(getdate())
)
select * from Question;
select * from Category;
select * from Choise;
insert into Category(Name) values('���ʶ')
insert into Question(Name,[Desc],Answer,CategoryId)
values('�麣�����Ǹ�ʡ�ݣ�','�麣�����Ǹ�ʡ�ݣ�','�㶫ʡ',4)
insert into Choise(Name,QuestionId)
values('����ʡ',2)