using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BaiDuOCR
{
    /// <summary>
    /// 身份证
    /// </summary>
    public class IdcardOCRModel
    {
        /// <summary>
        /// 证件姓名
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { set; get; }

        /// <summary>
        /// 民族
        /// </summary>
        public string Nation { set; get; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birth { set; get; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { set; get; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Id { set; get; }

        /// <summary>
        /// 证件的有效期
        /// </summary>
        public string ValidDate { set; get; }

        /// <summary>
        /// 发证机关
        /// </summary>
        public string Authority { set; get; }

        /// <summary>
        /// 身份证图片名称，用于和真人图片对比时做标识
        /// </summary>
        public string ImgageName { set; get; }
    }
}