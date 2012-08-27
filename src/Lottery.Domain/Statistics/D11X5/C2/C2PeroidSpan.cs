﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Lottery.Statistics.D11X5.C2
{
    using Data;
    using Model.D11X5;
    using Model.Common;
    using Data.SQLServer.D11X5;
    using Data.SQLServer.Common;

    /// <summary>
    /// 统计C2期数间隔的分布。
    /// Field:{Number:任五,Spans:与上次之间的间隔期数,N:当前所在期，LastN:前一次所在期}
    /// </summary>
    public class C2PeroidSpan : BaseStatistics, IStatistics
    {
        public void StatAll()
        {
            List<DmCategory> categories = DmCategoryBiz.Instance.GetEnabledCategories("11X5");
            Thread[] threads = new Thread[categories.Count];
            for (int i = 0; i < categories.Count; i++)
            {
                threads[i] = new Thread(new ParameterizedThreadStart(this.Stat));
                threads[i].Start(categories[i].DbName);
                //this.Stat(categories[j].dbName);
            }
        }

        private void Stat(object dbName)
        {
            DwNumberBiz biz = new DwNumberBiz(dbName.ToString());
            List<DwNumber> allNumbers = biz.DataAccessor.SelectWithCondition(string.Empty, "P", SortTypeEnum.ASC, null, "C2", "P", "N", "Date");
            for (int i = 0; i < allNumbers.Count; i++) allNumbers[i].Seq = i + 1;
            string[] allC2Numbers = NumberBiz.Instance.C2.Keys.ToArray();

            string fileName = string.Format("Dw{0}_{1}.txt", dbName.ToString(), "C2_PeroidSpan");
            string filePath = string.Format("{0}\\{1}", this._rootPath, fileName);
            StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8);
            foreach (string numberId in allC2Numbers)
            {
                var numbers = allNumbers.Where(x => x.C2.Trim().Equals(numberId)).OrderBy(x => x.Seq).ToArray();
                for (int j = 0; j < numbers.Length; j++)
                {
                    int spans = -1;
                    int lastN = 0;
                    if (j > 0)
                    {
                        spans = numbers[j].Seq - numbers[j - 1].Seq - 1;
                        lastN = numbers[j - 1].N;
                    }
                    string line = string.Format("{0},{1},{2},{3}", numbers[j].P, numberId, spans, lastN);
                    writer.WriteLine(line);
                }
            }

            writer.Close();
            Console.WriteLine("{0},F2PeroidSpan,Finished", dbName);
        }

    }
}
