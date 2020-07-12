-- =============================================
-- Author:		Dinic Damian
-- Create date: 06/07/2020
-- Description:	Recuperation des vehicule
-- =============================================
ALTER PROCEDURE [dbo].[SP_GetVehicules]
AS

DECLARE @TODAY DATETIME;
SET @TODAY = (CAST(GETDATE() AS DATE));

SELECT
	COALESCE(V.[MAT_REF], 'N/A'),
	COALESCE(V.[MAT_LIBELLE], 'N/A'),
	COALESCE(V.[CLI_REF], 'N/A'),
	COALESCE(V.[PRP_CODE], 'N/A'),
	COALESCE(V.[PRP_CAT], 'N/A'),
	COALESCE(V.[MAT_IMATRICULATION], 'N/A'),
	COALESCE(V.[MAT_CHAUFFEUR],	 'N/A'),
	COALESCE(V.[MAT_NUMSERIE], 'N/A'),
	COALESCE(convert(nvarchar(10),V.[MAT_PTAC]), 'N/A'),
	COALESCE(convert(nvarchar(10),V.[MAT_LONGEUR]), 'N/A'),
	COALESCE(convert(nvarchar(10),V.[MAT_LARGEUR]), 'N/A'),
	COALESCE(convert(nvarchar(10),V.[MAT_HAUTEUR]), 'N/A'),
	COALESCE(convert(nvarchar(10),V.[MAT_POIDS]), 'N/A'),
	COALESCE(V.[REMARQUE], 'N/A'),
	COALESCE(V.[MAT_CHAUFFEUR_PORTABLE], 'N/A'),
	ISNULL(VP.[ID], '1')
FROM 
	[Vehicule] V
	LEFT JOIN [VehiculePlanning] VP ON V.[MAT_REF] = VP.[CODE] AND @TODAY = VP.[DATE_DEBUT]
	
WHERE
	[CLOTURE] = 0
-- Fin de la procédure
