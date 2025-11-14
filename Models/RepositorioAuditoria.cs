using System.Data;
using Proyecto_Inmobiliaria.Models;
using MySql.Data.MySqlClient;

public class RepositorioAuditoria : RepositorioBase
{
    public RepositorioAuditoria(IConfiguration configuration) : base(configuration)
    {

    }

    public void NuevaAuditoria(string accion, string Usuario, string? detalle = null)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = "INSERT INTO auditorias (accion, usuario, detalle) VALUES (@Accion, @Usuario, @Detalle)";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@Accion", accion);
                command.Parameters.AddWithValue("@Usuario", Usuario);
                command.Parameters.AddWithValue("@Detalle", detalle);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    public IList<Auditoria> ObtenerAuditorias()
    {
            var auditorias = new List<Auditoria>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT * FROM auditorias ORDER BY fecha DESC";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            auditorias.Add(new Auditoria
                            {
                                id = reader.GetInt32("id"),
                                Accion = reader.GetString("Accion"),
                                Usuario = reader.GetString("Usuario"),
                                Fecha = reader.GetDateTime("Fecha"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString("Detalle")
                            });
                        }
                    }
                }
            }
            return auditorias;
    }
    
}