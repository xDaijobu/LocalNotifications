using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LocalNotifications
{
    public static class Extensions
    {
        public static long GetMilliSecondsSinceEpoch(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0/*h*/, 0/*m*/, 0/*s*/, 0 /*ms*/, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime.ToUniversalTime() - epoch).TotalMilliseconds);
        }

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T JsonToObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static List<T> JsonToListObject<T>(this string json)
        {
            return JsonConvert.DeserializeObject<List<T>>(json);
        }
    }
}