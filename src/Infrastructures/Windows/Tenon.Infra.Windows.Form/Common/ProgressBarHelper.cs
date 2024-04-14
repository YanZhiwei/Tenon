using System.Windows.Forms;

namespace Tenon.Infra.Windows.Form.Common
{
    /// <summary>
    ///     ProgressBar 帮助类
    /// </summary>
    public static class ProgressBarHelper
    {
        #region Methods

        /// <summary>
        ///     启动
        /// </summary>
        /// <param name="progress">ProgressBar</param>
        public static void Start(this ProgressBar progress)
        {
            progress.Style = ProgressBarStyle.Marquee;
            progress.MarqueeAnimationSpeed = 30;
        }

        /// <summary>
        ///     停止
        /// </summary>
        /// <param name="progress">ProgressBar</param>
        /// <param name="value">停止时候刻度值</param>
        public static void Stop(this ProgressBar progress, int value)
        {
            progress.Value = 50;
            progress.Style = ProgressBarStyle.Continuous;
            progress.MarqueeAnimationSpeed = 0;
        }

        #endregion Methods
    }
}