﻿using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;

namespace Lottery.Data.SQLServer.SSC
{
    using Model.SSC;
    using Utils;
    using Configuration;
    using Logging;

    public class DwNumberBiz : SinglePKDataAccessBiz<DwNumberDAO, DwNumber>
    {
        private static object lockObj = new object();

        public DwNumberBiz(string dbName)
            : base(new DwNumberDAO(ConfigHelper.GetConnString(dbName)))
        {
        }

        public DateTime GetLatestDate()
        {
            int num = DataAccessor.SelectLatestDate(string.Empty);
            if (num == 0)
                return new DateTime(2010, 1, 5);

            return DateTime.ParseExact(num.ToString(), "yyyyMMdd", null);
        }

        public HashSet<long> GetPeroidsOfByDate(DateTime date)
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

        public bool Add(long p, int n, string code, int date, string datetime)
        {
            lock (lockObj)
            {
                string[] arr = code.Split(',');
                DwNumber number = new DwNumber();
                number.Code = code;
                number.Date = date;
                number.P = p;
                number.N = n;
                number.Seq = this.Count + 1;
                number.D5 = ConvertHelper.GetInt32(arr[0]);
                number.D4 = ConvertHelper.GetInt32(arr[1]);
                number.D3 = ConvertHelper.GetInt32(arr[2]);
                number.D2 = ConvertHelper.GetInt32(arr[3]);
                number.D1 = ConvertHelper.GetInt32(arr[4]);
                number.Created = ConvertHelper.GetDateTime(datetime);
                number.P5 = code.Replace(",", "");
                number.P4 = number.P5.Substring(1);
                number.P3 = number.P5.Substring(2);
                number.P2 = number.P5.Substring(3);
                number.C2 = NumberBiz.Instance.GetC2Id(number.P2);
                number.C3 = NumberBiz.Instance.GetC3Id(number.P3);

                return this.SaveToDB(number);
            }
        }

        private void AddSpan(DwNumber number, string filter)
        {
            string[] dmNames = new string[] {
                "Peroid", "DaXiao", "DanShuang", "ZiHe", "Lu012", 
                "He", "HeWei", "Ji", "JiWei", "KuaDu", "AC" 
            };

            List<DwSpan> dwSpans = new List<DwSpan>(dmNames.Length);
            foreach (string dmName in dmNames)
            {
                Dictionary<string, int> spanDict = this.DataAccessor.SelectSpansByNumberTypes(number, dmName, filter);
                DwSpan dwSpan = new DwSpan() { P = number.P };
                dwSpan.EntityName = ConfigHelper.GetDwSpanTableName(dmName);
                foreach (string key in spanDict.Keys)
                {
                    string propertyName = string.Format("{0}Spans", key);
                    dwSpan[propertyName] = spanDict[key];
                }
                dwSpans.Add(dwSpan);
            }

            DwSpanDAO spanDao = new DwSpanDAO(string.Empty, this.DataAccessor.ConnectionString);
            spanDao.Insert(dwSpans, SqlInsertMethod.MultiSqlText);
        }

        private bool SaveToDB(DwNumber number)
        {
            try
            {
                TransactionOptions option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadUncommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    this.AddSpan(number, string.Empty);
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

