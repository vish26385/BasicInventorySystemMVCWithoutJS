using ALLINONEPROJECTWITHOUTJS.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ALLINONEPROJECTWITHOUTJS.Services
{
    public class PartyService : IPartyService
    {
        private readonly string _connectionString;
        public PartyService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("ConnectionString");
        }

        public List<PartyMaster> GetAllParties(string PartyType)
        {
            var parties = new List<PartyMaster>();

            using var con = new SqlConnection(_connectionString);
            if (PartyType == "")
            {
                using var sda = new SqlDataAdapter("select * from PartyMasters", con);
                var partyData = new DataTable();
                sda.Fill(partyData);

                parties = partyData.AsEnumerable()
                    .Select(x => new PartyMaster
                    {
                        Id = Convert.ToInt32(x["Id"]),
                        Name = Convert.ToString(x["Name"]) ?? ""
                    })
                    .ToList();
            }
            else
            {
                using var cmd = new SqlCommand("sp_GetAllParties", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PartyType", PartyType);
                con.Open();
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    parties.Add(new PartyMaster
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Name = reader["Name"].ToString() ?? ""
                    });
                }
            }
            if (parties.Count == 0)
                parties.Add(new PartyMaster());

            return parties;
        }
    }
}
