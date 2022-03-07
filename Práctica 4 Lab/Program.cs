using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Práctica_4_Lab
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static void Main(string[] args)
        {
            log.Info("Se está iniciando el programa");
            log.Warn("Esto es una advertencia");
            log.Debug("Esto es un mensaje de depuración");
            log.Error("Esto es un error");
            log.Fatal("Esto es un error critico");

            SqlConnection conexión = new SqlConnection();
            SqlTransaction transaction = null;

            SqlCommand command = null;

            conexión.ConnectionString = ConfigurationManager.ConnectionStrings["Cn"].ConnectionString;
            
            conexión.Open();

            int idtormenta, tipo, resultado = 0;
            string nombre, vientos, presion, localizacion, movimientos, victimas;

            int terminar = 1;

            while (terminar == 1)
            {
                Console.Write("ID de la Tormenta: ");
                idtormenta = int.Parse(Console.ReadLine());
                Console.WriteLine();

                command = new SqlCommand();
                command.CommandText = "ppGetTormenta";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@IDTormenta", idtormenta);
                command.Connection = conexión;
                SqlDataReader res = command.ExecuteReader();

                if (res.HasRows)
                {
                    res.Read();
                    nombre = $"{res["nombre"]}";
                    Console.WriteLine("Nombre: " + nombre);
                    Console.WriteLine();

                    Console.WriteLine("Tipo: ");
                    Console.WriteLine("1. Tormenta tropical");
                    Console.WriteLine("2. Ciclón");
                    Console.WriteLine("3. Huracán");
                    Console.WriteLine("4. Depresión");
                    Console.WriteLine();
                    tipo = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    Console.Write("Vientos Máx: ");
                    vientos = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Presión Central: ");
                    presion = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Localización: ");
                    localizacion = Console.ReadLine();
                    Console.WriteLine();

                    movimientos = $"{res["movimientos"]}";
                    Console.WriteLine("Movimientos: " + movimientos);
                    Console.WriteLine();

                    Console.Write("Victimas Nuevas: ");
                    victimas = Console.ReadLine();
                    Console.WriteLine();

                }
                else
                {
                    Console.WriteLine("Tipo: ");
                    Console.WriteLine("1. Tormenta tropical");
                    Console.WriteLine("2. Ciclón");
                    Console.WriteLine("3. Huracán");
                    Console.WriteLine("4. Depresión");
                    Console.WriteLine();
                    tipo = int.Parse(Console.ReadLine());
                    Console.WriteLine();

                    Console.Write("Nombre: ");
                    nombre = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Vientos Máx: ");
                    vientos = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Presión Central: ");
                    presion = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Localización: ");
                    localizacion = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Movimientos: ");
                    movimientos = Console.ReadLine();
                    Console.WriteLine();

                    Console.Write("Victimas Nuevas: ");
                    victimas = Console.ReadLine();
                    Console.WriteLine();
                }

                res.Close();

                command = new SqlCommand();
                command.CommandText = "ppInsertTormenta";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@IDTormenta", idtormenta);
                command.Parameters.AddWithValue("@Tipo", tipo);
                command.Parameters.AddWithValue("@Nombre", nombre);
                command.Parameters.AddWithValue("@Vientos", vientos);
                command.Parameters.AddWithValue("@Presion", presion);
                command.Parameters.AddWithValue("@Localizacion", localizacion);
                command.Parameters.AddWithValue("@Movimientos", movimientos);
                command.Parameters.AddWithValue("@Victimas", victimas);

                command.Connection = conexión;

                try
                {
                    transaction = conexión.BeginTransaction();
                    command.Transaction = transaction;
                    resultado = command.ExecuteNonQuery();

                    command.Parameters.Clear();
                    command.CommandText = "ppHistorial";

                    command.Parameters.AddWithValue("@IDTormenta", idtormenta);
                    command.Parameters.AddWithValue("@Tipo", tipo);
                    command.Parameters.AddWithValue("@Vientos", vientos);
                    command.Parameters.AddWithValue("@Presion", presion);
                    command.Parameters.AddWithValue("@Localizacion", localizacion);
                    command.Parameters.AddWithValue("@Victimas", victimas);
                    //throw new Exception("Falló");

                    command.ExecuteNonQuery();
                    transaction.Commit();
                }
                catch (Exception err)
                {
                    transaction.Rollback();
                    Console.WriteLine("Aquí ocurrió un error: " + err.Message);
                    //throw;
                }

                

                Console.WriteLine();

                Console.WriteLine(resultado + " Registrados");

                Console.WriteLine();
                Console.Write("Desea Continuar: 1. Si / 2. No: ");
                terminar = int.Parse(Console.ReadLine());
            }

            command.CommandText = "ppGetTormenta";
            command.Parameters.Clear();

            SqlDataReader reader = command.ExecuteReader();

            Console.WriteLine();

            while (reader.Read())
            {
                Console.WriteLine($"{reader["IDTormenta"]}\t{reader["Nombre"]}\t{reader["Tipo"]}\t{reader["VientosMax"]}\t{reader["PresionCentral"]}\t {reader["Localizacion"]}\t Victimas Totales: {reader["VictimasTotales"]}");
            }


            Console.WriteLine(conexión.State);

            Console.ReadKey();
        }
        
        
    }
}
