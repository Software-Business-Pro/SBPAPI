-- =============================================
-- Author:		Dinic Damian
-- Create date: 06/07/2020
-- Description:	recuperation du planning par le prp code du vehicule
-- test : dupliqué depuis SP_o_rcr_s_01 (TP rubrique) 
-- =============================================
CREATE PROCEDURE [dbo].[SP_GetPlanning]
		(
		@PRP_CODE 	nvarchar(20) = NULL	-- PRP_MAT du vehicule
		)
AS

SELECT 
	COALESCE([CODE], 'N/A'),
	[DATE_DEBUT],
	COALESCE([H_DEB], 'N/A'),
	COALESCE([H_FIN], 'N/A'),
	COALESCE([H_DEB_AM], 'N/A'),
	COALESCE([H_FIN_AM], 'N/A'),
	COALESCE([H_DEB_PM], 'N/A'),
	COALESCE([H_FIN_PM], 'N/A'),
FROM 
	VehiculePlanning
WHERE
	CODE = @PRP_CODE
-- Fin de la procédure