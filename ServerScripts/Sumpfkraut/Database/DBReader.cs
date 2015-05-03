﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using SQLiteDataReader = Mono.Data.Sqlite.SqliteDataReader;
using SQLiteCommand = Mono.Data.Sqlite.SqliteCommand;

namespace GUC.Server.Scripts.Sumpfkraut.Database
{
    class DBReader
    {

        ///**
        // *   Executes a defined sql-query and stores the results as strings.
        // *   Stores the results for each sql-statement provided, so that after defining 2 statements 
        // *   (statement ends with ;) there is a list of 2 results. Each result ist in itself a list of rows.
        // *   Per row there can be 0 or more objects/table column entries of data.
        // *   @param results stores the results of 1 or multuple provided sql-queries
        // *   @param colTypes is the list which provides ways to convert the resulting strings to more defined server data
        // *   @param colTypesKeys stores the column names of the data table in a numeric manner (used in cata conversion)
        // *   @param colTypesVals stores the hints on data conversion used in a numeric manner (used in cata conversion)
        // *   @param convertData must be true if the resultings strings should be converted to data on load
        // *   @param select is the optional string after the SELECT-statement in sql
        // *   @param from is the optional string after the FROM-statement in sql
        // *   @param where is the optional string after the WHERE-statement in sql
        // *   @param orderBy is the optional string after the ORDER BY-statement in sql
        // *   @param completeQuery is an optional query which can be used to define more than one sql-statement in one shot (if provided, select, from, where, orderBy are ignored)
        // */

        /**
         *   Executes a defined sql-query and stores the results as strings.
         *   Stores the results for each sql-statement provided, so that after defining 2 statements 
         *   (statement ends with ;) there is a list of 2 results. Each result ist in itself a list of rows.
         *   Per row there can be 0 or more objects/table column entries of data.
         *   @param results stores the results of 1 or multuple provided sql-queries
         *   @param select is the optional string after the SELECT-statement in sql
         *   @param from is the optional string after the FROM-statement in sql
         *   @param where is the optional string after the WHERE-statement in sql
         *   @param orderBy is the optional string after the ORDER BY-statement in sql
         */
        public static void LoadFromDB (ref List<List<List<object>>> results,
            string select = "", string from = "", string where = "", string orderBy = "")
        { 
            string completeQuery = "SELECT " + select
                + " FROM " + from
                + " WHERE " + where
                + " ORDER BY " + orderBy
                + ";";

            LoadFromDB(ref results, completeQuery);
        }

        /**
         *   Executes a defined sql-query and stores the results as strings.
         *   Stores the results for each sql-statement provided, so that after defining 2 statements 
         *   (statement ends with ;) there is a list of 2 results. Each result ist in itself a list of rows.
         *   Per row there can be 0 or more objects/table column entries of data.
         *   @param results stores the results of 1 or multiple provided sql-queries
         *   @param completeQuery is the query which is send to the database (can have multiple statements seperated by ;)
         */
        public static void LoadFromDB (ref List<List<List<object>>> results,
            string completeQuery)
        {

            using (SQLiteCommand cmd = new SQLiteCommand(Sqlite.getSqlite().connection))
            {
                cmd.CommandText = completeQuery;

                SQLiteDataReader rdr = null;
                try
                {
                    rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                    {
                        return;
                    }

                    // temporary array to put all data of a row into
                    object[] rowArr = null;

                    do
                    {
                        // add new result-list
                        results.Add(new List<List<object>>());
                        while (rdr.Read())
                        {
                            // create and fill array of the temporary data row
                            rowArr = new object[rdr.FieldCount];
                            rdr.GetValues(rowArr);
                            results[results.Count - 1].Add(new List<object>(rowArr));
                        }
                    }
                    while (rdr.NextResult());
                    
                }
                catch (Exception ex)
                {
                    throw new Exception("Could not execute SQLiteDataReader in LoadFromDB: " + ex);
                }
                finally
                {
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                }
            }
        }

        public static int[] ParseParamToIntArray (string param)
        {
            string[] data = param.Split(new char[]{',', '='});
            int resultLength = data.Length / 2;
            int[] result = new int[resultLength];
            if ((data != null) && (data.Length > 0))
            {
                int resultIndex = 0;
                int val = 0;
                int i = 0;
                while (i < data.Length)
                {
                    if ((i + 1) >= data.Length)
                    {
                        break;
                    }

                    try
                    {
                        if (Int32.TryParse(data[i + 1], out val))
                        {
                            result[resultIndex] = val;
                            resultIndex++;
                        }
                        else
                        {
                            throw new Exception("Couldn't convert part of param-string to int in ParseMultiParamToArray.");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Couldn't cast converted part of param-string from int to enum-entry DamageTypeIndex: " + ex);
                    }
                                
                    i += 2;
                }
            }

            return result;
        }

        public static string[] ParseParamToStringArray (string param)
        {
            string[] data = param.Split(new char[]{',', '='});
            int resultLength = data.Length / 2;
            string[] result = new string[resultLength];
            if ((data != null) && (data.Length > 0))
            {
                int resultIndex = 0;
                int i = 0;
                while (i < data.Length)
                {
                    if ((i + 1) >= data.Length)
                    {
                        break;
                    }

                    result[resultIndex] = data[i + 1];
                    resultIndex++;
                                
                    i += 2;
                }
            }

            return result;
        }

    }
}
