using System.Data;
using Proyecto_Inmobiliaria.Models;
using MySql.Data.MySqlClient;

public class RepositorioPago : RepositorioBase, IRepositorioPago
{
    public RepositorioPago(IConfiguration configuration) : base(configuration)
    {

    }
    public int Alta(Pago p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO pagos (fechaPago, monto, idContrato, estado, detalle, multa)
                           VALUES (@fechaPago, @monto, @idContrato, @estado, @detalle, @multa);
                           SELECT LAST_INSERT_ID();";
            connection.Open();
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@fechaPago", p.FechaPago);
                command.Parameters.AddWithValue("@monto", p.Monto);
                command.Parameters.AddWithValue("@idContrato", p.IdContrato);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@detalle", p.Detalle);
                command.Parameters.AddWithValue("@multa", p.Multa);
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
            string sql = @$"UPDATE pagos SET estado = 'Cancelado' WHERE {nameof(Pago.id)} = @id";
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
    public int Modificacion(Pago p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"UPDATE pagos SET fechaPago=@fechaPago,
                            monto = @monto,
                            idContrato = @idContrato,
                            estado = @estado,
                            detalle = @detalle, 
                            multa = @multa
                            WHERE {nameof(Pago.id)} = @id;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@fechaPago", p.FechaPago);
                command.Parameters.AddWithValue("@monto", p.Monto);
                command.Parameters.AddWithValue("@idContrato", p.IdContrato);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@detalle", p.Detalle);
                command.Parameters.AddWithValue("@multa", p.Multa);
                command.Parameters.AddWithValue("@id", p.id);
                connection.Open();
                res = command.ExecuteNonQuery();
                p.id = res;
                connection.Close();
            }
        }
        return res;
    }

    public IList<Pago> ObtenerTodos()
    {
        IList<Pago> res = new List<Pago>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT p.* FROM pagos p
                        INNER JOIN contratos c ON p.idContrato = c.id";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Pago p = new Pago
                    {
                        id = reader.GetInt32(nameof(Pago.id)),
                        IdContrato = reader.GetInt32("idContrato"),
                        FechaPago = reader.GetDateTime("fechaPago"),
                        Monto = reader.GetDecimal("monto"),
                        Estado = reader.GetString("estado"),
                        Detalle = reader.GetString("detalle"),
                        Multa = reader.GetBoolean("multa")
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }

    public IList<Pago> ObtenerTodosPaginado(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Pago> res = new List<Pago>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT p.* FROM pagos p
                        INNER JOIN contratos c ON p.idContrato = c.id
                        LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Pago p = new Pago
                    {
                        id = reader.GetInt32(nameof(Pago.id)),
                        IdContrato = reader.GetInt32("idContrato"),
                        FechaPago = reader.GetDateTime("fechaPago"),
                        Monto = reader.GetDecimal("monto"),
                        Estado = reader.GetString("estado"),
                        Detalle = reader.GetString("detalle"),
                        Multa = reader.GetBoolean("multa")
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }

    public Pago? ObtenerPorId(int id)
    {
        Pago? p = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT 
					id, fechaPago, monto, idContrato, estado, detalle, multa
					FROM pagos
					WHERE id=@id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id).Value = id;
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    p = new Pago
                    {
                        id = reader.GetInt32(nameof(Pago.id)),
                        FechaPago = reader.GetDateTime("fechaPago"),
                        Monto = reader.GetDecimal("monto"),
                        IdContrato = reader.GetInt32("idContrato"),
                        Estado = reader.GetString("estado"),
                        Detalle = reader.GetString("detalle"),
                        Multa = reader.GetBoolean("multa")
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
                    FROM pagos
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

    public IList<Pago>? PagosPorContrato(int idContrato)
    {
        IList<Pago> pagos = new List<Pago>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT *
                           FROM pagos
                           WHERE idContrato = @idContrato";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idContrato", idContrato);
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Pago pago = new Pago
                    {
                        id = reader.GetInt32(nameof(Pago.id)),
                        FechaPago = reader.GetDateTime("fechaPago"),
                        Monto = reader.GetDecimal("monto"),
                        IdContrato = reader.GetInt32("idContrato"),
                        Estado = reader.GetString("estado"),
                        Detalle = reader.GetString("detalle"),
                        Multa = reader.GetBoolean("multa")
                    };
                    pagos.Add(pago);
                }
                connection.Close();
            }
        }
        return pagos;
    }
    public IList<Pago>? PagosPendientes(int idContrato)
    {
        var pagos = new List<Pago>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT *
                           FROM pagos
                           WHERE idContrato = @idContrato AND estado = 'Pendiente'";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@idContrato", idContrato);
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Pago pago = new Pago
                    {
                        id = reader.GetInt32(nameof(Pago.id)),
                        FechaPago = reader.GetDateTime("fechaPago"),
                        Monto = reader.GetDecimal("monto"),
                        IdContrato = reader.GetInt32("idContrato"),
                        Estado = reader.GetString("estado"),
                        Detalle = reader.GetString("detalle"),
                        Multa = reader.GetBoolean("multa")
                    };
                    pagos.Add(pago);
                }
                connection.Close();
            }
        }
        return pagos;
    }

    public void Multar(int idContrato, decimal monto)
    {
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO pagos (fechaPago, monto, idContrato, detalle, multa)
                           VALUES (@fechaPago, @monto, @idContrato, 'Multa por terminar contrato anticipadamente', true);";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@fechaPago", DateTime.Now);
                command.Parameters.AddWithValue("@monto", monto);
                command.Parameters.AddWithValue("@idContrato", idContrato);
                command.CommandType = CommandType.Text;
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
} 