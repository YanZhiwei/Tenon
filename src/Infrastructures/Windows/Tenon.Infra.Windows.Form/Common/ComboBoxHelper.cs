namespace Tenon.Infra.Windows.Form.Common;

/// <summary>
///     ComboBox 帮助类
/// </summary>
public static class ComboBoxHelper
{
    #region Methods

    /// <summary>
    ///     为ComboBox绑定数据源并提供下拉提示
    /// </summary>
    /// <typeparam name="T">泛型</typeparam>
    /// <param name="combox">ComboBox</param>
    /// <param name="list">数据源</param>
    /// <param name="displayMember">显示字段</param>
    /// <param name="valueMember">隐式字段</param>
    /// <param name="displayText">下拉提示文字</param>
    public static void SetDataSource<T>(this ComboBox combox, List<T> list, string displayMember,
        string valueMember, string displayText)
        where T : class
    {
        AddItem(list, displayMember, displayText);
        combox.DataSource = list;
        combox.DisplayMember = displayMember;
        if (!string.IsNullOrEmpty(valueMember))
            combox.ValueMember = valueMember;
    }

    private static void AddItem<T>(IList<T> list, string displayMember, string displayText)
    {
        object obj = Activator.CreateInstance<T>();
        var type = obj.GetType();
        if (!string.IsNullOrEmpty(displayMember))
        {
            var displayProperty = type.GetProperty(displayMember);
            if (displayProperty != null) displayProperty.SetValue(obj, displayText, null);
        }

        list.Insert(0, (T) obj);
    }

    #endregion Methods
}