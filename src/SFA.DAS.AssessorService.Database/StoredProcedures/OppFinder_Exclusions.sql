CREATE PROCEDURE [dbo].[OppFinder_Exclusions]
AS
	SELECT * FROM 
	(VALUES 
		('Able Seafarer (Deck)','ST0274'),
		('Aviation maintenance mechanic (military)','ST0014'),
		('Chartered Surveyor (degree)','ST0331'),
		('HM Forces Service person (Public Services)','ST0222'),
		('Installation Electrician/Maintenance Electrician','ST0152'),
		('Maritime Electrical / Mechanical Mechanic','ST0276'),
		('Maritime Operations Officer','ST0394'),
		('Military construction engineering technician','ST0414'),
		('Solicitor','ST0246'),
		('Surveying Technician','ST0332'),
		('Survival Equipment Fitter','ST0015')) AS Exclusions(StandardName, StandardReference);
