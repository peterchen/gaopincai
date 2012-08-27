namespace Lottery.Data
{
    /// <summary>
    /// SQL����е���������ö��
    /// </summary>
    public enum SortTypeEnum
    {
        /// <summary>
        /// ����
        /// </summary>
        ASC,

        /// <summary>
        /// ����
        /// </summary>
        DESC
    }

    /// <summary>
    /// SQL��������Ӿ�����
    /// </summary>
    public enum SqlClause : int
    {
        /// <summary>
        /// �޴Ӿ�
        /// </summary>
        None = 0,

        /// <summary>
        /// WHERE�Ӿ�
        /// </summary>
        Where = 1,

        /// <summary>
        /// Having�Ӿ�
        /// </summary>
        Having = 2
    }

    /// <summary>
    /// ��������
    /// </summary>
    public enum Bracket : int
    {
        /// <summary>
        /// '('
        /// </summary>
        Left = 0,

        /// <summary>
        /// ')'
        /// </summary>
        Rgiht = 1,
    }
}