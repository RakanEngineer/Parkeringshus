using Microsoft.Data.SqlClient;
using System;
using System.Threading;
using static System.Console;

namespace Parkeringshus
{
    class Program
    {
        // Server=""
        static string connectionString = "Data Source=.;Initial Catalog=Parkeringshus;Integrated Security=true; Encrypt=True;Trust Server Certificate=True";

        static void Main(string[] args)
        {

            bool shouldNotExit = true;

            while (shouldNotExit)
            {
                WriteLine("1. Register arrival");
                WriteLine("2. Register departure");
                WriteLine("3. Show parking registry");
                WriteLine("4. Exit");

                ConsoleKeyInfo keyPressed = ReadKey(true);

                Clear();

                switch (keyPressed.Key)
                {
                    case ConsoleKey.D1:
                    case ConsoleKey.NumPad1:

                        Write("Customer: ");

                        string customer = ReadLine();

                        Write("Contact details: ");

                        string contactDetails = ReadLine();

                        Write("Registration number: ");

                        string registrationNumber = ReadLine();

                        Write("Description: ");

                        string description = ReadLine();

                        DateTime arrival = DateTime.Now;

                        RegisterArrival(customer, contactDetails, registrationNumber, description, arrival);
                        
                        Clear();

                        WriteLine("Parking registered");

                        Thread.Sleep(2000);

                        break;

                    case ConsoleKey.D2:
                    case ConsoleKey.NumPad2:

                        {

                            Write("Registration number: ");

                            registrationNumber = ReadLine();

                            DateTime departure = DateTime.Now;

                            RegisterDeparture(registrationNumber, departure);
                        }

                        Clear();

                        // TODO: Create notice snippet

                        WriteLine("Departure registered");

                        Thread.Sleep(2000);

                        Clear();


                        break;

                    case ConsoleKey.D3:
                    case ConsoleKey.NumPad3:

                        ShowParkingRegistry();
                        break;

                    case ConsoleKey.D4:
                    case ConsoleKey.NumPad4:

                        shouldNotExit = false;

                        break;
                }

                Clear();
            }

        }

        private static void RegisterArrival(string? customer, string? contactDetails, string? registrationNumber, string? description, DateTime arrival)
        {
            string query = @"INSERT INTO Parking (Customer, 
                                                              ContactDetails, 
                                                              RegistrationNumber, 
                                                              Description, 
                                                              Arrival)
                                          VALUES (@Customer, 
                                                  @ContactDetails, 
                                                  @RegistrationNumber, 
                                                  @Description, 
                                                  @Arrival)";

            SqlConnection connection = new SqlConnection(connectionString);
            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Customer", customer);
            command.Parameters.AddWithValue("@ContactDetails", contactDetails);
            command.Parameters.AddWithValue("@RegistrationNumber", registrationNumber);
            command.Parameters.AddWithValue("@Description", description);
            command.Parameters.AddWithValue("@Arrival", arrival);

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

        private static void ShowParkingRegistry()
        {
            WriteLine("ID   Registration Number     Arrival                 Departure");
            WriteLine("---------------------------------------------------------------");

            string query = @"SELECT Id
                                              ,Customer
                                              ,ContactDetails
                                              ,RegistrationNumber
                                              ,Description
                                              ,Arrival
                                              ,Departure
                                              FROM Parking";

            // SqlConnection är det vi använder för att upprätta (establish) anslutning till servern och databasen hanteraren.  
            SqlConnection connection = new SqlConnection(connectionString);

            // SqlCommand är det ett Objekt som represeterar det kommandot vi köra. 
            SqlCommand command = new SqlCommand(query, connection);

            connection.Open();

            SqlDataReader dataReader = command.ExecuteReader();

            // den Read() inne i While loopen kommer att retunera en boolen --> true eller false
            // den  Read()--> försöker läsa en rad från databasen och om det gick och läsa, Om det fanns
            // en rad att läsa då retunera den true och då kan vi komma åt värden i dataReader.  
            while (dataReader.Read())
            {
                string id = dataReader["Id"].ToString();

                string registrationNumber = dataReader["RegistrationNumber"].ToString();

                string arrival = dataReader["Arrival"].ToString();

                string departure = dataReader["Departure"].ToString();

                Write(id.PadRight(5, ' '));
                Write(registrationNumber.PadRight(24, ' '));
                Write(arrival.PadRight(20, ' '));
                WriteLine(departure);

            }


            connection.Close();

            WriteLine();
            WriteLine("<Press key to continue");

            ReadKey(true);

            Clear();
        }

        private static void RegisterDeparture(string registrationNumber, DateTime departure)
        {
            //  GETDATE() är samma som DateTime.Now.
            string query = @"UPDATE Parking   
                                           SET Departure = @Departure
	                                       WHERE RegistrationNumber = @RegistrationNumber
	                                       AND Departure IS NULL";

            SqlConnection connection = new SqlConnection(connectionString);

            SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Departure", departure);
            command.Parameters.AddWithValue("@RegistrationNumber", registrationNumber);

            

            connection.Open();

            command.ExecuteNonQuery();

            connection.Close();
        }

    }
}