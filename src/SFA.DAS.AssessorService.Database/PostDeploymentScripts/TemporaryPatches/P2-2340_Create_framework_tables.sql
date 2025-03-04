-- there are 29 original data tables for frameworks
-- these table definitions are based on original MYSQL tables
-- except data fields have been created as varchar 
-- to prevent invalid dates causing data load failures
-- NOTE there were no FK constraints on original MYSQL tables

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'employer_sector' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[employer_sector] 
(
  [id] int NOT NULL,
  [sector_name] varchar(100) NOT NULL,
  PRIMARY KEY ([id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'evidance' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[evidance] 
(
  [id] int NOT NULL,
  [fid] int NOT NULL,
  [name] nvarchar(max) NOT NULL,
  [req_1] int,
  [req_2] int,
  [req_3] int,
  [req_4] int,
  [req_5] int,
  [req_6] int,
  [req_7] int,
  [req_8] int,
  [printed] int,
  [enabled] tinyint,
  [evidence_type] int,
  [evidence_reference] int,
  [qan_reference] varchar(50),
  PRIMARY KEY ([id]),
--INDEX [fid] ([fid])
);

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'evidance_app' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[evidance_app] 
(
  [id] int NOT NULL,
  [aid] int NOT NULL,
  [eid] int NOT NULL,
  [file_id] int NOT NULL,
  [checked] smallint,
  [notes] nvarchar(max),
  [admin_name] nvarchar(80),
  [notes_date] varchar(30),
  [updated_by] int,
  [body_notes] nvarchar(max),
  [body_name] varchar(50),
  [body_notes_date] varchar(30),
  PRIMARY KEY ([id]),
--INDEX [file_id] ([file_id]),
--INDEX [aid] ([aid])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'evidence_bands' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[evidence_bands]
(
  [id] int NOT NULL,
  [name] nvarchar(100) NOT NULL,
  [limit] smallint,
  [check_level] smallint,
  PRIMARY KEY ([id])
) 

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'evidence_global' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[evidence_global]
(
  [id] bigint NOT NULL,
  [slug] nvarchar(200) NOT NULL,
  [name] nvarchar(200) NOT NULL,
  [description] nvarchar(500) NOT NULL,
  [active] smallint NOT NULL,
  PRIMARY KEY ([id]),
--INDEX [slug] ([slug])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'evidence_global_app' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[evidence_global_app]
(
  [id] bigint NOT NULL,
  [eg_id] bigint NOT NULL,
  [aid] bigint NOT NULL,
  [fid] bigint,
  [declared] smallint,
  [declared_at] varchar(30),
  [declared_by] bigint,
  [declared_by_name] nvarchar(120),
  [checked] smallint,
  [checked_at] varchar(30),
  [checked_by] bigint,
  [checked_by_name] nvarchar(120),
  [required_upload] smallint,
  [reset_by_name] nvarchar(120),
  [reset_at] varchar(30),
  [reset_by] bigint,
  PRIMARY KEY ([id]),
--INDEX [aid] ([aid]),
--INDEX [fid] ([fid]),
--INDEX [eg_id] ([eg_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'leaving_reasons' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[leaving_reasons]
(
  [r_id] smallint NOT NULL,
  [r_text] nvarchar(200) NOT NULL,
  PRIMARY KEY ([r_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_advroutes' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_advroutes]
(
  [r_id] bigint NOT NULL,
  [r_apprId] bigint,
  [r_level] int,
  [r_code] nvarchar(15),
  [r_name] nvarchar(200),
  [r_bodyId] bigint,
  [r_retired] int,
  [r_opt] int, --  'How many OPTional UNITS needs to be chosen',
  [r_rmd] smallint, --  'Recommended Minimum Duration',
  PRIMARY KEY ([r_id]),
--INDEX [r_apprId] ([r_apprId]),
--INDEX [r_bodyId] ([r_bodyId])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice]
(
  [apprentice_id] bigint NOT NULL,
  [apprentice_forename] nvarchar(30) NOT NULL,
  [apprentice_updated_time] varchar(30) NULL,
  [apprentice_surname] nvarchar(50) NOT NULL,
  [apprentice_middlename] nvarchar(30),
  [apprentice_prefix] int,
  [apprentice_dob] varchar(30),
  [apprentice_centre] bigint,
  [apprentice_appr_id] bigint,
  [apprentice_notes] nvarchar(max),
  [apprentice_active] int,
  [apprentice_onhold] smallint,
  [apprentice_status_type] int,
  [apprentice_status_time] varchar(30),
  [apprentice_status_notes] nvarchar(max),
  [apprentice_status_visible] smallint,
  [apprentice_gender] int,
  [apprentice_disability] int,
  [apprentice_ethnicity] int,
  [apprentice_registered_date] varchar(30),
  [apprentice_agreement] smallint,
  [apprentice_status_modified_by] bigint,
  [apprentice_root_number] int,
  [apprentice_status_pci_visible] smallint,
  [apprentice_info_marital] int,
  [apprentice_ni_number] nvarchar(15),
  [apprentice_startdate] varchar(30),
  [apprentice_enddate] varchar(30),
  [apprentice_submission_date] varchar(30),
  [apprentice_jobtitle] nvarchar(50),
  [apprentice_reasonforleaving] nvarchar(max),
  [apprentice_reasonforleaving_id] smallint,
  [apprentice_funding] smallint,
  [apprentice_aln] nvarchar(400),
  [apprentice_employer_size] int,
  [apprentice_unique_nr] nvarchar(100),
  [apprentice_certificateName] nvarchar(200),
  [apprentice_asylumSeeker] smallint,
  [apprentice_mental] smallint,
  [apprentice_unemployedLength] nvarchar(10),
  [apprentice_esol] smallint,
  [apprentice_ethnicGroup] int,
  [apprentice_deleted] varchar(30),
  [apprentice_deleted_by] bigint,
  [apprentice_exemption] nvarchar(400),
  [apprentice_employer_contact] nvarchar(200),
  [apprentice_country] int,
  [apprentice_system_number] nvarchar(11),
  [apprentice_cert_number] nvarchar(15),
  [apprentice_cert_duplicate] int,
  [apprentice_cert_date] varchar(30),
  [apprentice_priorLearning] smallint,
  [apprentice_bsl] smallint,
  [apprentice_sen] smallint,
  [test_valid] smallint, --  '1 - OK, NULL - not checked, 2 - invalid',
  [apprentice_lastdayinlearning] varchar(30),
  [apprentice_countup] int,
  [apprentice_porder] nvarchar(100),
  [apprentice_pilot] smallint,
  [appr_temp] smallint,
  [appr_old_ni] nvarchar(15),
  [apprentice_awardBody_nr] nvarchar(100),
  [payment_ref] nvarchar(50),
  [resubmitted] smallint, --  'How many times has been resubmitted by Centre',
  [apprentice_cost_centre] nvarchar(50),
  [apprentice_body] int,
  [apprentice_archived] int,
  [apprentice_employer_sector] smallint,
  [apprentice_last_certificate] varchar(30),
  [apprentice_updated_by] bigint,
  [employer_same_as_centre] int,
  [apprentice_super_centre] bigint,
  [apprentice_manual] smallint NOT NULL,
  [apprentice_hold] smallint NOT NULL,
  [transferable_skills] bigint,
  PRIMARY KEY ([apprentice_id]),
  --INDEX [apprentice_registered_date] ([apprentice_registered_date]),
  --INDEX [apprentice_root_number] ([apprentice_root_number]),
  --INDEX [apprentice_id] ([apprentice_id]),
  --INDEX [apprentice_onhold] ([apprentice_onhold]),
  --INDEX [apprentice_dob] ([apprentice_dob]),
  --INDEX [apprentice_countup] ([apprentice_countup]),
  --INDEX [apprentice_sort] ([apprentice_countup],[apprentice_status_time]),
  --INDEX [apprentice_system_number] ([apprentice_system_number]),
  --INDEX [apprentice_cert_number] ([apprentice_cert_number]),
  --INDEX [apprentice_unique_nr] ([apprentice_unique_nr]),
  --INDEX [apprentice_ni_number] ([apprentice_ni_number]),
  --INDEX [apprentice_forename] ([apprentice_forename]),
  --INDEX [apprentice_surname] ([apprentice_surname]),
  --INDEX [apprentice_middlename] ([apprentice_middlename]),
  --INDEX [apprentice_appr_id] ([apprentice_appr_id],[apprentice_centre]),
  --INDEX [apprentice_body] ([apprentice_body]),
  --INDEX [apprentice_sort_new] ([apprentice_body],[apprentice_countup],[apprentice_status_time],[apprentice_active]),
  --INDEX [payment_ref] ([payment_ref]),
  --INDEX [apprentice_status_type] ([apprentice_status_type],[apprentice_status_time]),
  --INDEX [apprentice_super_centre] ([apprentice_super_centre],[apprentice_centre],[apprentice_active]),
  --INDEX [apprentice_sc] ([apprentice_super_centre]),
  --INDEX [apprentice_centre] ([apprentice_centre]),
  --INDEX [apprentice_cost_centre] ([apprentice_cost_centre]),
  --INDEX [apprentice_awardBody_nr] ([apprentice_awardBody_nr]),
  --INDEX [apprentice_porder] ([apprentice_porder]),
  --INDEX [apprentice_cert_date] ([apprentice_cert_date]),
  --INDEX [apprentice_status_active] ([apprentice_id],[apprentice_status_type],[apprentice_active])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice_achievement_dates' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice_achievement_dates]
(
  [id] bigint NOT NULL,
  [apprentice_id] bigint NOT NULL,
  [learning_event_type] varchar(400),
  [learning_event_id] bigint NOT NULL,
  [achievement_award_date] varchar(30),
  [grade] nvarchar(5),
  PRIMARY KEY ([id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice_fskills' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice_fskills]
(
  [id] bigint NOT NULL,
  [apprentice_id] bigint NOT NULL,
  [eng_fskill_id] bigint,
  [math_fskill_id] bigint,
  [ict_fskill_id] bigint,
  [updated_by] bigint,
  [err_fskill_value] int NOT NULL,
  [plts_fskill_value] int NOT NULL,
  PRIMARY KEY ([id]),
--INDEX [apprentice_id] ([apprentice_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice_quals' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice_quals]
(
  [id] bigint NOT NULL,
  [apprentice_id] bigint NOT NULL,
  [comp_qualification_id] bigint,
  [know_qualification_id] bigint,
  [comb_qualification_id] bigint,
  [updated_by] bigint,
  PRIMARY KEY ([id]),
--INDEX [apprentice_id] ([apprentice_id]),
--INDEX [comp_qualification_id] ([comp_qualification_id]),
--INDEX [know_qualification_id] ([know_qualification_id]),
--INDEX [comb_qualification_id] ([comb_qualification_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice_rejections' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice_rejections]
(
  [id] bigint NOT NULL,
  [aid] bigint NOT NULL, --  'Apprentice Id',
  [sc_id] bigint NOT NULL, --  'Apprentice SuperCentre',
  [notes] text NOT NULL,
  [reason_id] bigint NOT NULL, --  'Rejection Reason Id',
  [rejected_at] varchar(30) NOT NULL,
  [rejected_by] bigint NOT NULL,
  PRIMARY KEY ([id]),
--INDEX [aid] ([aid]),
--INDEX [rejected_by] ([rejected_by]),
--INDEX [sc_id] ([sc_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprentice_status' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprentice_status]
(
  [status_id] int NOT NULL,
  [status_name] nvarchar(45) NOT NULL,
  [status_order] bigint NOT NULL,
  PRIMARY KEY ([status_id]),
--INDEX [status_id] ([status_id]),
--INDEX [status_name] ([status_name],[status_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_apprenticeships' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_apprenticeships]
(
  [appr_id] bigint NOT NULL,
  [appr_name] nvarchar(200),
  [appr_notes] nvarchar(max),
  [appr_reference] nvarchar(50),
  [appr_sector] nvarchar(255) NOT NULL,
  [appr_active] smallint,
  [appr_active_from] varchar(30),
  [appr_active_to] varchar(30),
  [appr_ict] int,
  [appr_math] int,
  [appr_english] int,
  [add_ma_contact] nvarchar(50),
  [add_telephone] nvarchar(50),
  [add_email] nvarchar(100),
  [add_website] nvarchar(100),
  [appr_type] smallint,
  [appr_higher] smallint,
  [appr_err] int,
  [appr_plts] int,
  [appr_funding_id] nvarchar(10),
  PRIMARY KEY ([appr_id]),
--INDEX [appr_name] ([appr_name])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_centres' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_centres]
(
  [centre_id] bigint NOT NULL,
  [centre_name] nvarchar(100) NOT NULL,
  [centre_notes] nvarchar(max),
  [centre_body] bigint,
  [centre_usersNo] int,
  [centre_mandatory_gender] smallint,
  [centre_mandatory_disability] smallint,
  [centre_mandatory_ethnicity] smallint,
  [centre_active] smallint,
  [centre_status_pfr] smallint,
  [centre_status_pci] smallint,
  [centre_optional_aln] smallint,
  [centre_terms] nvarchar(400),
  [centre_optional_confirmation] smallint,
  [centre_submitted] bigint,
  [centre_created] varchar(30) NULL,
  [centre_credits] bigint NOT NULL,
  [centre_payments_active] smallint, --  '0 - Inactive, 1 - Active',
  [dougal_company_id] bigint,
  PRIMARY KEY ([centre_id]),
--INDEX [centre_active] ([centre_active]),
--INDEX [centre_name] ([centre_name]),
--INDEX [centre_body] ([centre_body])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_centres_app' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_centres_app]
(
  [centre_app_id] bigint NOT NULL,
  [centre_id] bigint NOT NULL,
  [app_id] bigint NOT NULL,
  PRIMARY KEY ([centre_app_id]),
--INDEX [centre_id] ([centre_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_certificates' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_certificates]
(
  [cert_id] bigint NOT NULL,
  [cert_description] nvarchar(400),
  [cert_details] nvarchar(400),
  [cert_file] nvarchar(200),
  [cert_code] nvarchar(max),
  PRIMARY KEY ([cert_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_ethnicity' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_ethnicity]
(
  [ethnic_id] int NOT NULL,
  [ethnic_name] nvarchar(60)
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_functional_skills' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_functional_skills]
(
  [fs_id] bigint NOT NULL,
  [fs_name] nvarchar(300) NOT NULL,
  [fs_type] smallint NOT NULL,
  [fs_ref_number] nvarchar(20) NOT NULL,
  PRIMARY KEY ([fs_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_qualification_award' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_qualification_award]
(
  [qa_id] bigint NOT NULL,
  [qa_awarding_body_id] bigint NOT NULL,
  [qa_number] nvarchar(50) NOT NULL,
  [qa_ref_number] nvarchar(50) NOT NULL,
  [qa_qualification_id] bigint NOT NULL,
  [qa_active] smallint NOT NULL,
  PRIMARY KEY ([qa_id]),
--INDEX [qa_awarding_body_id] ([qa_awarding_body_id]),
--INDEX [qa_qualification_id] ([qa_qualification_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_qualifications' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_qualifications]
(
  [qual_id] bigint NOT NULL,
  [qual_name] nvarchar(300) NOT NULL,
  [qual_type] int NOT NULL,
  [qual_pathway_id] bigint NOT NULL,
  [qual_active] smallint,
  PRIMARY KEY ([qual_id]),
--INDEX [qual_pathway_id] ([qual_pathway_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_rejection_reasons' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_rejection_reasons]
(
  [reason_id] smallint NOT NULL,
  [reason_name] nvarchar(200),
  [reason_active] smallint,
  [reason_order] smallint,
  PRIMARY KEY ([reason_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_bodies' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_bodies]
(
  [body_id] bigint NOT NULL,
  [body_name] nvarchar(100) NOT NULL,
  [body_code] nvarchar(50),
  [body_sector_notes] nvarchar(400),
  [body_notes] nvarchar(400),
  [body_usersNo] int,
  [body_active] smallint,
  [body_terms] nvarchar(400),
  [body_advRoute] smallint,
  [body_mandatoryEmail] smallint,
  [body_exemptionBox] smallint,
  [body_finance_monthly_email] nvarchar(50),
  [body_payments_active] smallint NOT NULL, --  '0 - Inactive, 1 - Active, 2 - Inactive (2nd time and more), 3 - Active (2nd time and more)',
  [body_sun_number] nvarchar(50),
  [body_cps_live_date] varchar(30),
  [body_logo] nvarchar(50),
  [dougal_company_id] bigint,
  PRIMARY KEY ([body_id]),
--INDEX [body_cps_live_date] ([body_cps_live_date])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_body_app' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_body_app]
(
  [body_app_id] bigint NOT NULL,
  [body_id] bigint,
  [app_id] bigint,
  [adv_route] smallint,
  PRIMARY KEY ([body_app_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_body_cert' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_body_cert]
(
  [cert_body_id] bigint NOT NULL,
  [cert_body_body_id] bigint,
  [cert_body_cert_id] bigint,
  PRIMARY KEY ([cert_body_id])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_location' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_location]
(
  [loc_id] bigint NOT NULL,
  [loc_name] nvarchar(100),
  [loc_street] nvarchar(4000),
  [loc_postcode] nvarchar(50),
  [loc_town] nvarchar(100),
  [loc_country] nvarchar(100),
  [loc_region] nvarchar(50),
  [loc_phone] nvarchar(50),
  [loc_fax] nvarchar(50),
  [loc_email] nvarchar(100),
  [loc_notes] nvarchar(400),
  [loc_type] int,
  [loc_relatedId] bigint,
  [loc_person] nvarchar(100),
  [loc_position] nvarchar(100),
  [loc_updated_by] bigint,
  [loc_also_trading_as] nvarchar(100),
  [active] bigint NOT NULL,
  [created_at] varchar(30) NULL,
  [updated_at] varchar(30) NULL,
  PRIMARY KEY ([loc_id]),
--INDEX [loc_relatedId] ([loc_relatedId]),
--INDEX [loc_type] ([loc_type]),
--INDEX [loc_type_and_name] ([loc_name],[loc_type]),
  --FULLTEXT INDEX [loc_name] ([loc_name])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'maig_numbernames' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[maig_numbernames] 
(
  [number_typeId] int NOT NULL,
  [number_typeName] nvarchar(300),
  [number_typeTransferName] nvarchar(400),
  PRIMARY KEY ([number_typeId]),
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'super_centre' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[super_centre] 
(
  [id] int NOT NULL,
  [name] nvarchar(300),
  [active] smallint,
  [notes] nvarchar(max),
  [submitted] varchar(30),
  [launched] smallint,
  [live_date] varchar(30),
  [evidence_band_lock] smallint,
  [evidence_band] smallint,
  [ukprn_code] varchar(8), -- 'UK Provider Reference Number',
  [dougal_company_id] bigint,
  PRIMARY KEY ([id]),
--INDEX [name] ([name])
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'map_apprenticeships' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[map_apprenticeships]
(
[appr_id] BIGINT PRIMARY KEY,
[FrameworkCode] INT NULL,
[FrameworkName] NVARCHAR(200)
);
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'map_advroutes' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[map_advroutes_backup]
(
[r_id] bigint primary key,
[TrainingCode] varchar(15),
[PathwayName] nvarchar(200)
);
GO


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'map_ULN' AND schema_id = SCHEMA_ID('frameworks'))
CREATE TABLE [frameworks].[map_ULN]
(
[ULN] bigint,
[ApprenticeshipId] bigint,
[TrainingType] int,
[TrainingCode] nvarchar(20)
);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('[frameworks].[map_ULN]') AND name = 'IX_map_ULN')
BEGIN
    CREATE INDEX IX_map_ULN
    ON [frameworks].[map_ULN] ([Uln], [TrainingCode])
    INCLUDE ([ApprenticeshipId], [TrainingType]);
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('[frameworks].[map_ULN]') AND name = 'IXU_map_ULN')
BEGIN
    CREATE UNIQUE INDEX IXU_map_ULN
    ON [frameworks].[map_ULN] ([Uln], [ApprenticeshipId]);
END
