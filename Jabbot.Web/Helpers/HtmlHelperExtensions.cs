using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string Base64Encode(this HtmlHelper helper, string plainText)
        {
            byte[] toEncodeAsBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            string returnValue = System.Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        public static string FormatWithCommas(this HtmlHelper helper, long number)
        {
            return number.ToString("n0");
        }

        public static string FormatTimeAgo(this HtmlHelper helper, DateTimeOffset dateTimeOffset, bool includeAgo)
        {
            StringBuilder builder = new StringBuilder();

            TimeSpan timeAgo = DateTimeOffset.UtcNow - dateTimeOffset;

            if (timeAgo.TotalDays >= 365)
            {
                builder.Append(String.Format("{0} years", Math.Floor(timeAgo.TotalDays / 365)));
            }
            else if (timeAgo.TotalDays >= 1)
            {
                builder.Append(String.Format("{0} days", Math.Floor(timeAgo.TotalDays)));
            }
            else if (timeAgo.TotalHours >= 1)
            {
                builder.Append(String.Format("{0} hours", Math.Floor(timeAgo.TotalHours)));
            }
            else if (timeAgo.TotalMinutes >= 1)
            {
                builder.Append(String.Format("{0} minutes", Math.Floor(timeAgo.TotalMinutes)));
            }
            else
            {
                builder.Append(String.Format("{0} seconds", Math.Ceiling(timeAgo.TotalSeconds)));
            }

            if (includeAgo)
            {
                builder.Append(" ago");
            }

            return builder.ToString();
        }
    }
}