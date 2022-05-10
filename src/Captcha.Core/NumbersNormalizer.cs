﻿using System;
namespace Captcha.Core
{
    internal static class NumbersNormalizer
    {
        /// <summary>
        /// Converts Persian and Arabic digits of a given string to their equivalent English digits.
        /// </summary>
        /// <param name="data">Persian number</param>
        internal static string ToEnglishNumbers(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            return
                data.Replace("\u0660", "0", StringComparison.Ordinal) //٠
                    .Replace("\u06F0", "0", StringComparison.Ordinal) //۰
                    .Replace("\u0661", "1", StringComparison.Ordinal) //١
                    .Replace("\u06F1", "1", StringComparison.Ordinal) //۱
                    .Replace("\u0662", "2", StringComparison.Ordinal) //٢
                    .Replace("\u06F2", "2", StringComparison.Ordinal) //۲
                    .Replace("\u0663", "3", StringComparison.Ordinal) //٣
                    .Replace("\u06F3", "3", StringComparison.Ordinal) //۳
                    .Replace("\u0664", "4", StringComparison.Ordinal) //٤
                    .Replace("\u06F4", "4", StringComparison.Ordinal) //۴
                    .Replace("\u0665", "5", StringComparison.Ordinal) //٥
                    .Replace("\u06F5", "5", StringComparison.Ordinal) //۵
                    .Replace("\u0666", "6", StringComparison.Ordinal) //٦
                    .Replace("\u06F6", "6", StringComparison.Ordinal) //۶
                    .Replace("\u0667", "7", StringComparison.Ordinal) //٧
                    .Replace("\u06F7", "7", StringComparison.Ordinal) //۷
                    .Replace("\u0668", "8", StringComparison.Ordinal) //٨
                    .Replace("\u06F8", "8", StringComparison.Ordinal) //۸
                    .Replace("\u0669", "9", StringComparison.Ordinal) //٩
                    .Replace("\u06F9", "9", StringComparison.Ordinal) //۹
                ;
        }

        /// <summary>
        /// Converts English digits of a given string to their equivalent Persian digits.
        /// </summary>
        /// <param name="data">English number</param>
        internal static string ToPersianNumbers(this string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return string.Empty;
            return
               data
                .ToEnglishNumbers()
                .Replace("0", "\u06F0", StringComparison.Ordinal)
                .Replace("1", "\u06F1", StringComparison.Ordinal)
                .Replace("2", "\u06F2", StringComparison.Ordinal)
                .Replace("3", "\u06F3", StringComparison.Ordinal)
                .Replace("4", "\u06F4", StringComparison.Ordinal)
                .Replace("5", "\u06F5", StringComparison.Ordinal)
                .Replace("6", "\u06F6", StringComparison.Ordinal)
                .Replace("7", "\u06F7", StringComparison.Ordinal)
                .Replace("8", "\u06F8", StringComparison.Ordinal)
                .Replace("9", "\u06F9", StringComparison.Ordinal)
                .Replace(".", ",", StringComparison.Ordinal);
        }
    }
}