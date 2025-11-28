-- Crear tabla de configuración de notificaciones por email
CREATE TABLE IF NOT EXISTS configuracion_notificaciones_email (
    id_configuracion SERIAL PRIMARY KEY,
    id_usuario INTEGER NOT NULL REFERENCES usuario(id_usuario) ON DELETE CASCADE,
    email VARCHAR(255) NOT NULL,
    notificar_incumplimientos BOOLEAN DEFAULT TRUE,
    notificar_por_vencer BOOLEAN DEFAULT TRUE,
    enviar_resumen_diario BOOLEAN DEFAULT FALSE,
    hora_resumen_diario TIME DEFAULT '08:00:00',
    activo BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT NOW(),
    fecha_actualizacion TIMESTAMP NULL,
    UNIQUE(id_usuario, activo)
);

-- Crear índice para búsquedas rápidas
CREATE INDEX idx_config_email_usuario ON configuracion_notificaciones_email(id_usuario);
CREATE INDEX idx_config_email_activo ON configuracion_notificaciones_email(activo);
