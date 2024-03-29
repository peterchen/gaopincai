﻿using System;
using System.Linq;
using System.Collections.Generic;

namespace Lottery.Data.SQLServer.Common
{
    using Model.Common;
    using Utils;
    using Configuration;

    /// <summary>
    /// DimensionNumberTypeBiz提供业务逻辑类。
    /// </summary>
    public partial class DimensionNumberTypeBiz :
        SinglePKDataAccessBiz<DimensionNumberTypeDAO, DimensionNumberType>
    {
        #region 私有字段

        private Dictionary<string, Dictionary<string, HashSet<string>>> tdntCache;

        #endregion

        #region 构造函数

        /// <summary>
        /// NumberTypeDimBiz类的一个单件。
        /// </summary>
        public static readonly DimensionNumberTypeBiz Instance = new DimensionNumberTypeBiz();

        protected DimensionNumberTypeBiz()
            : base(new DimensionNumberTypeDAO(ConfigHelper.CommonTableConnString))
        {
            this.LoadToCache();
        }

        #endregion

        #region 公共业务逻辑成员

        public void RefreshCache()
        {
            this.LoadToCache();
        }

        /// <summary>
        /// 根据彩种类型及号码类型获取其对应的维度集合。
        /// </summary>
        /// <param name="type">彩种类型取值为：(11X5,SSC,3D,PL35等)</param>
        /// <param name="numberType">号码类型</param>
        /// <returns>维度集合</returns>
        public List<Dimension> GetDimensions(string type, string numberType)
        {
            if (!this.tdntCache.ContainsKey(type))
                throw new ArgumentException("Not found this type");
            if (string.IsNullOrEmpty(numberType))
                throw new ArgumentNullException("numberType");

            string numType = (numberType.Length > 2) ? numberType.Substring(0, 2) : numberType;
            string[] dimensions = this.tdntCache[type]
                .Where(x => x.Value.Contains(numType))
                .Select(x => x.Key).ToArray();
            List<Dimension> list = new List<Dimension>(dimensions.Length);

            foreach (var dimension in dimensions)
            {
                var dim = DimensionBiz.Instance.GetByCode(dimension);
                list.Add(dim);
            }

            return list.OrderBy(x => x.Seq).ToList();
        }

        /// <summary>
        /// 根据维度类型获与维度名称获取所有玩法数字类型集合。
        /// </summary>
        /// <param name="type">彩种类型取值为：(11X5,SSC,3D,PL35等)</param>
        /// <param name="dimension">维度名称(区分大小写,取值为:Peroid,DaXiao,DanShuang,ZiHe,Lu012,He,HeWei,Ji,JiWei,KuaDu,AC)</param>
        /// <returns>玩法数字类型集合</returns>
        public string[] GetNumberTypes(string type, string dimension)
        {
            if (this.tdntCache.ContainsKey(type) &&
                this.tdntCache[type].ContainsKey(dimension))
                return this.tdntCache[type][dimension].OrderBy(x => x).ToArray();

            throw new ArgumentException("Not found this dimType and name");
        }

        /// <summary>
        /// 根据维度类型获取所有维度名称集合(Peroid,DaXiao,DanShuang,ZiHe,Lu012,He,HeWei,Ji,JiWei,KuaDu,AC);
        /// </summary>
        /// <param name="type">彩种类型取值为：(11X5,SSC,3D,PL35等)</param>
        /// <returns>维度名称集合</returns>
        public string[] GetDimensions(string type)
        {
            if (this.tdntCache.ContainsKey(type))
                return this.tdntCache[type].Keys.OrderBy(x => x).ToArray();

            throw new ArgumentException("Not found this dimType");
        }
        #endregion

        #region 私有方法成员

        private void LoadToCache()
        {
            if (this.tdntCache == null)
                this.tdntCache = new Dictionary<string, Dictionary<string, HashSet<string>>>(10);
            if (this.tdntCache.Count > 0) this.tdntCache.Clear();

            var entities = this.DataAccessor.SelectGroupByTypeDimensionNumberType();
            foreach (var entity in entities)
            {
                if (this.tdntCache.ContainsKey(entity.Type))
                {
                    if (this.tdntCache[entity.Type].ContainsKey(entity.Dimension))
                    {
                        this.tdntCache[entity.Type][entity.Dimension].Add(entity.NumberType); 
                        continue;
                    }

                    HashSet<string> hashset = new HashSet<string>();
                    hashset.Add(entity.NumberType);
                    this.tdntCache[entity.Type].Add(entity.Dimension, hashset);
                    continue;
                }
                this.AddTypeDict(entity);
            }

            foreach (var kp in this.tdntCache)
            {
                HashSet<string> hashset = this.tdntCache[kp.Key]["DaXiao"];
                this.tdntCache[kp.Key].Add("Peroid", hashset);
            }
        }

        private void AddTypeDict(TypeDimensionNumberType entity)
        {
            Dictionary<string, HashSet<string>> dimDict = new Dictionary<string, HashSet<string>>(20);
            HashSet<string> hashset = new HashSet<string>();
            hashset.Add(entity.NumberType);
            dimDict.Add(entity.Dimension, hashset);
            this.tdntCache.Add(entity.Type, dimDict);
        }

        #endregion
    }
}