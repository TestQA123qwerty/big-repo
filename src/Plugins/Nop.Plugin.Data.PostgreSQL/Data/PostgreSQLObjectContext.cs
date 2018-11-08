using System.Linq;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Nop.Plugin.Data.PostgreSQL.Data
{
    /// <summary>
    /// Represents base object context
    /// </summary>
    public partial class NopObjectContext : Nop.Data.NopObjectContext
    {
        public NopObjectContext(DbContextOptions<Nop.Data.NopObjectContext> options) : base(options)
        {
        }

        public override IQueryable<TEntity> EntityFromSql<TEntity>(string sql, params object[] parameters)// where TEntity : BaseEntity
        {
            return this.Set<TEntity>().FromSql(CreateSqlWithParameters(sql, parameters), parameters);
        }

        protected override string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            var paramstring =
                parameters?.Select(p => p as NpgsqlParameter).Where(p => p != null).Select(p => p.ParameterName)
                    .Aggregate(string.Empty, (all, curent) => $"{all}, {curent}").TrimStart(',', ' ');

            //if (parameters != null)
            //{
            //    for (var i = 0; i < parameters.Length; i++)
            //    {
            //        if (!(parameters[i] is NpgsqlParameter parameter))
            //            continue;

            //        paramstring = $"{paramstring}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";
            //    }
            //}
           
            sql = $"SELECT * FROM {sql} ({paramstring ?? string.Empty})";

            return sql;
        }

    }



}