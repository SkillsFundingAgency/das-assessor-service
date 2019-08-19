-- PostCode Regions Lookup

CREATE TABLE PostCodeRegion (
    PostCodePrefix NVARCHAR(2) NOT NULL PRIMARY KEY,
    Region  NVARCHAR(50)  NOT NULL,
    DeliveryAreaId [int] NULL
)
GO

