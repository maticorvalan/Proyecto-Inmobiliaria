using System;
using System.Data;
using Proyecto_Inmobiliaria.Models;
using MySql.Data.MySqlClient;

public class RepositorioInquilino : RepositorioBase, IRepositorioInquilino
{
    public RepositorioInquilino(IConfiguration configuration) : base(configuration)
    {

    }

    public int Alta(Inquilino p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"INSERT INTO inquilinos (nombre, apellido, dni, telefono, email, estado)
                           VALUES (@nombre, @apellido, @dni, @telefono, @email, @estado);
                           SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.DNI);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@estado", p.Estado);
                connection.Open();
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
            string sql = @$"DELETE FROM inquilinos WHERE {nameof(Inquilino.id)} = @id";
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
    public int Modificacion(Inquilino p)
    {
        int res = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"UPDATE inquilinos SET nombre=@nombre, apellido = @apellido, dni = @dni, telefono = @telefono,
                        email = @email, estado = @estado WHERE {nameof(Inquilino.id)} = @id;";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@nombre", p.Nombre);
                command.Parameters.AddWithValue("@apellido", p.Apellido);
                command.Parameters.AddWithValue("@dni", p.DNI);
                command.Parameters.AddWithValue("@telefono", p.Telefono);
                command.Parameters.AddWithValue("@email", p.Email);
                command.Parameters.AddWithValue("@estado", p.Estado);
                command.Parameters.AddWithValue("@id", p.id);
                connection.Open();
                res = command.ExecuteNonQuery();
                p.id = res;
                connection.Close();
            }
        }
        return res;
    }
    public IList<Inquilino> ObtenerTodos()
    {
        IList<Inquilino> res = new List<Inquilino>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT id, nombre, apellido, DNI, telefono, email, estado FROM inquilinos";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Inquilino p = new Inquilino
                    {
                        id = reader.GetInt32(nameof(Inquilino.id)),
                        Nombre = reader.GetString("nombre"),
                        Apellido = reader.GetString("apellido"),
                        DNI = reader.GetInt32("DNI"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("email"),
                        Estado = reader.GetBoolean("estado")
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }
    public IList<Inquilino> ObtenerLista(int paginaNro = 1, int tamPagina = 10)
    {
        IList<Inquilino> res = new List<Inquilino>();
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT id, nombre, apellido, DNI, telefono, email, estado FROM inquilinos
            LIMIT {tamPagina} OFFSET {(paginaNro - 1) * tamPagina}";

            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Inquilino p = new Inquilino
                    {
                        id = reader.GetInt32(nameof(Inquilino.id)),
                        Nombre = reader.GetString("nombre"),
                        Apellido = reader.GetString("apellido"),
                        DNI = reader.GetInt32("DNI"),
                        Telefono = reader.GetString("telefono"),
                        Email = reader.GetString("email"),
                        Estado = reader.GetBoolean("estado")
                    };
                    res.Add(p);
                }
                connection.Close();
            }
        }
        return res;
    }
    public Inquilino? ObtenerPorEmail(string email)
    {
        Inquilino? p = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @$"SELECT 
					{nameof(Inquilino.id)}, Nombre, Apellido, Dni, Telefono, Email, Clave 
					FROM inquilinos
					WHERE Email=@email";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@email", email);
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    p = new Inquilino
                    {
                        id = reader.GetInt32(nameof(Inquilino.id)),//más seguro
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        DNI = reader.GetInt32("DNI"),
                        Telefono = reader.GetString("Telefono"),
                        Email = reader.GetString("Email"),
                        Estado = reader.GetBoolean("Estado"),
                    };
                }
                connection.Close();
            }
        }
        return p;
    }

    public Inquilino? ObtenerPorId(int id)
    {
        Inquilino? p = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            string sql = @"SELECT 
					id, nombre, apellido, DNI, telefono, email, estado
					FROM inquilinos
					WHERE id=@id";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", id).Value = id;
                command.CommandType = CommandType.Text;
                connection.Open();
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    p = new Inquilino
                    {
                        id = reader.GetInt32(nameof(Inquilino.id)),
                        Nombre = reader.GetString("Nombre"),
                        Apellido = reader.GetString("Apellido"),
                        DNI = reader.GetInt32("DNI"),
                        Telefono = reader.GetString("Telefono"),
                        Email = reader.GetString("Email"),
                        Estado = reader.GetBoolean("Estado"),
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
					FROM inquilinos
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
}