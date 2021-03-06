﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MudHook.Core.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string ToSlug(this String source)
        {
            var regex = new Regex(@"([^a-z0-9\-]?)");
            var slug = "";

            if (!string.IsNullOrEmpty(source))
            {
                slug = source.Trim().ToLower();
                slug = slug.Replace(' ', '-');
                slug = slug.Replace("---", "-");
                slug = slug.Replace("--", "-");
                if (regex != null)
                    slug = regex.Replace(slug, "");

                if (slug.Length * 2 < source.Length)
                    return "";

                if (slug.Length > 100)
                    slug = slug.Substring(0, 100);
            }
            return slug;

        }
    }
}
