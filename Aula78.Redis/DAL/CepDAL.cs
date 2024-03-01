using Aula78.Redis.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aula78.Redis.DAL
{
    public class CepDAL
    {
        public static SqlConnection Conectar()
        {
            string stringConexao = "Data Source=localhost,1433;User ID=sa;Password=senha@1234xxxy;Initial Catalog=DBCep;";
            SqlConnection conectar = new SqlConnection(stringConexao);
            return conectar;
        }

        public async Task<List<CepDTO>> ObterListaCEPBancoDeDadosSQL()
        {
            List<CepDTO> ceps = new List<CepDTO>();
            CepDTO cep;

            SqlConnection conexao = Conectar();
            string query = "SELECT * FROM CEP;";
            SqlCommand comando = new SqlCommand(query, conexao);

            conexao.Open();

            SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                cep = new CepDTO();;
                if (!reader["CEP"].Equals(DBNull.Value)) cep.CEP = (String)reader["CEP"];
                if (!reader["CIDADE"].Equals(DBNull.Value)) cep.Cidade = (String)reader["CIDADE"];
                if (!reader["ESTADO"].Equals(DBNull.Value)) cep.Estado = (String)reader["ESTADO"];
                if (!reader["BAIRRO"].Equals(DBNull.Value)) cep.Bairro = (String)reader["BAIRRO"];
                if (!reader["LOGRADOURO"].Equals(DBNull.Value)) cep.Logradouro = (String)reader["CEP"];
                ceps.Add(cep);
            }
            reader.Close();
            return ceps;
        }

        public async Task AlterarCEPBancoDeDadosSQL(CepDTO cep)
        {
            SqlConnection conexao = Conectar();
            string query = "UPDATE CEP SET CIDADE = @CIDADE, ESTADO = @ESTADO, BAIRRO = @BAIRRO, LOGRADOURO = @LOGRADOURO WHERE CEP = @CEP;";
            SqlCommand comando = new SqlCommand(query, conexao);

            comando.Parameters.AddWithValue("@CEP", cep.CEP);
            comando.Parameters.AddWithValue("@CIDADE", cep.Cidade);
            comando.Parameters.AddWithValue("@ESTADO", cep.Estado);
            comando.Parameters.AddWithValue("@BAIRRO", cep.Bairro);
            comando.Parameters.AddWithValue("@LOGRADOURO", cep.Logradouro);

            conexao.Open();
            comando.ExecuteNonQuery();
        }
    }
}
