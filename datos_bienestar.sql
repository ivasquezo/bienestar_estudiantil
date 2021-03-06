/****** Script for SelectTopNRows command from SSMS  ******/
SELECT *
  FROM [bienestar2].[dbo].[INSCRIPCION]
  where [INSCODIGOI] in (
  SELECT [INSCODIGOI]
  FROM [bienestar2].[dbo].[MATRICULA] 
  where [PRDCODIGOI] in (9, 12) and [NVLCODIGOI] > 0)
  
  
  
INSERT INTO BE_BECA_TIPO
  (NOMBRE)
VALUES
  ('Para familiares de docentes o estudiantes y trabajadores activos de la Uisrael'),
  ('Por excelencia académica'),
  ('Apoyo socio-económico'),
  ('Por discapacidad')

INSERT INTO BE_BECA_TIPO_DOCUMENTO
  (CODIGOTIPO,NOMBRE,DESCRIPCION)
VALUES
  (1, 'Copia de la cédula a color del solicitante', 'Copia de la cédula a color del solicitante'),
  (1, 'Record académico emitido por secretaría académica de la Uisrael.', 'Record académico emitido por secretaría académica de la Uisrael.'),
  (2, 'Copia de la cédula a color del solicitante', 'Copia de la cédula a color del solicitante'),
  (3, 'Copia de la cédula a color del solicitante', 'Copia de la cédula a color del solicitante'),
  (3, 'Certificado laboral, que incluya la remuneración mensual del solicitante.', 'Certificado laboral, que incluya la remuneración mensual del solicitante.'),
  (4, 'Copia de la cédula a color del solicitante', 'Copia de la cédula a color del solicitante'),
  (4, 'Otros documentos que respalden y apoyen la solicitud.', 'Otros documentos que respalden y apoyen la solicitud.'),
  (4, 'Entrevista con el coordinador de bienestar estudiantil.', 'Entrevista con el coordinador de bienestar estudiantil.'),
  (4, 'Carné del CONADIS.', 'Carné del CONADIS.')
