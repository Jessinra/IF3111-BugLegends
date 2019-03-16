// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Linq;
// using System.Text;
// using UnityEngine;

// namespace DataBank {

//     public class HistoryDB : SqliteHelper {

//         private const String Tag = "HistoryDB:\t";
//         [SerializeField] private const String TABLE_NAME = "History";

//         private const String KEY_ID = "id";
//         private const String KEY_NAME = "name";
//         private const String KEY_SCORE = "score";
//         private const String KEY_DATE = "date";


//         private String[] COLUMNS = new String[] { KEY_ID, KEY_NAME, KEY_SCORE, KEY_DATE };

//         public HistoryDB() : base() {
//             IDbCommand dbcmd = getDbCommand();
//             dbcmd.CommandText = "CREATE TABLE IF NOT EXISTS " + TABLE_NAME + " ( " +
//                 KEY_ID + " TEXT PRIMARY KEY, " +
//                 KEY_NAME + " TEXT, " +
//                 KEY_SCORE + " TEXT, " +
//                 KEY_DATE + " DATETIME DEFAULT CURRENT_TIMESTAMP )";
//             dbcmd.ExecuteNonQuery();
//         }

//         public void addData(HistoryEntity history) {
//             IDbCommand dbcmd = getDbCommand();
//             dbcmd.CommandText =
//                 "INSERT INTO " + TABLE_NAME +
//                 " ( " +
//                 KEY_ID + ", " +
//                 KEY_NAME + ", " +
//                 KEY_SCORE + " ) "

//                 +
//                 "VALUES ( '" +
//                 history._id + "', '" +
//                 history._name + "', '" +
//                 history._score + "' )";
//             dbcmd.ExecuteNonQuery();
//         }

//         public override IDataReader getDataById(int id) {
//             return base.getDataById(id);
//         }

//         public override IDataReader getDataByString(string str) {
//             Debug.Log(Tag + "Getting history: " + str);

//             IDbCommand dbcmd = getDbCommand();
//             dbcmd.CommandText =
//                 "SELECT * FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + str + "'";
//             return dbcmd.ExecuteReader();
//         }

//         public override void deleteDataByString(string id) {
//             Debug.Log(Tag + "Deleting history: " + id);

//             IDbCommand dbcmd = getDbCommand();
//             dbcmd.CommandText =
//                 "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
//             dbcmd.ExecuteNonQuery();
//         }

//         public override void deleteDataById(int id) {
//             base.deleteDataById(id);
//         }

//         public override void deleteAllData() {
//             Debug.Log(Tag + "Deleting Table");

//             base.deleteAllData(TABLE_NAME);
//         }

//         public override IDataReader getAllData() {
//             return base.getAllData(TABLE_NAME);
//         }

//         public IDataReader getLatestTimeStamp() {
//             IDbCommand dbcmd = getDbCommand();
//             dbcmd.CommandText =
//                 "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_DATE + " DESC LIMIT 1";
//             return dbcmd.ExecuteReader();
//         }
//     }
// }