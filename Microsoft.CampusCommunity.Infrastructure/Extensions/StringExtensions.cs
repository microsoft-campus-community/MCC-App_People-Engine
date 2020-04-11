// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.CampusCommunity.Infrastructure.Extensions
{
    public static class StringExtensions
    {
        public static SecureString ToSecureString(this string @string)
        {
            var secureString = new SecureString();

            if (@string.Length > 0)
                foreach (var c in @string.ToCharArray())
                    secureString.AppendChar(c);

            return secureString;
        }

        public static string ToUnsecureString(this SecureString secureString)
        {
            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);

                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }

        public static string ToLowerString(this string value)
        {
            return value?.TrimSpecialCharacters().ToLower();
        }

        public static string TrimSpecialCharacters(this string value)
        {
            char[] trimCharacters =
            {
                '\r',
                '\n',
                (char) 60644, // 
                (char) 60932, // 
                (char) 59540, // 
                (char) 60038, // 
                (char) 61424, // 
                (char) 59902, //
            };

            var result = value?.Trim()
                .Trim(trimCharacters)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Replace(Environment.NewLine, string.Empty);

            return result;
        }

        public static bool Contains(this string source, string value, StringComparison compare)
        {
            return source.IndexOf(value, compare) >= 0;
        }

        public static bool IsEmptyValue(this string fieldValue)
        {
            return string.IsNullOrWhiteSpace(fieldValue?.Trim('-')); // null, Empty or "---"
        }

        public static bool IsValueEqualsTo(this string fieldValue, string expected)
        {
            return fieldValue == expected || fieldValue.IsEmptyValue() && expected.IsEmptyValue();
        }

        public static string RemoveDiacritics(this string s)
        {
            var normalizedString = s.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }
    }
}