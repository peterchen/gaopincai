﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Lottery.Data.SQLServer.SSC
{
    using Common;
    using Configuration;
    using Logging;
    using Model.SSC;
    using Utils;

    public class DwNumberBiz : SinglePKDataAccessBiz<DwNumberDAO, DwNumber>
    {
        public DwNumberBiz(string dbName)
            : base(new DwNumberDAO(ConfigHelper.GetConnString(dbName)))
        {
        }

        public DateTime GetLatestDate()
        {
            int num = DataAccessor.SelectLatestDate(string.Empty);
            if (num == 0) return DateTime.Now.AddDays(-3);
            return DateTime.ParseExact(num.ToString(), "yyyyMMdd", null);
        }

        public long GetLatestPeroid()
        {
            return this.DataAccessor.SelectLatestPeroid(string.Empty);
        }

        public HashSet<long> GetPeroidsByDate(DateTime date)
        {
            string dayOfBefore3 = date.AddDays(-1).ToString("yyyyMMdd");
            Operand operand = Restrictions.Clause(SqlClause.Where)
                .Append(Restrictions.GreaterThanOrEqual(DwNumber.C_Date, dayOfBefore3));
            var dtos = this.DataAccessor.SelectWithCondition(operand.ToString(), DwNumber.C_P);
            HashSet<long> hashSet = new HashSet<long>();
            foreach (var dto in dtos) hashSet.Add(dto.P);
            return hashSet;
        }

        public int Count
        {
            get { return this.DataAccessor.Count(); }
        }

        private DwNumber Create(long p, int n, string code, int date, string datetime)
        {
            string[] arr = code.Split(',');
            DwNumber number = new DwNumber();
            number.P = p;
            number.N = n;
            number.Seq = this.Count + 1;
            number.Date = date;
            number.Created = ConvertHelper.GetDateTime(datetime);
            number.D5 = ConvertHelper.GetInt32(arr[0]);
            number.D4 = ConvertHelper.GetInt32(arr[1]);
            number.D3 = ConvertHelper.GetInt32(arr[2]);
            number.D2 = ConvertHelper.GetInt32(arr[3]);
            number.D1 = ConvertHelper.GetInt32(arr[4]);
            number.P5 = code.Replace(",", "");
            number.P4 = number.P5.Substring(1);
            number.P3 = number.P5.Substring(2);
            number.P2 = number.P5.Substring(3);
            number.C2 = NumberCache.Instance.GetNumberId("C2", number.P2);
            number.C3 = NumberCache.Instance.GetNumberId("C3", number.P3);
            number.C4 = NumberCache.Instance.GetNumberId("C4", number.P4);
            number.C5 = NumberCache.Instance.GetNumberId("C5", number.P5);
            return number;
        }

        public bool Add(long p, int n, string code, int date, string datetime)
        {
            DwNumber number = Create(p, n, code, date, datetime);
            return this.SaveToDB(number);
        }

        private void AddSpan(DwNumber number)
        {
            string[] dmNames = DimensionNumberTypeBiz.Instance.GetDimensions("SSC");
            List<BatchEntity<DwSpan>> batchEntities = new List<BatchEntity<DwSpan>>(dmNames.Length);
            foreach (string dmName in dmNames)
            {
                string[] numberTypes = DimensionNumberTypeBiz.Instance.GetNumberTypes("SSC", dmName);
                Dictionary<string, int> spanDict = this.DataAccessor.SelectSpansByNumberTypes(number, dmName, numberTypes);
                DwSpan dwSpan = new DwSpan() { P = number.P };
                foreach (string key in spanDict.Keys)
                {
                    string propertyName = string.Format("{0}Spans", key);
                    dwSpan[propertyName] = spanDict[key];
                }

                string destTableName = ConfigHelper.GetDwSpanTableName(dmName);
                string[] columnNames = numberTypes.Select(x => x + "Spans").Union(new string[] { "P" }).ToArray();
                BatchEntity<DwSpan> batchEntity = new BatchEntity<DwSpan>(dwSpan, destTableName, columnNames);
                batchEntities.Add(batchEntity);
            }

            DwSpanDAO spanDao = new DwSpanDAO(string.Empty, this.DataAccessor.ConnectionString);
            spanDao.Insert(batchEntities);
        }

        private bool SaveToDB(DwNumber number)
        {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    this.AddSpan(number);
                    this.Add(number);
                    scope.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.Write(ex.ToString());
                return false;
            }
        }
    }
}

