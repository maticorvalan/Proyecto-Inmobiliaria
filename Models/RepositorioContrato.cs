using System.Data;
using Proyecto_Inmobiliaria.Models;
using MySql.Data.MySqlClient;

public class RepositorioContrato : RepositorioBase, IRepositorioContrato
{
    public RepositorioContrato(IConfiguration configuration) : base(configuration)
    {

    }
    public int Alta(Contrato p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO contratos (fechaInicio, fechaFin, monto, idInquilino, idInmueble, estado)
                           VALUES (@fechaInicio, @fechaFin, @monto, @idInquilino, @idInmueble, @estado);
                           SELECT LAST_INSERT_ID();";
            connection.Open();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@fechaInicio", p.FechaInicio);
                command.Parameters.AddWithValue("@fechaFin", p.FechaFin);
                command.Parameters.AddWithValue("@monto", p.Monto);
                command.Parameters.AddWithValue("@idInquilino", p.idInquilino);
                command.Parameters.AddWithValue("@idInmueble", p.idInmueble);
                command.Parameters.AddWithValue("@estado", p.Estado);
                ;
                res = Convert.ToInt32(command.ExecuteScalar());
                p.id = res;
                connection.Close();
            }
        }
        return res;
    }
    public int Baja(int id)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"DELETE FROM contratos WHERE {nameof(Contrato.id)} = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@id", id);
                connection.Open();
                res = command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return res;
    }
    public int Modificacion(Contrato p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"UPDATE contratos SET fechaInicio=@fechaInicio, fechaFin = @fechaFin, monto = @monto, idInquilino = @idInquilino,
                        idInmueble = @idInmueble, estado = @estado, fechaterminacionefectiva = @fechaterminacionefectiva WHERE {nameof(Contrato.id)} = @id;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@fechaInicio", p.FechaInicio);
                command.Parameters.AddWithValue("@fechaFin", p.FechaFin);
                command.Parameters.AddWithValue("@monto", p.Monto);
                command.Parameters.AddWithValue("@idInquilino", p.idInquilino);
                command.Parameters.AddWithValue("@idInmueble", p.idInmueble);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@fechaterminacionefectiva", p.FechaTerminacionEfectiva);
                command.Parameters.AddWithValue("@id", p.id);
                connection.Open();
                res = command.ExecuteNonQuery();
                p.id = res;
                connection.Close();
            }
        }
        return res;
    }

    public IList<Contrato> ObtenerTodos()
    {
        IList<Contrato> res = new List<Contrato>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT c.*, i.nombre, i.apellido, m.direccion FROM contratos c
                        INNER JOIN inquilinos i ON c.idInquilino = i.id
                        INNER JOIN inmuebles m ON c.idInmueble = m.id";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contrato p = new Contrato
                    {
                        id = reader.GetInt32(nameof(Contrato.id)),
                        FechaInicio = reader.GetDateTime("fechaInicio"),
                        FechaFin = reader.GetDateTime("fechaFin"),
                        Monto = reader.GetDecimal("monto"),
                        idInquilino = reader.GetInt32("idInquilino"),
                        idInmueble = reader.GetInt32("idInmueble"),
                        Estado = reader.GetBoolean("estado"),
                        FechaTerminacionEfectiva = reader.IsDBNull(reader.GetOrdinal("fechaterminacionefectiva"))
                            ? (DateTime?)null
                            : reader.GetDateTime("fechaterminacionefectiva"),
                        Inquilino = new Inquilino
                        {
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido")
                        },
                        Inmueble = new Inmueble
                        {
                            Direccion = reader.GetString("direccion")
                        }
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }

    public IList<Contrato> ObtenerTodosPaginado(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Contrato> res = new List<Contrato>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT c.*, i.nombre, i.apellido, m.direccion FROM contratos c
                        INNER JOIN inquilinos i ON c.idInquilino = i.id
                        INNER JOIN inmuebles m ON c.idInmueble = m.id
                        LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Contrato p = new Contrato
                    {
                        id = reader.GetInt32(nameof(Contrato.id)),
                        FechaInicio = reader.GetDateTime("fechaInicio"),
                        FechaFin = reader.GetDateTime("fechaFin"),
                        Monto = reader.GetDecimal("monto"),
                        idInquilino = reader.GetInt32("idInquilino"),
                        idInmueble = reader.GetInt32("idInmueble"),
                        Estado = reader.GetBoolean("estado"),
                        FechaTerminacionEfectiva = reader.IsDBNull(reader.GetOrdinal("fechaterminacionefectiva"))
                            ? (DateTime?)null
                            : reader.GetDateTime("fechaterminacionefectiva"),
                        Inquilino = new Inquilino
                        {
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido")
                        },
                        Inmueble = new Inmueble
                        {
                            Direccion = reader.GetString("direccion")
                        }
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }

    public Contrato? ObtenerPorId(int id)
    {
        Contrato? p = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT c.*, i.nombre, i.apellido, m.direccion FROM contratos c
                        INNER JOIN inquilinos i ON c.idInquilino = i.id
                        INNER JOIN inmuebles m ON c.idInmueble = m.id
                        WHERE c.id = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id).Value = id;
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    p = new Contrato
                    {
                        id = reader.GetInt32(nameof(Contrato.id)),
                        FechaInicio = reader.GetDateTime("fechaInicio"),
                        FechaFin = reader.GetDateTime("fechaFin"),
                        Monto = reader.GetDecimal("monto"),
                        idInquilino = reader.GetInt32("idInquilino"),
                        idInmueble = reader.GetInt32("idInmueble"),
                        Estado = reader.GetBoolean("estado"),
                        FechaTerminacionEfectiva = reader.IsDBNull(reader.GetOrdinal("fechaterminacionefectiva"))
                            ? (DateTime?)null
                            : reader.GetDateTime("fechaterminacionefectiva"),
                        Inquilino = new Inquilino
                        {
                            Nombre = reader.GetString("nombre"),
                            Apellido = reader.GetString("apellido")
                        },
                        Inmueble = new Inmueble
                        {
                            Direccion = reader.GetString("direccion")
                        }
                    };
                }
                connection.Close();
            }
        }
        return p;
    }
    public int ObtenerCantidad()
    {
        int res = 0;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"
                    SELECT COUNT(id)
                    FROM contratos
            ";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    res = reader.GetInt32(0);
                }
                connection.Close();
            }
        }
        return res;
    }

    public Contrato? ObtenerPorInmueble(int idInmueble)
    {
        Contrato? contrato = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT id, fechaInicio, fechaFin, monto, idInquilino, idInmueble, estado
                           FROM contratos
                           WHERE idInmueble = @idInmueble
                             AND fechaFin > @hoy
                           LIMIT 1";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                command.Parameters.AddWithValue("@hoy", DateTime.Today);
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    contrato = new Contrato
                    {
                        id = reader.GetInt32(nameof(Contrato.id)),
                        FechaInicio = reader.GetDateTime("fechaInicio"),
                        FechaFin = reader.GetDateTime("fechaFin"),
                        Monto = reader.GetDecimal("monto"),
                        idInquilino = reader.GetInt32("idInquilino"),
                        idInmueble = reader.GetInt32("idInmueble"),
                        Estado = reader.GetBoolean("estado")
                    };
                }
                connection.Close();
            }
        }
        return contrato;
    }
    public bool ControlSuperPosicion(int idInmueble, DateTime fechaInicio, DateTime fechaFin, int? id = null)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM contratos
                WHERE idInmueble = @idInmueble
                AND (@fechaInicio <= fechaFin AND @fechaFin >= fechaInicio)
                AND (@id IS NULL OR id <> @id);";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idInmueble", idInmueble);
                command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                command.Parameters.AddWithValue("@fechaFin", fechaFin);
                command.Parameters.AddWithValue("@id", (object?)id ?? DBNull.Value);

                connection.Open();
                var count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }
    }

}