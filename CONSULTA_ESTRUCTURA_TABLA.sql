-- Consulta para ver la estructura de la tabla EntradasMercancia
-- Ejecuta esto en SQL Server Management Studio y comparte el resultado

SELECT 
    COLUMN_NAME AS NombreColumna,
    DATA_TYPE AS TipoDato,
    CHARACTER_MAXIMUM_LENGTH AS LongitudMaxima,
    IS_NULLABLE AS PermiteNull,
    COLUMN_DEFAULT AS ValorPorDefecto
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'inv' 
  AND TABLE_NAME = 'EntradasMercancia'
ORDER BY ORDINAL_POSITION;

-- También puedes ejecutar esto para ver las claves foráneas:
SELECT 
    fk.name AS NombreFK,
    tp.name AS TablaPadre,
    cp.name AS ColumnaPadre,
    tr.name AS TablaReferenciada,
    cr.name AS ColumnaReferenciada
FROM sys.foreign_keys AS fk
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables AS tp ON fkc.parent_object_id = tp.object_id
INNER JOIN sys.columns AS cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables AS tr ON fkc.referenced_object_id = tr.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE tp.name = 'EntradasMercancia' AND SCHEMA_NAME(tp.schema_id) = 'inv';

