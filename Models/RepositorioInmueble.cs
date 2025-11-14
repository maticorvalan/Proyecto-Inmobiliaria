using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_Inmobiliaria.Models
{
	public class RepositorioInmueble : RepositorioBase, IRepositorioInmueble
	{
		public RepositorioInmueble(IConfiguration configuration) : base(configuration)
		{

		}

		public int Alta(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"INSERT INTO Inmuebles 
					(direccion, ambientes, uso, tipo, superficie, latitud, longitud, estado, precio, idPropietario)
					VALUES (@direccion, @ambientes, @uso, @tipo, @superficie, @latitud, @longitud, @estado, @precio, @idPropietario);
					SELECT LAST_INSERT_ID();";//devuelve el id insertado (LAST_INSERT_ID para mysql)
				using (var command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", entidad.Direccion == null ? DBNull.Value : entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@uso", entidad.Uso);
					command.Parameters.AddWithValue("@tipo", entidad.Tipo);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@estado", entidad.Estado);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					entidad.Id = res;
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
				string sql = @$"DELETE FROM Inmuebles WHERE id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
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
		public int Modificacion(Inmueble entidad)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					UPDATE Inmuebles SET
					direccion=@direccion, tipo=@tipo, uso=@uso, ambientes=@ambientes, superficie=@superficie, latitud=@latitud, longitud=@longitud, estado=@estado, precio=@precio, idPropietario=@idPropietario
					WHERE id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@direccion", entidad.Direccion);
					command.Parameters.AddWithValue("@ambientes", entidad.Ambientes);
					command.Parameters.AddWithValue("@tipo", entidad.Tipo);
					command.Parameters.AddWithValue("@uso", entidad.Uso);
					command.Parameters.AddWithValue("@superficie", entidad.Superficie);
					command.Parameters.AddWithValue("@latitud", entidad.Latitud);
					command.Parameters.AddWithValue("@longitud", entidad.Longitud);
					command.Parameters.AddWithValue("@estado", entidad.Estado);
					command.Parameters.AddWithValue("@precio", entidad.Precio);
					command.Parameters.AddWithValue("@idPropietario", entidad.IdPropietario);
					command.Parameters.AddWithValue("@id", entidad.Id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public int ModificarPortada(int id, string url)
		{
			int res = -1;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					UPDATE Inmuebles SET
					urlPortada=@portada
					WHERE Id = @id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@portada", String.IsNullOrEmpty(url) ? DBNull.Value : url);
					command.Parameters.AddWithValue("@id", id);
					command.CommandType = CommandType.Text;
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT i.id, i.direccion, i.uso, i.tipo, i.ambientes, i.superficie, i.latitud, i.longitud, i.estado, i.precio, i.idPropietario, 
                            p.nombre, p.apellido, p.DNI
                            FROM Inmuebles i INNER JOIN Propietarios p ON i.idPropietario = p.id";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
							Uso = reader.GetString(nameof(Inmueble.Uso)),
							Tipo = reader.GetString(nameof(Inmueble.Tipo)),
							Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
							Latitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Latitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Longitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Longitud)),
							Estado = reader.GetString(nameof(Inmueble.Estado)),
							Precio = reader.GetInt32(nameof(Inmueble.Precio)),
							IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								Dni = reader.GetInt32(nameof(Propietario.Dni)),

							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public int ObtenerCantidad()
		{
			int res = 0;
			using (MySqlConnection connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT COUNT(id)
					FROM inmuebles
				";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					using (MySqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							res = reader.GetInt32(0);
						}
					}
					connection.Close();
				}
			}
			return res;
		}

		public Inmueble ObtenerPorId(int id)
		{
			Inmueble entidad = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT i.Id, i.Direccion, i.Ambientes, i.Uso, i.Tipo, i.Superficie, 
						i.Latitud, i.Longitud, i.Estado, i.Precio, i.UrlPortada, i.IdPropietario, 
						p.Nombre, p.Apellido
					FROM Inmuebles i 
					JOIN Propietarios p ON i.IdPropietario = p.Id
					WHERE i.{nameof(Inmueble.Id)} = @id";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@id", id);
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Id))),
							Direccion = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Direccion))) ? "" : reader.GetString(reader.GetOrdinal(nameof(Inmueble.Direccion))),
							Ambientes = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Ambientes))),
							Tipo = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Tipo))),
							Uso = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Uso))),
							Superficie = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Superficie))),
							Latitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Latitud))) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Latitud))),
							Longitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Longitud))) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal(nameof(Inmueble.Longitud))),
							Estado = reader.GetString(reader.GetOrdinal(nameof(Inmueble.Estado))),
							Precio = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.Precio))),
							UrlPortada = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.UrlPortada))) ? null : reader.GetString(reader.GetOrdinal(nameof(Inmueble.UrlPortada))),
							IdPropietario = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(reader.GetOrdinal(nameof(Inmueble.IdPropietario))),
								Nombre = reader.GetString(reader.GetOrdinal(nameof(Propietario.Nombre))),
								Apellido = reader.GetString(reader.GetOrdinal(nameof(Propietario.Apellido)))
							}
						};
					}
					connection.Close();
				}
			}
			return entidad;
		}


		public IList<Inmueble> BuscarPorPropietario(int idPropietario)
		{
			List<Inmueble> res = new List<Inmueble>();
			Inmueble entidad = null;
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @$"
					SELECT i.Id, i.Direccion, i.Ambientes, i.Uso, i.Tipo, i.Superficie, i.Latitud, i.Longitud, i.Estado, i.Precio, i.idPropietario, i.urlPortada, p.Nombre, p.Apellido
					FROM Inmuebles i JOIN Propietarios p ON i.idPropietario = p.id
					WHERE idPropietario=@idPropietario";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.Parameters.AddWithValue("@idPropietario", idPropietario);
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader["Direccion"] == DBNull.Value ? "" : reader.GetString("Direccion"),
							Ambientes = reader.GetInt32("Ambientes"),
							Uso = reader.GetString("Uso"),
							Tipo = reader.GetString("Tipo"),
							Superficie = reader.GetInt32("Superficie"),
							Latitud = reader.GetDecimal("Latitud"),
							Longitud = reader.GetDecimal("Longitud"),
							Estado = reader.GetString("Estado"),
							Precio = reader.GetInt32("Precio"),
							UrlPortada = reader.IsDBNull(reader.GetOrdinal("urlPortada")) ? null : reader.GetString("urlPortada"),
							IdPropietario = reader.GetInt32("idPropietario"),
							Propietario = new Propietario
							{
								Id = reader.GetInt32("idPropietario"),
								Nombre = reader.GetString("Nombre"),
								Apellido = reader.GetString("Apellido"),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}
		public IList<Inmueble> ObtenerDisponibles()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"SELECT i.id, i.direccion, i.uso, i.tipo, i.ambientes, i.superficie, i.latitud, i.longitud, i.estado, i.precio, i.idPropietario, 
                            p.nombre, p.apellido, p.DNI
                            FROM Inmuebles i
							INNER JOIN Propietarios p 
							ON i.idPropietario = p.id
							WHERE i.estado = 'Disponible'";
				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
							Uso = reader.GetString(nameof(Inmueble.Uso)),
							Tipo = reader.GetString(nameof(Inmueble.Tipo)),
							Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
							Latitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Latitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Longitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Longitud)),
							Estado = reader.GetString(nameof(Inmueble.Estado)),
							Precio = reader.GetInt32(nameof(Inmueble.Precio)),
							IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								Dni = reader.GetInt32(nameof(Propietario.Dni)),

							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerDisponiblesPorFecha(DateTime fechaInicio, DateTime fechaFin)
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (var connection = new MySqlConnection(connectionString))
			{
				string sql = @"
					SELECT i.id, i.direccion, i.uso, i.tipo, i.ambientes, i.superficie, 
						i.latitud, i.longitud, i.estado, i.precio, i.idPropietario,
						p.nombre, p.apellido, p.DNI
					FROM Inmuebles i
					INNER JOIN Propietarios p ON i.idPropietario = p.id
					WHERE i.estado = 'Disponible'
					AND i.id NOT IN (
						SELECT c.idInmueble
						FROM Contratos c
						WHERE c.estado = 1
							AND (
								(c.fechaInicio <= @fechaFin AND c.fechaFin >= @fechaInicio)
							)
					)";

				using (MySqlCommand command = new MySqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@fechaInicio", fechaInicio);
					command.Parameters.AddWithValue("@fechaFin", fechaFin);
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble entidad = new Inmueble
						{
							Id = reader.GetInt32(nameof(Inmueble.Id)),
							Direccion = reader[nameof(Inmueble.Direccion)] == DBNull.Value ? "" : reader.GetString(nameof(Inmueble.Direccion)),
							Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
							Uso = reader.GetString(nameof(Inmueble.Uso)),
							Tipo = reader.GetString(nameof(Inmueble.Tipo)),
							Superficie = reader.GetInt32(nameof(Inmueble.Superficie)),
							Latitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Latitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Latitud)),
							Longitud = reader.IsDBNull(reader.GetOrdinal(nameof(Inmueble.Longitud))) ? (decimal?)null : reader.GetDecimal(nameof(Inmueble.Longitud)),
							Estado = reader.GetString(nameof(Inmueble.Estado)),
							Precio = reader.GetInt32(nameof(Inmueble.Precio)),
							IdPropietario = reader.GetInt32(nameof(Inmueble.IdPropietario)),
							Propietario = new Propietario
							{
								Id = reader.GetInt32(nameof(Propietario.Id)),
								Nombre = reader.GetString(nameof(Propietario.Nombre)),
								Apellido = reader.GetString(nameof(Propietario.Apellido)),
								Dni = reader.GetInt32(nameof(Propietario.Dni)),
							}
						};
						res.Add(entidad);
					}
					connection.Close();
				}
			}
			return res;
		}



	}
	
}
