using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;
using System.Data;
using TMPro;

public class Database : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TMP_InputField Input_Id;
    [SerializeField] TMP_InputField Input_Password;
    [SerializeField] TextMeshProUGUI Text_Log;

    [Header("ConnectionInfo")]
    [SerializeField] string _ip = "3.38.210.157";
    [SerializeField] string _dbName = "login";
    [SerializeField] string _uid = "root";
    [SerializeField] string _pwd = "1234";

    private bool _isConnectTestComplete; // 중요하진 않음

    private static MySqlConnection _dbConnection;


    public void Start()
    {
        TestDBConnect();
    }

    private void SendQuery(string queryStr, string tableName)
    {
        // 있으면 Select 관련 함수 호출
        if (queryStr.Contains("SELECT"))
        {
            DataSet dataSet = OnSelectRequest(queryStr, tableName);

        }
        else // 없다면 Insert 또는 Update 관련 쿼리
        {
            bool isSuccess = OnInsertOnUpdateRequest(queryStr);
            if (isSuccess)
            {
                Text_Log.text = "회원가입이 완료되었습니다!";
            }
        }
    }

    public static bool OnInsertOnUpdateRequest(string query)
    {
        try
        {
            MySqlCommand sqlCommand = new MySqlCommand();
            sqlCommand.Connection = _dbConnection;
            sqlCommand.CommandText = query;

            _dbConnection.Open();
            sqlCommand.ExecuteNonQuery();
            _dbConnection.Close();
            return true;
        }
        catch (MySqlException ex)
        {
            if (ex.Number == 1062) // Duplicate entry error code
            {
                Debug.LogWarning("같은 아이디가 있습니다.");
                return false;
            }
            else
            {
                Debug.LogWarning(ex.Message);
                return false;
            }
        }
        finally
        {
            if (_dbConnection.State == ConnectionState.Open)
            {
                _dbConnection.Close();
            }
        }
    }

    private string DeformatResult(DataSet dataSet)
    {
        string resultStr = string.Empty;

        foreach (DataTable table in dataSet.Tables)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    resultStr += $"{column.ColumnName} : {row[column]} \n";
                }
            }
        }
        return resultStr;
    }

    public static DataSet OnSelectRequest(string query, string tableName)
    {
        try
        {
            _dbConnection.Open();
            MySqlCommand sqlCmd = new MySqlCommand();
            sqlCmd.Connection = _dbConnection;
            sqlCmd.CommandText = query;

            MySqlDataAdapter sd = new MySqlDataAdapter(sqlCmd);
            DataSet dataSet = new DataSet();
            sd.Fill(dataSet, tableName);

            _dbConnection.Close();
            return dataSet;
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    private bool ConnectTest()
    {
        string connectsStr = $"Server={_ip};Database={_dbName};Uid={_uid};Pwd={_pwd};";

        try
        {
            using (MySqlConnection conn = new MySqlConnection(connectsStr))
            {
                _dbConnection = conn;
                conn.Open();
            }
            Text_Log.text = "DB 연결을 성공했습니다!";
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"e: {e.ToString()}");
            Text_Log.text = "DB 연결 실패!!!!!!!!!!!!!!!!!!!";
            return false;
        }
    }

    public void TestDBConnect()
    {
        _isConnectTestComplete = ConnectTest();
    }

    public void OnSubmit_SendRegisterQuery()
    {
        Text_Log.text = string.Empty;

        Debug.Log("?");
        string RegisterQuery = $"INSERT INTO login (NickName, Password) VALUES('{Input_Id.text}', '{Input_Password.text}');";

        bool isSuccess = OnInsertOnUpdateRequest(RegisterQuery);
        if (isSuccess)
        {
            Text_Log.text = "회원가입이 완료되었습니다!";
        }
        else
        {
            Text_Log.text = "같은 아이디가 있습니다.";
        }
    }

    public void OnSubmit_Login()
    {
        Text_Log.text = string.Empty;

        string Nickname = Input_Id.text;
        string Password = Input_Password.text;

        string query = $"SELECT Password FROM login WHERE NickName='{Nickname}';";
        DataSet dataSet = OnSelectRequest(query, "login");

        if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
        {
            string dbPassword = dataSet.Tables[0].Rows[0]["Password"].ToString();
            if (dbPassword == Password)
            {
                Text_Log.text = "로그인 성공!";
                OnClick_CloseDatabaseUI();
            }
            else
            {
                Text_Log.text = "비밀번호가 일치하지 않습니다.";
            }
        }
        else
        {
            Text_Log.text = "닉네임이 존재하지 않습니다.";
        }
    }

    public void OnClick_OpenDatabaseUI()
    {
        this.gameObject.SetActive(true);
    }

    public void OnClick_CloseDatabaseUI()
    {
        this.gameObject.SetActive(false);
    }
}
