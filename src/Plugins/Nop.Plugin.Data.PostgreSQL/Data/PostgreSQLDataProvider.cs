﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using Nop.Core.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Data.Extensions;

namespace Nop.Plugin.Data.PostgreSQL.Data
{
    /// <summary>
    /// Represents SQL Server data provider
    /// </summary>
    class PostgreSQLDataProvider : IDataProvider
    {
        #region Methods

        /// <summary>
        /// Initialize database
        /// </summary>
        /// <param name="tablename">Used table name</param>
        public virtual void InitializeDatabase() //__(string tablename)
        {
            var tablename = "";
            var context = EngineContext.Current.Resolve<IDbContext>();

            //check some of table names to ensure that we have nopCommerce 2.00+ installed
            var tableNamesToValidate = new List<string> { "Customer", "Discount", "Order", "Product", "ShoppingCartItem" };
            var existingTableNames1 = context
              .QueryFromSql<StringQueryType>(
                  $"SELECT table_name AS Value FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' and table_catalog = '{tablename}'");

            var existingTableNames = context
                .QueryFromSql<StringQueryType>(
                    $"SELECT table_name AS Value FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE' and table_catalog = '{tablename}'")

                .Select(stringValue => stringValue.Value).ToList();
            var createTables = !existingTableNames.Intersect(tableNamesToValidate, StringComparer.InvariantCultureIgnoreCase).Any();
            if (!createTables)
                return;

            var fileProvider = EngineContext.Current.Resolve<INopFileProvider>();

            //create tables
            //EngineContext.Current.Resolve<IRelationalDatabaseCreator>().CreateTables();
            //(context as DbContext).Database.EnsureCreated();
            context.ExecuteSqlScript(context.GenerateCreateScript());

            //create indexes
            //context.ExecuteSqlScriptFromFile(fileProvider.MapPath(NopDataDefaults.SqlServerIndexesFilePath));

            //create stored procedures 
            //context.ExecuteSqlScriptFromFile(fileProvider.MapPath(NopDataDefaults.SqlServerStoredProceduresFilePath));
        }

        /// <summary>
        /// Get a support database parameter object (used by stored procedures)
        /// </summary>
        /// <returns>Parameter</returns>
        public virtual DbParameter GetParameter()
        {
            return new SqlParameter();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this data provider supports backup
        /// </summary>
        public virtual bool BackupSupported => true;

        /// <summary>
        /// Gets a maximum length of the data for HASHBYTES functions, returns 0 if HASHBYTES function is not supported
        /// </summary>
        public virtual int SupportedLengthOfBinaryHash => 8000; //for SQL Server 2008 and above HASHBYTES function has a limit of 8000 characters.

        public string DataProviderName => "PostgreSQL";
        #endregion
    }
}
