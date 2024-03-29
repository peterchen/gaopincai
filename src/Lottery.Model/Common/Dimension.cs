﻿using System;

namespace Lottery.Model.Common
{
    using Data;

    /// <summary>
    /// 维度实体。
    /// </summary>
    [Serializable]
    public class Dimension : BaseModel
    {
        public static string ENTITYNAME = "Dimension";

        #region Const Members

        /// <summary>
        /// 列名Id,维度ID  
        /// </summary>
        public static readonly String C_Id = "Id";
        /// <summary>
        /// 列名Name,维度名称
        /// </summary>
        public static readonly String C_Name = "Name";
        /// <summary>
        /// 列名Code,维度代码
        /// </summary>
        public static readonly String C_Code = "Code";
        /// <summary>
        /// 列名Seq,在记录集中排序号
        /// </summary>
        public static readonly String C_Seq = "Seq";
        /// <summary>
        /// 列名Status,维度状态
        /// </summary>
        public static readonly String C_Status = "Status";

        #endregion

        #region Field Members


        private Int32 _id;

        private String _name;

        private String _code;

        private Int32 _seq = 10;

        private Byte _status;

        #endregion

        #region Property Members

        /// <summary>
        /// 获取或设置维度ID
        /// </summary>
        [Column(Name = "Id",IsIdentity=true)]
        public virtual Int32 Id
        {
            get { return this._id; }
            set { this._id = value; }
        }
        /// <summary>
        /// 获取或设置维度名称
        /// </summary>
        [Column(Name = "Name")]
        public virtual String Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        /// <summary>
        /// 获取或设置维度代号
        /// </summary>
        [Column(Name = "Code")]
        public virtual String Code
        {
            get { return this._code; }
            set { this._code = value; }
        }
        /// <summary>
        /// 获取或设置在记录集中排序号
        /// </summary>
        [Column(Name = "Seq")]
        public Int32 Seq
        {
            get { return this._seq; }
            set { this._seq = value; }
        }
        /// <summary>
        /// 获取或设置状态
        /// </summary>
        [Column(Name = "Status")]
        public Byte Status
        {
            get { return this._status; }
            set { this._status = value; }
        }
        #endregion
    }
}