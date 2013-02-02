using System.Windows.Forms;
using System.Data;
using System;

namespace CTool.CUtility
{
    /// <summary>
    /// Treeview控件操作类
    /// </summary>
    public class TreeViewTool
    {
        /// <summary>
        /// 递归选中或者取消该节点下的所有级的全部子节点
        /// </summary>
        /// <param name="tnParent">父节点</param>
        /// <param name="isChecked">选中：true/取消选中：false</param>
        public static void SetAllChildNodeChecked(TreeNode tnParent, bool isChecked)
        {
            foreach (TreeNode tnchild in tnParent.Nodes)
            {
                tnchild.Checked = isChecked;
                SetAllChildNodeChecked(tnchild, isChecked);
            }
        }


        #region 递归加载treeview树目录通用方法
        /// <summary>
        /// 根据Datatable进行树形节点的加载，该方法适用于一次性加载全部父子级的treeview控件
        /// Datatable包含字段为（节点id，节点名称，所属父节点id）
        /// 节点id保存在节点的Tag属性中
        /// </summary>
        /// <param name="tv">treeview控件</param>
        /// <param name="sourceDt">数据源</param>
        /// <param name="parentFilter">父级节点的筛选条件</param>
        /// <param name="fieldName">节点的字段名</param>
        /// <param name="fieldId">节点的id的字段名</param>
        /// <param name="fieldParentId">父节点的id的字段名</param>
        public static void LoadAllNode(ref TreeView tv, DataTable sourceDt, string parentFilter, string fieldId, string fieldName, string fieldParentId)
        {
            DataView dv = sourceDt.DefaultView;
            dv.RowFilter = parentFilter;
            DataTable firstDt = dv.ToTable();
            foreach (DataRow row in firstDt.Rows)
            {
                TreeNode tnParent = tv.Nodes.Add(row[fieldName].ToString());
                tnParent.Tag = row[fieldId];
                LoadChildNode(tnParent, sourceDt, fieldId, fieldName, fieldParentId);
            }
        }
        /// <summary>
        /// 递归加载子节点
        /// </summary>
        static void LoadChildNode(TreeNode parentNode, DataTable sourceDt, string fieldId, string fieldName, string fieldParentId)
        {
            DataView dvchil = sourceDt.DefaultView;
            dvchil.RowFilter = fieldParentId + "=" + parentNode.Tag;
            DataTable newchilTb = dvchil.ToTable();
            foreach (DataRow dr in newchilTb.Rows)
            {
                TreeNode tnNew = parentNode.Nodes.Add(dr[fieldName].ToString());
                tnNew.Tag = dr[fieldId];
                LoadChildNode(tnNew, sourceDt, fieldId, fieldName, fieldParentId);
            }
        }
        #endregion


        /// <summary>
        /// 根据父节点id加载该父节点的子节点,从第一级开始加载，加载第一级传第一级id
        /// 由于只加载第一级，所以前面不会有+号，为了出现+号，必需传一个是否有子集的标记,然后动态添加一个空节点让父节点出现+号
        /// 所以DataTable的结构为（节点id，节点名称，所属父节点id，是否有子集）
        /// </summary>
        /// <param name="tnParent">父级节点</param>
        /// <param name="tv">treeview控件</param>
        /// <param name="sourceDt">子级数据源</param>
        /// <param name="isFirstNode">当treeview节点为空的时候传true，也就是要加载第一级节点</param>
        /// <param name="fieldId">节点id字段名</param>
        /// <param name="fieldName">节点名称字段名</param>
        /// <param name="fieldHasChild">是否有子节点</param>
        public static void GetChildNodeWithParentId(TreeNode tnParent, ref TreeView tv, DataTable sourceDt, bool isFirstNode, string fieldId, string fieldName, string fieldHasChild)
        {
            DataTable dt = sourceDt;
            foreach (DataRow dr in dt.Rows)
            {
                TreeNode tnNew = null;
                if (!isFirstNode)
                    tnNew = tv.Nodes.Add(dr[fieldName].ToString());
                else
                    tnNew = tnParent.Nodes.Add(dr[fieldName].ToString());
                tnNew.Tag = dr[fieldId];
                if (Convert.ToBoolean(dr[fieldHasChild]))
                    tnNew.Nodes.Add("");
            }
        }


        /// <summary>
        /// 递归加载所有打钩的非第一级节点的id集合
        /// </summary>
        /// <param name="tnParent">第一级节点</param>
        /// <param name="resultTypes">最终的集合（返回的结果如:,1,45,67,88  如果不为空字符串，需将第一个逗号去掉）</param>
        public static void GetAllTagWithoutFirstNode(TreeNode tnParent, ref string resultTypes)
        {
            foreach (TreeNode nodeChild in tnParent.Nodes)
            {
                if (nodeChild.Checked)
                {
                    resultTypes += "," + nodeChild.Tag;
                }
                GetAllTagWithoutFirstNode(nodeChild, ref resultTypes);
            }
        }
    }
}
