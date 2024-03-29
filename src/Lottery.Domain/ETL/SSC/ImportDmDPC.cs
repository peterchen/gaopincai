﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Lottery.ETL.SSC
{
    using Model.Common;
    using Model.SSC;
    using Data.SQLServer;
    using Data.SQLServer.SSC;
    using Data.SQLServer.Common;
    using Utils;

    public class ImportDmDPC
    {
        public static void UpdateNumberType()
        {
            UpdateSSC();
            Update3D();
            UpdatePL35();
        }

        public static void Update()
        {
            List<Category> categories = CategoryBiz.Instance.GetEnabledCategories("SSC");
            foreach (var category in categories)
            {
                //Modify(category.DbName, "DX");
                //Modify(category.DbName, "P2");
                //Modify(category.DbName, "P3");
                //Modify(category.DbName, "P4");
                //Modify(category.DbName, "P5");
                //Modify(category.DbName, "C2");
                //Modify(category.DbName, "C3");
            }
        }

        public static void UpdateSSC()
        {
            List<Category> categories = CategoryBiz.Instance.GetEnabledCategories("SSC");
            foreach (var category in categories)
            {
                ModifyNumberType(category.DbName, "C2");
                ModifyNumberType(category.DbName, "C3");
                ModifyNumberType(category.DbName, "C4");
                ModifyNumberType(category.DbName, "C5");
            }
        }

        public static void Update3D()
        {
            List<Category> categories = CategoryBiz.Instance.GetEnabledCategories("3D");
            foreach (var category in categories)
            {
                ModifyNumberType(category.DbName, "C2");
                ModifyNumberType(category.DbName, "C3");
            }
        }

        public static void UpdatePL35()
        {
            List<Category> categories = CategoryBiz.Instance.GetEnabledCategories("PL35");
            foreach (var category in categories)
            {
                ModifyNumberType(category.DbName, "C2");
                ModifyNumberType(category.DbName, "C3");
                ModifyNumberType(category.DbName, "C4");
                ModifyNumberType(category.DbName, "C5");
            }
        }

        private static void ModifyNumberType(string name, string type)
        {
            DmDPCBiz biz = new DmDPCBiz(name, type);
            var numbers = biz.GetAll("Id", "Number");
            foreach (var number in numbers)
            {
                number.NumberType = number.Id.GetNumberType();
                biz.Modify(number, number.Id, DmDPC.C_NumberType);
            }
        }

        private static void Modify(string name, string type)
        {
            DmDPCBiz biz = new DmDPCBiz(name, type);
            var numbers = biz.GetAll("Id", "Number");
            foreach (var number in numbers)
            {
                List<int> digits = number.Number.ToList(' ');
                number.ZiHe = digits.GetZiHe();
                number.ZiCnt = number.ZiHe.Count("1");
                number.HeCnt = number.ZiHe.Count("0");
                number.Ji = digits.GetJi();
                number.JiWei = number.Ji.GetWei();
                number.KuaDu = digits.GetKuaDu();
                number.AC = digits.GetAC();

                //biz.Modify(number, number.Id, DmFCAn.C_ZiHe, DmFCAn.C_ZiCnt, DmFCAn.C_HeCnt,
                //    DmFCAn.C_Ji, DmFCAn.C_JiWei, DmFCAn.C_KuaDu, DmFCAn.C_AC);
            }
        }

        /// <summary>
        /// 导入时时彩所有玩法的号码及相关属性到指定输出设备
        /// </summary>
        /// <param name="output">db|txt</param>
        public static void Add(string output)
        {
            List<Category> categories = CategoryBiz.Instance.GetEnabledCategories("SSC");
            foreach (var category in categories)
            {
                // P(category.DbName, "D1", 1, output);
                // P(category.DbName, "P2", 2, output);
                // P(category.DbName, "P3", 3, output);
                //P(category.DbName, "P4", 4, output);
                //P(category.DbName, "P5", 5, output);
                // C(category.DbName, "C2", 2, output);
                // C(category.DbName, "C3", 3, output);
                //C(category.DbName, "C4", 4, output);
                //C(category.DbName, "C5", 5, output);
                //break;
            }
        }

        public static void P(string name, string type, int length, string output)
        {
            int count = (int)Math.Pow(10, length);
            List<string> list = new List<string>(count);
            string format = "D" + length;
            for (int i = 0; i < count; i++)
            {
                list.Add(i.Format(format, ","));
            }

            if (output.Equals("txt"))
            {
                SaveToText(name,type, list);
                return;
            }

            SaveToDB(name, type, list);
        }

        public static void C(string name, string type, int length, string output)
        {
            int count = (int)Math.Pow(10, length);
            List<string> list = new List<string>(10000);
            string format = "D" + length;
            for (int i = 0; i < count; i++)
            {
                var digits = i.ToString(format).ToArray();
                Permutations<char> p = new Permutations<char>(digits, length);
                List<string> pn = p.Get(",");
                if (pn.Exists(x => list.Contains(x))) continue;

                list.Add(i.Format(format, ","));
            }

            if (output.Equals("txt"))
            {
                SaveToText(name, type, list);
                return;
            }

            SaveToDB(name, type, list);
        }

        private static void SaveToText(string name, string type, List<string> list)
        {
            string fileName = string.Format(@"d:\{0}_{1}.txt", name, type);
            StreamWriter writer = new StreamWriter(fileName, false, Encoding.UTF8);
            foreach (var str in list)
            {
                var dto = GetDmFCAn(str.ToList());
                writer.WriteLine(dto.ToString());
            }
            writer.Close();
        }

        private static void SaveToDB(string name, string type, List<string> list)
        {
            DmDPCBiz biz = new DmDPCBiz(name, type);
            List<DmDPC> entities = new List<DmDPC>(list.Count);
            foreach (string str in list)
            {
                entities.Add(GetDmFCAn(str.ToList()));
            }
            biz.DataAccessor.Insert(entities, SqlInsertMethod.SqlBulkCopy);
        }

        private static DmDPC GetDmFCAn(IList<int> list)
        {
            DmDPC dto = new DmDPC();
            dto.Id = list.ToString("");
            dto.Number = list.ToString(" ");
            dto.DaXiao = list.GetDaXiao(4);
            dto.DanShuang = list.GetDanShuang();
            dto.ZiHe = list.GetZiHe();
            dto.Lu012 = list.GetLu012();
            dto.He = list.GetHe();
            dto.HeWei = dto.He.GetWei();
            dto.DaCnt = dto.DaXiao.Count("1");
            dto.XiaoCnt = dto.DaXiao.Count("0");
            dto.DanCnt = dto.DanShuang.Count("1");
            dto.ShuangCnt = dto.DanShuang.Count("0");
            dto.ZiCnt = dto.ZiHe.Count("1");
            dto.HeCnt = dto.ZiHe.Count("0");
            dto.Lu0Cnt = dto.Lu012.Count("0");
            dto.Lu1Cnt = dto.Lu012.Count("1");
            dto.Lu2Cnt = dto.Lu012.Count("2");
            dto.Ji = list.GetJi();
            dto.JiWei = dto.Ji.GetWei();
            dto.KuaDu = list.GetKuaDu();
            dto.AC = list.GetAC();
            dto.DaXiaoBi = list.GetDaXiaoBi(4);
            dto.ZiHeBi = list.GetZiHeBi();
            dto.DanShuangBi = list.GetDanShuangBi();
            dto.Lu012Bi = list.GetLu012Bi();

            return dto;
        }
    }
}
