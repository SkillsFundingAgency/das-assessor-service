-- Add records to PostCodeRegion
BEGIN TRANSACTION
	DECLARE @rowCount int
	SELECT @rowCount = COUNT(*) FROM PostCodeRegion

	IF @rowCount = 0 
	BEGIN

		INSERT INTO PostCodeRegion(PostCodePrefix,Region)
		VALUES
		('GY','Channel Islands'),
		('JE','Channel Islands'),
		('DE','East Midlands'),
		('LE','East Midlands'),
		('LN','East Midlands'),
		('NG','East Midlands'),
		('NN','East Midlands'),
		('AL','East of England'),
		('CB','East of England'),
		('CM','East of England'),
		('CO','East of England'),
		('IP','East of England'),
		('LU','East of England'),
		('NR','East of England'),
		('PE','East of England'),
		('SG','East of England'),
		('SS','East of England'),
		('WD','East of England'),
		('BR','London'),
		('CR','London'),
		('E','London'),
		('EC','London'),
		('EN','London'),
		('HA','London'),
		('IG','London'),
		('N','London'),
		('NW','London'),
		('RM','London'),
		('SE','London'),
		('SM','London'),
		('SW','London'),
		('TW','London'),
		('UB','London'),
		('W','London'),
		('WC','London'),
		('DH','North East England'),
		('DL','North East England'),
		('NE','North East England'),
		('SR','North East England'),
		('TS','North East England'),
		('BB','North West England'),
		('BL','North West England'),
		('CA','North West England'),
		('CH','North West England'),
		('CW','North West England'),
		('FY','North West England'),
		('L','North West England'),
		('LA','North West England'),
		('M','North West England'),
		('OL','North West England'),
		('PR','North West England'),
		('SK','North West England'),
		('WA','North West England'),
		('WN','North West England'),
		('BT','Northern Ireland'),
		('AB','Scotland'),
		('DD','Scotland'),
		('DG','Scotland'),
		('EH','Scotland'),
		('FK','Scotland'),
		('G','Scotland'),
		('HS','Scotland'),
		('IV','Scotland'),
		('KA','Scotland'),
		('KW','Scotland'),
		('KY','Scotland'),
		('ML','Scotland'),
		('PA','Scotland'),
		('PH','Scotland'),
		('TD','Scotland'),
		('ZE','Scotland'),
		('BN','South East England'),
		('CT','South East England'),
		('DA','South East England'),
		('GU','South East England'),
		('HP','South East England'),
		('KT','South East England'),
		('ME','South East England'),
		('MK','South East England'),
		('OX','South East England'),
		('PO','South East England'),
		('RG','South East England'),
		('RH','South East England'),
		('SL','South East England'),
		('SO','South East England'),
		('TN','South East England'),
		('BA','South West England'),
		('BH','South West England'),
		('BS','South West England'),
		('DT','South West England'),
		('EX','South West England'),
		('GL','South West England'),
		('PL','South West England'),
		('SN','South West England'),
		('SP','South West England'),
		('TA','South West England'),
		('TQ','South West England'),
		('TR','South West England'),
		('CF','Wales'),
		('LD','Wales'),
		('LL','Wales'),
		('NP','Wales'),
		('SA','Wales'),
		('B','West Midlands'),
		('CV','West Midlands'),
		('DY','West Midlands'),
		('HR','West Midlands'),
		('ST','West Midlands'),
		('SY','West Midlands'),
		('TF','West Midlands'),
		('WR','West Midlands'),
		('WS','West Midlands'),
		('WV','West Midlands'),
		('BD','Yorkshire and the Humber'),
		('DN','Yorkshire and the Humber'),
		('HD','Yorkshire and the Humber'),
		('HG','Yorkshire and the Humber'),
		('HU','Yorkshire and the Humber'),
		('HX','Yorkshire and the Humber'),
		('LS','Yorkshire and the Humber'),
		('S','Yorkshire and the Humber'),
		('WF','Yorkshire and the Humber'),
		('YO','Yorkshire and the Humber'),
		('ZZ','distance or e-learning')
	END
	
	-- add DeliveryArea to PostCode Region 
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 0 WHERE [Region] = 'Channel Islands'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 0 WHERE [Region] = 'distance or e-learning'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 1 WHERE [Region] = 'East Midlands'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 2 WHERE [Region] = 'East of England'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 3 WHERE [Region] = 'London'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 4 WHERE [Region] = 'North East England'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 5 WHERE [Region] = 'North West England'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 0 WHERE [Region] = 'Northern Ireland'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 0 WHERE [Region] = 'Scotland'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 6 WHERE [Region] = 'South East England'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 7 WHERE [Region] = 'South West England'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 0 WHERE [Region] = 'Wales'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 8 WHERE [Region] = 'West Midlands'
	UPDATE  [PostCodeRegion] SET [DeliveryAreaId] = 9 WHERE [Region] = 'Yorkshire and the Humber'

COMMIT TRANSACTION