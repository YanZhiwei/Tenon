using System.Windows.Forms;

namespace Tenon.Infra.Windows.Form.Common
{
    /// <summary>
    ///     TabPage 帮助类
    /// </summary>
    /// 时间：2016-01-12 17:28
    public static class TabPageHelper
    {
        #region Methods

        /// <summary>
        ///     设置Enabled属性
        /// </summary>
        /// <param name="tabpage">TabPage</param>
        /// <param name="enable">if set to <c>true</c> [enable].</param>
        public static void SetEnabled(this TabPage tabpage, bool enable)
        {
            ((Control) tabpage).Enabled = false;
        }

        #endregion Methods
    }
}