


	select StandardCode, EndPointAssessorName,
    row_number() over(partition by StandardCode order by StandardCode) seq into #sequencedOrgStandardDetails
  from (select os.StandardCode, EndPointAssessorName
	from OrganisationStandard os
	inner join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId) as orgStandards
 
 
 CREATE TABLE #OrganisationStandardTableSummary
(
	StandardCode int,
	EP_AAO_1 nvarchar(500),
	EP_AAO_2 nvarchar(500),
	EP_AAO_3 nvarchar(500),
	EP_AAO_4 nvarchar(500),
	EP_AAO_5 nvarchar(500),
	EP_AAO_6 nvarchar(500),
	EP_AAO_7 nvarchar(500),
	EP_AAO_8 nvarchar(500),
	EP_AAO_9 nvarchar(500),
	EP_AAO_10 nvarchar(500),
	EP_AAO_11 nvarchar(500),
	EP_AAO_12 nvarchar(500),
	EP_AAO_13 nvarchar(500),
	EP_AAO_14 nvarchar(500),
	EP_AAO_15 nvarchar(500),
	EP_AAO_16 nvarchar(500),
	EP_AAO_17 nvarchar(500),
	EP_AAO_18 nvarchar(500),
	EP_AAO_19 nvarchar(500),
	EP_AAO_20 nvarchar(500),
	EP_AAO_21 nvarchar(500),
	EP_AAO_22 nvarchar(500),
	EP_AAO_23 nvarchar(500),
	EP_AAO_24 nvarchar(500),
	EP_AAO_25 nvarchar(500),
	EP_AAO_26 nvarchar(500),
	EP_AAO_27 nvarchar(500),
	EP_AAO_28 nvarchar(500),
	EP_AAO_29 nvarchar(500),	
	EP_AAO_30 nvarchar(500)		
)

-- OPERATION 3.2 Create containment table for details and populate table OrganisationStandardTableSummary
insert into #OrganisationStandardTableSummary (StandardCode) select distinct StandardCode from #sequencedOrgStandardDetails

DECLARE @SQLToUpdateStandardDetails varchar(max)

DECLARE @cnt INT = 1;

WHILE @cnt < 31
BEGIN
  select @SQLToUpdateStandardDetails ='update osts  set EP_AAO_' + convert(varchar,@cnt) + ' = sas.EndPointAssessorName from #OrganisationStandardTableSummary osts left join #sequencedOrgStandardDetails sas on sas.StandardCode = osts.StandardCode and sas.seq =' + convert(varchar,@cnt);
	exec(@SQLToUpdateStandardDetails)
   SET @cnt = @cnt + 1;
END;

DROP TABLE #sequencedOrgStandardDetails

select '' as Trailblazer,
	isnull(JSON_VALUE(StandardData,'$.Category'),'') [Industry Sector],
	Title as Apprentice_standards,
	StandardId as LARS_Code,
	ReferenceNumber as IFA_Code,
	isnull(JSON_VALUE(StandardData,'$.Level'),'') [Level],
    EP_AAO_1,
	EP_AAO_2,
	EP_AAO_3,
	EP_AAO_4,
	EP_AAO_5,
	EP_AAO_6,
	EP_AAO_7,
	EP_AAO_8,
	EP_AAO_9,
	EP_AAO_10,
	EP_AAO_11,
	EP_AAO_12,
	EP_AAO_13,
	EP_AAO_14,
	EP_AAO_15,
	EP_AAO_16,
	EP_AAO_17,
	EP_AAO_18,
	EP_AAO_19,
	EP_AAO_20,
	EP_AAO_21,
	EP_AAO_22,
	EP_AAO_23,
	EP_AAO_24,
	EP_AAO_25,
	EP_AAO_26,
	EP_AAO_27,
	EP_AAO_28,
	EP_AAO_29,	
	EP_AAO_30
 from standardCollation sc
 left outer join #OrganisationStandardTableSummary sts on sts.StandardCode = sc.StandardId
WHERE ISJSON(StandardData) > 0
and ReferenceNumber is not null
and JSON_Value(standardData, '$.EffectiveFrom') is not null
--and StandardCode in (select standardCode from organisationStandard)
order by Title

DROP TABLE #OrganisationStandardTableSummary


--	Agriculture, environmental and animal care	Animal Care and Welfare Assistant	332	ST0397	2	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	NULL	256	332	ST0397	Animal Care and Welfare Assistant	{"Level":2,"Category":"Agriculture, environmental and animal care","IfaStatus":"Approved for delivery","EffectiveFrom":"2018-08-08T00:00:00","EffectiveTo":null,"LastDateForNewStarts":null,"IfaOnly":false}	2018-11-13 10:41:27.360	2018-11-13 14:25:49.487	NULL	1