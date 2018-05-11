using MongoDB.Bson;

namespace NETMP.Module8.MongoDb.DataAccess.Converters
{
    public static class BsonValuesConverter
    {
        public static BsonValue ConvertStringToBsonValue(string value)
        {
            if (value == null)
            {
                return BsonNull.Value;
            }

            return BsonValue.Create(value);
        }

        public static BsonValue ConvertNullableToBsonValue(int? value)
        {
            if (value == null)
            {
                return BsonNull.Value;
            }

            return BsonValue.Create(value);
        }

        public static string ConvertBsonValueToString(BsonValue value)
        {
            if (value is BsonNull)
            {
                return null;
            }

            return value.AsString;
        }
    }
}
