CREATE PROCEDURE [dbo].[EPAO_Register_list_of_standards]
    
    AS


SET NOCOUNT ON;


select StandardCode, EndPointAssessorName,
    row_number() over(partition by StandardCode order by StandardCode) seq into #sequencedOrgStandardDetails
  from (select os.StandardCode, EndPointAssessorName
    from OrganisationStandard os
    inner join organisations o on os.EndPointAssessorOrganisationId = o.EndPointAssessorOrganisationId and o.[status] = 'Live'
    left outer join StandardCollation sc on os.StandardCode = sc.StandardId
    WHERE o.EndPointAssessorOrganisationId <> 'EPA0000'
    and (os.effectiveTo is null OR os.EffectiveTo > GETDATE())
    and (
        JSON_Value(StandardData,'$.EffectiveTo') is null OR
        JSON_Value(StandardData,'$.EffectiveTo') > GETDATE()
        )
    AND os.[Status] = 'Live'
    ) as orgStandards

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
	EP_AAO_30 nvarchar(500),
	EP_AAO_31 nvarchar(500),
	EP_AAO_32 nvarchar(500),
	EP_AAO_33 nvarchar(500),
	EP_AAO_34 nvarchar(500),
	EP_AAO_35 nvarchar(500),
	EP_AAO_36 nvarchar(500),
	EP_AAO_37 nvarchar(500),
	EP_AAO_38 nvarchar(500),
	EP_AAO_39 nvarchar(500),	
	EP_AAO_40 nvarchar(500),
	EP_AAO_41 nvarchar(500),
	EP_AAO_42 nvarchar(500),
	EP_AAO_43 nvarchar(500),
	EP_AAO_44 nvarchar(500),
	EP_AAO_45 nvarchar(500),
	EP_AAO_46 nvarchar(500),
	EP_AAO_47 nvarchar(500),
	EP_AAO_48 nvarchar(500),
	EP_AAO_49 nvarchar(500),	
	EP_AAO_50 nvarchar(500)		
)

-- OPERATION 3.2 Create containment table for details and populate table OrganisationStandardTableSummary
insert into #OrganisationStandardTableSummary (StandardCode) select distinct StandardCode from #sequencedOrgStandardDetails

DECLARE @SQLToUpdateStandardDetails varchar(max)

DECLARE @cnt INT = 1;

WHILE @cnt < 51
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
	EP_AAO_30,
	EP_AAO_31,
	EP_AAO_32,
	EP_AAO_33,
	EP_AAO_34,
	EP_AAO_35,
	EP_AAO_36,
	EP_AAO_37,
	EP_AAO_38,
	EP_AAO_39,	
	EP_AAO_40,
	EP_AAO_41,
	EP_AAO_42,
	EP_AAO_43,
	EP_AAO_44,
	EP_AAO_45,
	EP_AAO_46,
	EP_AAO_47,
	EP_AAO_48,
	EP_AAO_49,	
	EP_AAO_50
 from standardCollation sc
 left outer join #OrganisationStandardTableSummary sts on sts.StandardCode = sc.StandardId
WHERE ISJSON(StandardData) > 0
and ReferenceNumber is not null
and JSON_Value(standardData, '$.EffectiveFrom') is not null
and (
		JSON_Value(StandardData,'$.EffectiveTo') is null OR
		JSON_Value(StandardData,'$.EffectiveTo') > GETDATE()
		)
--and StandardCode in (select standardCode from organisationStandard)
order by Title

DROP TABLE #OrganisationStandardTableSummary

