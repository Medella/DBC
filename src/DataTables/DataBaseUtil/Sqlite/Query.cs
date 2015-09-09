﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using DataTables.DatabaseUtil;

namespace DataTables.DatabaseUtil.Sqlite
{
    public class Query : DataTables.Query
    {
        public Query(Database db, string type)
            : base(db, type)
        {
        }

        override protected void _Prepare(string sql)
        {
            DbProviderFactory provider = DbProviderFactories.GetFactory(_db.Adapter());
            DbParameter param;
            DbCommand cmd = provider.CreateCommand();

            cmd.CommandText = sql;
            cmd.Connection = _db.Conn();
            cmd.Transaction = _transaction;

            // Bind values
            for (int i = 0, ien = _bindings.Count; i < ien; i++)
            {
                var binding = _bindings[i];

                param = cmd.CreateParameter();
                param.ParameterName = binding.Name;
                param.Value = binding.Value ?? DBNull.Value;

                if (binding.Type != null)
                {
                    param.DbType = binding.Type;
                }

                cmd.Parameters.Add(param);
            }

            _stmt = cmd;
        }

        override protected DataTables.Result _Exec()
        {
            var dt = new System.Data.DataTable();
            var dr = _stmt.ExecuteReader();

            dt.Load(dr);
            dr.Close();

            return new Sqlite.Result(_db, dt, this);
        }
    }
}