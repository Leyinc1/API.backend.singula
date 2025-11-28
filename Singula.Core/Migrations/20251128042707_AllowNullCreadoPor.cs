using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Singula.Core.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullCreadoPor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "area",
                columns: table => new
                {
                    id_area = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nombre_area = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("area_pkey", x => x.id_area);
                });

            migrationBuilder.CreateTable(
                name: "estado_alerta_catalogo",
                columns: table => new
                {
                    id_estado_alerta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_alerta_catalogo_pkey", x => x.id_estado_alerta);
                });

            migrationBuilder.CreateTable(
                name: "estado_solicitud_catalogo",
                columns: table => new
                {
                    id_estado_solicitud = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_solicitud_catalogo_pkey", x => x.id_estado_solicitud);
                });

            migrationBuilder.CreateTable(
                name: "estado_usuario_catalogo",
                columns: table => new
                {
                    id_estado_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("estado_usuario_catalogo_pkey", x => x.id_estado_usuario);
                });

            migrationBuilder.CreateTable(
                name: "permiso",
                columns: table => new
                {
                    id_permiso = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("permiso_pkey", x => x.id_permiso);
                });

            migrationBuilder.CreateTable(
                name: "prioridad_catalogo",
                columns: table => new
                {
                    id_prioridad = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    nivel = table.Column<int>(type: "integer", nullable: false),
                    sla_multiplier = table.Column<decimal>(type: "numeric", nullable: false),
                    icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    color = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("prioridad_catalogo_pkey", x => x.id_prioridad);
                });

            migrationBuilder.CreateTable(
                name: "rol_registro",
                columns: table => new
                {
                    id_rol_registro = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    bloque_tech = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    es_activo = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    nombre_rol = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rol_registro_pkey", x => x.id_rol_registro);
                });

            migrationBuilder.CreateTable(
                name: "roles_sistema",
                columns: table => new
                {
                    id_rol_sistema = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    es_activo = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    nombre = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("roles_sistema_pkey", x => x.id_rol_sistema);
                });

            migrationBuilder.CreateTable(
                name: "tipo_alerta_catalogo",
                columns: table => new
                {
                    id_tipo_alerta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tipo_alerta_catalogo_pkey", x => x.id_tipo_alerta);
                });

            migrationBuilder.CreateTable(
                name: "tipo_solicitud_catalogo",
                columns: table => new
                {
                    id_tipo_solicitud = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    activo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tipo_solicitud_catalogo_pkey", x => x.id_tipo_solicitud);
                });

            migrationBuilder.CreateTable(
                name: "rol_permiso",
                columns: table => new
                {
                    id_rol_sistema = table.Column<int>(type: "integer", nullable: false),
                    id_permiso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("rol_permiso_pkey", x => new { x.id_rol_sistema, x.id_permiso });
                    table.ForeignKey(
                        name: "rol_permiso_id_permiso_fkey",
                        column: x => x.id_permiso,
                        principalTable: "permiso",
                        principalColumn: "id_permiso",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "rol_permiso_id_rol_sistema_fkey",
                        column: x => x.id_rol_sistema,
                        principalTable: "roles_sistema",
                        principalColumn: "id_rol_sistema",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    correo = table.Column<string>(type: "character varying(190)", maxLength: 190, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    id_rol_sistema = table.Column<int>(type: "integer", nullable: false),
                    id_estado_usuario = table.Column<int>(type: "integer", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ultimo_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("usuario_pkey", x => x.id_usuario);
                    table.ForeignKey(
                        name: "usuario_id_estado_usuario_fkey",
                        column: x => x.id_estado_usuario,
                        principalTable: "estado_usuario_catalogo",
                        principalColumn: "id_estado_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "usuario_id_rol_sistema_fkey",
                        column: x => x.id_rol_sistema,
                        principalTable: "roles_sistema",
                        principalColumn: "id_rol_sistema",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "config_sla",
                columns: table => new
                {
                    id_sla = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    codigo_sla = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    descripcion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    dias_umbral = table.Column<int>(type: "integer", nullable: true),
                    es_activo = table.Column<bool>(type: "boolean", nullable: true, defaultValue: true),
                    id_tipo_solicitud = table.Column<int>(type: "integer", nullable: false),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    creado_por = table.Column<int>(type: "integer", nullable: true),
                    actualizado_por = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("config_sla_pkey", x => x.id_sla);
                    table.ForeignKey(
                        name: "config_sla_actualizado_por_fkey",
                        column: x => x.actualizado_por,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "config_sla_creado_por_fkey",
                        column: x => x.creado_por,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "config_sla_id_tipo_solicitud_fkey",
                        column: x => x.id_tipo_solicitud,
                        principalTable: "tipo_solicitud_catalogo",
                        principalColumn: "id_tipo_solicitud",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "personal",
                columns: table => new
                {
                    id_personal = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_usuario = table.Column<int>(type: "integer", nullable: false),
                    nombres = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    apellidos = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    documento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    estado = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("personal_pkey", x => x.id_personal);
                    table.ForeignKey(
                        name: "personal_id_usuario_fkey",
                        column: x => x.id_usuario,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reporte",
                columns: table => new
                {
                    id_reporte = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tipo_reporte = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    formato = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    filtros_json = table.Column<string>(type: "jsonb", nullable: true),
                    generado_por = table.Column<int>(type: "integer", nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    ruta_archivo = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reporte_pkey", x => x.id_reporte);
                    table.ForeignKey(
                        name: "reporte_generado_por_fkey",
                        column: x => x.generado_por,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "solicitud",
                columns: table => new
                {
                    id_solicitud = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_personal = table.Column<int>(type: "integer", nullable: false),
                    id_rol_registro = table.Column<int>(type: "integer", nullable: false),
                    id_sla = table.Column<int>(type: "integer", nullable: false),
                    id_area = table.Column<int>(type: "integer", nullable: false),
                    id_estado_solicitud = table.Column<int>(type: "integer", nullable: false),
                    fecha_solicitud = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    fecha_ingreso = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    num_dias_sla = table.Column<int>(type: "integer", nullable: true),
                    resumen_sla = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    origen_dato = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    prioridad = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    creado_por = table.Column<int>(type: "integer", nullable: true),
                    creado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualizado_por = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("solicitud_pkey", x => x.id_solicitud);
                    table.ForeignKey(
                        name: "solicitud_actualizado_por_fkey",
                        column: x => x.actualizado_por,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "solicitud_creado_por_fkey",
                        column: x => x.creado_por,
                        principalTable: "usuario",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "solicitud_id_area_fkey",
                        column: x => x.id_area,
                        principalTable: "area",
                        principalColumn: "id_area",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "solicitud_id_estado_solicitud_fkey",
                        column: x => x.id_estado_solicitud,
                        principalTable: "estado_solicitud_catalogo",
                        principalColumn: "id_estado_solicitud",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "solicitud_id_personal_fkey",
                        column: x => x.id_personal,
                        principalTable: "personal",
                        principalColumn: "id_personal",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "solicitud_id_rol_registro_fkey",
                        column: x => x.id_rol_registro,
                        principalTable: "rol_registro",
                        principalColumn: "id_rol_registro",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "solicitud_id_sla_fkey",
                        column: x => x.id_sla,
                        principalTable: "config_sla",
                        principalColumn: "id_sla",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "alerta",
                columns: table => new
                {
                    id_alerta = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_solicitud = table.Column<int>(type: "integer", nullable: false),
                    id_tipo_alerta = table.Column<int>(type: "integer", nullable: false),
                    id_estado_alerta = table.Column<int>(type: "integer", nullable: false),
                    nivel = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mensaje = table.Column<string>(type: "text", nullable: true),
                    enviado_email = table.Column<bool>(type: "boolean", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "now()"),
                    fecha_lectura = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    actualizado_en = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("alerta_pkey", x => x.id_alerta);
                    table.ForeignKey(
                        name: "alerta_id_estado_alerta_fkey",
                        column: x => x.id_estado_alerta,
                        principalTable: "estado_alerta_catalogo",
                        principalColumn: "id_estado_alerta",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "alerta_id_solicitud_fkey",
                        column: x => x.id_solicitud,
                        principalTable: "solicitud",
                        principalColumn: "id_solicitud",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "alerta_id_tipo_alerta_fkey",
                        column: x => x.id_tipo_alerta,
                        principalTable: "tipo_alerta_catalogo",
                        principalColumn: "id_tipo_alerta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reporte_detalle",
                columns: table => new
                {
                    id_reporte = table.Column<int>(type: "integer", nullable: false),
                    id_solicitud = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("reporte_detalle_pkey", x => new { x.id_reporte, x.id_solicitud });
                    table.ForeignKey(
                        name: "reporte_detalle_id_reporte_fkey",
                        column: x => x.id_reporte,
                        principalTable: "reporte",
                        principalColumn: "id_reporte",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "reporte_detalle_id_solicitud_fkey",
                        column: x => x.id_solicitud,
                        principalTable: "solicitud",
                        principalColumn: "id_solicitud",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alerta_estado",
                table: "alerta",
                column: "id_estado_alerta");

            migrationBuilder.CreateIndex(
                name: "ix_alerta_solicitud",
                table: "alerta",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "ix_alerta_tipo",
                table: "alerta",
                column: "id_tipo_alerta");

            migrationBuilder.CreateIndex(
                name: "area_nombre_area_key",
                table: "area",
                column: "nombre_area",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "config_sla_codigo_sla_key",
                table: "config_sla",
                column: "codigo_sla",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_config_sla_activo",
                table: "config_sla",
                column: "es_activo");

            migrationBuilder.CreateIndex(
                name: "IX_config_sla_actualizado_por",
                table: "config_sla",
                column: "actualizado_por");

            migrationBuilder.CreateIndex(
                name: "IX_config_sla_creado_por",
                table: "config_sla",
                column: "creado_por");

            migrationBuilder.CreateIndex(
                name: "ix_config_sla_tipo",
                table: "config_sla",
                column: "id_tipo_solicitud");

            migrationBuilder.CreateIndex(
                name: "estado_alerta_catalogo_codigo_key",
                table: "estado_alerta_catalogo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "estado_solicitud_catalogo_codigo_key",
                table: "estado_solicitud_catalogo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "estado_usuario_catalogo_codigo_key",
                table: "estado_usuario_catalogo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "permiso_codigo_key",
                table: "permiso",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_personal_documento",
                table: "personal",
                column: "documento");

            migrationBuilder.CreateIndex(
                name: "ix_personal_estado",
                table: "personal",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "personal_id_usuario_key",
                table: "personal",
                column: "id_usuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reporte_filtros_gin",
                table: "reporte",
                column: "filtros_json")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "ix_reporte_generado",
                table: "reporte",
                columns: new[] { "generado_por", "fecha_generacion" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_reporte_tipo_formato",
                table: "reporte",
                columns: new[] { "tipo_reporte", "formato" });

            migrationBuilder.CreateIndex(
                name: "ix_repdet_solicitud",
                table: "reporte_detalle",
                column: "id_solicitud");

            migrationBuilder.CreateIndex(
                name: "ix_rol_permiso_perm",
                table: "rol_permiso",
                column: "id_permiso");

            migrationBuilder.CreateIndex(
                name: "rol_registro_nombre_rol_key",
                table: "rol_registro",
                column: "nombre_rol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "roles_sistema_codigo_key",
                table: "roles_sistema",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_solicitud_actualizado_por",
                table: "solicitud",
                column: "actualizado_por");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_area",
                table: "solicitud",
                column: "id_area");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_creado_por",
                table: "solicitud",
                column: "creado_por");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_estado",
                table: "solicitud",
                column: "id_estado_solicitud");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_fechas",
                table: "solicitud",
                columns: new[] { "fecha_solicitud", "fecha_ingreso" });

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_personal",
                table: "solicitud",
                column: "id_personal");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_rol_registro",
                table: "solicitud",
                column: "id_rol_registro");

            migrationBuilder.CreateIndex(
                name: "ix_solicitud_sla",
                table: "solicitud",
                column: "id_sla");

            migrationBuilder.CreateIndex(
                name: "tipo_alerta_catalogo_codigo_key",
                table: "tipo_alerta_catalogo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "tipo_solicitud_catalogo_codigo_key",
                table: "tipo_solicitud_catalogo",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_estado",
                table: "usuario",
                column: "id_estado_usuario");

            migrationBuilder.CreateIndex(
                name: "ix_usuario_rol",
                table: "usuario",
                column: "id_rol_sistema");

            migrationBuilder.CreateIndex(
                name: "usuario_correo_key",
                table: "usuario",
                column: "correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "usuario_username_key",
                table: "usuario",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerta");

            migrationBuilder.DropTable(
                name: "prioridad_catalogo");

            migrationBuilder.DropTable(
                name: "reporte_detalle");

            migrationBuilder.DropTable(
                name: "rol_permiso");

            migrationBuilder.DropTable(
                name: "estado_alerta_catalogo");

            migrationBuilder.DropTable(
                name: "tipo_alerta_catalogo");

            migrationBuilder.DropTable(
                name: "reporte");

            migrationBuilder.DropTable(
                name: "solicitud");

            migrationBuilder.DropTable(
                name: "permiso");

            migrationBuilder.DropTable(
                name: "area");

            migrationBuilder.DropTable(
                name: "estado_solicitud_catalogo");

            migrationBuilder.DropTable(
                name: "personal");

            migrationBuilder.DropTable(
                name: "rol_registro");

            migrationBuilder.DropTable(
                name: "config_sla");

            migrationBuilder.DropTable(
                name: "usuario");

            migrationBuilder.DropTable(
                name: "tipo_solicitud_catalogo");

            migrationBuilder.DropTable(
                name: "estado_usuario_catalogo");

            migrationBuilder.DropTable(
                name: "roles_sistema");
        }
    }
}
