Create or Alter PROCEDURE pGetThreeClosestStores (@lat float, @long float)
AS
SELECT TOP(3) id, Street, City Reigion, Latitude, Longitude
 
 exec pDistanceFormula lat, long, Stores.Latitude, Stores.Longitude AS 'Distance' 

FROM Stores