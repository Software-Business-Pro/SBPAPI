-- =============================================
-- Author:		Dinic Damian
-- Create date: 06/07/2020
-- Description:	insertion du lien d'image en base
-- =============================================
CREATE PROCEDURE [dbo].[SP_SaveImageLinks]
		(
		@PRP_CODE 	nvarchar(20) = NULL,	-- PRP_MAT du vehicule
		@LINK nvarchar(500) = NULL,			-- Lien de l'image
		@DATE datetime = NULL				-- Date upload
		)
AS

INSERT INTO ImageLinks (CODE, LINK, [DATE])
VALUES (@PRP_CODE, @LINK, @DATE)
-- Fin de la procédure