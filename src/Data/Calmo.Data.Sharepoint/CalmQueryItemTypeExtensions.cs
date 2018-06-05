using System.Linq.Expressions;

namespace Calmo.Data.Sharepoint
{
    internal enum CalmQueryItemType
    {
        Contains,
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LowerThan,
        LowerThanOrEqual,
        IsNull,
        IsNotNull,
        BeginsWith
    }

    internal static class CalmQueryItemTypeExtensions
    {
        public static string GetCalmTag(this CalmQueryItemType enumItem)
        {
            switch (enumItem)
            {
                case CalmQueryItemType.Contains:
                    return "Contains";
                case CalmQueryItemType.Equal:
                    return "Eq";
                case CalmQueryItemType.NotEqual:
                    return "Neq";
                case CalmQueryItemType.GreaterThan:
                    return "Gt";
                case CalmQueryItemType.GreaterThanOrEqual:
                    return "Geq";
                case CalmQueryItemType.LowerThan:
                    return "Lt";
                case CalmQueryItemType.LowerThanOrEqual:
                    return "Leq";
                case CalmQueryItemType.IsNull:
                    return "IsNull";
                case CalmQueryItemType.IsNotNull:
                    return "IsNotNull";
                case CalmQueryItemType.BeginsWith:
                    return "BeginsWith";
            }

            return null;
        }

        public static CalmQueryItemType ToCalmQueryItemType(this ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return CalmQueryItemType.Equal;
                case ExpressionType.NotEqual:
                    return CalmQueryItemType.NotEqual;
                case ExpressionType.GreaterThan:
                    return CalmQueryItemType.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return CalmQueryItemType.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return CalmQueryItemType.LowerThan;
                case ExpressionType.LessThanOrEqual:
                    return CalmQueryItemType.LowerThanOrEqual;
            }

            return default(CalmQueryItemType);
        }
    }
}