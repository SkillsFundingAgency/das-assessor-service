
CREATE TABLE #DataDefinitions
(
	[Field Name] nvarchar(50),
	[Data type] nvarchar(20),
	Size int,
	[format] nvarchar(20)
) 

insert into #DataDefinitions select 'EPA_organisation_identifier', 'string',7,''
insert into #DataDefinitions select 'EPA_organisation', 'string',256,''	
insert into #DataDefinitions select 'Organisation_type', 'string',256,'lookup'
insert into #DataDefinitions select 'Website_link', 'url',256,''	
insert into #DataDefinitions select 'Contact_name', 'string',200,''	
insert into #DataDefinitions select 'Contact_address1', 'string',50,''	
insert into #DataDefinitions select 'Contact_address2', 'string',50,''	
insert into #DataDefinitions select 'Contact_address3', 'string',50,''	
insert into #DataDefinitions select 'Contact_address4', 'string',50,''
insert into #DataDefinitions select 'Contact_postcode', 'string',8,''	
insert into #DataDefinitions select 'Contact_phonenumber', 'string',18,''	
insert into #DataDefinitions select 'Contact_email', 'string',100,''	
insert into #DataDefinitions select 'Standard_code', 'integer',5,''	
insert into #DataDefinitions select 'Effective_from', 'date',8,'dd/mm/yy'
insert into #DataDefinitions select 'Effective_to', 'date',8,'dd/mm/yy'
insert into #DataDefinitions select 'Delivery_area', 'string',256,'lookup'
select * from #DataDefinitions

drop table #DataDefinitions