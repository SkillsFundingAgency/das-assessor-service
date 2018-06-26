CREATE PROCEDURE [dbo].[GetPreviousAddresses]
	-- Add the parameters for the stored procedure here
	  @UserName nvarchar(30)	
AS
BEGIN	
	declare @OrganisationId uniqueidentifier

	SET NOCOUNT ON;

	set @OrganisationId = (SELECT OrganisationId FROM Contacts 
			WHERE UserName = @UserName)
  
	SELECT DISTINCT TOP 10 
	c.OrganisationId
    ,JSON_VALUE([CertificateData],'$.ContactAddLine1') AS AddressLine1
   ,JSON_VALUE([CertificateData],'$.ContactAddLine2') AS AddressLine2
   ,JSON_VALUE([CertificateData],'$.ContactAddLine3') AS AddressLine3
   ,JSON_VALUE([CertificateData],'$.ContactAddLine4') AS City
      ,JSON_VALUE([CertificateData],'$.ContactPostCode') AS PostCode
	  ,MAX(c.CreatedAt) As CreatedAt
	FROM [dbo].[Certificates] c
	group by  c.OrganisationId, JSON_VALUE([CertificateData],'$.ContactAddLine1'),
	JSON_VALUE([CertificateData],'$.ContactAddLine2'),
	JSON_VALUE([CertificateData],'$.ContactAddLine3'),
	JSON_VALUE([CertificateData],'$.ContactAddLine4'),
	JSON_VALUE([CertificateData],'$.ContactPostCode'),
	c.Status
	having c.OrganisationId = @OrganisationId AND c.CreatedBy = 'Manual' AND (c.Status = 'Submitted' OR c.Status = 'Printed' OR c.Status = 'Reprint')
	order by MAX(c.CreatedAt) desc
END
