CREATE OR ALTER PROCEDURE pDistanceFormula (@lat1 float, @lng1 float, @lat2 float, @lng2 float)
AS
DECLARE @distance float
SET @distance = SQRT(POWER(@lat2 - @lat1, 2) + POWER(@lng2 - @lng1, 2)) * 62.1371192-- miles
PRINT 'The distance between these two point is ' + convert(varchar(15), @distance) + ' miles'