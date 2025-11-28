-- Script para insertar los bloques tecnológicos en la BD
-- Primero eliminamos las áreas genéricas que no son bloques tecnológicos
DELETE FROM area WHERE nombre_area IN ('Tecnología', 'Recursos Humanos', 'Testing Area Updated');

-- Insertamos los bloques tecnológicos estándar
INSERT INTO area (nombre_area, descripcion) VALUES
('Backend', 'Desarrollo de servicios backend y APIs'),
('Frontend', 'Desarrollo de interfaces de usuario y aplicaciones web'),
('Mobile', 'Desarrollo de aplicaciones móviles iOS y Android'),
('QA', 'Control de calidad y pruebas de software'),
('DevOps', 'Infraestructura, CI/CD y operaciones'),
('Data', 'Análisis de datos, Big Data y Data Science')
ON CONFLICT (nombre_area) DO NOTHING;

-- Verificar la inserción
SELECT id_area, nombre_area, descripcion FROM area ORDER BY nombre_area;
