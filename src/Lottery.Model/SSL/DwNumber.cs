﻿using System;

namespace Lottery.Model.SSL
{
    using Data;

    [Serializable]
    public class DwNumber : BaseModel
    {
        public static string ENTITYNAME = "DwNumber";

        #region Const Members

        /// <summary>
        /// 列名P  
        /// </summary>
        public static readonly String C_P = "P";
        /// <summary>
        /// 列名D1  
        /// </summary>
        public static readonly String C_D1 = "D1";
        /// <summary>
        /// 列名D2  
        /// </summary>
        public static readonly String C_D2 = "D2";
        /// <summary>
        /// 列名D3  
        /// </summary>
        public static readonly String C_D3 = "D3";
        /// <summary>
        /// 列名P2  
        /// </summary>
        public static readonly String C_P2 = "P2";
        /// <summary>
        /// 列名C2  
        /// </summary>
        public static readonly String C_C2 = "C2";
        /// <summary>
        /// 列名P3  
        /// </summary>
        public static readonly String C_P3 = "P3";
        /// <summary>
        /// 列名C3  
        /// </summary>
        public static readonly String C_C3 = "C3";
        /// <summary>
        /// 列名N  
        /// </summary>
        public static readonly String C_N = "N";
        /// <summary>
        /// 列名Seq  
        /// </summary>
        public static readonly String C_Seq = "Seq";
        /// <summary>
        /// 列名Date  
        /// </summary>
        public static readonly String C_Date = "Date";
        /// <summary>
        /// 列名Created  
        /// </summary>
        public static readonly String C_Created = "Created";

        #endregion

        #region Field Members

        private Int64 _p;

        private Int32 _d1;

        private Int32 _d2;

        private Int32 _d3;

        private String _p2;

        private String _c2;

        private String _p3;

        private String _c3;

        private Int32 _n;

        private Int32 _seq;

        private Int32 _date;

        private DateTime _created;

        #endregion

        #region Table Property Members

        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "P", IsPrimaryKey = true)]
        public virtual Int64 P
        {
            get { return this._p; }
            set { this._p = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "D1")]
        public virtual Int32 D1
        {
            get { return this._d1; }
            set { this._d1 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "D2")]
        public virtual Int32 D2
        {
            get { return this._d2; }
            set { this._d2 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "D3")]
        public virtual Int32 D3
        {
            get { return this._d3; }
            set { this._d3 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "P2")]
        public virtual String P2
        {
            get { return this._p2; }
            set { this._p2 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "C2")]
        public virtual String C2
        {
            get { return this._c2; }
            set { this._c2 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "P3")]
        public virtual String P3
        {
            get { return this._p3; }
            set { this._p3 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "C3")]
        public virtual String C3
        {
            get { return this._c3; }
            set { this._c3 = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "N")]
        public virtual Int32 N
        {
            get { return this._n; }
            set { this._n = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "Seq")]
        public virtual Int32 Seq
        {
            get { return this._seq; }
            set { this._seq = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "Date")]
        public virtual Int32 Date
        {
            get { return this._date; }
            set { this._date = value; }
        }
        /// <summary>
        /// 获取或设置
        /// </summary>
        [Column(Name = "Created")]
        public virtual DateTime Created
        {
            get { return this._created; }
            set { this._created = value; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                this._p, this._d1, this._d2, this._d3, this._p2, this._c2, this._p3, this._c3,
                this._n, this._seq, this._date, this._created);
        }
    }
}

